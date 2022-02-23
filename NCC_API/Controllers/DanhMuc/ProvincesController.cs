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

namespace Timensit_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/provinces")]
    [EnableCors("TimensitPolicy")]
    public class ProvincesController : ControllerBase
    {
        List<CheckFKModel> FKs;
        LoginController lc;
        private NCCConfig _config;
        public ProvincesController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "DM_District",
                PKColumn = "ProvinceID",
                name = "Quận huyện"
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
            Provinces province = new Provinces();
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
                    string dieukienSort = "ProvinceName", dieukien_where = " ((dm_provinces.disable is null) or (dm_provinces.disable=0))";
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "Id_row", "Id_row"}, // " Giống biến model", "Tên cột"
                            { "ProvinceName", "ProvinceName"},
                        };
                    #endregion
                    #region Code sort
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    if (!string.IsNullOrEmpty(query.filter["ProvinceName"]))
                    {
                        dieukien_where += " and ProvinceName like N'%'+@keyword+'%'";
                        Conds.Add("keyword", query.filter["ProvinceName"]);
                    }
                    #endregion
                    #region Xét quyền xem/chỉnh sửa
                    //if (DpsLibs.StockPermit.Permit.IsReadOnlyPermit("3354", loginData.UserName))
                    //{
                    //    Visible = false;
                    //}
                    #endregion
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    sqlq = @"select Id_row, ProvinceName, '' as editpermit, dm_provinces.CustemerID, dm_provinces.LastModified, fullname as Nguoisua from dm_provinces
left join Dps_user on Dps_user.UserID = dm_provinces.Nguoisua
where " + dieukien_where + " order by " + dieukienSort;
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
                                   ProvinceName = r["ProvinceName"],
                                   CustemerID = r["CustemerID"],
                                   NgayCapNhat = r["LastModified"],
                                   NguoiCapNhat = r["Nguoisua"],
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
        
        // GET api/<controller>
        /// <summary>
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("Insert")]
        [HttpPost]
        public async Task<object> Insert([FromBody]ProvincesAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            BaseModel<ProvincesAddData> model = new BaseModel<ProvincesAddData>();
            if (string.IsNullOrEmpty(data.ProvinceName))
                return JsonResultCommon.BatBuoc("Tên tỉnh thành");
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("ProvinceName", data.ProvinceName);
                    val.Add("DateCreated", DateTime.Now);
                    val.Add("Nguoitao", iduser);
                    val.Add("custemerid", 1);
                    string strCheck = "select count(*) from DM_Provinces where disable=0 and ProvinceName=@name";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "name", data.ProvinceName } }).ToString()) > 0)
                        return JsonResultCommon.Trung("Tỉnh/thành phố");
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "DM_Provinces") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('DM_Provinces')").ToString();
                    cnn.EndTransaction();
                    {
                        model.status = 1;
                        data.Id_row = int.Parse(idc);
                        model.data = data;
                        return model;
                    }
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
        
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("Update")]
        [HttpPost]
        public async Task<BaseModel<object>> Update([FromBody] ProvincesAddData data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            if (string.IsNullOrEmpty(data.ProvinceName))
                return JsonResultCommon.BatBuoc("Tên tỉnh thành");
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("ProvinceName", data.ProvinceName);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("Nguoisua", iduser);
                    string strCheck = "select count(*) from DM_Provinces where disable=0 and ProvinceName=@name and id_row<>@idrow";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "name", data.ProvinceName }, { "idrow", data.Id_row } }).ToString()) > 0)
                        return JsonResultCommon.Trung("Tỉnh/thành phố");
                    cnn.BeginTransaction();
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("Id_row", data.Id_row);
                    if (cnn.Update(val, sqlcond, "DM_Provinces") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
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
                    string sqlq = "select ISNULL((select count(*) from DM_Provinces where id_row = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai("Tỉnh/thành phố");
                    #region check đã được sử dụng
                    string msg = "";
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                        return JsonResultCommon.Custom("Tỉnh thành" + " đang được sử dụng cho " + msg + ", không thể xóa");
                    #endregion
                    sqlq = "update DM_Provinces set Disable = 1 where Id_row = " + id;
                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    return JsonResultCommon.ThanhCong(true);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
    }
}