using Hr.Resume.IService;
using Hr.Resume.Model;
using Microsoft.AspNetCore.Mvc;
using Newtonsoft.Json.Linq;
using System.Collections.Generic;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class AuditController : ControllerBase
    {
        private readonly IDbRepository _db;
        public AuditController(IDbRepository db)
        {
            _db = db;
        }

        /// <summary>
        /// 根据人员身份证号获取主管评论等信息
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        [HttpGet("{idCardNo}")]
        public HR_MANAGER_COMMENT GetManagerCommentByIdCard(string idCardNo)
        {
            return _db.GetManagerCommentByIdCard(idCardNo);
        }

        /// <summary>
        /// 根据员工类型，状态，工种类型，填表时间，身份证后四位，当日批次，员工姓名查询人员资料SearchEmployee
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        [HttpGet]
        public List<SearchEmployee> GetEmployeeList([FromQuery]SearchEmployee list)//如果参数是实体类，必须加FromQuery，否则报错：媒体类型不支援。入股是普通类型则不需要加
        {
            return _db.GetEmployeeList(list);
        }
        
        /// <summary>
        /// 更新人员状态
        /// </summary>
        /// <param name="form"></param>
        /// <returns></returns>
        [HttpPut]
        public ActionResult<string> UpdateStatus([FromBody]UpdateParams updateParams)
        {
            return Ok(_db.UpdateStatus(updateParams));
        }

        /// <summary>
        /// 新增主管评论等
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> InsertComment([FromBody] ManagerParams param)
        {
            return Ok(_db.InsertComment(param));
        }


        /// <summary>
        /// 新增最终面试结论（职称等）
        /// </summary>
        /// <param name="updateParams"></param>
        /// <returns></returns>
        [HttpPost("InsertResult")]
        public ActionResult<string> InsertResult([FromBody] HrConfirmParams param)
        {
            return Ok(_db.InsertResult(param));
        }
    }
}
