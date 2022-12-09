using System;
using System.Collections.Generic;
using System.Web;

namespace SmsApiTest.model.Request
{
    public class BaseSmsRequest
    {
        /// <summary>
        /// 账号
        /// </summary>
        public string Account { get; set; }
        /// <summary>
        /// 密码
        /// </summary>
        public string Pwd { get; set; }
        /// <summary>
        /// 发送内容
        /// </summary>
        public string Content { get; set; }
        /// <summary>
        /// 发送号码
        /// </summary>
        public string Mobile { get; set; }

        //发送时间戳
        public long? SendTime { get; set; }

        /// <summary>
        /// 用户签名
        /// </summary>
        public long SignId { get; set; }
        /// <summary>
        /// 扩展码
        /// </summary>
        public string ExtNo { get; set; }
    }
}