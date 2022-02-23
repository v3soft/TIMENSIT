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
    /// chức danh : chucvu
    /// Quyền xem: 77	Quản lý chức danh
    /// quyền sửa: 78	Quản lý chức danh
    /// </summary>
    [ApiController]
    [Route("api/chuc-danh")]
    [EnableCors("TimensitPolicy")]
    public class ChucDanhController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Chức danh";
        public ChucDanhController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "chucdanh",
                PKColumn = "id_cv",
                DisabledColumn = "Disable",
                name = "chức vụ"
            });
        }
        /// <summary>
        /// danh sách
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "77")]
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
                    string dieukienSort = "Cap desc", dieukienWhere = " (ChucVu.Disable = 0)";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "Id_CV", "id_cv"}, // " Giống biến model", "Tên cột"
                            { "TenCV", "TenCV"},
                            { "MaCV", "MaCV"},
                            { "Cap", "Cap"},
                            { "IsManager", "IsManager"},
                            { "IsTaiXe", "IsTaiXe"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    if (!User.IsInRole("78"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select Id_CV, MaCV, tencv, cap, ismanager, ngaycapnhat, fullname as nguoicapnhat, IsTaiXe from chucvu 
                        left join dps_user on dps_user.UserID = chucvu.nguoicapnhat
                        where " + dieukienWhere + @" order by " + dieukienSort;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, Visible);
                    pageModel.TotalCount = dem;
                    if (query.more)
                    {
                        query.page = 1;
                        query.record = pageModel.TotalCount;
                    }
                    pageModel.AllPage = (int)Math.Ceiling(dem / (decimal)query.record);
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
                                   Id_CV = r["id_cv"],
                                   TenCV = r["tencv"],
                                   MaCV = r["macv"],
                                   Cap = r["Cap"],
                                   IsManager = r["IsManager"],
                                   NgayCapNhat = r["ngaycapnhat"],
                                   NguoiCapNhat = r["nguoicapnhat"],
                                   IsTaiXe = r["IsTaiXe"],
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
        [Authorize(Roles = "77")]
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
                    if (!User.IsInRole("78"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    string sqlq = @"select * from chucvu where (ChucVu.Disable = 0) and id_cv=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    var data = (from r in dt.AsEnumerable()
                                select new
                                {
                                    Id_CV = r["id_cv"],
                                    TenCV = r["tencv"],
                                    MaCV = r["macv"],
                                    Cap = r["Cap"],
                                    IsManager = r["IsManager"],
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
        /// thêm
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "78")]
        [HttpPost]
        public async Task<object> Insert([FromBody] ChucDanhModel data)
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
                    Hashtable val = new Hashtable();
                    val.Add("macv", data.MaCV);
                    val.Add("tencv", data.TenCV);
                    if (data.Cap != "")
                        val.Add("cap", data.Cap);
                    val.Add("ismanager", data.IsManager);
                    val.Add("IsTaiXe", data.IsTaiXe);
                    cnn.BeginTransaction();
                    string sqlq = "select count(*) from chucvu where Disable=0 and (TenCV = @Name or macv = @MaChucVu)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.TenCV), { "MaChucVu", data.MaCV } }) > 0)
                        return JsonResultCommon.Trung("Chức danh hoặc mã");
                    if (cnn.Insert(val, "chucvu") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('chucvu')").ToString();
                    data.id_cv = long.Parse(idc);
                    logHelper.Ghilogfile<ChucDanhModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.id_cv, loginData.Id, Name), Name);
                    cnn.EndTransaction();
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
        [Authorize(Roles = "78")]
        [HttpPut("{id}")]
        public async Task<BaseModel<object>> Update(long id, [FromBody] ChucDanhModel data)
        {
            BaseModel<ChucDanhModel> model = new BaseModel<ChucDanhModel>();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    string sqlq = "select ISNULL((select count(*) from chucvu where id_cv = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    Hashtable val = new Hashtable();
                    int cap = 0;
                    if (data.Cap == null || data.Cap.ToString().Equals(""))
                    {

                    }
                    else
                    {
                        if ((!int.TryParse(data.Cap, out cap)))
                            return JsonResultCommon.Custom("Cấp không hợp lệ");
                    }
                    if (data.Cap == null || data.Cap.ToString().Equals(""))
                        val.Add("Cap", DBNull.Value);
                    else
                        val.Add("cap", cap);
                    val.Add("macv", data.MaCV);
                    val.Add("tencv", data.TenCV);
                    val.Add("ismanager", data.IsManager);
                    val.Add("ngaycapnhat", DateTime.Now);
                    val.Add("nguoicapnhat", iduser);
                    val.Add("IsTaiXe", data.IsTaiXe);
                    sqlq = "select count(*) from chucvu where Disable=0 and (TenCV = @Name or macv = @MaChucVu) and id_cv<>@Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.TenCV), { "MaChucVu", data.MaCV }, { "Id", id } }) > 0)
                        return JsonResultCommon.Trung("Chức danh hoặc mã");
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id_cv", id);
                    if (cnn.Update(val, sqlcond, "chucvu") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<ChucDanhModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.id_cv, loginData.Id, Name), Name);
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
        [Authorize(Roles = "78")]
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
                    string sqlq = "select * from chucvu where id_cv = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "update chucvu set Disable = 1, DeletedDate = getdate(),DeletedBy = " + iduser + " where id_cv = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["TenCV"].ToString() };
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
