using Hangfire;
using Hangfire.Oracle.Core;
using Hr.Contract.BackgroundTask;
using Hr.Resume.IService;
using Hr.Resume.IService.Baidu;
using Hr.Resume.IService.JWT;
using Hr.Resume.Service;
using Hr.Resume.Service.Baidu;
using Hr.Resume.Service.JWT;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.IdentityModel.Tokens;
using Microsoft.OpenApi.Models;
using Mitac.Core.Configuration;
using Mitac.Core.Filters;
using Mitac.Core.Utilities;
using SqlSugar;
using System;
using System.Linq;
using System.Text;

namespace Mitac.Net5.WebApi
{
    public class Startup
    {
        public Startup(IConfiguration configuration)
        {
            Configuration = configuration;
        }

        public IConfiguration Configuration { get; }

        // This method gets called by the runtime. Use this method to add services to the container.
        public void ConfigureServices(IServiceCollection services)
        {
            #region 注册自定义服务
            //先注入服务，每个页面即可在构造函数中引用
            services.AddSingleton<IJwtAuthService, JwtAuthService>();
            services.AddSingleton<IDbRepository, DbRepository>();
            services.AddSingleton<IBaiduAccessToken, BaiduAccessToken>();

            // 获取config方式一
            // var config = Configuration["secret:JWT"];
            //获取config方式二，通过注册使所有页面都可以访问到AppSettings
            var appSettings = Configuration.Get<AppSettings>();
            services.AddSingleton<AppSettings>(appSettings);

            //services.AddSingleton<ResponseData>(option => new ResponseData());
            #endregion

            #region 配置sqlSugar数据库
            services.AddTransient<ISqlSugarClient>(option =>
            {
                SqlSugarClient client = new SqlSugarClient(new ConnectionConfig()
                {
                    ConnectionString = appSettings.OracleProd,
                    DbType = DbType.Oracle,
                    InitKeyType = InitKeyType.Attribute,//从特性读取主键和自增列信息
                    IsAutoCloseConnection = true,//开启自动释放模式和EF原理一样
                });

                client.Aop.OnLogExecuting = (sql, pars) =>
                {
                    //Console.WriteLine($"Sql語句是:{sql}\r\n" +
                    //                  client.Utilities.SerializeObject(pars.ToDictionary(it => it.ParameterName, it => it.Value)));
                    //AppLog.Info(sql);
                };

                return client;
            });
            #endregion

            #region 添加jwt验证
            services.AddAuthentication(JwtBearerDefaults.AuthenticationScheme)
              .AddJwtBearer(options =>
              {
                  options.TokenValidationParameters = new TokenValidationParameters
                  {
                      ValidateIssuer = true,//是否验证Issuer
                      ValidateAudience = true,//是否验证Audience
                      ValidateLifetime = true,//是否验证失效时间
                      ClockSkew = TimeSpan.FromSeconds(30),
                      ValidateIssuerSigningKey = true,//是否验证SecurityKey
                      ValidAudience = appSettings.secret.Audience,//Audience
                      ValidIssuer = appSettings.secret.Issuer,//Issuer，这两项和前面签发jwt的设置一致
                      IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(appSettings.secret.JWT))//拿到SecurityKey
                  };
              });
            #endregion

            #region 配置跨域
            services.AddCors(options =>
            {
                options.AddDefaultPolicy(// 亦可设置多个域options.AddPolicy("Domain",builder=>....
                    builder =>
                    {
                        builder.SetIsOriginAllowed((host) => true) // 允许所有域访问，=AllowAnyOrigin()(netcore3写法)
                        //.WithOrigins(appSettings.corsUrls.Split(",")) //只允許指定的網址
                          .AllowAnyHeader()
                          .AllowAnyMethod()
                          .AllowCredentials();
                        
                    });
            });
            #endregion

            #region 注册Hangfire,使用oracle数据记录配置信息
            //Add Hangfire services.
            services.AddHangfire(configuration => configuration.UseStorage(
                new OracleStorage(appSettings.OracleProd, new OracleStorageOptions
                {
                    QueuePollInterval = TimeSpan.FromSeconds(15),
                    JobExpirationCheckInterval = TimeSpan.FromHours(1),
                    CountersAggregateInterval = TimeSpan.FromMinutes(5),
                    PrepareSchemaIfNecessary = true,
                    DashboardJobListLimit = 50000,
                    TransactionTimeout = TimeSpan.FromMinutes(1)
                })));
            services.AddHangfireServer(options => { options.WorkerCount = 1; options.ServerName = "RFQHangfireJobServer"; });
            #endregion

            #region 注册controller
            //services.AddControllers();
            //注册过滤器到全局
            services.AddControllers(option =>
            {
                option.Filters.Add(typeof(ExceptionFilter));
                option.Filters.Add(typeof(ActionFilter)); 
                option.Filters.Add(typeof(ResultFilter));
            }).AddNewtonsoftJson();
            #endregion

            // IMemoryCache注册
            services.AddMemoryCache();

            // 由于[ApiController] 默认自带有400模型验证[BadRequestResult]，且优先级比较高
            // 如果需要自定义模型验证，则需要先关闭默认的模型验证
            services.Configure<ApiBehaviorOptions>(options => options.SuppressModelStateInvalidFilter = true);

            // api文档配置
            services.AddSwaggerGen(c =>
            {
                c.SwaggerDoc("v1", new OpenApiInfo { Title = "Net5.WebApi", Version = "v1" });
            });
        }

        // This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
        #region 配置启动管道
        public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
        {
            var appSettings = Configuration.Get<AppSettings>();
            if (env.IsDevelopment())
            {
                app.UseDeveloperExceptionPage();
                // 如果想正式发布后也有swagger，需要把如下两行挪出去
                app.UseSwagger();
                app.UseSwaggerUI(c => c.SwaggerEndpoint("/swagger/v1/swagger.json", "Mitac.Net5.WebApi v1"));
            }

            app.UseHttpsRedirection();

            app.UseRouting();
            // 添加跨域，UseRouting必须在上面
            app.UseCors();

            // 添加jwt验证
            app.UseAuthentication();

            app.UseAuthorization();

            // webapi路由，restful风格
            app.UseEndpoints(endpoints =>
            {
                endpoints.MapControllers();
            });

            #region MVC路由配置（传统路由conventional）
            //app.UseEndpoints(endpoints =>
            //{
            //    endpoints.MapControllerRoute(
            //    name: "default",
            //    pattern: "api/{controller=JWTAuth}/{action=getstring}/{id?}");

            //    endpoints.MapAreaControllerRoute(
            //    name: "areas", "areas",
            //    pattern: "{area:exists}/{controller=Home}/{action=Index}/{id?}");
            //});
            #endregion

            #region Hangfire 
            app.UseHangfireDashboard("/hangfire", new DashboardOptions()
            {
                Authorization = new[] { new HangfireAuthFilter() }//Hangfire 默认增加了授权配置,加此代码去除验证
            });

            //BackgroundJob.Enqueue<ContractDraftSend>(x => x.Test());
            //执行backgroundtask
            if (appSettings.BackgroundTaskEnabled)
            {
                RecurringJob.AddOrUpdate<ContractDraftSend>("ContractDraftSend", x => x.Execute(), appSettings.ContractDraftSend_Cron, TimeZoneInfo.Local);
                RecurringJob.AddOrUpdate<ContractDownload>("ContractDownload", x => x.Execute(), appSettings.ContractDownload_Cron, TimeZoneInfo.Local);
            }
            #endregion
        }
        #endregion

    }
}
