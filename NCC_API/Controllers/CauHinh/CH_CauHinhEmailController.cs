using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Microsoft.AspNetCore.Hosting;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timensit_API.Controllers.CauHinh
{
    [ApiController]
    [Route("api/chemail")]
    [EnableCors("TimensitPolicy")]
    public class CH_CauHinhEmailController : ControllerBase
    {
        LoginController lc;
        private NCCConfig _config;
        string Name = "Email";
        LogHelper logHelper;
        public CH_CauHinhEmailController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 12);
        }

        /// <summary>
        /// Danh sách Danh mục
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "54")]
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
                    string where = "";
                    SqlConditions conds = new SqlConditions();

                    string sqlq = $@"select e.Id,e.DonVi,coalesce( d.DonVi,'') as TenDonVi,Server,Port,EnableSSL,UserName,Password,e.Locked from Tbl_CauHinh_Mail e 
left join DM_DonVi d on e.DonVi=d.Id where e.Disabled=0 {where}";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                            { "TenDonVi","TenDonVi" },
                            { "Server","Server" },
                            { "Port","Port" },
                            { "Locked","Locked" },
                            { "UserName","UserName" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["TenDonVi"]))
                    {
                        string keyword = query.filter["TenDonVi"].ToLower();
                        temp = temp.Where(x => x["TenDonVi"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Server"]))
                    {
                        string keyword = query.filter["Server"].ToLower();
                        temp = temp.Where(x => x["Server"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Port"]))
                    {
                        string keyword = query.filter["Port"].ToLower();
                        temp = temp.Where(x => x["Port"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["UserName"]))
                    {
                        string keyword = query.filter["UserName"].ToLower();
                        temp = temp.Where(x => x["UserName"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["IdDonVi"]))
                    {
                        string keyword = query.filter["IdDonVi"].ToLower();
                        temp = temp.Where(x => x["DonVi"].ToString().ToLower() == keyword);
                    }
                    if (query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Count() > 0)
                    {
                        string[] keyword = query.filterGroup["Locked"];
                        temp = temp.Where(x => keyword.Contains(x["Locked"].ToString().ToLower()));
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
                                   Id = r["Id"],
                                   TenDonVi = r["TenDonVi"],
                                   Server = r["Server"],
                                   Port = r["Port"],
                                   UserName = r["UserName"],
                                   Locked = r["Locked"]
                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Chi tiết Danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "54")]
        [HttpGet]
        [Route("detail")]
        public BaseModel<object> Detail(long id)
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
                    string sqlq = @"select * from Tbl_CauHinh_Mail where Disabled=0 and Id=@Id
select a.*,b.DonVi as TenDonVi from Tbl_CauHinh_DonViCon a inner join DM_DonVi b on a.DonVi=b.Id where Loai=1 and IdCauHinh=@Id and a.Disabled=0";
                    var ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || ds == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    logHelper.LogXemCT((int)id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in ds.Tables[0].AsEnumerable()
                                                       select new
                                                       {
                                                           Id = r["Id"],
                                                           Server = r["Server"],
                                                           Port = r["Port"],
                                                           UserName = r["UserName"],
                                                           Locked = r["Locked"],
                                                           Password = r["Password"],
                                                           EnableSSL = r["EnableSSL"],
                                                           DonVi = r["DonVi"],
                                                           DonViCon = (from dv in ds.Tables[1].AsEnumerable()
                                                                       select new
                                                                       {
                                                                           Check = true,
                                                                           Id = dv["DonVi"],
                                                                           DonVi = dv["TenDonVi"]
                                                                       })
                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Thêm mới Danh mục khác
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "55")]
        [Route("create")]
        [HttpPost]
        public BaseModel<object> Create([FromBody] CauHinhEmailModel data)
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

                if (string.IsNullOrEmpty(data.UserName.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Username";

                if (string.IsNullOrEmpty(data.Server.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Server";

                if (string.IsNullOrEmpty(data.Port.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Port";

                if (string.IsNullOrEmpty(data.Password.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Password";

                if (data.DonVi < 0)
                    strRe += (strRe == "" ? "" : ", ") + "Đơn vị";

                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    if (data.DonVi == 0)//email dùng chung
                    {
                        string sqlq = "select count(*) from Tbl_CauHinh_Mail where Disabled=0 and DonVi = 0";
                        if ((int)cnn.ExecuteScalar(sqlq) > 0)
                            return JsonResultCommon.Custom("Email dùng chung đã được cấu hình");
                    }
                    Hashtable val = new Hashtable();

                    val.Add("UserName", data.UserName);
                    val.Add("Server", data.Server);
                    val.Add("Port", data.Port);
                    val.Add("Password", data.Password);
                    val.Add("DonVi", data.DonVi);

                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", iduser);

                    if (cnn.Insert(val, "Tbl_CauHinh_Mail") == 1)
                    {
                        if (data.DonViCon.Count > 0)
                        {
                            string sqlq = "select IDENT_CURRENT('Tbl_CauHinh_Mail') as Id";
                            var id = cnn.CreateDataTable(sqlq).Rows[0]["Id"].ToString();
                            foreach (var item in data.DonViCon)
                            {
                                Hashtable val_dvc = new Hashtable();
                                val_dvc.Add("DonVi", item);
                                val_dvc.Add("Loai", 1);
                                val_dvc.Add("IdCauHinh", id);
                                cnn.Insert(val_dvc, "Tbl_CauHinh_DonViCon");
                            }

                        }
                        logHelper.Ghilogfile<CauHinhEmailModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
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
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cập nhật thông tin Danh mục
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "55")]
        [Route("update")]
        [HttpPost]
        public BaseModel<object> Update([FromBody] CauHinhEmailModel data)
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


                if (string.IsNullOrEmpty(data.UserName.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Username";

                if (string.IsNullOrEmpty(data.Server.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Server";

                if (string.IsNullOrEmpty(data.Port.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Port";

                if (string.IsNullOrEmpty(data.Password.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Password";

                if (string.IsNullOrEmpty(data.DonVi.ToString()))
                    strRe += (strRe == "" ? "" : ", ") + "Đơn vị";


                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Tbl_CauHinh_Mail where Disabled=0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", data.Id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    if (data.DonVi == 0)
                    {
                        sqlq = "select count(*) from Tbl_CauHinh_Mail where Disabled=0 and Id <> @Id and  DonVi = 0";
                        if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", data.Id) }) > 0)
                            return JsonResultCommon.Custom("Email dùng chung đã được cấu hình");
                    }
                    Hashtable val = new Hashtable();
                    val.Add("UserName", data.UserName);
                    val.Add("Server", data.Server);
                    val.Add("Port", data.Port);
                    val.Add("Password", data.Password);
                    val.Add("DonVi", data.DonVi);

                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", iduser);


                    if (cnn.Update(val, new SqlConditions { { "Id", data.Id } }, "Tbl_CauHinh_Mail") == 1)
                    {
                        logHelper.Ghilogfile<CauHinhEmailModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.Id, loginData.Id, Name), Name);

                        string sql_del = "Update Tbl_CauHinh_DonViCon set Disabled=1, UpdatedDate=getdate(), UpdatedBy=" + iduser + "  where IdCauHinh=" + data.Id;
                        cnn.ExecuteNonQuery(sql_del);

                        foreach (var item in data.DonViCon)
                        {
                            Hashtable val_dvc = new Hashtable();
                            val_dvc.Add("DonVi", item);
                            val_dvc.Add("Loai", 1);
                            val_dvc.Add("IdCauHinh", data.Id);
                            cnn.Insert(val_dvc, "Tbl_CauHinh_DonViCon");
                        }

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
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Xóa Danh mục
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "55")]
        [Route("delete")]
        [HttpGet]
        public BaseModel<object> Delete(long id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    //string sqlq = "select * from DM_DanhMuc where Disabled = 0 and Id = @Id";
                    //if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                    //    return JsonResultCommon.KhongTonTai(Name);
                    string sqlq = "";

                    sqlq = "update Tbl_CauHinh_Mail set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string sql_del = "Update Tbl_CauHinh_DonViCon set Disabled=1, UpdatedDate=getdate(), UpdatedBy=" + iduser + "  where IdCauHinh=" + id;
                        cnn.ExecuteNonQuery(sql_del);

                        var data = new LiteModel() { id = id };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong();
                    }
                    else
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize(Roles = "56")]
        [Route("LockAndUnLock")]
        [HttpGet]
        public BaseModel<object> LockAndUnLock(long id, bool Value)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Tbl_CauHinh_Mail where Disabled = 0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "";

                    sqlq = "update Tbl_CauHinh_Mail set Locked = " + (!Value == false ? 0 : 1) + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string note = Value ? "Mở khóa" : "Khóa";
                        var data = new LiteModel() { id = id, data = new { Locked = Value } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật - " + note, logHelper.LogSua((int)id, loginData.Id, Name, note), Name);
                        return JsonResultCommon.ThanhCong();
                    }
                    else
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
    }
}
