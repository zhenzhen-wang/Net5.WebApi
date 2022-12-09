using Hangfire;
using Hr.Contract.Model;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Utilities;
using QiyuesuoClient.Http;
using QiyuesuoSDK.Bean.Request;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.IO;
using System.Security.Principal;

namespace Hr.Contract.BackgroundTask
{
    public class ContractDownload
    {
        private string rootDirPath = "";
        private string zipfilePath = "";
        private List<MHT_QYS_CONTRACTS_ALL> infoList = null;

        private ILogger<ContractDownload> _logger;
        private readonly AppSettings _appSettings;
        private readonly ISqlSugarClient _client;

        public ContractDownload(ILogger<ContractDownload> logger, AppSettings appSettings, ISqlSugarClient client)
        {
            _logger = logger;
            _appSettings = appSettings;
            _client = client;
        }

        /// <summary>
        /// job主函数入口
        /// </summary>
        [AutomaticRetry(Attempts = 3, OnAttemptsExceeded = AttemptsExceededAction.Delete)]
        public void Execute()
        {
            _logger.LogInformation("Download_Job開始運行......");

            // 1.查询合同'签核完成',且'待下载'人员
            infoList = GetDownloadList(((int)Enums.contractStatus.Complete).ToString(),
                ((int)Enums.downLoadStatus.Wait).ToString());

            //循环依次下载合同并解压
            foreach (var item in infoList)
            {
                #region 模拟AD域用户执行
                try
                {
                    using (WindowsLogin wi = new WindowsLogin("mkl_hr_treaty", "mitacad", "Hello@123"))
                    {
                        WindowsIdentity.RunImpersonated(wi.Identity.AccessToken, () =>
                        {//WindowsIdentity.GetCurrent().Name 模拟用户名

                            //2.串出契约锁下载文件根目录路径及文件路径
                            GetFilePath(item, out rootDirPath, out zipfilePath);

                            //3.下载合同
                            int result = DownloadContract(item, zipfilePath, rootDirPath);

                            //4. 解压合同
                            UnzipContract(result, item, zipfilePath);
                        });

                    }
                }
                catch (Exception e)
                {
                    _logger.LogError($"模拟AD域用户执行出错：{e.Message}");
                }
                #endregion
            }
        }

        /// <summary>
        /// 查询原始表中的status为4且download_status为0的人员
        /// </summary>
        /// <param name="status"></param>
        /// <param name="downloadStatus"></param>
        /// <returns></returns>
        public List<MHT_QYS_CONTRACTS_ALL> GetDownloadList(string status, string downloadStatus)
        {
            try
            {
                infoList = _client.Queryable<MHT_QYS_CONTRACTS_ALL>()
                            .Where(it => it.STATUS == status
                            && it.DOWNLOAD_STATUS == downloadStatus).ToList();
                //&& SqlFunc.EqualsNull(it.DOWNLOAD_STATUS, downloadStatus)                
            }
            catch (Exception e)
            {
                _logger.LogError($"查询数据库异常：{e.Message}");
            }
            return infoList;
        }

        /// <summary>
        /// 串出契约锁下载文件根路径及文件路径
        /// </summary>
        /// <param name="item"></param>
        /// <param name="rootDirPath"></param>
        /// <param name="zipfilePath"></param>
        public void GetFilePath(MHT_QYS_CONTRACTS_ALL item, out string rootDirPath, out string zipfilePath)
        {
            try
            {
                //判断是否存在文件存放的rootpath，没有则新建
                rootDirPath = _appSettings.DownloadPath + item.COMPANY + "\\"
                    + item.CONTRACT_ST_DATE.Substring(0, 4) + "\\" + item.EMPNO + "_"
                    + item.NAME + "_" + DateTime.Parse(item.CONTRACT_ST_DATE).ToString("yyyyMMdd");
                if (!Directory.Exists(rootDirPath))
                {
                    Directory.CreateDirectory(rootDirPath);
                }

                //合同保存路径
                zipfilePath = rootDirPath + "\\" + item.EMPNO + "_" + item.NAME + "_"
                    + DateTime.Parse(item.CONTRACT_ST_DATE).ToString("yyyyMMdd") + ".zip";
                if (File.Exists(zipfilePath))
                {
                    File.Delete(zipfilePath);
                }
            }
            catch (Exception e)
            {
                _logger.LogError($"新建合同路径出错：{e.Message}");
                rootDirPath = "";
                zipfilePath = "";
            }
        }

        /// <summary>
        /// 下载合同
        /// </summary>
        /// <param name="item"></param>
        /// <param name="zipfilePath"></param>
        /// <returns></returns>
        public int DownloadContract(MHT_QYS_CONTRACTS_ALL item, string zipfilePath, string rootDirPath)
        {
            int result = -1;
            string msg = "";
            try
            {
                // 调取不同公司的apptoken下载合同
                SDKClient client = new SDKClient(GetAccessKey(item.COMPANY_ID), GetAccessSecret(item.COMPANY_ID), _appSettings.QiYueSuoUrl);
                Stream stream = new FileStream(zipfilePath, FileMode.Create);
                ContractDownloadRequest request = new ContractDownloadRequest();
                request.ContractId = item.CONTRACT_ID;
                //发起合同下载请求
                client.Download(request, ref stream);

                stream.Close();
                stream.Dispose(); //释放资源,否则无法解压
                try
                {
                    // 回填合同狀態
                    result = _client.Updateable<MHT_QYS_CONTRACTS_ALL>()
                                    .SetColumns(it => it.DOWNLOAD_PATH == rootDirPath) //下载保存路径
                                    .SetColumns(it => it.DOWNLOAD_STATUS == ((int)Enums.downLoadStatus.Success).ToString()) //"2",下载成功
                                    .SetColumns(it => it.LAST_UPDATED_BY == "DownloadJob")
                                    .SetColumns(it => it.LAST_UPDATE_DATE == DateTime.Now)
                                    .Where(it => it.CONTRACT_ID == item.CONTRACT_ID)
                                    .ExecuteCommand();
                    _logger.LogInformation($"{item.EMPNO} 合同下载成功");
                }
                catch (Exception e)
                {
                    msg = $"{item.EMPNO}：{item.PHONE_NUMBER}的合同下载保存成功，但是状态回調失敗：{e.Message}";
                }
            }
            catch (Exception e)
            {
                msg = $"{item.EMPNO}的合同下载时出现异常：{e.Message}";
            }
            if (result < 0)
            {
                _logger.LogError(msg);
                // 邮件提醒
                MailSend.SendMailToDeveloper(msg, _appSettings.DevelopMailAddress);
            }
            return result;
        }

        /// <summary>
        /// 解压合同
        /// </summary>
        /// <param name="downloadFlag"></param>
        /// <param name="item"></param>
        /// <param name="zipfilePath"></param>
        /// <returns></returns>
        public int UnzipContract(int downloadFlag, MHT_QYS_CONTRACTS_ALL item, string zipfilePath)
        {
            int result = -1;
            //下载成功，则开始解压
            if (downloadFlag > 0)
            {
                try
                {
                    List<FileInfo> listFile = new List<FileInfo>();
                    listFile = HandleFile.UnZipFile(zipfilePath);
                    if (listFile.Count <= 0)
                    {
                        _logger.LogInformation($"{item.EMPNO}合同解压文件为空，请检查!");
                        MailSend.SendMailToDeveloper($"{item.EMPNO}合同解压文件为空，请检查!", _appSettings.DevelopMailAddress);
                    }
                    else
                    {
                        // 回填合同狀態
                        result = _client.Updateable<MHT_QYS_CONTRACTS_ALL>()
                                        .SetColumns(it => it.DOWNLOAD_STATUS == ((int)Enums.downLoadStatus.UnZipSuccess).ToString()) //"3",解压成功
                                        .SetColumns(it => it.LAST_UPDATE_DATE == DateTime.Now)
                                        .Where(it => it.CONTRACT_ID == item.CONTRACT_ID)
                                        .ExecuteCommand();
                    }
                    //删除压缩档，只保留压缩后的文件
                    //File.Delete(zipfilePath);

                    _logger.LogInformation($"{item.EMPNO} 合同解压成功");
                }
                catch (Exception e)
                {
                    string msg = $"{item.EMPNO}：{item.PHONE_NUMBER}的合同解压失敗：{e.Message}";
                    _logger.LogError(msg);
                    // 邮件提醒
                    //MailSend.SendMailToDeveloper(msg, _appSettings.DevelopMailAddress);
                }
            }
            return result;
        }

        /// <summary>
        /// 获取契约锁不同公司别的accessKey
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public string GetAccessKey(int companyId)
        {
            string accessKey = _client.Ado.GetDataTable(
                "SELECT TOKEN FROM CONTRACT_SECRET_TOKEN WHERE COMPANY_ID=@COMPANY_ID",
                new { COMPANY_ID = companyId }).Rows[0][0].ToString();
            return accessKey;
        }

        /// <summary>
        /// 获取契约锁不同公司别的accessSecret
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public string GetAccessSecret(int companyId)
        {
            string accessSecret = _client.Ado.GetDataTable(
                "SELECT SECRET FROM CONTRACT_SECRET_TOKEN WHERE COMPANY_ID=@COMPANY_ID",
                new { COMPANY_ID = companyId }).Rows[0][0].ToString();
            return accessSecret;
        }

    }
}
