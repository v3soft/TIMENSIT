using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Timensit_API.Controllers.Common;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Timensit_API.Models.DanhMuc;


namespace Timensit_API.Controllers.DanhMuc
{
    /// <summary>
    /// Chức vụ: chucdanh
    /// Quyền xem: 79	Quản lý chức vụ
    /// Quyền sửa: 80	Cập nhật chức vụ
    /// </summary>
    [ApiController]
    [Route("api/chuc-vu")]
    [EnableCors("TimensitPolicy")]
    public class ChucVuController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Chức vụ";
        public ChucVuController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "tbl_chucdanh",
                PKColumn = "nhom",
                DisabledColumn = "Disable",
                name = "Sơ đồ tổ chức"
            });
        }
        /// <summary>
        /// Danh sách
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "79")]
        [HttpGet]
        public object List([FromQuery] QueryParams query)// (bool more = false, int? page = 1, int? record = 10)
        {
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            bool Visible = true;
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string sqlq = "";
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    string dieukienSort = "tenchucdanh", dieukienWhere = " chucdanh.disable = 0 ";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "Id_row", "Id_row"}, // " Giống biến model", "Tên cột"
                            { "Tenchucdanh", "Tenchucdanh"},
                            { "Tentienganh", "Tentienganh"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        dieukienWhere += " and (chucdanh.tenchucdanh like @kw)";
                        Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                    }
                    if (!string.IsNullOrEmpty(query.filter["Id_CV"]))
                    {
                        dieukienWhere += " and Chucdanh.id_cv = @id_cv";
                        Conds.Add("id_cv", query.filter["Id_CV"]);
                    }
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    if (!User.IsInRole("80"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select fullname as nguoicapnhat, chucdanh.LastUpdate, islanhdao, isnull(Tb1.SoLuong,0) as Soluong, Chucdanh.Tenchucdanh, ChucVu.TenCV, Chucdanh.Id_row, Chucdanh.Id_cv,Chucdanh.tentienganh
                        from chucdanh left join  (SELECT COUNT(Id_row) AS SoLuong, Nhom
                        from Tbl_Chucdanh where (Disable = 0)
                        group by nhom) AS Tb1 ON Chucdanh.Id_row = Tb1.Nhom INNER JOIN
                        ChucVu ON Chucdanh.Id_cv = ChucVu.Id_CV 
                        left join dps_user on dps_user.UserID = chucdanh.NguoiCapNhat
                        where " + dieukienWhere + @" order by " + dieukienSort;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    int total = dt.Rows.Count;
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
                    var temp1 = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id_row = r["Id_row"],
                                   Tenchucdanh = r["Tenchucdanh"],
                                   Tentienganh = r["Tentienganh"],
                                   Id_CV = r["Id_CV"],
                                   IsLanhDao = r["IsLanhDao"],
                                   NgayCapNhat = r["LastUpdate"],
                                   NguoiCapNhat = r["nguoicapnhat"],
                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "79")]
        [HttpGet("{id}")]
        public object Detail(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            bool Visible = true;
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    #region Xét quyền xem/chỉnh sửa
                    if (!User.IsInRole("80"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    string sqlq = @"select * from chucdanh where (chucdanh.Disable = 0) and Id_row=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    var data = (from r in dt.AsEnumerable()
                                select new
                                {
                                    Id_row = r["Id_row"],
                                    Tenchucdanh = r["Tenchucdanh"],
                                    Tentienganh = r["Tentienganh"],
                                    Id_CV = r["Id_CV"],
                                    IsLanhDao = r["IsLanhDao"],
                                    AllowEdit = Visible
                                }).FirstOrDefault();
                    return JsonResultCommon.ThanhCong(data);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// Thêm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "79")]
        [HttpPost]
        public async Task<object> Insert([FromBody] ChucVuModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    string strCheck = "select count(*) from chucdanh where disable=0 and tenchucdanh = @tenchucdanh";
                    var temp = cnn.ExecuteScalar(strCheck, new SqlConditions() { { "tenchucdanh", data.Tenchucdanh } });
                    if (int.Parse(temp.ToString()) > 0)
                        return JsonResultCommon.Trung(Name);
                    Hashtable val = new Hashtable();
                    val.Add("id_cv", data.Id_CV);
                    val.Add("tenchucdanh", data.Tenchucdanh);
                    if (data.Tentienganh != null || data.Tentienganh == string.Empty)
                        val.Add("tentienganh", data.Tentienganh);
                    else
                        val.Add("tentienganh", DBNull.Value);
                    val.Add("islanhdao", data.IsLanhDao);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "chucdanh") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('chucdanh')").ToString();
                    cnn.EndTransaction();
                    data.Id_row = int.Parse(idc);
                    logHelper.Ghilogfile<ChucVuModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Sửa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "80")]
        [HttpPut("{id}")]
        public async Task<BaseModel<object>> Update(long id, [FromBody] ChucVuModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    string sqlq = "select ISNULL((select count(*) from chucdanh where id_row = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    Hashtable val = new Hashtable();
                    val.Add("id_cv", data.Id_CV);
                    val.Add("tenchucdanh", data.Tenchucdanh);
                    if (string.IsNullOrEmpty(data.Tentienganh))
                        val.Add("tentienganh", DBNull.Value);
                    else
                        val.Add("tentienganh", data.Tentienganh);
                    val.Add("LastUpdate", DateTime.Now);
                    val.Add("nguoicapnhat", iduser);
                    val.Add("islanhdao", data.IsLanhDao);
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id_row", id);
                    string strCheck = " select count(*) from chucdanh where disable=0 and id_row<>@idrow and tenchucdanh = @tenchucdanh";
                    var temp = cnn.ExecuteScalar(strCheck, new SqlConditions() { { "idrow", id }, { "tenchucdanh", data.Tenchucdanh } });
                    if (int.Parse(temp.ToString()) > 0)
                        return JsonResultCommon.Trung(Name);
                    if (cnn.Update(val, sqlcond, "chucdanh") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<ChucVuModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.Id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
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
        [Authorize(Roles = "80")]
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(long id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                long iduser = loginData.Id;
                ErrorModel error = new ErrorModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from chucdanh where id_row = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "update chucdanh set Disable = 1, DeletedDate = getdate(),DeletedBy = " + iduser + " where id_row = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["tenchucdanh"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(true);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
    }
}
