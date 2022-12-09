using System;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_ATTACHMENT")]
    public partial class HR_ATTACHMENT
    {
        public HR_ATTACHMENT()
        {


        }
        /// <summary>
        /// Desc:
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
        [SugarColumn(IsPrimaryKey = true, ColumnName = "ATTACH_ID", OracleSequenceName = "HR_ATTACHMENT_SEQ")]
        public decimal attach_id { get; set; }

        /// <summary>
        /// Desc:附件名稱
        /// Default:
        /// Nullable:False
        /// </summary>   
        [SugarColumn(ColumnName = "PIC_NAME")]
        public string pic_name { get; set; }

        /// <summary>
        /// Desc:附件base64 src
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "PIC_SRC")]
        public byte[] pic_src { get; set; }

        /// <summary>
        /// Desc:附件类型
        /// Default:
        /// Nullable:False
        /// </summary>           
        [SugarColumn(ColumnName = "FILE_TYPE")]
        public string file_type { get; set; }
        

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "LAST_UPDATE_DATE")]
        public DateTime? last_update_date { get; set; }

        /// <summary>
        /// Desc:
        /// Default:
        /// Nullable:True
        /// </summary>           
        [SugarColumn(ColumnName = "LAST_UPDATED_BY")]
        public string last_updated_by { get; set; }

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
