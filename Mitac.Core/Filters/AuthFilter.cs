using Microsoft.AspNetCore.Mvc.Filters;
using System;

namespace Mitac.Core.Filters
{

    public class AuthFilter : Attribute, IAuthorizationFilter
    {        
        public void OnAuthorization(AuthorizationFilterContext context)
        {
            throw new NotImplementedException();
        }

        //api filter写法
        //public override void OnAuthorization(HttpActionContext actionContext)
        //{
        //    //如果用户方位的Action带有AllowAnonymousAttribute，则不进行授权验证
        //    if (actionContext.ActionDescriptor.GetCustomAttributes<AllowAnonymousAttribute>().Any())
        //    {
        //        return;
        //    }
        //    var verifyResult = actionContext.Request.Headers.Authorization != null &&  //要求请求中需要带有Authorization头
        //                       actionContext.Request.Headers.Authorization.Parameter == "123456"; //并且Authorization参数为123456则验证通过

        //    if (!verifyResult)
        //    {
        //        //如果验证不通过，则返回401错误，并且Body中写入错误原因
        //        actionContext.Response = actionContext.Request.CreateErrorResponse(HttpStatusCode.Unauthorized, new HttpError("Token 不正确"));
        //    }
        //}
    }
}
