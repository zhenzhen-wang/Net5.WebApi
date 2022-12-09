using Hr.Resume.IService;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class EmployerController : ControllerBase
    {
        private readonly IDbRepository _db;
        public EmployerController( IDbRepository db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据身份证号，获取该员工是否在黑名单中
        /// </summary>
        /// <param name="IdCardNo"></param>
        [HttpGet("{IdCardNo}")]
        public ActionResult<string> GetBlackInfo(string IdCardNo)
        {
            return Ok(_db.GetBlackInfo(IdCardNo));
        }

        /// <summary>
        /// 根据身份证号，及其状态查询人员资料
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public Dictionary<string,object> GetDetailByIdStatus(string idCardNo, string status)
        {
            return _db.GetDetailByIdStatus(idCardNo, status);
        }

        /// <summary>
        /// 前端传过来的个人简历保存
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> InsertHrResume([FromBody] JObject form)
        {     
            return Ok(_db.InsertHrResume(form)); 
        }

        [HttpPut]
        public ActionResult<string> UpdateHrResume([FromBody] JObject form)
        {
            return Ok(_db.UpdateHrResume(form));
        }
    }
}
