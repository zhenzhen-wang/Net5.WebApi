using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_FAMILY_INFO")]
    public partial class HR_FAMILY_INFO
    {
           public HR_FAMILY_INFO(){


           }
           /// <summary>
           /// Desc:面試者唯一id
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="PERSON_ID")]
           public decimal person_id {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(IsPrimaryKey=true,ColumnName="FAMILY_ID", OracleSequenceName = "HR_FAMILY_INFO_SEQ")]
           public decimal family_id {get;set;}

           /// <summary>
           /// Desc:親屬姓名
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="NAME")]
           public string name {get;set;}

           /// <summary>
           /// Desc:親屬性別
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="SEX")]
           public string sex {get;set;}

           /// <summary>
           /// Desc:與親屬關係
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="KINSHIP")]
           public string kinship {get;set;}

           /// <summary>
           /// Desc:親屬工作
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="JOB")]
           public string job {get;set;}

           /// <summary>
           /// Desc:親屬聯繫號碼
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="TEL")]
           public decimal tel {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ATTRIBUTE1")]
           public string attribute1 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ATTRIBUTE2")]
           public string attribute2 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ATTRIBUTE3")]
           public string attribute3 {get;set;}

    }
}
