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
using Timensit_API.Controllers.Common;
using Timensit_API.Models.DanhMuc;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.DanhMuc
{
    /// <summary>
    /// Danh mục Nguồn kinh phí
    /// Quyền xem: 67 - Quản lý danh mục nguồn kinh phí
    /// Quyền sửa: 68 - Cập nhật danh mục nguồn kinh phí
    /// </summary>
    [ApiController]
    [Route("api/nguon-kinh-phi")]
    [EnableCors("TimensitPolicy")]
    public class NguonKinhPhiController : ControllerBase
    {
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Nguồn kinh phí";

        public NguonKinhPhiController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
        }

        /// <summary>
        /// Danh sách 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        /// 
        [Authorize(Roles = "67")]
        [HttpGet]
        public BaseModel<object> List([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                bool Visible = User.IsInRole("68");
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select dm.*, u.FullName as NguoiTao, u1.FullName as NguoiSua from DM_NguonKinhPhi dm
join Dps_User u on dm.CreatedBy=u.UserID
left join Dps_User u1 on dm.UpdatedBy=u1.UserID
where dm.Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "NguonKinhPhi","NguonKinhPhi" },
                        { "Locked","Locked" },
                        { "Priority","Priority" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["NguonKinhPhi"]))
                    {
                        string keyword = query.filter["NguonKinhPhi"].ToLower();
                        temp = temp.Where(x => x["NguonKinhPhi"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    }
                    #endregion
                    int total = temp.Count();
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, Visible);
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
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {

                                   Id = r["Id"],

                                   NguonKinhPhi = r["NguonKinhPhi"],

                                   Locked = r["Locked"],

                                   Priority = r["Priority"],

                                   CreatedBy = r["NguoiTao"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
                                   UpdatedBy = r["NguoiSua"],
                                   UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["UpdatedDate"])

                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Thêm mới 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "68")]
        [HttpPost]
        public BaseModel<object> Create([FromBody] NguonKinhPhiModel data)
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
                if (string.IsNullOrEmpty(data.NguonKinhPhi))
                    strRe += (strRe == "" ? "" : ", ") + "Diện chỉnh hình";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from DM_NguonKinhPhi where Disabled=0 and (NguonKinhPhi = @Name)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.NguonKinhPhi) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("NguonKinhPhi", data.NguonKinhPhi);
                    val.Add("Locked", false);
                    if (data.Priority > 0)
                        val.Add("Priority", data.Priority);
                    else
                        val.Add("Priority", 1);
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", loginData.Id);

                    if (cnn.Insert(val, "DM_NguonKinhPhi") == 1)
                    {
                        data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('DM_NguonKinhPhi') AS Current_Identity; ").ToString());
                        logHelper.Ghilogfile<NguonKinhPhiModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
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
        /// Cập nhật thông tin 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "68")]
        [HttpPut("{id}")]
        public BaseModel<object> Update(int id, [FromBody] NguonKinhPhiModel data)
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
                if (string.IsNullOrEmpty(data.NguonKinhPhi))
                    strRe += (strRe == "" ? "" : ", ") + "Diện chỉnh hình";

                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_NguonKinhPhi where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from DM_NguonKinhPhi where Disabled=0 and (NguonKinhPhi = @Name) and Id <> @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "Id", id }, new SqlCondition("Name", data.NguonKinhPhi) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("NguonKinhPhi", data.NguonKinhPhi);
                    val.Add("Locked", data.Locked);
                    val.Add("Priority", data.Priority);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);

                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "DM_NguonKinhPhi") == 1)
                    {
                        logHelper.Ghilogfile<NguonKinhPhiModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
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
        /// Chi tiết 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "67")]
        [HttpGet("{id}")]
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
                    string sqlq = @"select * from DM_NguonKinhPhi where Disabled=0 and Id = @Id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {

                                                           Id = r["Id"],

                                                           NguonKinhPhi = r["NguonKinhPhi"],

                                                           Locked = r["Locked"],

                                                           Priority = r["Priority"],
                                                           AllowEdit = User.IsInRole("68")
                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Xóa 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "68")]
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(int id)
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
                    string sqlq = "select * from DM_NguonKinhPhi where Disabled = 0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    sqlq = "update DM_NguonKinhPhi set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["NguonKinhPhi"].ToString() };
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

        /// <summary>
        /// Khóa 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "68")]
        [Route("lock")]
        [HttpGet]
        public BaseModel<object> Lock(long id, bool Value = true)
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
                    string sqlq = "select count(*) from DM_NguonKinhPhi where Disabled = 0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "";

                    sqlq = "update DM_NguonKinhPhi set Locked = " + (Value ? 1 : 0) + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string note = Value ? "Khóa" : "Mở khóa";
                        var data = new LiteModel() { id = id, data = new { Locked = Value } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật - " + note, logHelper.LogSua((int)id, loginData.Id, Name, note),Name);
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
