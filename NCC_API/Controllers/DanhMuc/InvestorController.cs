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
    /// Quyền xem: 77	Quản lý investor
    /// quyền sửa: 78	Quản lý investor
    /// </summary>
    [ApiController]
    [Route("api/investor")]
    [EnableCors("TimensitPolicy")]
    public class InvestorController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "investor";
        public InvestorController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "investor",
                PKColumn = "ID",
                DisabledColumn = "is_deleted",
                name = "Chủ đầu tư"
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
                    string dieukienSort = "Name desc", dieukienWhere = " (Investor.is_deleted = 0)";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "ID", "ID"}, // " Giống biến model", "Tên cột"
                            { "Name", "Name"},
                            { "Email", "Email"},
                            { "Phone", "Phone"},
                            { "Address", "Address"},
                            { "CitizenID", "CitizenID"},
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
                    sqlq = @"select ID, Name, Investor.Email, Phone, Address, CitizenID
                            , is_deleted, created_date, creator
                            , edited_date, editor, fullname as nguoicapnhat
                            from Investor 
                            left join dps_user on dps_user.UserID = Investor.editor
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
                                   ID = r["ID"],
                                   Name = r["Name"],
                                   Email = r["Email"],
                                   Phone = r["Phone"],
                                   Address = r["Address"],
                                   edited_date = r["edited_date"],
                                   NguoiCapNhat = r["nguoicapnhat"],
                                   CitizenID = r["CitizenID"],
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
                    string sqlq = @"select * from Investor where (Investor.is_deleted = 0) and ID=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    var data = (from r in dt.AsEnumerable()
                                select new
                                {
                                    ID = r["ID"],
                                    Name = r["Name"],
                                    Email = r["Email"],
                                    Phone = r["Phone"],
                                    Address = r["Address"],
                                    CitizenID = r["CitizenID"],
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
        public async Task<object> Insert([FromBody] InvestorModel data)
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
                    val.Add("Name", data.Name);
                    val.Add("Email", data.Email);
                    if (data.Phone != "")
                        val.Add("Phone", data.Phone);
                    val.Add("Address", data.Address);
                    val.Add("CitizenID", data.CitizenID);
                    val.Add("created_date", DateTime.Now);
                    val.Add("creator", iduser);
                    cnn.BeginTransaction();
                    string sqlq = "select count(*) from Investor where is_deleted=0 and (Name = @Name or email = @Email)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.Name), { "Email", data.Email } }) > 0)
                        return JsonResultCommon.Trung("Tên hoặc email");
                    if (cnn.Insert(val, "Investor") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('chucvu')").ToString();
                    data.ID = long.Parse(idc);
                    logHelper.Ghilogfile<InvestorModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.ID, loginData.Id, Name), Name);
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
        public async Task<BaseModel<object>> Update(long id, [FromBody] InvestorModel data)
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
                    string sqlq = "select ISNULL((select count(*) from Investor where id = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    Hashtable val = new Hashtable();
                    if (data.Phone == null || data.Phone.ToString().Equals(""))
                        val.Add("Phone", DBNull.Value);
                    else
                        val.Add("Phone", data.Phone);
                    val.Add("Name", data.Name);
                    val.Add("Email", data.Email);
                    val.Add("Address", data.Address);
                    val.Add("edited_date", DateTime.Now);
                    val.Add("editor", iduser);
                    val.Add("CitizenID", data.CitizenID);
                    sqlq = "select count(*) from investor where is_deleted=0 and (Name = @Name or Email = @Email) and id<>@Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.Name), { "Email", data.Email }, { "Id", id } }) > 0)
                        return JsonResultCommon.Trung("Tên hoặc email");
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id", id);
                    if (cnn.Update(val, sqlcond, "Investor") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<InvestorModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.ID, loginData.Id, Name), Name);
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
                    string sqlq = "select * from Investor where id = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    #region check đã được sử dụng
                    //string msg = "";
                    //if (LiteController.InUse(cnn, FKs, id, out msg))
                    //    return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion

                    //sqlq = "update Investor set is_deleted = 1, deleted_date = getdate(),deleted_by = " + iduser + " where id = " + id;
                    sqlq = "exec ssp_Investor 3,'" + id + "','','','','','','" + iduser + "'";

                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["name"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);

                    DataTable dt = cnn.CreateDataTable(sqlq);
                    if (dt != null && dt.Rows.Count == 1)
                    {
                        if (dt.Rows[0][0].ToString().Equals("1"))
                        {
                            return JsonResultCommon.ThanhCong(true);
                        }
                        else
                        {
                            return JsonResultCommon.SQL(dt.Rows[0][1].ToString());
                        }
                    }
                    else
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }

                    //if (cnn.ExecuteNonQuery(sqlq) != 1)
                    //    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    //var data = new LiteModel() { id = id, title = dtF.Rows[0]["name"].ToString() };
                    //logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                    //return JsonResultCommon.ThanhCong(true);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
    }
}
