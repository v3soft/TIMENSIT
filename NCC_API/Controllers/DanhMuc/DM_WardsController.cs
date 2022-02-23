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
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/dm_wards")]
    [EnableCors("TimensitPolicy")]
    public class DM_WardsController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LoginController lc;
        private NCCConfig _config;
        private IOptions<NCCConfig> _configLogin;
        LogHelper logHelper;
        string Name = "Xã";

        public DM_WardsController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            _configLogin = configLogin;
            lc = new LoginController();
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "Tbl_DoiTuongNhanQua",
                PKColumn = "Id_Xa",
                DisabledColumn = "Disabled",
                name = "Đối tượng nhận quà"
            });
            FKs.Add(new CheckFKModel
            {
                TableName = "Tbl_NCC",
                PKColumn = "Id_Xa",
                DisabledColumn = "Disabled",
                name = "Người có công"
            });
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
        }

        [Authorize(Roles = "5")]
        [Route("ListAll")]
        [HttpGet]
        public object ListAll([FromQuery] QueryParams query)// (bool more = false, int? page = 1, int? record = 10)
        {
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            bool Visible = User.IsInRole("7");
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string sqlq = "";
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    string dieukienSort = "WardName",
                    dieukien_where = "ProvinceID =" + loginData.IdTinh;
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "RowID", "Rowid"}, // " Giống biến model", "Tên cột"
                            { "WardName", "title"},
                            { "DistrictName", "DistrictName"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    if (!string.IsNullOrEmpty(query.filter["DistrictID"]))
                    {
                        dieukien_where += " and DistrictID = @DistrictID";
                        Conds.Add("DistrictID", query.filter["DistrictID"]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["WardName"]))
                    {
                        dieukien_where += " and title like N'%'+ @keyword + '%'";
                        Conds.Add("keyword", query.filter["WardName"]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["DistrictName"]))
                    {
                        dieukien_where += " and DistrictName like N'%'+ @keyword + '%'";
                        Conds.Add("keyword", query.filter["DistrictName"]);
                    }
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    //if (DpsLibs.StockPermit.Permit.IsReadOnlyPermit("3354", loginData.UserName))
                    //{
                    //    Visible = false;
                    //}
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện

                    sqlq = @"SELECT Rowid, provinceid, Title as WardName, districtid,DM_District.DistrictName, DM_Wards.CustemerID, DM_Wards.CreatedDate, DM_Wards.LastModified, '' as editpermit, fullname as nguoicapnhat 
from DM_Wards join DM_District on DM_District.id_row = DistrictID
left join Dps_user on Dps_user.UserID = dm_wards.nguoicapnhat where " + dieukien_where + "  order by " + dieukienSort;
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
                                   RowID = r["Rowid"],
                                   WardName = r["WardName"],
                                   DistrictName = r["DistrictName"],
                                   LastModified = r["LastModified"],
                                   DistrictID = r["districtid"],
                                   ProvinceID = r["provinceid"],
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
        public async Task<object> Insert([FromBody] WardAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            BaseModel<WardAddData> model = new BaseModel<WardAddData>();
            string LogContent = "", LogEditContent = "";
            try
            {

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("Title", data.WardName);
                    val.Add("DistrictID", data.DistrictID);
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("custemerid", 1);
                    string strCheck = "select count(*) from DM_Wards where disable=0  and DistrictID=@district and Title=@name";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "district", data.DistrictID }, { "name", data.WardName } }).ToString()) > 0)
                        return JsonResultCommon.Trung("Phường/xã");
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "DM_Wards") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('DM_Wards')").ToString();
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<WardAddData>(data, loginData, "Thêm mới", logHelper.LogThem(data.RowID, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
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
        public async Task<BaseModel<object>> Update([FromBody] WardAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            BaseModel<WardAddData> model = new BaseModel<WardAddData>();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("Title", data.WardName);
                    val.Add("DistrictID", data.DistrictID);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("NguoiCapNhat", iduser);

                    string strCheck = "select count(*) from DM_Wards where disable=0 and DistrictID=@district and Title=@name and RowID<>@idrow";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "district", data.DistrictID }, { "name", data.WardName }, { "idrow", data.RowID } }).ToString()) > 0)
                        return JsonResultCommon.Trung("Phường/xã");
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("RowID", data.RowID);
                    string s = "select Title,DistrictID from DM_Wards where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    if (cnn.Update(val, sqlcond, "DM_Wards") != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    logHelper.Ghilogfile<WardAddData>(data, loginData, "Cập nhật", logHelper.LogSua(data.RowID, loginData.Id, Name), Name);
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
            try
            {
                long iduser = loginData.Id;
                ErrorModel error = new ErrorModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from DM_Wards where rowid = " + id;
                    DataTable dtFind = cnn.CreateDataTable(sqlq);
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Phường/xã");
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom("Phường, xã" + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "delete DM_Wards where rowid = " + id;

                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    cnn.EndTransaction();
                    var data = new LiteModel() { id = id, title = dtFind.Rows[0]["Title"].ToString() };
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