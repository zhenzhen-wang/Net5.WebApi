using System;
using System.Collections.Generic;
using System.Text;

namespace SmsApiTest.model
{
    public static class ErrorCode
    {
        /// <summary>
        /// 成功
        /// </summary>
        public static readonly int SuccessCode = 0;
        /// <summary>
        /// 账户为空
        /// </summary>
        public static readonly int EmptyAccount = 10001;
        /// <summary>
        /// 密码不能为空
        /// </summary>
        public static readonly int EmptyPwd = 10002;
        /// <summary>
        /// 新密码不能为空
        /// </summary>
        public static readonly int NewPwd = 10003;
        /// <summary>
        /// 短信内容围攻
        /// </summary>
        public static readonly int EmptyContent = 10004;
        /// <summary>
        /// 手机号码不能为空
        /// </summary>
        public static readonly int EmptyMobile = 10005;
        /// <summary>
        /// 手机号码有误
        /// </summary>
        public static readonly int InvalidMobile = 10006;
        /// <summary>
        /// 发送号码个数限制
        /// </summary>
        public static readonly int LimitMobile = 10007;
        /// <summary>
        /// 模板不能为空
        /// </summary>
        public static readonly int EmptyTemplate = 10008;
        /// <summary>
        /// 未找到模板
        /// </summary>
        public static readonly int NotFoundTemplate = 10009;
        /// <summary>
        /// 所传参数和模板不匹配
        /// </summary>
        public static readonly int NotMatch = 10010;

        /// <summary>
        /// 暂无状态报告
        /// </summary>
        public static readonly int NoReport = 10011;
        /// <summary>
        /// 暂无上行
        /// </summary>
        public static readonly int NoMo = 10012;
        /// <summary>
        /// 没有签名Id
        /// </summary>
        public static readonly int EmptySignId= 10017;
        /// <summary>
        /// 签名Id有误
        /// </summary>
        public static readonly int ErrorSignId = 10018;
        /// <summary>
        /// 错误的时间戳
        /// </summary>
        public static readonly int ErrorStamp = 10019;
        ///////////////////////////////////账号状态开始////////////////////////////////////////

        /// <summary>
        /// 用户名或密码错误
        /// </summary>
        public static readonly int InvalidAccount = 20001;
        /// <summary>
        /// 账号被停用，请联系客服！
        /// </summary>
        public static readonly int StopAccount = 20002;
        /// <summary>
        /// 账号被注销，请联系客服！
        /// </summary>
        public static readonly int Cancellation = 20003;
        /// <summary>
        /// IP地址鉴权失败，请联系客服！
        /// </summary>
        public static readonly int IPError = 20004;
        /// <summary>
        /// 余额不足，请及时充值
        /// </summary>
        public static readonly int BalanceNotEnough = 20005;

        ///////////////////////////////////账号信息结束///////////////////////////////////////
        /// <summary>
        /// 通道屏蔽字
        /// </summary>
        public static readonly int GateBlack = -99;
        /// <summary>
        /// IP黑名单
        /// </summary>
        public static readonly int BlackIP = -999;
        /// <summary>
        /// 系统错误
        /// </summary>
        public static readonly int SystemError = -9999;
    }
}
