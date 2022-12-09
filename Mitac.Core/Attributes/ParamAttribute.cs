using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Mitac.Core.Attributes
{
    public class ParamAttribute:Attribute
    {
        public string Remark;  //{ get; set; }
        public ParamAttribute(string remark)
        {
            Remark = remark;
        }
    }
}
