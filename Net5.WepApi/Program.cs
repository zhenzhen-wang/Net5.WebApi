using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Mitac.Net5.WebApi;
using NLog.Web;
using System;

namespace Mitac.Net5.WepApi
{
    public class Program
    {
        public static void Main(string[] args)
        {
            NLogBuilder.ConfigureNLog("nlog.config").GetCurrentClassLogger();
            CreateHostBuilder(args).Build().Run();           
        }

        public static IHostBuilder CreateHostBuilder(string[] args) =>
            Host.CreateDefaultBuilder(args)
                .ConfigureWebHostDefaults(webBuilder =>
                {
                    //webBuilder.UseStartup<Startup>();
                    webBuilder.ConfigureKestrel((context, options) =>
                    {
                        options.Configure(context.Configuration.GetSection("Kestrel"));
                    }).UseStartup<Startup>();
                })
                // 将NLog集成到.NetCore的Logging组件中
                // NLog将logging组件生成的log转移到指定路径显示，console跟debug页面都不显示
                .ConfigureLogging(logging =>
                {
                    // 可通过ILoggerFactory在startup.cs中的Configure方法添加额外的日志提供,也可在此添加
                    //logging.AddConsole();
                    //logging.AddDebug(); // netcore默认会在控制台和侦错窗口显示log信息
                    //logging.AddEventLog(); // 可以另外再加一个netcore内置的日志提供器

                    logging.ClearProviders();  //清空netcore默认的provider：Console/Debug
                    // The Logging configuration 'default' specified in appsettings.json overrides any call to SetMinimumLevel
                    logging.SetMinimumLevel(LogLevel.Debug); //最低显示debug信息
                })
                 .UseNLog();  // NLog: Setup NLog for Dependency injection
    }
}
