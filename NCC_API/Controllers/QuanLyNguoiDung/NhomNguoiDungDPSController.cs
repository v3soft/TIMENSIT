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
using Timensit_API.Models.QuanLyNguoiDung;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Http;
using DocumentFormat.OpenXml.Office2013.PowerPoint.Roaming;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.QuanLyNguoiDung
{
    /// <summary>
    /// Quản lý vai trò (vai trò)<para/>
    /// Quyền 12:	Xem danh sách và thông tin vai trò
    /// Quyền 13:	Cập nhật thông tin vai trò
    /// Quyền 23:	Phân quyền cho vai trò
    /// </summary>
    [ApiController]
    [Route("api/nhom-nguoi-dung")]
    [EnableCors("TimensitPolicy")]
    public class NhomNguoiDungDPSController : ControllerBase
    {
        LogHelper logHelper;
        UserManager dpsUserMr;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Vai trò";
        public NhomNguoiDungDPSController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            dpsUserMr = new UserManager();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 5);
        }
        /// <summary>
        /// Danh sách vai trò
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "12")]
        [HttpGet]
        [Route("list")]
        public BaseModel<object> LayDSNhom([FromQuery] QueryParams query)
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
                    //filterCap: 0-Tất cả, 1- Đơn vị cha,2-Đơn vị hiện tại,3-Đơn vị con,4- Đơn vị cùng cấp,5- Đơn vị n cấp
                    bool emptyRow = false;
                    string strW = "";
                    if (!string.IsNullOrEmpty(query.filter["emptyRow"]))
                        emptyRow = bool.Parse(query.filter["emptyRow"]);

                    if (!string.IsNullOrEmpty(query.filter["filterCap"]))
                    {
                        string keyword = query.filter["filterCap"];
                        string n = query.filter["filterCapN"];
                        if (keyword != "0")
                        {
                            if (keyword == "2")
                                strW += " and g.DonVi = " + loginData.Id;
                            else
                            {
                                string strS = "";
                                switch (keyword)
                                {
                                    case "1":
                                        strS = "select Parent from DM_DonVi where Id=" + loginData.IdDonVi;
                                        break;
                                    case "3":
                                        strS = "select Id from DM_DonVi where Parent=" + loginData.IdDonVi;
                                        break;
                                    case "4":
                                        strS = $@"WITH cte AS (
                                                SELECT ID, DonVi, Parent, 0 AS level
                                                FROM DM_DonVi
                                                WHERE Parent IS NULL and Disabled=0
                                                UNION ALL 
                                                SELECT t1.ID, t1.DonVi, t1.Parent, t2.level + 1
                                                FROM DM_DonVi t1
                                                INNER JOIN cte t2 ON t1.Parent = t2.ID 
                                                where Disabled=0
                                            )

                                            SELECT ID, DonVi, Parent, Level
                                            FROM cte
                                            where Level=(select Level from cte where Id={loginData.Id})
                                            ORDER BY ID";
                                        break;
                                    case "5":
                                        strS = $@"WITH cte AS (
                                                SELECT ID, DonVi, Parent, 0 AS level
                                                FROM DM_DonVi
                                                WHERE Parent IS NULL  and Disabled=0
                                                UNION ALL 
                                                SELECT t1.ID, t1.DonVi, t1.Parent, t2.level + 1
                                                FROM DM_DonVi t1
                                                INNER JOIN cte t2 ON t1.Parent = t2.ID
                                                where Disabled=0
                                            )

                                            SELECT ID, DonVi, Parent, Level
                                            FROM cte
                                            where Level={n}
                                            ORDER BY ID";
                                        break;
                                }
                                DataTable dtDV = cnn.CreateDataTable(strS);
                                if (dtDV == null || dtDV.Rows.Count == 0)
                                {
                                    strW += " and 0=1";
                                }
                                else
                                {
                                    string lst = string.Join(",", dtDV.AsEnumerable().Select(x => x[0].ToString()).ToList());
                                    if (string.IsNullOrEmpty(lst))
                                        strW += " and g.DonVi in (" + lst + ")";
                                }
                            }
                        }
                    }
                    string sqlq = @"SELECT g.*,dv.DonVi as TenDonVi, cv.ChucVu as TenChucVu, u.FullName as ModifiedByUser, uu.FullName as CreatedByUser FROM [dbo].[Dps_UserGroups] g
join DM_DonVi dv on g.DonVi=dv.Id
left join DM_ChucVu cv on g.ChucVu=cv.Id
join Dps_User uu on g.CreatedBy=uu.UserID
left join Dps_User u on g.ModifiedBy=u.UserID
where IsDel = 0" + strW;
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }

                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "GroupName", "GroupName"}, // " Giống biến model", "Tên cột"
                            { "Ma", "Ma"},
                            { "DisplayOrder", "DisplayOrder"},
                            { "GhiChu", "GhiChu"},
                            { "Locked", "Locked"},
                        };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["GroupName"]))
                    {
                        string keyword = query.filter["GroupName"].ToLower();
                        temp = temp.Where(x => x["GroupName"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Ma"]))
                    {
                        string keyword = query.filter["Ma"].ToLower();
                        temp = temp.Where(x => x["Ma"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["GhiChu"]))
                    {
                        string keyword = query.filter["GhiChu"].ToLower();
                        temp = temp.Where(x => x["GhiChu"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["DonVi"]))
                    {
                        string keyword = query.filter["DonVi"];
                        temp = temp.Where(x => x["DonVi"].ToString() == keyword);
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    }
                    #endregion
                    int i = temp.Count();
                    if (i == 0)
                    {
                        if (emptyRow)
                        {
                            dt.Rows.Clear();
                        }
                        else
                        {
                            return new BaseModel<object>
                            {
                                status = 1,
                                data = new List<string>(),
                                page = pageModel
                            };
                        }
                    }
                    if (!emptyRow)
                        dt = temp.CopyToDataTable();
                    if (emptyRow)
                    {
                        dt.Rows.InsertAt(dt.NewRow(), 0);
                    }
                    int total = dt.Rows.Count;
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);
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
                    logHelper.LogXemDS(loginData.Id);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          IdGroup = r["IdGroup"],
                                                          GroupName = r["GroupName"],
                                                          Ma = r["Ma"],
                                                          GhiChu = r["GhiChu"],
                                                          DisplayOrder = r["DisplayOrder"],
                                                          Locked = r["Locked"],
                                                          IdDonVi = r["DonVi"],
                                                          DonVi = r["TenDonVi"],
                                                          ChucVu = r["TenChucVu"],
                                                          IsDefault = r["IsDefault"],
                                                          ModifiedBy = r["ModifiedByUser"] != DBNull.Value ? r["ModifiedByUser"] : r["CreatedByUser"],
                                                          ModifiedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["ModifiedByUser"] != DBNull.Value ? r["ModifiedDate"] : r["CreatedDate"]),
                                                      }, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Chi tiết vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "12")]
        [HttpGet]
        [Route("detail")]
        public BaseModel<object> ChiTietNhom(int id)
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
                    string sqlq = @"SELECT *
                                  FROM [dbo].[Dps_UserGroups] where IsDel = 0
                                  and [IdGroup] = @id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "id", id } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    logHelper.LogXemCT(id, loginData.Id);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {
                                                           IdGroup = r["IdGroup"],
                                                           GroupName = r["GroupName"],
                                                           Ma = r["Ma"],
                                                           GhiChu = r["GhiChu"],
                                                           DisplayOrder = r["DisplayOrder"],
                                                           Locked = r["Locked"],
                                                           DonVi = r["DonVi"],
                                                           ChucVu = r["ChucVu"],
                                                           IsDefault = r["IsDefault"]
                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Thêm mới vai trò
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "13")]
        [Route("create")]
        [HttpPost]
        public BaseModel<object> ThemNhom([FromBody] NhomNguoiDungDPS data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                //if (!User.IsInRole("39"))
                //    return JsonResultCommon.PhanQuyen();
                string strRe = "";
                if (string.IsNullOrEmpty(data.Ma))
                    strRe += (strRe == "" ? "" : ", ") + "mã vai trò";
                if (string.IsNullOrEmpty(data.GroupName))
                    strRe += (strRe == "" ? "" : ", ") + "tên vai trò";
                if (data.DonVi <= 0)
                    strRe += (strRe == "" ? "" : ", ") + "đơn vị";
                if (strRe != "")
                    return JsonResultCommon.BatBuoc(strRe);
                data.GroupName = data.GroupName.Trim();
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Dps_UserGroups where IsDel = 0 and DonVi=@IdDonVi and (GroupName = @GroupName or Ma=@Ma)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "IdDonVi", data.DonVi }, { "Ma", data.Ma }, new SqlCondition("GroupName", data.GroupName) }) > 0)
                        return JsonResultCommon.Trung("Mã hoặc tên vai trò của đơn vị");
                    else
                    {
                        Hashtable val = new Hashtable();
                        val.Add("GroupName", data.GroupName);
                        val.Add("Ma", string.IsNullOrEmpty(data.Ma) ? "" : data.Ma);
                        val.Add("GhiChu", string.IsNullOrEmpty(data.GhiChu) ? "" : data.GhiChu);
                        val.Add("DisplayOrder", data.DisplayOrder);
                        val.Add("DonVi", data.DonVi);
                        if (data.ChucVu > 0)
                            val.Add("ChucVu", data.ChucVu);
                        val.Add("IsDefault", data.IsDefault);
                        val.Add("Locked", false);
                        val.Add("CreatedBy", iduser);
                        if (cnn.Insert(val, "Dps_UserGroups") == 1)
                        {
                            var id = cnn.ExecuteScalar("select IDENT_CURRENT('Dps_UserGroups')");
                            data.IdGroup = int.Parse(id.ToString());
                            logHelper.Ghilogfile<NhomNguoiDungDPS>(data, loginData, "Thêm mới", logHelper.LogThem(data.IdGroup, loginData.Id));
                            return JsonResultCommon.ThanhCong(data);
                        }
                        else
                        {
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cập nhật thông tin vai trò
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "13")]
        [Route("update")]
        [HttpPost]
        public BaseModel<object> SuaNhom([FromBody] NhomNguoiDungDPS data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                //if (!User.IsInRole("37"))
                //    return JsonResultCommon.PhanQuyen();
                if (string.IsNullOrEmpty(data.GroupName))
                    return JsonResultCommon.BatBuoc("tên vai trò");
                data.GroupName = data.GroupName.Trim();
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Dps_UserGroups where IsDel = 0 and IdGroup = @IdGroup";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("IdGroup", data.IdGroup) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from Dps_UserGroups where IsDel = 0 and DonVi=@IdDonVi and (GroupName = @GroupName or Ma=@Ma) and IdGroup <> @IdGroup";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "IdGroup", data.IdGroup }, { "IdDonVi", data.DonVi }, { "Ma", data.Ma }, new SqlCondition("GroupName", data.GroupName) }) > 0)
                        return JsonResultCommon.Trung("Mã hoặc tên vai trò của đơn vị");
                    Hashtable val = new Hashtable();
                    val.Add("GroupName", data.GroupName);
                    val.Add("Ma", string.IsNullOrEmpty(data.Ma) ? "" : data.Ma);
                    val.Add("GhiChu", string.IsNullOrEmpty(data.GhiChu) ? "" : data.GhiChu);
                    val.Add("DisplayOrder", data.DisplayOrder);
                    val.Add("Locked", data.Locked);
                    val.Add("DonVi", data.DonVi);
                    if (data.ChucVu > 0)
                        val.Add("ChucVu", data.ChucVu);
                    else
                        val.Add("ChucVu", DBNull.Value);
                    val.Add("IsDefault", data.IsDefault);
                    val.Add("ModifiedBy", iduser);
                    val.Add("ModifiedDate", DateTime.Now);
                    if (cnn.Update(val, new SqlConditions { { "IdGroup", data.IdGroup } }, "Dps_UserGroups") == 1)
                    {
                        logHelper.Ghilogfile<NhomNguoiDungDPS>(data, loginData, "Cập nhật", logHelper.LogSua(data.IdGroup, loginData.Id));
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
        /// Xóa vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "13")]
        [Route("delete")]
        [HttpGet]
        public BaseModel<object> XoaNhom(long id)
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
                    string sqlq = "select * from Dps_UserGroups where IsDel = 0 and IdGroup = @IdGroup ";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("IdGroup", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from Dps_User_GroupUser where disabled=0 and IdGroupUser= @IdGroup";
                    int souser = (int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("IdGroup", id) });
                    if (souser > 0)
                    {
                        return JsonResultCommon.Custom("Không thể xoá vai trò vì đang có " + souser.ToString() + " người dùng sử dụng");
                    }
                    sqlq = "update Dps_UserGroups set IsDel = 1, ModifiedDate = getdate(), ModifiedBy = " + iduser + " where IdGroup = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["GroupName"].ToString() };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id));
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
        /// Khóa/mở khóa vai trò
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "13")]
        [Route("lock")]
        [HttpGet]
        public BaseModel<object> Lock(int id, bool islock)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                if (id <= 0)
                {
                    return JsonResultCommon.KhongTonTai(Name);
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Dps_UserGroups where IsDel = 0 and IdGroup = @IdGroup ";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("IdGroup", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "update Dps_UserGroups set Locked = " + (islock ? "1" : "0") + ", ModifiedDate = getdate(), ModifiedBy = " + iduser + " where IdGroup = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {

                        string note = islock ? "Khóa" : "Mở khóa";
                        var data = new LiteModel() { id = id, data = new { Locked = islock } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật - " + note, logHelper.LogSua(id, loginData.Id, "", note));
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

        #region quyền
        /// <summary>
        /// Update quyền
        /// </summary>
        /// <returns></returns>
        [HttpPost]
        [Route("update-quyen")]
        [Authorize(Roles = "23")]
        public BaseModel<object> UpdateQuyen([FromBody] QuyenModel data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                try
                {
                    cnn.BeginTransaction();
                    int kq = cnn.Delete(new SqlConditions() { { "IDGroupUser", data.IdGroup } }, "Dps_UserGroupRoles");
                    if (kq >= 0)
                    {
                        foreach (int quyen in data.Quyens)
                        {
                            Hashtable val = new Hashtable();
                            val["IDGroupUser"] = data.IdGroup;
                            val["IDGroupRole"] = quyen;
                            kq = cnn.Insert(val, "Dps_UserGroupRoles");
                            if (kq != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<QuyenModel>(data, loginData, "Thiết lập quyền cho vai trò", logHelper.Log(3, loginData.Id, "Thiết lập quyền cho vai trò", data.IdGroup));
                    return JsonResultCommon.ThanhCong();
                }
                catch (Exception ex)
                {
                    return JsonResultCommon.Exception(ex, ControllerContext);
                }
            }

        }
        #endregion
    }
}