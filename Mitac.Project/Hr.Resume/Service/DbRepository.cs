using Hr.Resume.IService;
using Hr.Resume.Model;
using Microsoft.Extensions.Logging;
using Mitac.Core.Configuration;
using Mitac.Core.Utilities;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Oracle.ManagedDataAccess.Client;
using SqlSugar;
using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Configuration;
using System.Data;
using System.Linq;

namespace Hr.Resume.Service
{
    public class DbRepository : IDbRepository
    {
        private readonly AppSettings _appSettings;
        private readonly ISqlSugarClient _client;
        private readonly ILogger<DbRepository> _logger;

        /// <summary>
        /// 构造函数
        /// </summary>
        /// <param name="appSettings"></param>
        /// <param name="client"></param>
        public DbRepository(AppSettings appSettings, ISqlSugarClient client, ILogger<DbRepository> logger)
        {
            _appSettings = appSettings;
            _client = client;
            _logger = logger;
        }

        /// <summary>
        /// 保存个人简历信息
        /// </summary>
        /// <param name="formData"></param>
        /// <param name="person_id"></param>
        /// <returns></returns>
        public string InsertHrResume(JObject form)
        {
            try
            {
                //errMsg = ModelValidate(hrBaseInfo);
                //if (!string.IsNullOrEmpty(errMsg)) return errMsg;                

                //解析前台传过来的form表单
                JObject obj = (JObject)JsonConvert.DeserializeObject(form["formData"].ToString());
                HR_BASIC_INFO hrBaseInfo = JsonConvert.DeserializeObject<HR_BASIC_INFO>(obj["hr_base_info"].ToString());

                // 如果该人员资料已经经过HR确认，则无法再次修改
                if (IsInfoConfirmed(hrBaseInfo.id_card_no, "0")) return "资料已经经过HR确认，无法再次提交";

                //事务开始
                _client.Ado.BeginTran();

                //1.主表插入
                HR_BASIC_INFO hrBaseInfoOld = _client.Queryable<HR_BASIC_INFO>().Where(it => it.id_card_no == hrBaseInfo.id_card_no).First();
                if (hrBaseInfoOld != null)//如果已经存在旧人事资料，先删除。
                {
                    _client.Deleteable<HR_BASIC_INFO>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_FAMILY_INFO>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_EDUCATION>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_RELATIONSHIP>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_WORK_EXPERIENCE>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_ATTACHMENT>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_EXAM>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_STATUS_INFO>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_HRCONFIRM>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                    _client.Deleteable<HR_MANAGER_COMMENT>().Where(it => it.person_id == hrBaseInfoOld.person_id).ExecuteCommand();
                }
                hrBaseInfo.created_by = hrBaseInfo.name;
                hrBaseInfo.last_updated_by = hrBaseInfo.name;
                hrBaseInfo.creation_date = DateTime.Now;
                hrBaseInfo.last_update_date = DateTime.Now;
                decimal person_id = _client.Insertable(hrBaseInfo).ExecuteReturnBigIdentity();

                //2.家庭成员信息插入
                List<HR_FAMILY_INFO> hrFamilyInfo = JsonConvert.DeserializeObject<List<HR_FAMILY_INFO>>(obj["hr_family_info"].ToString());
                if (hrFamilyInfo.Count > 0)
                {
                    foreach (var item in hrFamilyInfo)
                    {
                        item.person_id = person_id;
                    }
                    _client.Insertable(hrFamilyInfo.ToArray()).ExecuteCommand();
                }

                //3.教育信息插入
                List<HR_EDUCATION> hrEducation = JsonConvert.DeserializeObject<List<HR_EDUCATION>>(obj["hr_education"].ToString());
                if (hrEducation.Count > 0)
                {
                    foreach (var item in hrEducation)
                    {
                        item.person_id = person_id;
                    }
                    _client.Insertable(hrEducation.ToArray()).ExecuteCommand();
                }

                //4.三等亲家属信息插入
                List<HR_RELATIONSHIP> hrRelationship = JsonConvert.DeserializeObject<List<HR_RELATIONSHIP>>(obj["hr_relationship"].ToString());
                if (hrRelationship.Count > 0)
                {
                    foreach (var item in hrRelationship)
                    {
                        item.person_id = person_id;
                    }
                    _client.Insertable(hrRelationship.ToArray()).ExecuteCommand();
                }

                //5.工作经验信息插入
                List<HR_WORK_EXPERIENCE> hrWorkExperience = JsonConvert.DeserializeObject<List<HR_WORK_EXPERIENCE>>(obj["hr_work_experience"].ToString());
                if (hrWorkExperience.Count > 0)
                {
                    foreach (var item in hrWorkExperience)
                    {
                        item.person_id = person_id;
                    }
                    _client.Insertable(hrWorkExperience.ToArray()).ExecuteCommand();
                }

                //6.附件图片信息插入
                JArray hrAttachment = (JArray)JsonConvert.DeserializeObject(obj["hr_attachment"].ToString());
                //处理pic_src，去掉前缀"data:image/jpg;base64,"
                for (var i = 0; i < hrAttachment.Count; i++)
                {
                    string base64data = hrAttachment[i]["pic_src"].ToString();
                    String base64 = base64data.Substring(base64data.IndexOf(",") + 1);
                    hrAttachment[i]["pic_src"] = Convert.FromBase64String(base64);
                }
                List<HR_ATTACHMENT> hrAttachmentNew = (List<HR_ATTACHMENT>)hrAttachment.ToObject(typeof(List<HR_ATTACHMENT>));
                foreach (var item in hrAttachmentNew)//循环插入每个图片
                {
                    var dt = _client.Ado.GetDataTable(@"
                                INSERT INTO HR_ATTACHMENT
                                    (PERSON_ID, ATTACH_ID, PIC_NAME, PIC_SRC,FILE_TYPE,LAST_UPDATED_BY,LAST_UPDATE_DATE)
                                VALUES
                                    (@PERSON_ID, HR_ATTACHMENT_SEQ.NEXTVAL, @PIC_NAME, @PIC_SRC, @FILE_TYPE,
                                        @LAST_UPDATED_BY,@LAST_UPDATE_DATE)
                                ", new List<SugarParameter>(){
                  new SugarParameter("@PERSON_ID",person_id),
                  new SugarParameter("@PIC_SRC",item.pic_src),
                  new SugarParameter("@PIC_NAME",item.pic_name),
                  new SugarParameter("@FILE_TYPE",item.file_type),
                  new SugarParameter("@LAST_UPDATED_BY",hrBaseInfo.name),
                  new SugarParameter("@LAST_UPDATE_DATE",DateTime.Now)});
                }

                //7.状态插入,待审核状态：0
                var hrStatus = new HR_STATUS_INFO()
                {
                    person_id = person_id,
                    status = "0",
                    creation_date = DateTime.Now,
                    created_by = hrBaseInfo.name
                };
                _client.Insertable(hrStatus).ExecuteCommand();

                //8.考试信息插入
                //List<HR_EXAM> hrExam = JsonConvert.DeserializeObject<List<HR_EXAM>>(obj["hr_exam"].ToString());
                //foreach (var item in hrExam)
                //{
                //    item.person_id = person_id;
                //}
                //_client.Insertable(hrExam.ToArray()).ExecuteCommand();

                //提交
                _client.Ado.CommitTran();

                return "";
            }
            catch (Exception ex)
            {
                _client.Ado.RollbackTran();
                _logger.LogError("保存个人简历异常：" + ex.Message);
                return "保存个人简历失败";
            }
        }


        /// <summary>
        /// 更新个人简历信息
        /// </summary>
        /// <param name="formData"></param>
        /// <returns></returns>
        public string UpdateHrResume(JObject form)
        {
            try
            {
                //事务开始
                _client.Ado.BeginTran();

                //解析前台传过来的form表单
                JObject obj = (JObject)JsonConvert.DeserializeObject(form["formData"].ToString());

                //1.主表更新
                HR_BASIC_INFO hrBaseInfo = JsonConvert.DeserializeObject<HR_BASIC_INFO>(obj["hr_base_info"].ToString());
                hrBaseInfo.last_updated_by = "ITMS";
                hrBaseInfo.last_update_date = DateTime.Now;
                _client.Updateable(hrBaseInfo).UpdateColumns(it =>
                new
                {
                    it.name,
                    it.id_card_no,
                    it.telphone,
                    it.birthday,
                    it.sex,
                    it.race,
                    it.education_degree,
                    it.family_address,
                    it.current_address,
                    it.emergency_name,
                    it.emergency_tel,
                    it.salary_type,
                    it.work_type,
                    it.recruit_path,
                    it.batch_no,
                    it.is_criminal,
                    it.is_drug,
                    it.is_evil,
                    it.job_position,
                    it.job_type,
                    it.height,
                    it.weight,
                    it.marriage,
                    it.mail_address
                }).ExecuteCommand();

                //有如下五个表中面试者对应的笔数不一定，故先删除再插入，不做更新。
                //2.家庭成员信息插入
                List<HR_FAMILY_INFO> hrFamilyInfo = JsonConvert.DeserializeObject<List<HR_FAMILY_INFO>>(obj["hr_family_info"].ToString());
                if (hrFamilyInfo.Count > 0)
                {
                    _client.Deleteable<HR_FAMILY_INFO>().Where(it => it.person_id == hrBaseInfo.person_id).ExecuteCommand();
                    foreach (var item in hrFamilyInfo)
                    {
                        item.person_id = hrBaseInfo.person_id;//防止在页面中新增一笔记录后，无person_id
                    }
                    _client.Insertable(hrFamilyInfo.ToArray()).InsertColumns("person_id", "name", "sex", "kinship", "job", "tel").ExecuteCommand();
                }

                //3.教育信息插入
                List<HR_EDUCATION> hrEducation = JsonConvert.DeserializeObject<List<HR_EDUCATION>>(obj["hr_education"].ToString());
                if (hrEducation.Count > 0)
                {
                    _client.Deleteable<HR_EDUCATION>().Where(it => it.person_id == hrBaseInfo.person_id).ExecuteCommand();
                    foreach (var item in hrEducation)
                    {
                        item.person_id = hrBaseInfo.person_id;
                    }
                    _client.Insertable(hrEducation.ToArray()).ExecuteCommand();
                }


                //4.三等亲家属信息插入
                List<HR_RELATIONSHIP> hrRelationship = JsonConvert.DeserializeObject<List<HR_RELATIONSHIP>>(obj["hr_relationship"].ToString());
                if (hrRelationship.Count > 0)
                {
                    _client.Deleteable<HR_RELATIONSHIP>().Where(it => it.person_id == hrBaseInfo.person_id).ExecuteCommand();
                    foreach (var item in hrRelationship)
                    {
                        item.person_id = hrBaseInfo.person_id;
                    }
                    _client.Insertable(hrRelationship.ToArray()).ExecuteCommand();
                }

                //5.工作经验信息插入
                List<HR_WORK_EXPERIENCE> hrWorkExperience = JsonConvert.DeserializeObject<List<HR_WORK_EXPERIENCE>>(obj["hr_work_experience"].ToString());
                if (hrWorkExperience.Count > 0)
                {
                    _client.Deleteable<HR_WORK_EXPERIENCE>().Where(it => it.person_id == hrBaseInfo.person_id).ExecuteCommand();
                    foreach (var item in hrWorkExperience)
                    {
                        item.person_id = hrBaseInfo.person_id;
                    }
                    _client.Insertable(hrWorkExperience.ToArray()).ExecuteCommand();
                }

                //6.附件图片信息插入
                _client.Deleteable<HR_ATTACHMENT>().Where(it => it.person_id == hrBaseInfo.person_id).ExecuteCommand();
                JArray hrAttachment = (JArray)JsonConvert.DeserializeObject(obj["hr_attachment"].ToString());
                //处理url，去掉前缀"data:image/jpg;base64,"
                for (var i = 0; i < hrAttachment.Count; i++)//循环插入每个图片
                {
                    string base64data = hrAttachment[i]["url"].ToString();
                    String base64 = base64data.Substring(base64data.IndexOf(",") + 1);
                    byte[] url = Convert.FromBase64String(base64);
                    string name = hrAttachment[i]["name"].ToString();
                    int loc = hrAttachment[i]["name"].ToString().IndexOf('.');
                    if (loc > 0)
                    {
                        name = hrAttachment[i]["name"].ToString().Substring(0, hrAttachment[i]["name"].ToString().LastIndexOf('.'));
                    }

                    var dt = _client.Ado.GetDataTable(@"
                                INSERT INTO HR_ATTACHMENT
                                    (PERSON_ID, ATTACH_ID, PIC_NAME, PIC_SRC)
                                VALUES
                                    (@PERSON_ID, HR_ATTACHMENT_SEQ.NEXTVAL, @PIC_NAME, @PIC_SRC)
                                ", new List<SugarParameter>(){
                  new SugarParameter("@PERSON_ID",hrBaseInfo.person_id),
                  new SugarParameter("@PIC_SRC",url),
                  new SugarParameter("@PIC_NAME",name)});
                }

                //7.考试信息插入
                //List<HR_EXAM> hrExam = JsonConvert.DeserializeObject<List<HR_EXAM>>(obj["hr_exam"].ToString());
                //foreach (var item in hrExam)
                //{
                //    item.person_id = person_id;
                //}
                //cm.Db.Insertable(hrExam.ToArray()).ExecuteCommand();

                //提交
                _client.Ado.CommitTran();

                return "";
            }
            catch (Exception ex)
            {
                _client.Ado.RollbackTran();
                _logger.LogError("更新个人简历异常：" + ex.Message);
                return "更新个人简历失败";
            }
        }

        /// <summary>
        /// 查询黑名单信息
        /// </summary>
        /// <param name="IdCardNo"></param>
        /// <returns></returns>
        public string GetBlackInfo(string IdCardNo)
        {
            var dt = _client.Ado.GetDataTable("select get_black_lst@papr_papr(@id_card_no) black_info from dual", new { id_card_no = IdCardNo });
            return dt.Rows[0][0].ToString();
            //return "黑名單:-2022/04/13 12:40:08mhr_msl_ps_employee_hei舊工號:,H0131";
        }


        /// <summary>
        /// 根据类型获取参数（部门，招聘途径，子公司公司别等）
        /// </summary>
        /// <param name="lookupType"></param>
        /// <param name="type"></param>
        /// <returns></returns>
        public List<string> GetParameters(string lookupType, string type)
        {
            var infoList = _client.Ado.GetDataTable(
                "SELECT MEANING FROM HR_PARAMETERS WHERE ENABLED = 1 AND LOOKUP_TYPE=@LOOKUP_TYPE AND TYPE=@TYPE",
                new { LOOKUP_TYPE = lookupType, TYPE = type })
                .AsEnumerable().Select(d => d.Field<string>("meaning")).ToList(); // 返回List<>
            return infoList;
        }

        /// <summary>
        /// 根据身份证号,删除对应人员个人资料
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public void DeleteProfile(string idCardNo)
        {
            HR_BASIC_INFO item = _client.Queryable<HR_BASIC_INFO>().Where(it => it.id_card_no == idCardNo).First();

            _client.Deleteable<HR_BASIC_INFO>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_FAMILY_INFO>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_EDUCATION>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_RELATIONSHIP>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_WORK_EXPERIENCE>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_ATTACHMENT>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_EXAM>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_STATUS_INFO>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_HRCONFIRM>().Where(it => it.person_id == item.person_id).ExecuteCommand();
            _client.Deleteable<HR_MANAGER_COMMENT>().Where(it => it.person_id == item.person_id).ExecuteCommand();
        }

        /// <summary>
        /// 检查人员是否已在职
        /// </summary>
        /// <param name="ID_CARD_IN"></param>
        /// <returns></returns>
        public bool IsExistEmpNoByID(string ID_CARD_IN)
        {
            var emp = _client.Queryable<PS_EMPLOYEE>().Where(it => it.ID == ID_CARD_IN
            && it.STATUS == "1"
            && it.SITE == "MKL").First();
            return (emp != null) ? true : false;
        }

        /// <summary>
        /// 檢查該人員資料是否已經經過HR確認，即status>0,則返回true
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public bool IsInfoConfirmed(string idCardNo, string status)
        {
            var dt = _client.Ado.GetDataTable(@"
                        SELECT *
                          FROM HR_BASIC_INFO A, HR_STATUS_INFO B
                         WHERE A.PERSON_ID = B.PERSON_ID
                           AND B.STATUS > @status
                           AND A.ID_CARD_NO = @idCardNo",
                           new { status = Convert.ToInt32(status), idCardNo = idCardNo });
            if (dt.Rows.Count > 0) return true;
            return false;
        }

        /// <summary>
        /// 查询状态为personStatus，身份证号为idCardNo的人员的资料
        /// </summary>
        /// <param name="ID_CARD_IN"></param>
        /// <returns></returnsidCardNo
        public List<HR_BASIC_INFO> GetBaseInfoByIdStatus(string idCardNo, string status)
        {
            // 先将table序列化成string，然后再反序列化成list，防止直接返回table
            List<HR_BASIC_INFO> hrBaseInfo = JsonConvert.DeserializeObject<List<HR_BASIC_INFO>>
                    (JsonConvert.SerializeObject(_client.Ado.GetDataTable(
                       @"SELECT A.*
                          FROM HR_BASIC_INFO A, HR_STATUS_INFO B
                         WHERE A.PERSON_ID = B.PERSON_ID
                           AND A.ID_CARD_NO = @idCardNo
                           AND B.STATUS = NVL2(@status,TO_NUMBER(@status),B.STATUS)",//status如果为空，则使用恒等条件B.STATUS = B.STATUS
                        new { idCardNo = idCardNo, status = status })));
            return hrBaseInfo;
        }

        /// <summary>
        /// 根据人员身份照及状态位查询其详细资料
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <param name="status"></param>
        /// <returns></returns>
        public Dictionary<string, object> GetDetailByIdStatus(string idCardNo, string status)
        {
            Dictionary<string, object> formData = new Dictionary<string, object>();

            var hrBaseInfo = GetBaseInfoByIdStatus(idCardNo, status);//查询是否存在,如果status为空则不判断status

            if (hrBaseInfo.Count > 0)//存在符合条件人员
            {
                var personId = Convert.ToDecimal(hrBaseInfo[0].person_id.ToString());

                //5个辅表
                List<HR_FAMILY_INFO> hrFamilyInfo = _client.Queryable<HR_FAMILY_INFO>().Where(it => it.person_id == personId).ToList();

                List<HR_EDUCATION> hrEducation = _client.Queryable<HR_EDUCATION>().Where(it => it.person_id == personId).ToList();

                List<HR_RELATIONSHIP> hrRelationship = _client.Queryable<HR_RELATIONSHIP>().Where(it => it.person_id == personId).ToList();

                List<HR_WORK_EXPERIENCE> hrWorkExperience = _client.Queryable<HR_WORK_EXPERIENCE>().Where(it => it.person_id == personId).ToList();

                List<HR_ATTACHMENT> hrAttachment = _client.Queryable<HR_ATTACHMENT>().Where(it => it.person_id == personId).OrderBy(it => it.attach_id).ToList();

                HR_STATUS_INFO hrStatus = _client.Queryable<HR_STATUS_INFO>().Where(it => it.person_id == personId).First();

                HR_HRCONFIRM hrConfirm = _client.Queryable<HR_HRCONFIRM>().Where(it => it.person_id == personId).OrderByDescending(it => it.creation_date).First();

                List<HR_MANAGER_COMMENT> hrComment = _client.Queryable<HR_MANAGER_COMMENT>().Where(it => it.person_id == personId).OrderBy(it => it.creation_date).ToList();

                //List<HR_EXAM> hrExam = _client.Queryable<HR_EXAM>().Where(it => it.person_id == personId).ToList();

                formData = new Dictionary<string, object>()
                {
                    {"hr_base_info", hrBaseInfo[0] },
                    {"hr_family_info", hrFamilyInfo},
                    {"hr_education", hrEducation },
                    {"hr_relationship", hrRelationship},
                    {"hr_work_experience", hrWorkExperience },
                    {"hr_attachment", hrAttachment},
                    {"hr_status_info", hrStatus},
                    {"hr_hrconfirm", hrConfirm },
                    {"hr_manager_comment", hrComment},
                    //{"hr_exam", hrExam }
                };
            }

            return formData;
        }

        /// <summary>
        /// 未成年人驗證 true表示未成年，false表示成年
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public bool IsUnderAge(string idCardNo)
        {
            string Bir_date = idCardNo.Substring(6, 8);
            Bir_date = Bir_date.Insert(4, "-");
            Bir_date = Bir_date.Insert(7, "-");
            DateTime dtBirthday = Convert.ToDateTime(Bir_date);

            DateTime dt16 = dtBirthday.AddYears(16);

            DateTime dtNow = DateTime.Parse(DateTime.Now.ToString("yyyy-MM-dd"));

            if (dt16 >= dtNow)
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// 根据员工类型，状态，工种类型，填表时间，身份证后四位，当日批次，员工姓名
        /// 根据条件查询到的人员list
        /// </summary>
        /// <param name="list"></param>
        /// <returns></returns>
        public List<SearchEmployee> GetEmployeeList(SearchEmployee list)
        {
            string sql = @"SELECT A.*,B.STATUS
                          FROM HR_BASIC_INFO A, HR_STATUS_INFO B
                            WHERE A.PERSON_ID = B.PERSON_ID
                                AND B.STATUS IN(@STATUS)
                                AND A.WORK_TYPE=@WORK_TYPE";//,get_black_lst @papr_papr(id_card_no) as black_info

            List<SearchEmployee> employeeList = _client.SqlQueryable<SearchEmployee>(sql)
                .AddParameters(new { WORK_TYPE = list.work_type.Trim(), STATUS = list.status.Split(',') }).ToList()
                .WhereIF(!string.IsNullOrEmpty(list.batch_no), it => it.batch_no == list.batch_no)
                .WhereIF(!string.IsNullOrEmpty(list.idCardLast4), it => it.id_card_no.Contains(list.idCardLast4))
                .WhereIF(!string.IsNullOrEmpty(list.salary_type), it => it.salary_type == list.salary_type)
                .WhereIF(!string.IsNullOrEmpty(list.name), it => it.name == list.name)
                .WhereIF(list.creation_date.HasValue, it => it.creation_date?.ToString("yyyy-MM-dd") == list.creation_date?.ToString("yyyy-MM-dd")).ToList();

            return employeeList;
        }

        /// <summary>
        /// 根据身份证号更新其status状态（同时可能删除其已有资料）
        /// </summary>
        /// <param name="updateParams"></param>
        /// <returns></returns>
        public string UpdateStatus(UpdateParams updateParams)
        {
            string msg = "OK";
            foreach (var item in updateParams.idCardNoList)
            {
                try
                {
                    //事务开始
                    _client.Ado.BeginTran();
                    string sql = string.Format(@"UPDATE HR_STATUS_INFO
                                   SET STATUS = {0},
                                   LAST_UPDATE_DATE = TO_DATE('{1}','yyyy-MM-dd hh24:mi:ss'),
                                   LAST_UPDATED_BY = '{2}'
                                 WHERE PERSON_ID = (SELECT PERSON_ID
                                                      FROM HR_BASIC_INFO
                                                     WHERE ID_CARD_NO = '{3}')",
                    updateParams.status, DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"), updateParams.empno, item);
                    _client.Ado.ExecuteCommand(sql);

                    //需要删除其資料
                    if (updateParams.delete)
                    {
                        DeleteProfile(item);
                    }
                    //提交
                    _client.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    _client.Ado.RollbackTran();
                    msg = "个人状态更新失败";//返回给前端页面显示的数据
                    _logger.LogError($"{item}：个人状态更新失败，异常原因：{ex.ToString()}"); //存log

                }
            }
            return msg;
        }

        /// <summary>
        /// 根据身份证号获取personid
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public decimal GetPersonIdByIdCard(string idCardNo)
        {
            return _client.Queryable<HR_BASIC_INFO>().Where(it => it.id_card_no == idCardNo).First().person_id;
        }

        /// <summary>
        /// 新增主管评论等(insert评论等到hr_manager_comment，update状态及主管id到hr_status_info)
        /// </summary>
        /// <param name="managerParams"></param>
        /// <returns></returns>
        public string InsertComment(ManagerParams param)
        {
            string msg = "OK";

            // 赋值一个实例的属性值到另一个model实例
            var comment = new HR_MANAGER_COMMENT();
            param.CopyPropertiesTo<ManagerParams, HR_MANAGER_COMMENT>(comment);

            // 遍历同时选中的多人，进行其主管评价资料插入
            foreach (var item in param.id_card_no)
            {
                try
                {
                    //事务开始
                    _client.Ado.BeginTran();

                    //1.新增评论
                    comment.person_id = GetPersonIdByIdCard(item);
                    comment.employ_status = "Y";
                    comment.created_by = comment.manager_empno;
                    comment.creation_date = DateTime.Now;
                    //查询主管姓名
                    comment.manager_name = QueryNameByEmpno(comment.manager_empno);
                    decimal manager_id = _client.Insertable(comment).ExecuteReturnBigIdentity();

                    //2.更新状态表的状态
                    _client.Updateable<HR_STATUS_INFO>()
                        .SetColumns(it => new HR_STATUS_INFO()
                        {
                            status = param.status, //面试通过
                            manager_detail_id = manager_id,
                            last_updated_by = comment.manager_empno,
                            last_update_date = DateTime.Now
                        })
                        .Where(it => it.person_id == comment.person_id).ExecuteCommand();

                    //提交
                    _client.Ado.CommitTran();
                }
                catch (Exception ex)
                {
                    _client.Ado.RollbackTran();
                    msg = "插入主管评价失败";
                    _logger.LogError($"{item}：插入主管评价失败，异常原因：{ex.ToString()}");

                }
            }
            return msg;
        }

        /// <summary>
        /// 新增最终录用结论，职称等(insert结论到hr_hrconfirm，如果有则更新。update状态及hrid到hr_status_info)
        /// </summary>
        /// <param name="hrConfirmParams"></param>
        /// <returns></returns>
        public string InsertResult(HrConfirmParams param)
        {
            string msg = "OK";
            // 赋值一个实例的属性值到另一个model实例
            var result = new HR_HRCONFIRM();
            param.CopyPropertiesTo<HrConfirmParams, HR_HRCONFIRM>(result);
            try
            {
                //事务开始
                _client.Ado.BeginTran();

                //1.新增hr结论
                result.person_id = GetPersonIdByIdCard(param.id_card_no);
                result.created_by = param.hr_empno;
                result.creation_date = DateTime.Now;
                result.hire_date = Convert.ToDateTime(DateTime.Now.ToString("yyyy-MM-dd"));
                decimal hr_id = _client.Insertable(result).ExecuteReturnBigIdentity();

                //2.更新状态
                _client.Updateable<HR_STATUS_INFO>()
                    .SetColumns(it => new HR_STATUS_INFO()
                    {
                        status = "3",//确认录用
                        hr_detail_id = hr_id,
                        last_updated_by = result.hr_empno,
                        last_update_date = DateTime.Now
                    })
                    .Where(it => it.person_id == result.person_id).ExecuteCommand();

                //提交
                _client.Ado.CommitTran();
            }
            catch (Exception ex)
            {
                _client.Ado.RollbackTran();
                msg = "HR最终面试结果插入失败";
                _logger.LogError($"{param.id_card_no}：HR最终面试结果插入异常，原因：{ex.ToString()}");
            }
            return msg;
        }

        /// <summary>
        /// 根据人员身份证号获取主管评论等信息(最后一个主管)
        /// </summary>
        /// <param name="idCardNo"></param>
        /// <returns></returns>
        public HR_MANAGER_COMMENT GetManagerCommentByIdCard(string idCardNo)
        {
            decimal personId = GetPersonIdByIdCard(idCardNo);
            return _client.Queryable<HR_MANAGER_COMMENT>().Where(it => it.person_id == personId).OrderByDescending(it => it.creation_date).First();
        }

        /// <summary>
        /// 自定义模型验证,验证obj中值是否为空，未使用
        /// </summary>
        /// <param name="obj"></param>
        /// <returns></returns>
        public string ModelValidate(object obj)
        {
            string errMsg = "";
            var context = new ValidationContext(obj, null, null);

            var results = new List<ValidationResult>();
            Validator.TryValidateObject(obj, context, results, true);
            foreach (var validationResult in results)
            {
                errMsg = errMsg + validationResult.ErrorMessage;
            }
            return errMsg;
        }

        /// <summary>
        /// 查询访谈录信息
        /// </summary>
        /// <returns></returns>
        public DataTable GetInterviewData(string version)
        {
            DataTable dt = _client.Ado.GetDataTable("select * from hr_interview where interview_version=@version", new { version = version });
            return dt;
        }

        /// <summary>
        /// 根据工号查询人员基本信息，for客户稽核查询资料使用
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public List<HR_BASIC_INFO> QueryListByEmpNo(string empNo)
        {
            empNo = empNo.ToUpper();
            var hrBaseInfo = _client.Ado.SqlQuery<HR_BASIC_INFO>(@"
            SELECT HBI.*
              FROM HR_BASIC_INFO HBI, PS_EMPLOYEE PS
             WHERE HBI.ID_CARD_NO = PS.ID
               AND PS.EMPNO = @empNo",
           new { empNo = empNo });
            return hrBaseInfo;
        }

        /// <summary>
        /// 根据工号获取姓名
        /// </summary>
        /// <param name="empNo"></param>
        /// <returns></returns>
        public string QueryNameByEmpno(string empNo)
        {
            empNo = empNo.ToUpper();
            return _client.Ado.GetDataTable(@"
            SELECT NAME FROM PS_EMPLOYEE WHERE empno = @empNo",
           new { empNo = empNo }).AsEnumerable().Select(d => d.Field<string>("name")).First();
        }

        #region test
        // 返回数组
        //string[] d = _client.Ado.GetDataTable(
        //    "SELECT MEANING FROM HR_PARAMETERS WHERE LOOKUP_TYPE='DeptName'")
        //    .AsEnumerable().Select(d => d.Field<string>("meaning")).ToArray();

        //var d = _client.Queryable<HR_PARAMETERS>()
        //    .Where(it => it.lookup_type == "DeptName").Select("meaning").ToList();

        //    throw new DemoException($"获取{lookupType}参数失败");

        //1个主表
        //&& SqlFunc.EqualsNull(it.DOWNLOAD_STATUS, downloadStatus)  
        //var exp = Expressionable.Create<HR_BASIC_INFO>();
        //exp.AndIF(条件, it => it.Id == 1);//.AndIF 是条件成立才会拼接AND
        //exp.OrIF(条件, it => it.Id == 1);//.OrIf 是条件成立才会拼接OR: id.HasValue/string.isnullorempty
        //exp.Or(it => it.Name.Contains("jack"));//拼接OR
        //var list = _client.Queryable<HR_BASIC_INFO>().Where(exp.ToExpression()).ToList();

        //public void test()
        //{
        //    string[] test = new string[4];
        //    string[] test3 = { "12", "234" };
        //    string[] test1 = new string[2] { "12", "234" };
        //    string[] test2 = new string[] { "12", "234" };
        //    test[0] = "string";
        //    test1[3] = "fsdf"; //error?

        //    string a = test2.ToArray()[0];

        //    List<string> test4 = new();
        //    test4.Add("ff");
        //}
        #endregion
    }
}
