using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_EDUCATION")]
    public partial class HR_EDUCATION
    {
           public HR_EDUCATION(){


           }
           /// <summary>
           /// Desc:面試者id
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
           [SugarColumn(IsPrimaryKey=true,ColumnName="EDUCATION_ID",OracleSequenceName = "HR_EDUCATION_SEQ")]
           public decimal education_id {get;set;}

           /// <summary>
           /// Desc:學歷
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="EDUCATION")]
           public string education {get;set;}

           /// <summary>
           /// Desc:畢業學校
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="SCHOOL")]
           public string school {get;set;}

           /// <summary>
           /// Desc:專業
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="MAJOR")]
           public string major {get;set;}

           /// <summary>
           /// Desc:入學時間
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="ENROLLMENT_DATE")]
           public DateTime enrollment_date {get;set;}

           /// <summary>
           /// Desc:畢業時間
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="GRADUATION_DATE")]
           public DateTime graduation_date {get;set;}

           /// <summary>
           /// Desc:是否畢業
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="GRADUATED")]
           public string graduated {get;set;}

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
