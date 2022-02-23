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
    /// Danh mục đối tượng người có công
    /// Quyền xem: 1 - Quản lý danh mục đối tượng người có công
    /// Quyền sửa: 2 - Cập nhật danh mục đối tượng người có công
    /// </summary>
    [ApiController]
    [Route("api/doi-tuong-ncc")]
    [EnableCors("TimensitPolicy")]
    public class DoiTuongNCCController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Đối tượng người có công";
        public DoiTuongNCCController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "tbl_ncc",
                PKColumn = "Id_DoiTuongNCC",
                DisabledColumn = "Disabled",
                name = "Người có công"
            });
        }
        /// <summary>
        /// Danh sách
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "1")]
        [HttpGet]
        public BaseModel<object> List([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                bool allowEdit = User.IsInRole("2");
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select dm.*, u.FullName as NguoiTao, u1.FullName as NguoiSua from DM_DOITUONGNCC dm
join Dps_User u on dm.CreatedBy=u.UserID
left join Dps_User u1 on dm.UpdatedBy=u1.UserID
where dm.Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "DoiTuong","DoiTuong" },
                        { "MaDoiTuong","MaDoiTuong" },
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
                    if (!string.IsNullOrEmpty(query.filter["DoiTuong"]))
                    {
                        string keyword = query.filter["DoiTuong"].ToLower();
                        temp = temp.Where(x => x["DoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaDoiTuong"]))
                    {
                        string keyword = query.filter["MaDoiTuong"].ToLower();
                        temp = temp.Where(x => x["MaDoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    }
                    if (query.filterGroup != null && query.filterGroup["NhomLoaiDoiTuongNCC"] != null && query.filterGroup["NhomLoaiDoiTuongNCC"].Length > 0)
                    {
                        var groups = query.filterGroup["NhomLoaiDoiTuongNCC"].ToList();
                        temp = temp.Where(x => groups.Contains(x["NhomLoaiDoiTuongNCC"].ToString()));
                    }
                    #endregion
                    int i = temp.Count();
                    if (i == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, allowEdit);
                    dt = temp.CopyToDataTable();
                    int total = dt.Rows.Count;
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
                    var dtTemp = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (dtTemp.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, allowEdit);
                    dt = dtTemp.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {

                                   Id = r["Id"],

                                   DoiTuong = r["DoiTuong"],

                                   MaDoiTuong = r["MaDoiTuong"],

                                   MoTa = r["MoTa"],

                                   Id_LoaiQuyetDinh = r["Id_LoaiQuyetDinh"],

                                   Id_Template = r["Id_Template"],

                                   Id_Template_DiChuyen = r["Id_Template_DiChuyen"],

                                   Id_Template_ThanNhan = r["Id_Template_ThanNhan"],

                                   Id_Template_CongNhan = r["Id_Template_CongNhan"],

                                   Locked = r["Locked"],

                                   Priority = r["Priority"],

                                   CreatedBy = r["NguoiTao"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
                                   UpdatedBy = r["NguoiSua"],
                                   UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["UpdatedDate"]),

                                   Loai = r["Loai"],
                                   NhomLoaiDoiTuongNCC = LiteHelper.GetLiteById(r["Loai"], LiteHelper.NhomLoaiDoiTuongNCC)
                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel, allowEdit);
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
        [Authorize(Roles = "1")]
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
                    string sqlq = @"select * from DM_DOITUONGNCC where Disabled=0 and Id = @Id";
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

                                                           DoiTuong = r["DoiTuong"],

                                                           MaDoiTuong = r["MaDoiTuong"],

                                                           Id_LoaiQuyetDinh = r["Id_LoaiQuyetDinh"],

                                                           Id_Template = r["Id_Template"],

                                                           Id_Template_DiChuyen = r["Id_Template_DiChuyen"],

                                                           Id_Template_ThanNhan = r["Id_Template_ThanNhan"],

                                                           Id_Template_CongNhan = r["Id_Template_CongNhan"],

                                                           IsThanNhan = r["IsThanNhan"],

                                                           MoTa = r["MoTa"],

                                                           Locked = r["Locked"],

                                                           Priority = r["Priority"],
                                                           Loai = r["Loai"],
                                                           NhomLoaiDoiTuongNCC = LiteHelper.GetLiteById(r["Loai"], LiteHelper.NhomLoaiDoiTuongNCC),
                                                           AllowEdit = User.IsInRole("2")

                                                       }).FirstOrDefault());
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
        [Authorize(Roles = "2")]
        [HttpPost]
        public BaseModel<object> Create([FromBody] DoiTuongNCCModel data)
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
                if (string.IsNullOrEmpty(data.DoiTuong))
                    strRe += (strRe == "" ? "" : ", ") + "Đối tượng";
                if (string.IsNullOrEmpty(data.MaDoiTuong))
                    strRe += (strRe == "" ? "" : ", ") + "Mã đối tượng";
                if (data.Id_LoaiQuyetDinh <= 0)
                    strRe += (strRe == "" ? "" : ", ") + "Nhóm quyết định";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from DM_DOITUONGNCC where Disabled=0 and (DoiTuong = @Name or MaDoiTuong=@Ma)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "Ma", data.MaDoiTuong }, new SqlCondition("Name", data.DoiTuong) }) > 0)
                        return JsonResultCommon.Trung("Tên hoặc mã đối tượng");

                    Hashtable val = new Hashtable();
                    val.Add("DoiTuong", data.DoiTuong);
                    val.Add("MaDoiTuong", data.MaDoiTuong);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    val.Add("Locked", false);
                    val.Add("IsThanNhan", data.IsThanNhan);
                    val.Add("Id_LoaiQuyetDinh", data.Id_LoaiQuyetDinh);

                    if (data.Priority > 0)
                        val.Add("Priority", data.Priority);
                    else
                        val.Add("Priority", 1);
                    if (data.Loai.HasValue)
                    {
                        if (LiteHelper.GetLiteById(data.Loai, LiteHelper.NhomLoaiDoiTuongNCC) == "")
                            return JsonResultCommon.Custom("Nhóm đối tượng người có công không hợp lệ");
                        val.Add("Loai", data.Loai);
                    }
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", loginData.Id);

                    if (cnn.Insert(val, "DM_DOITUONGNCC") == 1)
                    {
                        data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('DM_DOITUONGNCC') AS Current_Identity; ").ToString());
                        logHelper.Ghilogfile<DoiTuongNCCModel>(data, loginData, "Thêm mới", logHelper.LogThem((int)data.Id, loginData.Id, Name), Name);
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
        [Authorize(Roles = "2")]
        [HttpPut("{id}")]
        public BaseModel<object> Update(int id, [FromBody] DoiTuongNCCModel data)
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
                if (string.IsNullOrEmpty(data.DoiTuong))
                    strRe += (strRe == "" ? "" : ", ") + "Đối tượng";
                if (string.IsNullOrEmpty(data.MaDoiTuong))
                    strRe += (strRe == "" ? "" : ", ") + "Mã đối tượng";

                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_DOITUONGNCC where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from DM_DOITUONGNCC where Disabled=0 and (DoiTuong = @Name or MaDoiTuong=@Ma) and Id <> @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "Ma", data.MaDoiTuong }, { "Id", id }, new SqlCondition("Name", data.DoiTuong) }) > 0)
                        return JsonResultCommon.Trung("Tên hoặc mã đối tượng");

                    Hashtable val = new Hashtable();
                    val.Add("DoiTuong", data.DoiTuong);
                    val.Add("MaDoiTuong", data.MaDoiTuong);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    val.Add("Locked", false);
                    val.Add("Loai", data.Loai);
                    val.Add("IsThanNhan", data.IsThanNhan);
                    val.Add("Id_LoaiQuyetDinh", data.Id_LoaiQuyetDinh);
                    val.Add("Priority", data.Priority);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);

                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "DM_DOITUONGNCC") == 1)
                    {
                        logHelper.Ghilogfile<DoiTuongNCCModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
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
        /// Update biểu mẫu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>

        [Authorize(Roles = "3")]
        [HttpPut("update-bieu-mau/{id}")]
        public BaseModel<object> UpdateBM(int id, [FromBody] DoiTuongNCCBieuMauModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_DoituongNCC where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    Hashtable val = new Hashtable();
                    if (data.Id_Template > 0)
                        val.Add("Id_Template", data.Id_Template);
                    else
                    {
                        val.Add("Id_Template", DBNull.Value);
                    }
                    if (data.Id_Template_DiChuyen > 0)
                        val.Add("Id_Template_DiChuyen", data.Id_Template_DiChuyen);
                    else
                    {
                        val.Add("Id_Template_DiChuyen", DBNull.Value);
                    }
                    if (data.Id_Template_CongNhan > 0)
                        val.Add("Id_Template_CongNhan", data.Id_Template_CongNhan);
                    else
                    {
                        val.Add("Id_Template_CongNhan", DBNull.Value);
                    }
                    if (data.Id_Template_ThanNhan > 0)
                        val.Add("Id_Template_ThanNhan", data.Id_Template_ThanNhan);
                    else
                    {
                        val.Add("Id_Template_ThanNhan", DBNull.Value);
                    }

                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);
                    cnn.BeginTransaction();
                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "DM_DoiTuongNCC") == 1)
                    {
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<DoiTuongNCCBieuMauModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        cnn.RollbackTransaction();
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
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "2")]
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
                    string sqlq = "select * from DM_DOITUONGNCC where Disabled = 0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "update DM_DOITUONGNCC set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["DoiTuong"].ToString() };
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
        [Authorize(Roles = "2")]
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
                    string sqlq = "select count(*) from DM_DOITUONGNCC where Disabled = 0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "";

                    sqlq = "update DM_DOITUONGNCC set Locked = " + (Value ? 1 : 0) + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string note = Value ? "Khóa" : "Mở khóa";
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