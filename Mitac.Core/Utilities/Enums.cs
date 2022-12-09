using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitac.Core.Utilities
{
    public class Enums
    {
        public enum ResultStatus
        {
            [Description("请求成功")]
            Success = 1,
            [Description("请求异常")]
            Error = 0,
            [Description("请求失败")]
            Fail = -1
            
        }

        public enum contractStatus
        {
            /// <summary>
            /// 合同待發起
            /// </summary>
            Wait = 0,
            /// <summary>
            /// 合同草稿已发送到契约锁平台
            /// </summary>
            Draft = 1,
            /// <summary>
            /// 合同發起簽署
            /// </summary>
            Launch = 2,
            /// <summary>
            /// 合同失敗
            /// </summary>
            Fail = 3,
            /// <summary>
            /// 合同完成
            /// </summary>
            Complete = 4,
            /// <summary>
            /// 合同作廢
            /// </summary>
            Discard = 5
        }

        public enum downLoadStatus
        {
            /// <summary>
            /// 待下載,null
            /// </summary>
            Wait = 0,
            /// <summary>
            /// 下載失敗
            /// </summary>
            Fail = 1,
            /// <summary>
            /// 下載成功
            /// </summary>
            Success = 2,
            /// <summary>
            /// 解壓成功
            /// </summary>
            UnZipSuccess = 3
        }
    }
}
