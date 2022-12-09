using Hr.Resume.IService;
using Hr.Resume.IService.Baidu;
using Microsoft.Extensions.Caching.Memory;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Utilities;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;

namespace Hr.Resume.Service.Baidu
{
    public class BaiduAccessToken : IBaiduAccessToken
    {
        // 返回token示例
        // public static String TOKEN = "24.adda70c11b9786206253ddb70affdc46.2592000.1493524354.282335-1234567";

        private readonly AppSettings _appSettings;
        private readonly ILogger<BaiduAccessToken> _logger;
        private readonly IMemoryCache _memoryCache;

        public BaiduAccessToken(ILogger<BaiduAccessToken> logger, AppSettings appSettings,
            IMemoryCache memoryCache)
        {
            _appSettings = appSettings;
            _logger = logger;
            _memoryCache = memoryCache;
        }

        public string GetToken()
        {            
            string token;
            // 如果本地服务器无缓存，则重新获取
            if (!_memoryCache.TryGetValue("baiduToken", out token))
            {
                // 不存在，则发起api请求token
                String authHost = "https://aip.baidubce.com/oauth/2.0/token";
                HttpClient client = new HttpClient();
                List<KeyValuePair<String, String>> paraList = new List<KeyValuePair<string, string>>();
                paraList.Add(new KeyValuePair<string, string>("grant_type", "client_credentials"));
                paraList.Add(new KeyValuePair<string, string>("client_id", _appSettings.baiduSecret.ClientId));
                paraList.Add(new KeyValuePair<string, string>("client_secret", _appSettings.baiduSecret.ClientSecret));

                HttpResponseMessage response = client.PostAsync(authHost, new FormUrlEncodedContent(paraList)).Result;
                String result = response.Content.ReadAsStringAsync().Result;

                // Save data in cache and set the relative expiration time to one day
                _memoryCache.Set("baiduToken", result, TimeSpan.FromDays(1)); // 百度默认30天过期 2592000
                token = result;
            }

            // 存在未过期的token
            return token;
        }
    }
}
