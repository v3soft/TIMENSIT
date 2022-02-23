using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SignalRChat.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Timensit_API.Controllers.Common;

namespace Timensit_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/district")]
    [EnableCors("TimensitPolicy")]
    public class DistrictController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LoginController lc;
        private NCCConfig _config;
        private IOptions<NCCConfig> _configLogin;
        LogHelper logHelper;
        string Name = "Huyện";

        public DistrictController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            _configLogin = configLogin;
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            lc = new LoginController();
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "DM_Wards",
                PKColumn = "DistrictID",
                name = "Phường, xã"
            });
        }

        [Authorize(Roles = "5")]
        [Route("ListAll")]
        [HttpGet]
        public object ListAll([FromQuery] QueryParams query)// (bool more = false, int? page = 1, int? record = 10)
        {
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            bool Visible = true;
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string sqlq = "";
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    string dieukienSort = "DistrictName", dieukien_where = " 1=1";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "Id_row", "Id_row"}, // " Giống biến model", "Tên cột"
                            { "DistrictName", "DistrictName"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    if (!string.IsNullOrEmpty(query.filter["ProvinceID"]))
                    {
                        dieukien_where += " and ProvinceID = @ProvinceID";
                        Conds.Add("ProvinceID", query.filter["ProvinceID"]);
                    }
                    else
                    {
                        dieukien_where += " and ProvinceID = 0";
                    }
                    if (!string.IsNullOrEmpty(query.filter["DistrictName"]))
                    {
                        dieukien_where += " and DistrictName like N'%'+ @keyword + '%'";
                        Conds.Add("keyword", query.filter["DistrictName"]);
                    }
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    //if (DpsLibs.StockPermit.Permit.IsReadOnlyPermit("5", loginData.UserName))
                    //{
                    //    Visible = false;
                    //}
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện

                    sqlq = @"SELECT Id_row, DM_District.Code, DistrictName, ProvinceID, DM_District.DateCreated, DM_District.LastModified, fullname as NguoiCapNhat, Note, CustemerID, '' as editpermit from DM_District
left join dps_user on dps_user.userid = DM_District.nguoicapnhat
where " + dieukien_where + "  order by " + dieukienSort;
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
                                   DistrictName = r["DistrictName"],
                                   Note = r["Note"],
                                   DateCreated = r["DateCreated"],
                                   LastModified = r["LastModified"],
                                   ProvinceID = r["ProvinceID"],
                                   CustemerID = r["CustemerID"],
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

        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("Insert")]
        [HttpPost]
        public async Task<object> Insert([FromBody] DistrictAddData data)
        {
            BaseModel<DistrictAddData> model = new BaseModel<DistrictAddData>();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            try
            {

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("DistrictName", data.DistrictName);
                    val.Add("ProvinceID", data.ProvinceID);
                    val.Add("Note", data.Note);
                    val.Add("DateCreated", DateTime.Now);
                    val.Add("custemerid", 1);
                    string strCheck = "select count(*) from DM_District where  ProvinceID=@province and DistrictName=@name ";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "province", data.ProvinceID }, { "name", data.DistrictName } }).ToString()) > 0)
                    {
                        return JsonResultCommon.Trung("Quận/huyện");
                    }
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "DM_District") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('DM_District')").ToString();
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<DistrictAddData>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id_row, loginData.Id, Name), Name);
                    model.status = 1;
                    data.Id_row = int.Parse(idc);
                    model.data = data;
                    return model;
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("Update")]
        [HttpPost]
        public async Task<BaseModel<object>> Update([FromBody] DistrictAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            BaseModel<DistrictAddData> model = new BaseModel<DistrictAddData>();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("DistrictName", data.DistrictName);
                    val.Add("ProvinceID", data.ProvinceID);
                    val.Add("Note", data.Note ?? "");
                    val.Add("LastModified", DateTime.Now);
                    val.Add("NguoiCapNhat", iduser);
                    string strCheck = "select count(*) from DM_District where ProvinceID=@province and DistrictName=@name and id_row<>@idrow";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "province", data.ProvinceID }, { "name", data.DistrictName }, { "idrow", data.Id_row } }).ToString()) > 0)
                        return JsonResultCommon.Trung("Quận/huyện");

                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("Id_row", data.Id_row);
                    string s = "select districtname,provinceid,note from DM_District where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    if (cnn.Update(val, sqlcond, "DM_District") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<DistrictAddData>(data, loginData, "Cập nhật", logHelper.LogSua((int)data.Id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("Delete")]
        [HttpGet]
        public BaseModel<object> Delete(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            BaseModel<bool> model = new BaseModel<bool>();
            try
            {
                long iduser = loginData.Id;
                ErrorModel error = new ErrorModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_District where id_row = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Quận/huyện");
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom("Xã" + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "delete DM_District where id_row = " + id;

                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["DistrictName"].ToString() };
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