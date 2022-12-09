using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Microsoft.Extensions.Logging;
using Mitac.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using static Mitac.Core.Utilities.Enums;

namespace Mitac.Core.Filters
{
    public class ExceptionFilter : Attribute, IExceptionFilter
    {
        private readonly ILogger<ExceptionFilter> _logger;

        public ExceptionFilter(ILogger<ExceptionFilter> logger)
        {
            _logger = logger;
        }

        public void OnException(ExceptionContext context)
        {
            if (!context.ExceptionHandled) //异常是否被处理过（有可能会有其他filter处理异常）
            {
                //如果截获异常为我们自定义，可以处理的异常则通过我们自己的规则处理
                if (context.Exception is CustomizeException)
                {
                    //记录日志
                    _logger.LogError(context.Exception.Message);

                }
                else
                {
                    //记录日志
                    _logger.LogError(context.Exception.Message);

                    //如果截获异常是我没无法预料的异常，则将通用的返回信息返回给用户，便于用户处理
                    context.Result = new JsonResult(new ResponseData<string>
                    {
                        code = ResultStatus.Error,
                        message = context.Exception.Message
                    });//中断式---请求到这里结束了，不再继续Action
                }
                context.ExceptionHandled = true;
            }

            //    //定义返回信息
            //    var res = new
            //    {
            //        Code = 500,
            //        Message = "发生错误,请联系管理员"
            //    };
            //context.Result = new ContentResult
            //{
            //    // 返回状态码设置为200，表示成功
            //    StatusCode = StatusCodes.Status200OK,
            //    // 设置返回格式
            //    //ContentType = "application/json;charset=utf-8",
            //    //Content = JsonConvert.SerializeObject(res)
            //    ContentType = "text/html;charset=utf-8",
            //    Content = $"<h2 style='color:red'>发生错误 ：{context.Exception.Message}</h2>"
            //};

        }

    }
}
