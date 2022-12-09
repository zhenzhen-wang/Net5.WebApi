using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Caching.Memory;
using Mitac.Core.Configuration;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Net.Http;

namespace Mitac.Net5.WepApi.Controllers.HrResume
{
    [Route("api/HrResume/[controller]")]
    [ApiController]
    public class WechatController : ControllerBase
    {
        private readonly AppSettings _appSettings;
        private readonly IMemoryCache _memoryCache;

        public WechatController(AppSettings appSettings, IMemoryCache memoryCache)
        {
            _appSettings = appSettings;
            _memoryCache = memoryCache;
        }

        [HttpGet]
        public ActionResult<string> GetToken()
        {
            string corpId = "wx4a47******"; //公司企业微信id
            string corpSecret = "2iWywQCU_******"; //应用秘钥
            string token;
            // 如果本地服务器无缓存，则重新获取
            if (!_memoryCache.TryGetValue("wxToken", out token))
            {
                // 不存在，则发起api请求token
                String authHost = $"https://qyapi.weixin.qq.com/cgi-bin/gettoken?corpid={corpId}&corpsecret={corpSecret}";
                HttpClient client = new HttpClient();

                HttpResponseMessage response = client.GetAsync(authHost).Result;
                String result = response.Content.ReadAsStringAsync().Result;
                JObject obj = (JObject)JsonConvert.DeserializeObject(result);
                token = obj["access_token"].ToString();

                // Save data in cache and set the relative expiration timeto one day
                _memoryCache.Set("wxToken", token, TimeSpan.FromHours(2)); // 2小时过期
            }

            // 存在未过期的token
            return Ok(token);
        }
    }
}
