using DpsLibs.Data;
using System;
using System.Data;
using System.Linq;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Timensit_API.Models;
using Timensit_API.Controllers.Users;
using Microsoft.Extensions.Configuration;
using Timensit_API.Models.Common;
using System.Collections.Generic;
using Microsoft.AspNetCore.Authorization;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.QuanLyNguoiDung
{
    /// <summary>
    /// Cấu hình hệ thống<para/>
    /// Quyền 52	Cấu hình hệ thống
    /// Quyền 53	Cập nhật cấu hình hệ thống
    /// </summary>
    [ApiController]
    [Route("api/config")]
    [EnableCors("TimensitPolicy")]
    public class ConfigController : ControllerBase
    {
        LoginController lc;
        private NCCConfig _config;
        LogHelper logHelper;
        string Name = "Cấu hình";
        public ConfigController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 12);
        }
        /// <summary>
        /// Danh sách Cấu hình
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "52")]
        [HttpGet]
        [Route("list")]
        public BaseModel<object> List([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select * from Sys_Config c join Sys_ConfigGroup g on c.IdGroup=g.IdRow where c.disabled=0 and g.disabled=0 and IsReadOnly=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                        { "Code","Code" },
                        { "Value","Value" },
                        { "IdGroup","ConfigGroup" },
                        { "Priority","Priority" },
                        { "Description","Description" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["Code"]))
                    {
                        string keyword = query.filter["Code"].ToLower();
                        temp = temp.Where(x => x["Code"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Value"]))
                    {
                        string keyword = query.filter["Value"].ToLower();
                        temp = temp.Where(x => x["Value"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Description"]))
                    {
                        string keyword = query.filter["Description"].ToLower();
                        temp = temp.Where(x => x["Description"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["IdGroup"] != null && query.filterGroup["IdGroup"].Length > 0)
                    {
                        var groups = query.filterGroup["IdGroup"];
                        temp = temp.Where(x => groups.Contains(x["IdGroup"].ToString()));
                    }
                    #endregion
                    int total = temp.Count();
                    if (total == 0)
                        return new BaseModel<object>
                        {
                            status = 1,
                            data = new List<string>(),
                            page = pageModel
                        };
                    pageModel.TotalCount = total;
                    if (query.more)
                    {
                        query.page = 1;
                        query.record = pageModel.TotalCount;
                    }
                    pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                    pageModel.Size = query.record;
                    pageModel.Page = query.page;
                    // Phân trang
                    var temp1 = temp.Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel());
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {

                                   IdRow = r["IdRow"],

                                   Code = r["Code"],

                                   Value = r["Value"],

                                   Type = r["Type"],

                                   IdGroup = r["IdGroup"],

                                   ConfigGroup = r["ConfigGroup"],

                                   Priority = r["Priority"],

                                   Description = r["Description"],

                                   Pattern = r["Pattern"],

                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Chi tiết Cấu hình
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "52")]
        [HttpGet]
        [Route("detail")]
        public BaseModel<object> Detail(int id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select * from Sys_Config where disabled=0 and IdRow = @Id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {
                                                           IdRow = r["IdRow"],

                                                           Code = r["Code"],

                                                           Value = r["Value"],

                                                           IdGroup = r["IdGroup"],

                                                           Priority = r["Priority"],

                                                           Description = r["Description"],

                                                           Pattern = r["Pattern"],

                                                           Type = r["Type"]
                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Cập nhật thông tin Cấu hình
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "53")]
        [Route("update")]
        [HttpPost]
        public BaseModel<object> Update([FromBody] ConfigModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string strRe = "";
                if (string.IsNullOrEmpty(data.Value))
                    strRe += (strRe == "" ? "" : ", ") + "Giá trị";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Sys_Config where disabled=0 and IdRow = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", data.IdRow) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    Hashtable val = new Hashtable();
                    val.Add("Value", data.Value);
                    val.Add("Priority", data.Priority);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);
                    if (cnn.Update(val, new SqlConditions { { "IdRow", data.IdRow } }, "Sys_Config") == 1)
                    {
                        logHelper.Ghilogfile<ConfigModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.IdRow.Value, loginData.Id, Name),Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Route("config-group")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> ConfigGroup()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Sys_ConfigGroup where disabled=0 order by Priority";
                    DataTable dt = cnn.CreateDataTable(sql);
                    List<LiteModel> data = (from pb in dt.AsEnumerable()
                                            select new LiteModel()
                                            {
                                                id = int.Parse(pb["IdRow"].ToString()),
                                                title = pb["ConfigGroup"].ToString()
                                            }).ToList();

                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
    }
}