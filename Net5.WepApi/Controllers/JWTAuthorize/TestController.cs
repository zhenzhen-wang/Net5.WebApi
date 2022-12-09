using Microsoft.AspNetCore.Mvc;
using Mitac.Core.Filters;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mitac.Net5.WepApi.Controllers.JWTAuthorize
{
    [Route("api/[controller]")]
    [ApiController]
    public class TestController : ControllerBase
    {
        // GET: api/<ValuesController>
        [HttpGet]
        [TypeFilter(typeof(ExceptionFilter))] //filter中存在有参数构造函数
        public IEnumerable<string> Get()
        {
            //throw new DemoException("这是个测试异常api");
            return new string[] { "value1", "value2" };
        }

        // GET api/<ValuesController>/5
        [HttpGet("{id}"), ActionFilter] //filter中无构造函数     
        public string Get(int id)
        {
            return "value";
        }

        //[HttpGet] // 不可以
        //public string GetList1(string a)
        //{
        //    return "value2";
        //}

        [HttpGet]
        [Route("getstring")]
        public IEnumerable<string> GetList()
        {
            return new string[] { "value1", "value2" };
        }

        // POST api/<ValuesController>
        [HttpPost]
        public void Post([FromBody] string value)
        {
        }

        // PUT api/<ValuesController>/5
        [HttpPut("{id}")]
        public void Put(int id, [FromBody] string value)
        {
        }

        // DELETE api/<ValuesController>/5
        [HttpDelete("{id}")]
        public void Delete(int id)
        {
        }
    }
}
