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
    /// Liên hệ: contact
    /// Quyền xem: 79	Quản lý NAV
    /// Quyền sửa: 80	Cập nhật NAV
    /// </summary>
    [ApiController]
    [Route("api/nav")]
    [EnableCors("TimensitPolicy")]
    public class NAVController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "NAV";
        public NAVController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "Contract",
                PKColumn = "InvestorID",
                DisabledColumn = "is_deleted",
                name = "Contract"
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
                    string dieukienSort = "ContractCode", dieukienWhere = "nav.is_deleted = 0 and inv.is_deleted = 0 and ct.is_deleted = 0";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "ID", "ID"}, // " Giống biến model", "Tên cột"
                            { "ContractCode", "ContractCode"},
                            { "Amount", "Amount"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        dieukienWhere += " and (ct.ContractCode like @kw)";
                        Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                    }
                    if (!string.IsNullOrEmpty(query.filter["InvestorID"]))
                    {
                        dieukienWhere += " and ct.InvestorID = @InvestorID";
                        Conds.Add("InvestorID", query.filter["InvestorID"]);
                    }
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    if (!User.IsInRole("80"))
                        Visible = false;
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select fullname as nguoicapnhat, nav.*, ContractCode, inv.Name as Investor_Name, InvestorID 
                            from nav left join contract ct 
                            on ct.ID = nav.ContractID
                            join Investor inv 
                            on inv.ID = ct.InvestorID
                            left join dps_user on dps_user.userid = ct.editor
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
                                   Id_row = r["id"],
                                   ContractCode = r["ContractCode"],
                                   Amount = r["Amount"],
                                   InvestorID = r["InvestorID"],
                                   RecordDate = string.Format("{0:dd/MM/yyyy}", r["RecordDate"]),
                                   Investor_Name = r["Investor_Name"],
                                   Type = r["Type"],
                                   NgayCapNhat = r["edited_date"],
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
                    string sqlq = @"select fullname as nguoicapnhat, nav.*, ContractCode, inv.Name as Investor_Name, InvestorID 
                            from nav left join contract ct 
                            on ct.ID = nav.ContractID
                            join Investor inv 
                            on inv.ID = ct.InvestorID
                            left join dps_user on dps_user.userid = ct.editor
                        where (nav.is_deleted = 0) and nav.id=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    int dem = dt.Rows.Count;
                    if (dem == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    var data = (from r in dt.AsEnumerable()
                                select new
                                {
                                    Id_row = r["id"],
                                    ContractCode = r["ContractCode"],
                                    Amount = r["Amount"],
                                    InvestorID = r["InvestorID"],
                                    RecordDate = r["RecordDate"],
                                    Investor_Name = r["Investor_Name"],
                                    Type = r["Type"],
                                    NgayCapNhat = r["edited_date"],
                                    NguoiCapNhat = r["nguoicapnhat"],
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
        public async Task<object> Insert([FromBody] NAVModel data)
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
                    if (data.EffectiveDate.HasValue && data.EffectiveDate.Value > DateTime.MinValue)
                        val.Add("RecordDate", data.EffectiveDate);
                    val.Add("Type", string.IsNullOrEmpty(data.Type) ? "" : data.Type);
                    val.Add("Amount", data.Amount);
                    val.Add("created_date", DateTime.Now);
                    val.Add("creator", iduser);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "NAV") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('NAV')").ToString();
                    // Gọi hàm tính NAV
                    SqlConditions conds = new SqlConditions();
                    conds.Add("cnt", 1);
                    conds.Add("cnt_total", 2);
                    conds.Add("Today", DateTime.Now);
                    conds.Add("Yesterday", DateTime.Now);
                    string sqlq = "exec ssp_NAV.Recalc @cnt, @cnt_total, @Today, @Yesterday";
                    DataTable Tb = cnn.CreateDataTable(sqlq, conds);
                    if (Tb == null || cnn.LastError != null)
                    {
                        return JsonResultCommon.SQL(cnn.LastError.Message);
                    }
                    cnn.EndTransaction();
                    data.ID = int.Parse(idc);
                    logHelper.Ghilogfile<NAVModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.ID, loginData.Id, Name), Name);
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
        public async Task<BaseModel<object>> Update(long id, [FromBody] NAVModel data)
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
                    string sqlq = "select ISNULL((select count(*) from Contract where id = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    Hashtable val = new Hashtable();
                    val.Add("ContractID", data.ContractID);
                    if (data.EffectiveDate.HasValue && data.EffectiveDate.Value > DateTime.MinValue)
                        val.Add("RecordDate", data.EffectiveDate);
                    val.Add("Type", string.IsNullOrEmpty(data.Type) ? "" : data.Type);
                    val.Add("Amount", data.Amount);
                    val.Add("edited_date", DateTime.Now);
                    val.Add("editor", iduser);
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id", id);
                    string strCheck = " select count(*) from NAV where is_deleted=0 and id<>@id ";
                    var temp = cnn.ExecuteScalar(strCheck, new SqlConditions() { { "id", id } });
                    if (int.Parse(temp.ToString()) > 0)
                        return JsonResultCommon.Trung(Name);
                    if (cnn.Update(val, sqlcond, "NAV") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<NAVModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.ID, loginData.Id, Name), Name);
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
                    string sqlq = "select * from Contract where id = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "update Contract set is_deleted = 1, deleted_date = getdate(),deleted_by = " + iduser + " where id = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["ContractCode"].ToString() };
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
