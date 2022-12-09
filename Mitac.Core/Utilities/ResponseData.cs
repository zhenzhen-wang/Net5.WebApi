using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Mitac.Core.Utilities.Enums;

namespace Mitac.Core.Utilities
{
    public class ResponseData<T>
    {
        public ResultStatus code { get; set; } = ResultStatus.Success;
        public string message { get; set; }
        public T result { get; set; }

        //public string type { get; set; } //'success' | 'error' | 'warning';
    }

    public class TokenInfo
    {
        public string refresh_token { get; set; }
        public string expires_in { get; set; }
        public string scope { get; set; }
        public string session_key { get; set; }
        public string access_token { get; set; }
        public string session_secret { get; set; }
    }
}
