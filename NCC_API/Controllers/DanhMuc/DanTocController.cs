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
    /// <summary>
    /// Quyền xem: 1104	Quản lý danh mục dân tộc
    /// Quyền sửa: 1105	Cập nhật danh mục dân tộc
    /// </summary>
    [ApiController]
    [Route("api/dantoc")]
    [EnableCors("TimensitPolicy")]
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class DanTocController : ControllerBase
    {
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Dân tộc";
        public DanTocController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
        }
        [Authorize(Roles = "1104")]
        [Route("ListAll")]
        [HttpGet]
        public object ListAll([FromQuery] QueryParams query)// (bool more = false, int? page = 1, int? record = 10)
        {
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            bool allowEdit = User.IsInRole("1105");
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
                    string dieukienSort = "Priority";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "Id_row", "Id_row"}, // " Giống biến model", "Tên cột"
                            { "Tendantoc", "Tendantoc"},
                            { "Priority", "Priority"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select CustemerID, Id_row, tendantoc,Priority, '' as editpermit, fullname as modifiedby, dm_dantoc.lastmodified from dm_dantoc
left join Dps_user on Dps_user.UserID = dm_dantoc.ModifiedBy
order by " + dieukienSort;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    int total = dt.Rows.Count;
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), allowEdit);
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
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), allowEdit);
                    dt = temp1.CopyToDataTable();
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          Id_row = r["Id_row"],
                                                          Tendantoc = r["Tendantoc"],
                                                          Priority = r["Priority"],
                                                          NgayCapNhat = r["lastmodified"],
                                                          NguoiCapNhat = r["modifiedby"],
                                                          AllowEdit = allowEdit
                                                      }, pageModel, allowEdit);
                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        // GET api/<controller>
        /// <summary>
        /// Tạo phòng ban
        /// Header truyền vào token
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "1105")]
        [Route("Insert")]
        [HttpPost]
        public async Task<object> Insert([FromBody] DanTocAddData data)
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
                    val.Add("tendantoc", data.Tendantoc);
                    val.Add("Priority", data.Priority);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "dm_dantoc") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //LogContent = LogEditContent = "Thêm mới dữ liệu dân tộc Tên dân tộc=" + data.Tendantoc;
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('dm_dantoc')").ToString();
                    logHelper.Ghilogfile<DanTocAddData>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id_row, loginData.Id, Name), Name);
                    cnn.EndTransaction();
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "1105")]
        [Route("Update")]
        [HttpPost]
        public async Task<BaseModel<object>> Update([FromBody] DanTocAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            BaseModel<DanTocAddData> model = new BaseModel<DanTocAddData>();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("tendantoc", data.Tendantoc);
                    val.Add("Priority", data.Priority);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("ModifiedBy", iduser);
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("Id_row", data.Id_row);
                    string s = "select tendantoc from dm_dantoc where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    if (cnn.Update(val, sqlcond, "dm_dantoc") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //DataTable dt = cnn.CreateDataTable(s, "(where)", sqlcond);
                    //LogEditContent = DpsPage.GetEditLogContent(old, dt);
                    //if (!LogEditContent.Equals(""))
                    //{
                    //    LogEditContent = "Chỉnh sửa dữ liệu (" + data.Id_row + ") :" + LogEditContent;
                    //    LogContent = "Chỉnh sửa thông tin dân tộc (" + data.Id_row + "), Chi tiết xem trong log chỉnh sửa chức năng";
                    //}
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<DanTocAddData>(data, loginData, "Cập nhật", logHelper.LogSua(data.Id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "1105")]
        [Route("Delete")]
        [HttpGet]
        public BaseModel<object> Delete(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            BaseModel<bool> model = new BaseModel<bool>();
            try
            {
                long iduser = loginData.Id;
                ErrorModel error = new ErrorModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from dm_dantoc where id_row = " + id;
                    DataTable dtF = cnn.CreateDataTable(sqlq);
                    if (dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    //if (BLayer.General.TestDuplicate("", id.ToString(), "-1", "tbl_nhanvien", "dantoc", "", "-1", cnn, "", true) == false)
                    //{
                    //    model.status = 0;
                    //    error.message = "Đang có nhân viên thuộc dân tộc này nên không thể xóa";
                    //    error.code = API_KD.Assets.Constant.ERRORCODE_SQL;
                    //    model.error = error;
                    //    model.data = false;
                    //    return model;
                    //}
                    sqlq = "delete dm_dantoc where id_row = " + id;

                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //string LogContent = "Xóa dữ liệu dân tộc (id_row=" + id + ")";
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogContent, LogContent, loginData.UserName);
                    cnn.EndTransaction();
                    var data = new LiteModel() { id = id, title = dtF.Rows[0]["tendantoc"].ToString() };
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
    public class DanTocAddData
    {
        public int Id_row { get; set; } = 0;
        public string Tendantoc { get; set; }
        public int Priority { get; set; }
    }
}