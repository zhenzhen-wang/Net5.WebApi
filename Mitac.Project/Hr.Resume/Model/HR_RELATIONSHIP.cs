using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Resume.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("HR_RELATIONSHIP")]
    public partial class HR_RELATIONSHIP
    {
           public HR_RELATIONSHIP(){


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
           [SugarColumn(IsPrimaryKey=true,ColumnName="RELATIONSHIP_ID", OracleSequenceName = "HR_RELATIONSHIP_SEQ")]
           public decimal relationship_id {get;set;}

           /// <summary>
           /// Desc:三等親姓名
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="NAME")]
           public string name {get;set;}

           /// <summary>
           /// Desc:所在集團公司名
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="COMPANY")]
           public string company {get;set;}

           /// <summary>
           /// Desc:關係
           /// Default:
           /// Nullable:False
           /// </summary>           
           [SugarColumn(ColumnName="RELATIONSHIP")]
           public string relationship {get;set;}

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
