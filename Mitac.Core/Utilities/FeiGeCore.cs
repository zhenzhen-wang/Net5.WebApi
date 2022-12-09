using Mitac.Core.Configuration;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Text;
using System.Threading.Tasks;

namespace Mitac.Core.Utilities
{
    public class FeiGeCore
    {
        /// <summary>
        /// 推送数据 POST方式
        /// </summary>
        /// <param name="weburl">POST到的网址</param>
        /// <param name="data">POST的参数及参数值</param>
        /// <param name="encode">编码方式</param>
        /// <returns></returns>
        public static string PushToWeb(string weburl, string data, Encoding encode)
        {
            try
            {
                byte[] byteArray = encode.GetBytes(data);

                HttpWebRequest webRequest = (HttpWebRequest)WebRequest.Create(new Uri(weburl));
                webRequest.Method = "POST";
                webRequest.ContentType = "application/x-www-form-urlencoded";
                webRequest.ContentLength = byteArray.Length;

                Stream newStream = webRequest.GetRequestStream();
                newStream.Write(byteArray, 0, byteArray.Length);
                newStream.Close();

                //接收返回信息：
                HttpWebResponse response = (HttpWebResponse)webRequest.GetResponse();
                StreamReader aspx = new StreamReader(response.GetResponseStream(), encode);
                string msg = aspx.ReadToEnd();
                aspx.Close();
                response.Close();
                return msg;
            }
            catch (Exception ex)
            {
                //记录错误日志
                return "";
            }
        }

        #region 获取时间戳
        /// <summary>
        /// 获取时间戳
        /// </summary>
        /// <param name="serverTime">时间</param>
        /// <param name="isMillisecond">是否精确到毫秒</param>
        /// <returns></returns>
        public static string ToUnixStamp(DateTime serverTime)
        {
            var ts = serverTime - TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            return Convert.ToInt64(ts.TotalSeconds).ToString();
        }
        #endregion

        #region 转换时间戳
        /// <summary>
        /// 转换时间戳
        /// </summary>
        /// <param name="unixStamp">时间戳</param>
        /// <param name="isMillisecond">是否精确到毫秒</param>
        /// <returns></returns>
        public static DateTime FromUnixStamp(long unixStamp)
        {
            DateTime dtStart = TimeZone.CurrentTimeZone.ToLocalTime(new DateTime(1970, 1, 1));
            long lTime = long.Parse(unixStamp + "0000000");
            TimeSpan toNow = new TimeSpan(lTime);
            return dtStart.Add(toNow);
        }
        #endregion



        private const string BASECODE = "0123456789";
        static Random ranNum = new Random((int)DateTime.Now.Ticks);
        /// <summary>
        /// 生成6位随机码
        /// </summary>
        public static string GetVCode(int length)
        {
            StringBuilder builder = new StringBuilder();
            for (int i = 0; i < length; i++)
            {
                int rnNum = ranNum.Next(BASECODE.Length);
                builder.Append(BASECODE[rnNum]);
            }
            return builder.ToString();
        }

    }
}
