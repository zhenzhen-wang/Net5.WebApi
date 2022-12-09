using Mitac.Core.Attributes;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Hr.Contract.Model
{
    public class CONTRACT_PARAMS
    {
        [Param("只是这个属性的备注")]
        public string EMPNO { get; set; }
        /// <summary>
        /// 直間接，D/I
        /// </summary>
        public string TYPE { get; set; }
        public string NAME { get; set; }
        public string SEX { get; set; }
        public string ID_CARD { get; set; }
        public string ADDRESS { get; set; }
        public string PHONE_NUMBER { get; set; }

        /// <summary>
        /// 合同类型：A/B
        /// </summary>
        public string CONTRACT_TYPE { get; set; }

        /// <summary>
        /// 工作类型：生产...
        /// </summary>
        public string WORK_TYPE { get; set; }

        public int COMPANY_ID { get; set; }
        public string COMPANY { get; set; }

        /// <summary>
        /// 是否为续签，续签是Y，新进则是N
        /// </summary>
        public string EXTENSION { get; set; }

        /// <summary>
        /// 固定期限勞動合同開始結束時間
        /// </summary>
        public string CONTRACT_ST_YEAR { get; set; }
        public string CONTRACT_ST_MONTH { get; set; }
        public string CONTRACT_ST_DAY { get; set; }
        public string CONTRACT_END_YEAR { get; set; }
        public string CONTRACT_END_MONTH { get; set; }
        public string CONTRACT_END_DAY { get; set; }

        public string PROBATION_ST_YEAR { get; set; }
        public string PROBATION_ST_MONTH { get; set; }
        public string PROBATION_ST_DAY { get; set; }
        public string PROBATION_END_YEAR { get; set; }
        public string PROBATION_END_MONTH { get; set; }
        public string PROBATION_END_DAY { get; set; }

        /// <summary>
        /// 永久合同，無固定期限勞動合同開始時間
        /// </summary>
        public string CONTRACT_ST_YEAR_P { get; set; }
        public string CONTRACT_ST_MONTH_P { get; set; }
        public string CONTRACT_ST_DAY_P { get; set; }
        /// <summary>
        /// 纸质合同人员重签电子合同，是则'Y'
        /// </summary>
        public string RESIGN { get; set; }
    }
}
