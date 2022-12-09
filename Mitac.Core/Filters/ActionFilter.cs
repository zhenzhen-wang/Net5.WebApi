using Microsoft.AspNetCore.Mvc.Filters;
using System;
using Newtonsoft.Json;
using System.Linq;
using System.Collections.Generic;
using Mitac.Core.Utilities;
using Microsoft.AspNetCore.Mvc;
using static Mitac.Core.Utilities.Enums;

namespace Mitac.Core.Filters
{
    public class ActionFilter : Attribute, IActionFilter
    {
        public void OnActionExecuting(ActionExecutingContext context)
        {
            //string param = JsonConvert.SerializeObject(context.HttpContext.Request.Query);
            //string controllerName = context.RouteData.Values["controller"].ToString();
            //string action = context.RouteData.Values["action"].ToString();
            //Console.WriteLine("before: " + param);

            // 处理模型验证异常（无法在exceptionFilter中捕捉到，因为modelbinding发生在exceptionfilter之后）
            if (!context.ModelState.IsValid)
            {
                //获取验证失败的模型字段ErrorCode
                var errorMsgList = context.ModelState.Keys.SelectMany(key => context.ModelState[key].Errors.Select(x => x.ErrorMessage)).ToList();

                //设置返回内容
                var str = string.Join("& ", errorMsgList);

                context.Result = new JsonResult(new ResponseData<string>
                {
                    code = ResultStatus.Error,
                    message = str
                });
            }
        }

        public void OnActionExecuted(ActionExecutedContext context)
        {
            //string param = JsonConvert.SerializeObject(context.Result);
            //Console.WriteLine("after: " + param);
        }
    }
}
