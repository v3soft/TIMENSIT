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
    /// Danh mục Đợt tặng quà
    /// Quyền Xem: 22- Quản lý
    /// Quyền sửa: 20-Cập nhật
    /// </summary>
    [ApiController]
    [Route("api/cap-quan-ly")]
    [EnableCors("TimensitPolicy")]
    public class CapQuanLyController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Cấp quản lý";
        public CapQuanLyController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "tbl_chucdanh",
                PKColumn = "Id_Capquanly",
                DisabledColumn = "Disable",
                name = "Chức danh trong sơ đồ tổ chức"
            });
        }

        [Authorize(Roles = "22")]
        [HttpGet]
        public object List([FromQuery] QueryParams query)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            bool Visible = true;
            string sqlq = "";
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string dieukienSort = "Title", dieukienWhere = " 1=1 ";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "RowID", "RowID"}, // " Giống biến model", "Tên cột"
                            { "Title", "Title"},
                            { "Range", "Range"},
                            { "Summary", "Summary"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    if (!User.IsInRole("20"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select RowID, Title, range, summary, dm_capquanly.lastmodified, fullname as nguoicapnhat from dm_capquanly 
left join Dps_user on Dps_user.UserID = dm_capquanly.ModifiedBy
where " + dieukienWhere+ " order by "+dieukienSort;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var temp = dt.AsEnumerable();
                    if (!string.IsNullOrEmpty(query.filter["Title"]))
                    {
                        string keyword = query.filter["Title"].ToLower();
                        temp = temp.Where(x => x["Title"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["summary"]))
                    {
                        string keyword = query.filter["summary"].ToLower();
                        temp = temp.Where(x => x["summary"].ToString().ToLower().Contains(keyword));
                    }
                    int dem = temp.Count();
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
                    var temp1 = temp.Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel());
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   RowID = r["RowID"],
                                   Title = r["Title"],
                                   Range = r["Range"],
                                   Summary = r["Summary"],
                                   NgayCapNhat = r["LastModified"],
                                   NguoiCapNhat = r["nguoicapnhat"],
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Detail
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "22")]
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
                    if (!User.IsInRole("20"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    string sqlq = @"select * from dm_capquanly where RowID=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    var data = (from r in dt.AsEnumerable()
                                select new
                                {
                                    RowID = r["RowID"],
                                    Title = r["Title"],
                                    Range = r["Range"],
                                    Summary = r["Summary"],
                                    AllowEdit = Visible
                                }).FirstOrDefault();
                    return JsonResultCommon.ThanhCong(data);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Tạo
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "20")]
        [HttpPost]
        public async Task<object> Insert([FromBody] DMCapQuanLyModel data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {

                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("Title", data.Title);
                    val.Add("Range", data.Range);
                    val.Add("Summary", data.Summary);
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", iduser);
                    SqlConditions conds = new SqlConditions();
                    conds.Add("Title", data.Title);
                    conds.Add("Range", data.Range);
                    string select = "select * from dm_capquanly where (where)";
                    DataTable dt = cnn.CreateDataTable(select, "(where)", conds);
                    if (dt.Rows.Count > 0)
                        return JsonResultCommon.Trung("Cấp và Tên cấp quản lý");
                    if (cnn.Insert(val, "DM_Capquanly") != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('DM_Capquanly')").ToString();
                    data.Rowid = int.Parse(idc);
                    logHelper.Ghilogfile<DMCapQuanLyModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Rowid, loginData.Id, Name),Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Sửa
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "20")]
        [HttpPut("{id}")]
        public object Update(long id, [FromBody] DMCapQuanLyModel data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("Title", data.Title);
                    val.Add("Range", data.Range);
                    val.Add("Summary", data.Summary);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("ModifiedBy", iduser);
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("rowid", data.Rowid);
                    string s = "select title, summary, range from dm_capquanly where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    SqlConditions conds = new SqlConditions();
                    conds.Add("Title", data.Title);
                    conds.Add("Range", data.Range);
                    conds.Add("rowid", data.Rowid, SqlOperator.NotEquals);
                    string select = "select * from dm_capquanly where (where)";
                    DataTable dt = new DataTable();
                    dt = cnn.CreateDataTable(select, "(where)", conds);
                    if (dt.Rows.Count > 0)
                        return JsonResultCommon.Trung("Cấp và Tên cấp quản lý");
                    if (cnn.Update(val, sqlcond, "DM_Capquanly") != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    logHelper.Ghilogfile<DMCapQuanLyModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.Rowid, loginData.Id, Name,Name));
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Xóa 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "20")]
        [HttpDelete("{id}")]
        public object Delete(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                long iduser = loginData.Id;
                ErrorModel error = new ErrorModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_Capquanly where rowid = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "delete DM_Capquanly where rowid = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["Title"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name),Name);
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
    }
}
