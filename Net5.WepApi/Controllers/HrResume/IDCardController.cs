using Hr.Resume.IService;
using Hr.Resume.IService.Baidu;
using Hr.Resume.Model;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Utilities;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Text;
using System.Threading.Tasks;
using static Mitac.Core.Utilities.Enums;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class IDCardController : ControllerBase
    {
        private readonly IBaiduAccessToken _token;
        private readonly IDbRepository _db;

        public IDCardController(IDbRepository db, IBaiduAccessToken token)
        {
            _db = db;
            _token = token;
        }

        /// <summary>
        /// 调用百度身份证识别接口，读取身份证正反面信息
        /// </summary>
        /// <param name="cardType">身份正面或反面</param>
        /// <returns></returns>
        [HttpPost]
        public ActionResult<string> PostIdCard([FromBody] CardType cardType)
        {
            TokenInfo tokenInfo = JsonConvert.DeserializeObject<TokenInfo>(_token.GetToken());

            string host = "https://aip.baidubce.com/rest/2.0/ocr/v1/idcard?access_token=" + tokenInfo.access_token;
            Encoding encoding = Encoding.Default;
            HttpWebRequest request = (HttpWebRequest)WebRequest.Create(host);
            request.Method = "post";
            request.KeepAlive = true;

            string requestParams = "id_card_side=" + cardType.side;
            requestParams += "&detect_direction=true";       //身份证朝向识别
            requestParams += "&detect_risk=true";       //身份证风险提醒
            requestParams += "&detect_photo=true";      //检测身份证头像
            requestParams += "&image=" + System.Web.HttpUtility.UrlEncode(cardType.base64Img);
            byte[] buffer = encoding.GetBytes(requestParams);
            request.ContentLength = buffer.Length;
            request.GetRequestStream().Write(buffer, 0, buffer.Length);
            HttpWebResponse response = (HttpWebResponse)request.GetResponse();
            StreamReader reader = new StreamReader(response.GetResponseStream(), Encoding.UTF8);
            string result = reader.ReadToEnd();

            return Ok(result);
        }

        [HttpGet]
        public ActionResult<string>CheckIdCard(string idCardNo)
        {
            string errMsg = "";

            //bool EmployeeStatus = DB.IsExistEmpNoByID(idCardNo);// 是否已在职
            bool IsInfoConfirmed = _db.IsInfoConfirmed(idCardNo, "0"); // 是否已存在经过HR确认的人员资料，即狀態為大於0的
            bool IsUnderAge = _db.IsUnderAge(idCardNo);

            if (IsInfoConfirmed)//已有资料，且已经HR确认，无法修改
            {
                errMsg = "您的资料HR已确认，无法再修改";
            }

            if (IsUnderAge == true)
            {
                errMsg = "您還是未成年人,不可填寫";
            }

            // 如果身份证是18位且格式不正确，或者15且格式不正确，则报错
            if ((idCardNo.Length == 18 && !CheckDataFormate.CheckIDCard18(idCardNo))
                || idCardNo.Length == 15 && !CheckDataFormate.CheckIDCard15(idCardNo))
            {
                errMsg = "身份证长度或格式不正确，请确认";
            }

            // 如果异常信息不为空，则抛出异常
            if (!string.IsNullOrEmpty(errMsg)) throw new Exception(errMsg);
            return Ok("OK");
        }
    }
}
