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
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.DanhMuc
{
    /// <summary>
    /// Quản lý danh mục khác
    /// Quyền 1: Xem thông tin
    /// Quyền 2: cập nhật thông tin
    /// </summary>
    [ApiController]
    [Route("api/_chucvu")]
    [EnableCors("TimensitPolicy")]
    public class _ChucVuController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Chức vụ";
        public _ChucVuController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironmen)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironmen, 2);
            FKs = new List<CheckFKModel>();
            //văn bản
            FKs.Add(new CheckFKModel
            {
                TableName = "DM_ChucVu",
                PKColumn = "IdParent",
                DisabledColumn = "Disabled",
                name = "Chức vụ"
            });
            FKs.Add(new CheckFKModel
            {
                TableName = "Dps_User",
                PKColumn = "IdChucVu",
                DisabledColumn = "Disabled",
                name = "Người dùng"
            });
        }
        /// <summary>
        /// Danh sách Danh mục khác
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "36")]
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
                bool Visible = User.IsInRole("37");
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select DM_ChucVu.Id,DM_ChucVu.IdParent, DM_ChucVu.ChucVu, DM_ChucVu.MaChucVu, DM_ChucVu.MoTa, DM_ChucVu.DonVi, DM_DonVi.DonVi as 'TenDonVi', DM_ChucVu.Locked, DM_ChucVu.Priority, DM_ChucVu.MoTa from DM_ChucVu, DM_DonVi
                                    where DM_ChucVu.Disabled = 0
                                    and DM_ChucVu.DonVi = DM_DonVi.Id";
                    if (!string.IsNullOrEmpty(query.filter["IdParent"]))
                        sqlq += string.Format(" and (DM_ChucVu.Id={0} or IdParent={0})", query.filter["IdParent"]);
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    temp = temp.OrderBy(x => x["Priority"]);
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                            { "ChucVu","ChucVu" },
                            { "MaChucVu","MaChucVu" },
                            { "TenDonVi","TenDonVi" },
                            { "Locked","Locked" },
                            { "Priority","Priority" },
                            { "MoTa","MoTa" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["DonVi"]))
                    {
                        string keyword = query.filter["DonVi"].ToLower();
                        temp = temp.Where(x => x["DonVi"].ToString() == keyword);
                    }
                    if (!string.IsNullOrEmpty(query.filter["ChucVu"]))
                    {
                        string keyword = query.filter["ChucVu"].ToLower();
                        temp = temp.Where(x => x["ChucVu"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaChucVu"]))
                    {
                        string keyword = query.filter["MaChucVu"].ToLower();
                        temp = temp.Where(x => x["MaChucVu"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString().ToLower()));
                    }

                    #endregion
                    int total = temp.Count();
                    if (total == 0)
                        return new BaseModel<object>
                        {
                            status = 1,
                            data = new List<string>(),
                            page = pageModel,
                            Visible = Visible
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
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["Id"],
                                   ChucVu = r["ChucVu"],
                                   MaChucVu = r["MaChucVu"],
                                   DonVi = r["DonVi"],
                                   Locked = r["Locked"],
                                   Priority = r["Priority"],
                                   TenDonVi = r["TenDonVi"],
                                   IdParent = r["IdParent"],
                                   MoTa = r["MoTa"]
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
        /// Chi tiết Danh mục khác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "36")]
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
                    string sqlq = @"select * from DM_ChucVu where Disabled=0 and Id = @Id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {
                                                           Id = r["Id"],
                                                           ChucVu = r["ChucVu"],
                                                           MaChucVu = r["MaChucVu"],
                                                           DonVi = r["DonVi"],
                                                           Locked = r["Locked"],
                                                           Priority = r["Priority"],
                                                           MoTa = r["MoTa"],
                                                           IdParent = r["IdParent"],
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
        [Authorize(Roles = "37")]
        [Route("create")]
        [HttpPost]
        public BaseModel<object> Create([FromBody] _ChucVuModel data)
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
                if (string.IsNullOrEmpty(data.ChucVu))
                    strRe += (strRe == "" ? "" : ", ") + "Chức vụ";
                if (string.IsNullOrEmpty(data.MaChucVu))
                    strRe += (strRe == "" ? "" : ", ") + "Mã chức vụ";
                if (data.DonVi <= 0)
                    strRe += (strRe == "" ? "" : ", ") + "đơn vị";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from DM_ChucVu where Disabled=0 and DonVi=@DonVi and (ChucVu = @Name or MaChucVu = @MaChucVu)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.ChucVu), { "MaChucVu", data.MaChucVu }, { "DonVi", data.DonVi } }) > 0)
                        return JsonResultCommon.Trung("Chức vụ hoặc mã");

                    #region check khóa ngoại
                    List<CheckFKModel> models = new List<CheckFKModel>();
                    models.Add(new CheckFKModel
                    {
                        TableName = "DM_DonVi",
                        PKColumn = "Id",
                        Id = data.DonVi.Value,
                        DisabledColumn = "Disabled",
                        LockedColumn = "Locked",
                        name = "Đơn vị"
                    });
                    if (data.IdParent > 0)
                    {
                        models.Add(new CheckFKModel
                        {
                            TableName = "DM_ChucVu",
                            PKColumn = "Id",
                            Id = data.IdParent.Value,
                            DisabledColumn = "Disabled",
                            LockedColumn = "Locked",
                            name = "Chức vụ lãnh đạo"
                        });
                    }
                    string msg = "";
                    if (!LiteController.CheckPF(cnn, models, out msg))
                    {
                        return JsonResultCommon.Custom("Giá trị các trường " + msg + " không tồn tại hoặc đã bị khóa");
                    }
                    #endregion
                    {
                        Hashtable val = new Hashtable();
                        if (data.Id > 0)
                            val.Add("Id", data.Id);
                        if (!string.IsNullOrEmpty(data.ChucVu))
                            val.Add("ChucVu", data.ChucVu);
                        if (!string.IsNullOrEmpty(data.MaChucVu))
                            val.Add("MaChucVu", data.MaChucVu);
                        if (!string.IsNullOrEmpty(data.MoTa))
                            val.Add("MoTa", data.MoTa);
                        val.Add("DonVi", data.DonVi);
                        val.Add("Locked", data.Locked);
                        val.Add("CreatedBy", iduser);
                        val.Add("CreatedDate", DateTime.Now);
                        if (data.IdParent > 0)
                            val.Add("IdParent", data.IdParent);

                        if (data.Priority > 0)
                            val.Add("Priority", data.Priority);
                        if (cnn.Insert(val, "DM_ChucVu") == 1)
                        {
                            logHelper.Ghilogfile<_ChucVuModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
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
        /// Cập nhật thông tin Danh mục khác
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "37")]
        [Route("update")]
        [HttpPost]
        public BaseModel<object> Update([FromBody] _ChucVuModel data)
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
                if (string.IsNullOrEmpty(data.ChucVu))
                    strRe += (strRe == "" ? "" : ", ") + "Chức vụ";
                if (string.IsNullOrEmpty(data.MaChucVu))
                    strRe += (strRe == "" ? "" : ", ") + "Mã chức vụ";
                if (data.DonVi <= 0)
                    strRe += (strRe == "" ? "" : ", ") + "đơn vị";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_ChucVu where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", data.Id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from DM_ChucVu where Disabled=0 and Id <> @Id and DonVi=@DonVi and (ChucVu = @Name or MaChucVu = @MaChucVu)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.ChucVu), { "MaChucVu", data.MaChucVu }, { "DonVi", data.DonVi }, { "Id", data.Id } }) > 0)
                        return JsonResultCommon.Trung("Chức vụ hoặc mã");
                    #region check khóa ngoại
                    List<CheckFKModel> models = new List<CheckFKModel>();
                    models.Add(new CheckFKModel
                    {
                        TableName = "DM_DonVi",
                        PKColumn = "Id",
                        Id = data.DonVi.Value,
                        DisabledColumn = "Disabled",
                        LockedColumn = "Locked",
                        name = "Đơn vị"
                    });
                    if (data.IdParent > 0)
                    {
                        models.Add(new CheckFKModel
                        {
                            TableName = "DM_ChucVu",
                            PKColumn = "Id",
                            Id = data.IdParent.Value,
                            DisabledColumn = "Disabled",
                            LockedColumn = "Locked",
                            name = "Chức vụ lãnh đạo"
                        });
                    }
                    string msg = "";
                    if (!LiteController.CheckPF(cnn, models, out msg))
                    {
                        return JsonResultCommon.Custom("Giá trị các trường " + msg + " không tồn tại hoặc đã bị khóa");
                    }
                    #endregion
                    #region check đã được sử dụng
                    if (data.DonVi.ToString() != dtFind.Rows[0]["DonVi"].ToString())
                    {
                        if (LiteController.InUse(cnn, FKs, data.Id, out msg))
                        {
                            return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể thay đổi đơn vị");
                        }
                    }
                    #endregion
                    Hashtable val = new Hashtable();
                    if (!string.IsNullOrEmpty(data.ChucVu))
                        val.Add("ChucVu", data.ChucVu);
                    if (!string.IsNullOrEmpty(data.MaChucVu))
                        val.Add("MaChucVu", data.MaChucVu);
                    if (!string.IsNullOrEmpty(data.MoTa))
                        val.Add("MoTa", data.MoTa);
                    val.Add("DonVi", data.DonVi);
                    val.Add("Locked", data.Locked);
                    val.Add("UpdatedBy", iduser);
                    val.Add("UpdatedDate", DateTime.Now);
                    if (data.IdParent > 0)
                        val.Add("IdParent", data.IdParent);
                    else
                        val.Add("IdParent", DBNull.Value);
                    if (data.Priority > 0)
                        val.Add("Priority", data.Priority);
                    if (cnn.Update(val, new SqlConditions { { "Id", data.Id } }, "DM_ChucVu") == 1)
                    {
                        logHelper.Ghilogfile<_ChucVuModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.Id, loginData.Id, Name), Name);
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
        /// Xóa chức vụ
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "37")]
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
                    string sqlq = "select * from DM_ChucVu where Disabled = 0 and Id = @Id";
                    var dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                    {
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    }
                    #endregion
                    //sqlq = "select count(*) from DM_ChucVu where Disabled = 0 and IdParent = @Id";
                    //if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) > 0)
                    //    return JsonResultCommon.Custom("Tồn tại chức vụ con, không thể xóa");

                    sqlq = "update DM_ChucVu set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {

                        var data = new LiteModel() { id = id };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa((int)id, loginData.Id, Name), Name);
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
        /// Xóa Danh mục khác
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "37")]
        [Route("lock")]
        [HttpGet]
        public BaseModel<object> Lock(long id, bool isLock)
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
                    string sqlq = "select count(*) from DM_ChucVu where Disabled = 0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "";

                    sqlq = "update DM_ChucVu set Locked = " + (isLock ? 1 : 0) + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string note = isLock ? "Khóa" : "Mở khóa";
                        var data = new LiteModel() { id = id, data = new { Locked = isLock } };
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