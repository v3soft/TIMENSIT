using DpsLibs.Data;
using System;
using System.Data;
using System.Linq;
using System.Collections;
using Microsoft.AspNetCore.Mvc;
using Timensit_API.Models;
using Timensit_API.Controllers.Users;
using Microsoft.Extensions.Configuration;
using Timensit_API.Models.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Hosting;
using System.IO;
using Microsoft.AspNetCore.Cors;

namespace Timensit_API.Controllers.QuanLy
{
    [ApiController]
    [Route("api/hdsd")]
    [EnableCors("TimensitPolicy")]
    public class HDSDController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Hướng dẫn sử dụng";

        public HDSDController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            _hostingEnvironment = hostingEnvironment;
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 10);
        }

        //[HttpGet]
        //public object List([FromQuery] QueryParams query)
        //{
        //    try
        //    {
        //        string Token = lc.GetHeader(Request);
        //        LoginData loginData = lc._GetInfoUser(Token);
        //        if (loginData == null)
        //            return JsonResultCommon.DangNhap();
        //        PageModel pageModel = new PageModel();
        //        string domain = "http://docs.vts-demo.com/ncc/HDSD/";
        //        string keyword = "";
        //        if (!string.IsNullOrEmpty(query.filter["keyword"]))
        //            keyword = query.filter["keyword"];
        //        var data = from f in LiteHelper.HDSDs
        //                   where string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && f.title.Contains(keyword))
        //                   select new
        //                   {
        //                       HDSD = f.title,
        //                       path = domain + f.id
        //                   };
        //        if (!string.IsNullOrEmpty(query.sortField) && query.sortField == "HDSD")
        //        {
        //            if ("asc".Equals(query.sortOrder))
        //                data = data.OrderBy(x => x.HDSD);
        //            else
        //                data = data.OrderByDescending(x => x.HDSD);
        //        }
        //        int total = data.Count();
        //        if (total == 0)
        //            return JsonResultCommon.ThanhCong(new List<string>(), pageModel, false);
        //        pageModel.TotalCount = total;
        //        pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
        //        pageModel.Size = query.record;
        //        pageModel.Page = query.page;
        //        if (query.more)
        //        {
        //            query.page = 1;
        //            query.record = pageModel.TotalCount;
        //        }
        //        // Phân trang
        //        data = data.Skip((query.page - 1) * query.record).Take(query.record).ToList();
        //        return JsonResultCommon.ThanhCong(data, pageModel, false);
        //    }
        //    catch (Exception ex)
        //    {
        //        return JsonResultCommon.Exception(ex,ControllerContext);
        //    }
        //}

        /// <summary>
        /// Danh sách 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public BaseModel<object> List([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select * from Tbl_HuongDan where Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "TenHuongDan","TenHuongDan" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["TenHuongDan"]))
                    {
                        string keyword = query.filter["TenHuongDan"].ToLower();
                        temp = temp.Where(x => x["TenHuongDan"].ToString().ToLower().Contains(keyword));
                    }
                    #endregion
                    bool allowEdit = User.IsInRole("1122"); //quản lý hdsd
                    var data = (from r in temp
                                select new
                                {

                                    Id = r["Id"],

                                    HDSD = r["TenHuongDan"],

                                    path = string.Format("{0}dulieu/HDSD/{1}", _config.LinkAPI, r["FileHuongDan"]),

                                    CreatedBy = r["NguoiTao"],

                                    CreatedDate = string.Format("{0:dd/MM/yyyy}", r["NgayTao"]),

                                    UpdatedBy = r["NguoiSua"],

                                    UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["NgaySua"]),

                                    AllowEdit = allowEdit,
                                }).ToList();

                    int total = data.Count();
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, allowEdit);
                    pageModel.TotalCount = total;
                    pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                    pageModel.Size = query.record;
                    pageModel.Page = query.page;
                    if (query.more)
                    {
                        query.page = 1;
                        query.record = pageModel.TotalCount;
                    }
                    // Phân trang
                    data = data.Skip((query.page - 1) * query.record).Take(query.record).ToList();
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel, allowEdit);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Chi tiết 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpGet("{id}")]
        public BaseModel<object> Detail(int id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select * from Tbl_HuongDan where Disabled=0 and Id = @Id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {
                                                           Id = r["Id"],

                                                           HDSD = r["TenHuongDan"],

                                                           AllowEdit = User.IsInRole("1122")

                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Thêm mới 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public BaseModel<object> Create([FromBody] HuongDanSDModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string strRe = "";
                if (string.IsNullOrEmpty(data.TenHuongDan))
                    strRe += (strRe == "" ? "" : ", ") + "tên hướng dẫn";
                if (data.FileDinhKem == null)
                    strRe += (strRe == "" ? "" : ", ") + "file nội dung";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Tbl_HuongDan where Disabled=0 and TenHuongDan = @Name";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.TenHuongDan) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("TenHuongDan", data.TenHuongDan);

                    string f = data.FileDinhKem.filename;
                    string x = "";
                    if (!UploadHelper.UploadFile(data.FileDinhKem.strBase64, f, "/HDSD/", _hostingEnvironment.ContentRootPath, ref x))
                        return JsonResultCommon.Custom(UploadHelper.error);
                    data.FileDinhKem.src = _config.LinkAPI + x;
                    val.Add("FileHuongDan", f);
                    val.Add("NgayTao", DateTime.Now);
                    val.Add("NguoiTao", loginData.Id);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "Tbl_HuongDan") == 1)
                    {
                        data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_HuongDan') AS Current_Identity; ").ToString());
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<HuongDanSDModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cập nhật thông tin, k đc sửa số
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public BaseModel<object> Update(int id, [FromBody] HuongDanSDModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                string strRe = "";
                if (data.IsUp)
                {
                    if (data.FileDinhKem == null)
                        strRe += (strRe == "" ? "" : ", ") + "file nội dung";
                }
                else
                {
                    if (string.IsNullOrEmpty(data.TenHuongDan))
                        strRe += (strRe == "" ? "" : ", ") + "tên hướng dẫn";
                }
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Tbl_HuongDan where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    cnn.BeginTransaction();
                    Hashtable val = new Hashtable();
                    val.Add("TenHuongDan", data.TenHuongDan);
                    val.Add("NgaySua", DateTime.Now);
                    val.Add("NguoiSua", loginData.Id);
                    if(data.IsUp)
                    {
                        string f = data.FileDinhKem.filename;
                        string x = "";
                        if (!UploadHelper.UploadFile(data.FileDinhKem.strBase64, f, "/HDSD/", _hostingEnvironment.ContentRootPath, ref x))
                            return JsonResultCommon.Custom(UploadHelper.error);
                        data.FileDinhKem.src = _config.LinkAPI + x;
                        val.Add("FileHuongDan", f);
                    }

                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "Tbl_HuongDan") == 1)
                    {
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<HuongDanSDModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
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
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(int id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Tbl_HuongDan where Disabled = 0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    sqlq = "update Tbl_HuongDan set Disabled = 1, NgaySua = getdate(), NguoiSua = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string f = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.RootUpload + "\\HDSD\\" + dtFind.Rows[0]["FileHuongDan"].ToString());
                        System.IO.File.Delete(f);
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["TenHuongDan"].ToString() };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong();
                    }
                    else
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
    }
}
