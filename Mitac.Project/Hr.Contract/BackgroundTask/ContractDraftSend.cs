using Hangfire;
using Hr.Contract.Model;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Filters;
using Mitac.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using QiyuesuoClient.Http;
using QiyuesuoSDK.Bean;
using QiyuesuoSDK.Bean.Request;
using QiyuesuoSDK.Bean.Response;
using QiyuesuoSDK.Tools;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Data;
using System.Reflection;

namespace Hr.Contract.BackgroundTask
{
    public class ContractDraftSend
    {
        private List<MHT_QYS_CONTRACTS_ALL> infoList = null;
        private ILogger<ContractDraftSend> _logger;
        private readonly AppSettings _appSettings;
        private readonly ISqlSugarClient _client;

        public ContractDraftSend(ILogger<ContractDraftSend> logger, AppSettings appSettings, ISqlSugarClient client)
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
            _logger.LogInformation($"Send_Job開始運行......");

            // 1.找出等待发送电子合同的人员list
            infoList = GetSendList(((int)Enums.contractStatus.Wait).ToString());

            // 循环所有人员，依次发送电子合同
            foreach (var item in infoList)//？？用filter將所有log及異常獨立出來
            {
                try
                {
                    // 2.處理數據庫實體類，得出合同需要的實際數據
                    CONTRACT_PARAMS param = GetContractParams(item);

                    // 3.发起合同
                    SdkResponse<Object> response = ContractSendOneByOne(param);

                    // 4.回填合同状态
                    UpdateContract(response, param, item);
                }
                catch (Exception e)
                {
                    _logger.LogError($"{item.EMPNO}的合同草稿发起出现异常：{e}");
                }
            }
        }

        /// <summary>
        /// 查询原始表中的status为0的人员，即找出等待发送电子合同的人
        /// </summary>
        /// <param name="status"></param>
        /// <returns></returns>
        public List<MHT_QYS_CONTRACTS_ALL> GetSendList(string status)
        {
            try
            {
                infoList = _client.Queryable<MHT_QYS_CONTRACTS_ALL>()
                .Where(it => it.STATUS == status).ToList();
            }
            catch (Exception e)
            {
                _logger.LogError($"查询数据库异常：{e.Message}");
            }
            return infoList;
        }

        /// <summary>
        /// 處理數據庫實體類，得出合同需要的參數
        /// </summary>
        /// <param name="item"></param>
        /// <returns></returns>
        public CONTRACT_PARAMS GetContractParams(MHT_QYS_CONTRACTS_ALL item)
        {
            CONTRACT_PARAMS param = new CONTRACT_PARAMS();
            param.EMPNO = item.EMPNO;
            param.NAME = ConvertUtil.ChineseConvert(item.NAME, "1");
            param.SEX = item.SEX == "M" ? "男" : "女";
            param.ID_CARD = item.ID_CARD;
            param.ADDRESS = ConvertUtil.ChineseConvert(item.ADDRESS, "1");
            param.PHONE_NUMBER = item.PHONE_NUMBER;
            param.CONTRACT_TYPE = item.CONTRACT_TYPE;
            param.TYPE = item.TYPE;
            param.RESIGN = item.ATTRIBUTE1;
            param.WORK_TYPE = ConvertUtil.ChineseConvert(item.WORK_TYPE, "1");
            param.COMPANY = GetCompanynameByCompanyId(item.COMPANY_ID);
            param.COMPANY_ID = item.COMPANY_ID;
            param.EXTENSION = item.PROBATION_ST_DATE is null ? "Y" : "N";//根據是否有實習時間，判斷是續簽還是新進合同

            if (item.CONTRACT_TYPE == "A")//固定期限合同時間
            {
                //正式合同开始结束时间
                param.CONTRACT_ST_YEAR = Convert.ToDateTime(item.CONTRACT_ST_DATE).Year.ToString();
                param.CONTRACT_ST_MONTH = Convert.ToDateTime(item.CONTRACT_ST_DATE).Month.ToString();
                param.CONTRACT_ST_DAY = Convert.ToDateTime(item.CONTRACT_ST_DATE).Day.ToString();
                param.CONTRACT_END_YEAR = Convert.ToDateTime(item.CONTRACT_END_DATE).Year.ToString();
                param.CONTRACT_END_MONTH = Convert.ToDateTime(item.CONTRACT_END_DATE).Month.ToString();
                param.CONTRACT_END_DAY = Convert.ToDateTime(item.CONTRACT_END_DATE).Day.ToString();
                //试用期开始结束时间
                if (item.PROBATION_ST_DATE is not null)
                {
                    param.PROBATION_ST_YEAR = Convert.ToDateTime(item.PROBATION_ST_DATE).Year.ToString();
                    param.PROBATION_ST_MONTH = Convert.ToDateTime(item.PROBATION_ST_DATE).Month.ToString();
                    param.PROBATION_ST_DAY = Convert.ToDateTime(item.PROBATION_ST_DATE).Day.ToString();
                    param.PROBATION_END_YEAR = Convert.ToDateTime(item.PROBATION_END_DATE).Year.ToString();
                    param.PROBATION_END_MONTH = Convert.ToDateTime(item.PROBATION_END_DATE).Month.ToString();
                    param.PROBATION_END_DAY = Convert.ToDateTime(item.PROBATION_END_DATE).Day.ToString();
                }
            }
            else //無固定期限勞動合同
            {
                param.CONTRACT_ST_YEAR_P = Convert.ToDateTime(item.CONTRACT_ST_DATE).Year.ToString();
                param.CONTRACT_ST_MONTH_P = Convert.ToDateTime(item.CONTRACT_ST_DATE).Month.ToString();
                param.CONTRACT_ST_DAY_P = Convert.ToDateTime(item.CONTRACT_ST_DATE).Day.ToString();
            }

            return param;
        }

        /// <summary>
        /// 依次发送合同草稿
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public SdkResponse<Object> ContractSendOneByOne(CONTRACT_PARAMS param)
        {
            // 不同公司调用其对应的apptoken发起合同
            SDKClient sDKClient = new SDKClient(GetAccessKey(param.COMPANY_ID),
                GetAccessSecret(param.COMPANY_ID), _appSettings.QiYueSuoUrl);
            ContractDraftRequest contractDraftRequest = new ContractDraftRequest();
            QiyuesuoSDK.Bean.Contract contract = new QiyuesuoSDK.Bean.Contract();

            contract.Signatories = GetSignatory(param);
            contract.TemplateParams = GetTemplateParams(param);
            contract.Category = GetCategory(param);
            contract.Send = _appSettings.SendContract; //false指不发送，先生成草稿发送到契约锁平台（此草稿只有little可见）
            contract.Ordinal = "true";
            //contract.Subject = "昆达电脑合同测试1"; //已在业务逻辑中设定

            contractDraftRequest.Contract = contract;

            string response = null; ;
            try
            {
                response = sDKClient.Service(contractDraftRequest);
            }
            catch (Exception e)
            {
                _logger.LogError("调用契约锁接口，发起合同草稿過程中發生異常： " + e.ToString());
            }
            SdkResponse<Object> responseObject = HttpJsonConvert.DeserializeResponse<Object>(response);
            //Console.WriteLine(HttpJsonConvert.SerializeObject(responseObject));
            return responseObject;
        }

        /// <summary>
        /// 获取个人及公司签署方信息
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<Signatory> GetSignatory(CONTRACT_PARAMS param)
        {
            List<Signatory> signatories = new List<Signatory>();

            //签署方：个人
            Signatory personSignatory = new Signatory();
            personSignatory.TenantType = "PERSONAL";
            personSignatory.TenantName = param.NAME;
            User personReceiver = new User();
            personReceiver.Contact = param.PHONE_NUMBER;
            personReceiver.Name = param.NAME;
            personReceiver.ContactType = "MOBILE";
            if (param.PHONE_NUMBER.IndexOf('@') > -1)
            {
                personReceiver.ContactType = "EMAIL";
            }
            personSignatory.Receiver = personReceiver;
            personSignatory.DelaySet = false;
            personSignatory.SerialNo = 1; //先是个人签署，后公司签署
            signatories.Add(personSignatory);

            //签署方：公司
            Signatory companySignatory = new Signatory();
            companySignatory.TenantType = "COMPANY";
            companySignatory.TenantName = param.COMPANY;
            User companyReceiver = new User();
            companyReceiver.Contact = _appSettings.SignatoryPhone;//注册公司账号使用的个人账号信息
            companyReceiver.Name = _appSettings.SignatoryName;
            companyReceiver.ContactType = "MOBILE";
            companySignatory.Receiver = companyReceiver;
            companySignatory.DelaySet = false;
            companySignatory.SerialNo = 2;
            signatories.Add(companySignatory);

            return signatories;
        }

        /// <summary>
        /// 获取合同参数名及参数值
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public List<TemplateParam> GetTemplateParams(CONTRACT_PARAMS param)
        {
            List<TemplateParam> templateParams = new List<TemplateParam>();

            //反射出实体类的属性名和属性值
            PropertyInfo[] properties = param.GetType().GetProperties(BindingFlags.Instance | BindingFlags.Public);
            foreach (PropertyInfo item in properties)
            {
                if (item.GetValue(param) is not null)
                {
                    TemplateParam templateParam = new TemplateParam();
                    templateParam.Name = item.Name;
                    templateParam.Value = item.GetValue(param, null).ToString();
                    templateParams.Add(templateParam);
                }
            }

            return templateParams;
        }

        /// <summary>
        /// 获取业务类别id
        /// </summary>
        /// <param name="param"></param>
        /// <returns></returns>
        public Category GetCategory(CONTRACT_PARAMS param)
        {
            var categoryId = new CONTRACT_PARAMETERS();
            if (param.RESIGN is null)//空，则表示正常签核
            {
                categoryId = _client.Queryable<CONTRACT_PARAMETERS>()
               .First(it => it.COMPANY_ID == param.COMPANY_ID
                       && it.EXTENSION == param.EXTENSION);
            }
            else //不为空，则表示纸质合同人员重签电子合同
            {
                categoryId = _client.Queryable<CONTRACT_PARAMETERS>()
                .First(it => it.COMPANY_ID == param.COMPANY_ID
                        && it.RESIGN == param.RESIGN);
            }

            Category category = new Category();
            category.Id = categoryId.CATEGORY_ID.ToString();

            return category;
        }

        /// <summary>
        /// 根据公司别id获取契约锁注册公司名称
        /// </summary>
        /// <param name="companyId"></param>
        /// <returns></returns>
        public string GetCompanynameByCompanyId(int companyId)
        {
            string name = _client.Ado.GetDataTable(
                "SELECT COMPANY_NAME FROM CONTRACT_SECRET_TOKEN WHERE COMPANY_ID=@COMPANY_ID",
                new { COMPANY_ID = companyId }).Rows[0][0].ToString();
            return name;
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

        /// <summary>
        /// 回填合同状态
        /// </summary>
        /// <param name="response"></param>
        /// <param name="param"></param>
        /// <param name="item"></param>
        public void UpdateContract(SdkResponse<Object> response, CONTRACT_PARAMS param, MHT_QYS_CONTRACTS_ALL item)
        {
            if (response.Code == 0)
            {
                _logger.LogInformation($"{param.EMPNO}的合同草稿发起成功！");

                JObject result = (JObject)JsonConvert.DeserializeObject(response.Result.ToString());

                string contractId = result["id"].ToString();

                try
                {
                    // 回填合同狀態及contract_id
                    _client.Updateable<MHT_QYS_CONTRACTS_ALL>()
                            .SetColumns(it => it.CONTRACT_ID == contractId)
                            .SetColumns(it => it.DOWNLOAD_STATUS == ((int)Enums.downLoadStatus.Wait).ToString()) //'0'合同待下载
                            .SetColumns(it => it.STATUS == ((int)Enums.contractStatus.Draft).ToString()) //"1"更改狀態為已發送合同草稿到契约锁平台
                            .SetColumns(it => it.LAST_UPDATED_BY == "SendJob")
                            .SetColumns(it => it.LAST_UPDATE_DATE == DateTime.Now)
                            .Where(it => it.EMPNO == item.EMPNO
                             && it.CONTRACT_ST_DATE == item.CONTRACT_ST_DATE
                             && it.STATUS == "0") // 不可以仅仅用empno，一个人可能有多笔资料，如果用empno来update会更新掉之前的那份资料信息
                            .ExecuteCommand();
                }
                catch (Exception e)
                {
                    string msg = $"{param.EMPNO}：{param.PHONE_NUMBER}的合同草稿发起成功，但是資料回調失敗：{e.Message}";
                    // 狀態未回填成功
                    _logger.LogError(msg);
                    // 邮件提醒
                    MailSend.SendMailToDeveloper(msg, _appSettings.DevelopMailAddress);
                }
            }
            else
            {
                _logger.LogError($"{param.EMPNO}:调用契约锁api发起的合同草稿失敗：{response.Message}");
            }
        }
    }
}
