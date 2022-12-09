using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Contract.Model
{
    public class MHT_QYS_CONTRACTS_ALL
    {
        public string PERSON_ID { get; set; }
        public string EMPNO { get; set; }
        public string TYPE { get; set; }
        public string NAME { get; set; }
        public string SEX { get; set; }
        public string ID_CARD { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }
        public string CONTRACT_TYPE { get; set; }

        public string CONTRACT_ST_DATE { get; set; }
        //{
        //    get { return CONTRACT_ST_YEAR + "-" + CONTRACT_ST_MONTH + "-" + CONTRACT_ST_DATE; }
        //    set
        //    {
        //        if (value == null)
        //        {
        //            CONTRACT_ST_YEAR = string.Empty;
        //            CONTRACT_ST_MONTH = string.Empty;
        //            CONTRACT_ST_DAY = string.Empty;
        //        }
        //        else
        //        {
        //            CONTRACT_ST_YEAR = Convert.ToDateTime(value).ToString("yyyy");
        //            CONTRACT_ST_MONTH = Convert.ToDateTime(value).Month.ToString();
        //            CONTRACT_ST_DAY = Convert.ToDateTime(value).Day.ToString();
        //        }
        //    }
        //}

        public string CONTRACT_END_DATE { get; set; }
        public string PROBATION_ST_DATE { get; set; }
        public string PROBATION_END_DATE { get; set; }

        public string WORK_TYPE { get; set; }
        public string COMPANY { get; set; }
        public int COMPANY_ID { get; set; }
        public string STATUS { get; set; }
        public string STATUS_TYPE { get; set; }
        public string CONTRACT_ID { get; set; }
        public string DOWNLOAD_STATUS { get; set; }
        public string DOWNLOAD_URL { get; set; }
        public string DOWNLOAD_PATH { get; set; }
        public DateTime CREATION_DATE { get; set; }
        public string CREATED_BY { get; set; }
        public DateTime LAST_UPDATE_DATE { get; set; }
        public string LAST_UPDATED_BY { get; set; }
        public string LAST_UPDATE_LOGIN { get; set; }
        public string ATTRIBUTE1 { get; set; }
    }
}
