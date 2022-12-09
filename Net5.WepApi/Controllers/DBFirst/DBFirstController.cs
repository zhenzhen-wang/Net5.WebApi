using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Mitac.Net5.WepApi.Controllers.DBFirst
{
    [Route("api/[controller]")]
    [ApiController]
    public class DBFirstController : Controller
    {
        private readonly ISqlSugarClient _client;
        public DBFirstController(ISqlSugarClient client)
        {
            this._client = client;
        }

        //directoryPath: "D:/WorkFolder/bonnie.wang/MKL_NewProj/Mobile/Mitac.Net5.WepApi/Mitac.Project/Hr.Resume/Model"
        //nameSpace: "Hr.Resume.Model"
        [HttpGet]
        public ActionResult<string> GetModels(string directoryPath, string nameSpace)
        {
            try
            {
                foreach (var item in _client.DbMaintenance.GetTableInfoList())
                {
                    string entityName = item.Name.ToUpper();/*实体名大写*/
                    _client.MappingTables.Add(entityName, item.Name);
                    foreach (var col in _client.DbMaintenance.GetColumnInfosByTableName(item.Name))
                    {
                        _client.MappingColumns.Add(col.DbColumnName.ToLower() /*类的属性小写*/, col.DbColumnName, entityName);
                    }
                }
                _client.DbFirst.Where(it => it.ToUpper().StartsWith("HR"))
                    .IsCreateAttribute().CreateClassFile(directoryPath, nameSpace);
            }
            catch (Exception exp)
            {
                return Ok(exp.ToString());
            }
            return Ok("Success");


            //_client.DbFirst.Where(it => it.ToUpper().StartsWith("HR")).IsCreateAttribute()
            //.CreateClassFile("D:/WorkFolder/bonnie.wang/MKL_NewProj/Mobile/Mitac.Net5.WepApi/Mitac.Project/Hr.Resume/Model", "Hr.Resume.Model");         
        }

    }
}
