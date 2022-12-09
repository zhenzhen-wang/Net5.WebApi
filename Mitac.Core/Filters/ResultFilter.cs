using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mitac.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mitac.Core.Utilities.Enums;

namespace Mitac.Core.Filters
{
    public class ResultFilter : Attribute, IResultFilter
    {
        public void OnResultExecuting(ResultExecutingContext context)
        {
            // 对返回的正确结果做处理
            if (context.Result is ObjectResult objectResult)
            {
                ResponseData<object> res = new ResponseData<object>();
                res.result = objectResult.Value;

                objectResult.Value = res;
            }
            //else if (context.Result is OkObjectResult okResult)
            //{
            //    //如果返回类型是简单类型string时，需要用Ok包装string返回，到这里处理
            //    ResponseData<string> res = new ResponseData<string>();
            //    res.result = (string)okResult.Value; //将对象类型转换成string

            //    okResult.Value = res;
            //}
        }

        public void OnResultExecuted(ResultExecutedContext context)
        {
            //throw new NotImplementedException();
        }
    }
}
