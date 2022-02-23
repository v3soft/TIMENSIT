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
    /// 1106	Quản lý danh mục tôn giáo
    /// 1108	Cập nhật danh mục tôn giáo
    /// </summary>
    [ApiController]
    [Route("api/tongiao")]
    [EnableCors("TimensitPolicy")]
    // [EnableCors(origins: "*", headers: "*", methods: "*")]
    public class TonGiaoController : ControllerBase
    {
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Tôn giáo";
        public TonGiaoController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
        }
        [Authorize(Roles = "1106")]
        [Route("ListAll")]
        [HttpGet]
        public object ListAll([FromQuery] QueryParams query)// (bool more = false, int? page = 1, int? record = 10)
        {
            BaseModel<object> chucdanhmodel = new BaseModel<object>();
            bool Visible = User.IsInRole("1108");
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
                            { "Tentongiao", "Tentongiao"},
                            { "Priority", "Priority"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select Id_row, Tentongiao,Priority, '' as editpermit, dm_tongiao.CustemerID, dm_tongiao.LastModified, Fullname as ModifiedBy 
from dm_tongiao left join Dps_user on Dps_user.UserID = dm_tongiao.ModifiedBy
 order by " + dieukienSort;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    int total = dt.Rows.Count;
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);

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
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          Id_row = r["Id_row"],
                                                          Tentongiao = r["Tentongiao"],
                                                          Priority = r["Priority"],
                                                          NgayCapNhat = r["LastModified"],
                                                          NguoiCapNhat = r["ModifiedBy"],
                                                      }, pageModel, Visible);

                    #endregion
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Tạo phòng ban
        /// Header truyền vào token
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "1108")]
        [Route("Insert")]
        [HttpPost]
        public async Task<object> Insert([FromBody] TonGiaoAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            BaseModel<TonGiaoAddData> model = new BaseModel<TonGiaoAddData>();
            //string LogEditContent = "", LogContent = "";
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("tentongiao", data.Tentongiao);
                    val.Add("Priority", data.Priority);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "dm_tongiao") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //LogContent = LogEditContent = "Thêm mới dữ liệu dân tộc Tên tôn giáo =" + data.Tentongiao;
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('dm_tongiao')").ToString();
                    cnn.EndTransaction();
                    data.Id_row = int.Parse(idc);
                    logHelper.Ghilogfile<TonGiaoAddData>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id_row, loginData.Id, Name),Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
        /// <summary>
        /// Cập nhật
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "1108")]
        [Route("Update")]
        [HttpPost]
        public async Task<BaseModel<object>> Update([FromBody] TonGiaoAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            //string LogEditContent = "", LogContent = "";
            BaseModel<TonGiaoAddData> model = new BaseModel<TonGiaoAddData>();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("tentongiao", data.Tentongiao);
                    val.Add("Priority", data.Priority);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("ModifiedBy", iduser);
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("Id_row", data.Id_row);
                    string s = "select tentongiao from dm_tongiao where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    if (cnn.Update(val, sqlcond, "dm_tongiao") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //DataTable dt = cnn.CreateDataTable(s, "(where)", sqlcond);
                    //LogEditContent = DpsPage.GetEditLogContent(old, dt);
                    //if (!LogEditContent.Equals(""))
                    //{
                    //    LogEditContent = "Chỉnh sửa dữ liệu (" + data.Id_row + ") :" + LogEditContent;
                    //    LogContent = "Chỉnh sửa thông tin tôn giáo (" + data.Id_row + "), chi tiết xem trong log chỉnh sửa chức năng";
                    //}
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<TonGiaoAddData>(data, loginData,  "Cập nhật", logHelper.LogSua(data.Id_row, loginData.Id, Name),Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
        /// <summary>
        /// xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "1108")]
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
                    string sqlq = "select * from dm_tongiao where id_row = " + id;
                    DataTable dtFind = cnn.CreateDataTable(sqlq);
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai();
                    //if (BLayer.General.TestDuplicate("", id.ToString(), "-1", "tbl_nhanvien", "tongiao", "", "-1", cnn, "", true) == false)
                    //{
                    //    model.status = 0;
                    //    error.message = "Đang có nhân viên thuộc tôn giáo này nên không thể xóa";
                    //    error.code = API_KD.Assets.Constant.ERRORCODE_SQL;
                    //    model.error = error;
                    //    model.data = false;
                    //    return model;
                    //}
                    sqlq = "delete dm_tongiao where id_row = " + id;
                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    var data = new LiteModel() { id = id, title = dtFind.Rows[0]["TenTonGiao"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
    }
    public class TonGiaoAddData
    {
        public int Id_row { get; set; } = 0;
        public string Tentongiao { get; set; }
        public int Priority { get; set; }
    }
}