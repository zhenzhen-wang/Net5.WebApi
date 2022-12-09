using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using Newtonsoft.Json.Linq;
using Hr.Resume.Model;
using System.Data;

namespace Hr.Resume.IService
{
    public interface IDbRepository
    {
        public List<HR_BASIC_INFO> GetBaseInfoByIdStatus(string idCardNo, string status);
        public Dictionary<string, object> GetDetailByIdStatus(string idCardNo, string status);
        public List<SearchEmployee> GetEmployeeList(SearchEmployee list);
        public HR_MANAGER_COMMENT GetManagerCommentByIdCard(string idCardNo);
        public List<string> GetParameters(string lookupType, string type);
        public string InsertHrResume(JObject obj);
        public string GetBlackInfo(string idCardNo);
        public string UpdateStatus(UpdateParams updateParams);
        public bool IsInfoConfirmed(string idCardNo, string v);
        public string InsertComment(ManagerParams param);
        public string InsertResult(HrConfirmParams param);
        public bool IsUnderAge(string idCardNo);
        public DataTable GetInterviewData(string version);
        public string UpdateHrResume(JObject form);
    }
}