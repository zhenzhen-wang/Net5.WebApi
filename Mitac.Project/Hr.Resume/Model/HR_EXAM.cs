using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_EXAM")]
    public partial class HR_EXAM
    {
           public HR_EXAM(){


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
           [SugarColumn(IsPrimaryKey=true,ColumnName="EXAM_ID", OracleSequenceName = "HR_EXAM_SEQ")]
           public decimal exam_id {get;set;}

           /// <summary>
           /// Desc:試題代碼
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="LOOKUP_CODE")]
           public string lookup_code {get;set;}

           /// <summary>
           /// Desc:面試者填寫結果
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="RESULT")]
           public string result {get;set;}

           /// <summary>
           /// Desc:得分
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="SCORE")]
           public decimal score {get;set;}

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
