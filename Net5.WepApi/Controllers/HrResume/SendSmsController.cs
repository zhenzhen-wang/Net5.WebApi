using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Utilities;
using Newtonsoft.Json;
using SmsApiTest.model.Request;
using SmsApiTest.model.Response;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class SendSmsController : ControllerBase
    {
        private readonly string apiurl = "http://api.feige.ee";
        private readonly AppSettings _appSettings;
        private ILogger<SendSmsController> _logger;

        public SendSmsController(ILogger<SendSmsController> logger, AppSettings appSettings)
        {
            _appSettings = appSettings;
            _logger = logger;
        }

        [HttpGet]
        public ActionResult<string> SendSms(string phone)
        {
            string code = FeiGeCore.GetVCode(6);
            CommonSmsRequest request = new CommonSmsRequest
            {
                Account = "139****", //飞鸽账户名
                Pwd = "*****",//登录web平台 http://sms.feige.ee  在管理中心--基本资料--接口密码 或者首页 接口秘钥 如登录密码修改，接口密码会发生改变，请及时修改程序
                Content = $"{code}(手机号验证码),您正在验证手机号码有效性，验证码15分钟内有效，请勿泄露。",
                Mobile = phone,
                SignId = 10000, //登录web平台 http://sms.feige.ee  在签名管理中--新增签名--获取id
                SendTime = Convert.ToInt64(FeiGeCore.ToUnixStamp(DateTime.Now))//定时短信 把时间转换成时间戳的格式
            };

            StringBuilder arge = new StringBuilder();
            arge.AppendFormat("Account={0}", request.Account);
            arge.AppendFormat("&Pwd={0}", request.Pwd);
            arge.AppendFormat("&Content={0}", request.Content);
            arge.AppendFormat("&Mobile={0}", request.Mobile);
            arge.AppendFormat("&SignId={0}", request.SignId);
            arge.AppendFormat("&SendTime={0}", request.SendTime);
            string weburl = apiurl + "/SmsService/Send";
            string resp = FeiGeCore.PushToWeb(weburl, arge.ToString(), Encoding.UTF8);
            //Console.WriteLine("SendSms:" + resp);

            SendSmsResponse response = JsonConvert.DeserializeObject<SendSmsResponse>(resp);
            if (response.Code == 0)
            {
                //成功则将验证码返回给前端，用于跟用户输入的做比对
                return Ok(code);
            }
            else
            {
                //失败
                throw new Exception($"验证失败，原因代码：{response.Code}");
            }
        }
    }
}
