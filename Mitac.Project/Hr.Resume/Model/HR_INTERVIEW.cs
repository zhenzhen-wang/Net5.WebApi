using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_INTERVIEW")]
    public partial class HR_INTERVIEW
    {
           public HR_INTERVIEW(){


           }
           /// <summary>
           /// Desc:訪談錄版本
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="INTERVIEW_VERSION")]
           public string interview_version {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="INTERVIEW_ID", OracleSequenceName = "HR_INTERVIEW_SEQ")]
           public string interview_id {get;set;}

           /// <summary>
           /// Desc:訪談問題描述
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="DESCRIPTION")]
           public string description {get;set;}

           /// <summary>
           /// Desc:訪談結果
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="RESULT")]
           public string result {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="CREATION_DATE")]
           public DateTime? creation_date {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="CREATED_BY")]
           public string created_by {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="LAST_UPDATE_DATE")]
           public DateTime? last_update_date {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="LAST_UPDATED_BY")]
           public string last_updated_by {get;set;}

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

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ATTRIBUTE4")]
           public string attribute4 {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:True
           /// </summary>           
           [SugarColumn(ColumnName="ATTRIBUTE5")]
           public string attribute5 {get;set;}

    }
}
