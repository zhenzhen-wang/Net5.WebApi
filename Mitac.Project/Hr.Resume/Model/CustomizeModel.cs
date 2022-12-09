using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Hr.Resume.Model
{
    public class CardType
    {
        public string side { get; set; }
        public string base64Img { get; set; }
    }

    public class SearchEmployee
    {
        public decimal person_id { get; set; }
        public string name { get; set; }   //姓名
        public string id_card_no { get; set; }
        public string status { get; set; }
        public string telphone { get; set; }
        public string salary_type { get; set; }//工种
        public string work_type { get; set; }
        public string batch_no { get; set; }
        public string idCardLast4 { get; set; }
        public DateTime? creation_date { get; set; }
    }

    public class ManagerParams
    {
        public string[] id_card_no { get; set; }
        public string manager_empno { get; set; }
        public string company { get; set; }
        public string add_work_year { get; set; }
        public string dept_name { get; set; }
        public string character { get; set; }
        public string english_talent { get; set; }
        public string computer_talent { get; set; }
        public string work_experience { get; set; }
        public string profession_talent { get; set; }
        public string manage_talent { get; set; }
        public string status { get; set; }
    }

    public class HrConfirmParams
    {
        public string id_card_no { get; set; }
        public string hr_empno { get; set; }
        public string company { get; set; }
        public string add_work_year { get; set; }
        public string dept_name { get; set; }
        public decimal? out_work_year { get; set; }
        public string job_appellation { get; set; }
        public string job_degree { get; set; }
        public DateTime? arrival_date { get; set; }
    }

    public class UpdateParams
    {
        public string[] idCardNoList { get; set; }
        public string status { get; set; }
        public bool delete { get; set; }
        public string empno { get; set; }
    }

    public class PS_EMPLOYEE
    {
        public string EMPNO { get; set; }
        public string NAME { get; set; }
        public string ID { get; set; }
        public string STATUS { get; set; }
        public string SITE { get; set; }
        public string SALARY_TYPE { get; set; }

    }

    public class NameList
    {
        public string name { get; set; }   //姓名
        public string salary_type { get; set; }//工种
        public string id_card_no { get; set; } //身份证号
        public DateTime creation_date { get; set; }
        public string batch_no { get; set; }
        public string idCardLast4 { get; set; }
        public string is_checked { get; set; } = "";     //是否选中

        //private string _black_info;
        //public string black_info   //黑名单信息
        //{
        //    get { if (_black_info.Contains("mhr_msl_ps_employee_hei")) { return "此人在黑名单"; } else { return ""; }; }
        //    set { if (value.ToString() == "") { _black_info = ""; } else { _black_info = value; } }
        //}
    }

    public class BlacklistInfo
    {
        public Boolean isEligible { get; set; } //符合条件
        public string blackListMessage { get; set; }

    }

}