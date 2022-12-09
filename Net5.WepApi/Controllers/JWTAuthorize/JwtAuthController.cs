using Hr.Resume.IService.JWT;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Filters;
using Mitac.Core.Filters;
using System.Collections.Generic;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Mitac.Net5.WepApi.Controllers
{

    [Route("api/[controller]")]
    [ApiController]
    public class JwtAuthController : ControllerBase
    {

        private readonly IJwtAuthService _jwtAuth;

        public JwtAuthController(IJwtAuthService jwtAuth)
        {
            _jwtAuth = jwtAuth;
        }

        //获取token
        //[HttpGet]
        //public IActionResult GetToken(string name, string pwd) =>
        //    Content(_jwtAuth.Authentication(name, pwd));

        [HttpGet]
        public ActionResult<string> GetToken(string name, string pwd) =>
           _jwtAuth.Authentication(name, pwd);

    }
}
