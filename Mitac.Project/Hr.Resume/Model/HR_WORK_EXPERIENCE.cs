using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_WORK_EXPERIENCE")]
    public partial class HR_WORK_EXPERIENCE
    {
        public HR_WORK_EXPERIENCE()
        {


        }
        /// <summary>
        /// Desc:面試者id
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "PERSON_ID")]
        public decimal person_id { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(IsPrimaryKey = true, ColumnName = "WORK_ID", OracleSequenceName = "HR_WORK_EXPERIENCE_SEQ")]
        public decimal work_id { get; set; }

        /// <summary>
        /// Desc:公司
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "COMPANY")]
        public string company { get; set; }

        /// <summary>
        /// Desc:職務
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "DUTY")]
        public string duty { get; set; }

        /// <summary>
        /// Desc:工作開始時間
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "WORK_FROMDATE")]
        public DateTime work_fromdate { get; set; }

        /// <summary>
        /// Desc:工作結束時間
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "WORK_TODATE")]
        public DateTime work_todate { get; set; }

        /// <summary>
        /// Desc:薪資
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "SALARY")]
        public string salary { get; set; }

        /// <summary>
        /// Desc:離職原因
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "LEAVE_REASON")]
        public string leave_reason { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "MANAGER_NAME")]
        public string manager_name { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "TEL")]
        public decimal tel { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "ATTRIBUTE1")]
        public string attribute1 { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "ATTRIBUTE2")]

        public string attribute2 { get; set; }
        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "ATTRIBUTE3")]
        public string attribute3 { get; set; }

    }
}
