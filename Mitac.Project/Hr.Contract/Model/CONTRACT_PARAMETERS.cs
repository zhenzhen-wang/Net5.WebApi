using System;
using System.Linq;
using System.Text;
using SqlSugar;

namespace Hr.Contract.Model
{
    ///<summary>
    ///
    ///</summary>
    [SugarTable("CONTRACT_PARAMETERS")]
    public partial class CONTRACT_PARAMETERS
    {
           public CONTRACT_PARAMETERS(){


           }
           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal COMPANY_ID {get;set;}

           /// <summary>
           /// Desc:是否是在职人员重签合同
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string RESIGN {get;set;}

           /// <summary>
           /// Desc:是否是续签
           /// Default:
           /// Nullable:False
           /// </summary>           
           public string EXTENSION {get;set;}

           /// <summary>
           /// Desc:
           /// Default:
           /// Nullable:False
           /// </summary>           
           public decimal CATEGORY_ID {get;set;}

    }
}
