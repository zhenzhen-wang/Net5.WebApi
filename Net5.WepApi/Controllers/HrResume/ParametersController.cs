using Hr.Resume.IService;
using Microsoft.AspNetCore.Mvc;
using Mitac.Core.Configuration;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Data;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class ParametersController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IDbRepository _db;

        public ParametersController(AppSettings appSettings, IDbRepository db)
        {
            _appSettings = appSettings;
            _db = db;
        }

        [HttpGet]
        //[TypeFilter(typeof(ExceptionFilter))] //filter中存在有参数构造函数,使用此方式局部使用
        public IEnumerable<string> GetParameterList([Required] string lookupType, [Required] string type)
        {
            var infoList = _db.GetParameters(lookupType, type);
            return infoList;
        }

        [HttpGet("{version}")]
        //[TypeFilter(typeof(ExceptionFilter))] //filter中存在有参数构造函数,使用此方式局部使用
        public DataTable GetInterviewData(string version)
        {
            var infoList = _db.GetInterviewData(version);
            return infoList;
        }
    }
}
