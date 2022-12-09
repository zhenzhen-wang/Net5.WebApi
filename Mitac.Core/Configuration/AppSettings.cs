namespace Mitac.Core.Configuration
{
    public class AppSettings
    {
        public BaiduSecret baiduSecret { get; set; }
        
        public Secret secret { get; set; }

        public DbConStrings dbConStrings { get; set; }

        public string OracleDev
        {
            get { return dbConStrings.OracleDev; }
        }

        public string OracleProd
        {
            get { return dbConStrings.OracleProd; }
        }

        public string QiYueSuoUrl { get; set; }

        public string SignatoryPhone { get; set; }
        public string SignatoryName { get; set; }

        public string corsUrls { get; set; }
        public bool BackgroundTaskEnabled { get; set; }
        public string ContractDownload_Cron { get; set; }
        public string ContractDraftSend_Cron { get; set; }
        public string SMTP { get; set; }
        public string DevelopMailAddress { get; set; }
        public bool SendContract { get; set; }
        public string DownloadPath { get; set; }
    }

    /// <summary>d
    /// 加密对应密钥Key
    /// </summary>
    public class Secret
    {
        /// <summary>
        /// jwt加密key
        /// </summary>
        public string JWT { get; set; }

        public string Audience { get; set; }
        public string Issuer { get; set; }
    }

    /// <summary>
    /// 数据库连接字符串
    /// </summary>
    public class DbConStrings
    {
        public string OracleDev { get; set; }
        public string OracleProd { get; set; }
        public string LiteDB { get; set; }
        public string Redis { get; set; }
    }

    public class BaiduSecret
    {
        public string ClientId { get; set; }
        public string ClientSecret { get; set; }
    }
}
