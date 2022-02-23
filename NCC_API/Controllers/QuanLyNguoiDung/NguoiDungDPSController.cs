using System;
using System.Linq;
using DpsLibs.Data;
using System.Data;
using System.Collections;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Cors;
using Timensit_API.Models;
using Timensit_API.Controllers.Users;
using Microsoft.Extensions.Configuration;
using Microsoft.AspNetCore.Authorization;
using Timensit_API.Models.Common;
using System.Collections.Generic;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using System.IO;
using WebCore_API.Models;
using OfficeOpenXml;
using System.Net.Http;
using DocumentFormat.OpenXml.Packaging;
using DocumentFormat.OpenXml;
using DocumentFormat.OpenXml.Spreadsheet;
using Timensit_API.Controllers.Common;
using Newtonsoft.Json;
using System.Net;
using System.Net.Http.Headers;

namespace Timensit_API.Controllers.QuanLyNguoiDung
{
    /// <summary>
    /// Quản lý user<para/>
    /// Quyền 8:	Xem danh sách và thông tin người dùng
    /// Quyền 10:	Cập nhật người dùng
    /// Quyền 11:	Cấp lại mật khẩu người dùng
    /// </summary>
    [ApiController]
    [Route("api/nguoi-dung")]
    [EnableCors("TimensitPolicy")]
    public class NguoiDungDPSController : ControllerBase
    {
        LogHelper logHelper;
        UserManager dpsUserMr;
        LoginController lc;
        private NCCConfig _config;
        private readonly IHostingEnvironment _hostingEnvironment;
        string Name = "Người dùng";
        ExportExcelHelper excelHelper;

        public NguoiDungDPSController(IOptions<NCCConfig> configLogin, IHostingEnvironment hostingEnvironment, IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = configLogin.Value;
            lc = new LoginController();
            dpsUserMr = new UserManager(configLogin, hostingEnvironment);
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 4);
            excelHelper = new ExportExcelHelper();
        }

        /// <summary>
        /// Lấy danh sách user
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "8")]
        [HttpGet]
        [Route("list")]
        public BaseModel<object> LayDSNguoiDungDPS([FromQuery] QueryParams query)
        {
            //if (!User.IsInRole("8"))
            //{
            //    return JsonResultCommon.PhanQuyen();
            //}
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string Token = lc.GetHeader(Request);
                    LoginData loginData = lc._GetInfoUser(Token);
                    if (loginData == null)
                    {
                        return JsonResultCommon.DangNhap();
                    }
                    PageModel pageModel = new PageModel();
                    string sqlq = @" select distinct u.*,px.DonVi, px.MaDonvi, px.Capcocau, cv.ChucVu, cv.MaChucVu from Dps_User u
                        inner join DM_DonVi px on u.IdDonVi = px.Id
						left join DM_ChucVu cv on u.IdChucVu=cv.Id and cv.Donvi=px.Id
                        where u.Deleted = 0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError == null && dt != null)
                    {
                        var temp = dt.AsEnumerable();
                        #region Sort/filter
                        Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "FullName", "FullName"}, // " Giống biến model", "Tên cột"
                            { "UserName", "UserName"},
                            { "PhoneNumber", "PhoneNumber"},
                            { "Email", "Email"},
                            { "GroupName", "GroupName"},
                            { "ViettelStudy", "ViettelStudy"},
                            { "Active", "Active"},
                        };
                        if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        {
                            if ("desc".Equals(query.sortOrder))
                                temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                            else
                                temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                        }
                        if (!string.IsNullOrEmpty(query.filter["FullName"]))
                        {
                            string keyword = query.filter["FullName"].ToLower();
                            temp = temp.Where(x => x["FullName"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["UserName"]))
                        {
                            string keyword = query.filter["UserName"].ToLower();
                            temp = temp.Where(x => x["UserName"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["PhoneNumber"]))
                        {
                            string keyword = query.filter["PhoneNumber"].ToLower();
                            temp = temp.Where(x => x["PhoneNumber"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["Email"]))
                        {
                            string keyword = query.filter["Email"].ToLower();
                            temp = temp.Where(x => x["Email"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["MaNV"]))
                        {
                            string keyword = query.filter["MaNV"].ToLower();
                            temp = temp.Where(x => x["MaNV"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["ViettelStudy"]))
                        {
                            string keyword = query.filter["ViettelStudy"].ToLower();
                            temp = temp.Where(x => x["ViettelStudy"].ToString().ToLower().Contains(keyword));
                        }
                        if (!string.IsNullOrEmpty(query.filter["Donvi"]))
                        {
                            string keyword = query.filter["Donvi"].ToLower();
                            temp = temp.Where(x => x["IdDonVi"].ToString() == keyword);
                        }
                        if (query.filter["ChuaCoVaiTro"] == "true")
                        {
                            string sqlVT = "select distinct IdUser from Dps_User_GroupUser where Disabled=0";
                            DataTable dtU = cnn.CreateDataTable(sqlVT);
                            if (dtU != null)
                            {
                                List<string> groups = dtU.AsEnumerable().Select(x => x[0].ToString()).ToList();
                                temp = temp.Where(x => !groups.Contains(x["UserID"].ToString()));
                            }
                        }
                        if (!string.IsNullOrEmpty(query.filter["VaiTro"]))
                        {
                            string sqlVT = "select IdUser from Dps_User_GroupUser where Disabled=0 and IdGroupUser = " + query.filter["VaiTro"];
                            DataTable dtU = cnn.CreateDataTable(sqlVT);
                            if (dtU != null)
                            {
                                List<string> groups = dtU.AsEnumerable().Select(x => x[0].ToString()).ToList();
                                temp = temp.Where(x => groups.Contains(x["UserID"].ToString()));
                            }
                        }
                        if (query.filterGroup != null && query.filterGroup["Active"] != null)
                        {
                            var groups = query.filterGroup["Active"].ToList();
                            temp = temp.Where(x => groups.Contains(x["Active"].ToString()));
                        }
                        if (query.filterGroup != null && query.filterGroup["NhanLichDonVi"] != null)
                        {
                            var groups = query.filterGroup["NhanLichDonVi"].ToList();
                            temp = temp.Where(x => groups.Contains(x["NhanLichDonVi"].ToString()));
                        }
                        if (query.filterGroup != null && query.filterGroup["ChucVu"] != null)
                        {
                            var groups = query.filterGroup["ChucVu"].ToList();
                            temp = temp.Where(x => groups.Contains(x["IdChucVu"].ToString()));
                        }
                        #endregion
                        int i = temp.Count();
                        if (i == 0)
                            return new BaseModel<object>
                            {
                                status = 1,
                                data = new List<string>(),
                                page = pageModel
                            };
                        dt = temp.CopyToDataTable();
                        int total = dt.Rows.Count;
                        pageModel.TotalCount = total;
                        pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                        pageModel.Size = query.record;
                        pageModel.Page = query.page;
                        if (query.more)
                        {
                            query.page = 1;
                            query.record = pageModel.TotalCount;
                        }
                        string str = "select * from Sys_Config where Code in ('EXP_ADD','EXP_PASS')";
                        DataTable conf = cnn.CreateDataTable(str);
                        if (conf == null || conf.Rows.Count == 0)
                        {
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        }
                        int numA = int.Parse(conf.Select("Code='EXP_ADD'")[0]["Value"].ToString());
                        int num = int.Parse(conf.Select("Code='EXP_PASS'")[0]["Value"].ToString());
                        // Phân trang
                        var temp1 = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                        if (temp1.Count() == 0)
                            return new BaseModel<object>
                            {
                                status = 1,
                                data = new List<string>(),
                                page = pageModel
                            };
                        dt = temp1.CopyToDataTable();
                        var data = from r in dt.AsEnumerable()
                                   select new
                                   {
                                       UserID = r["UserID"],
                                       FullName = r["FullName"],
                                       UserName = r["UserName"],
                                       PhoneNumber = r["PhoneNumber"],
                                       Email = r["Email"],
                                       Status = r["Status"],
                                       Active = r["Active"],
                                       MaNV = r["MaNV"],
                                       ChucVu = r["ChucVu"],
                                       ViettelStudy = r["ViettelStudy"],
                                       SimCA = r["SimCA"],
                                       LoaiChungThu = LiteHelper.GetLiteById(r["LoaiChungThu"], LiteHelper.LoaiChungThu),
                                       CMTND = r["CMTND"],
                                       GioiTinh = LiteHelper.GetLiteById(r["GioiTinh"], LiteHelper.GioiTinh),
                                       NhanLichDonVi = r["NhanLichDonVi"],
                                       DonVi = r["DonVi"] + " (" + LiteHelper.GetLiteById(r["Capcocau"], LiteHelper.CapCoCau) + ")",
                                       UserCreate = r["UserCreate"],
                                       CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
                                       LastLogin = r["LastLogin"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["LastLogin"]),
                                       ExpDate = r["LastUpdatePass"] == DBNull.Value || num == 0 ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", ((DateTime)r["LastUpdatePass"]).AddDays(numA + (int)r["GiaHan"])),
                                       IsQuaHan = r["LastUpdatePass"] == DBNull.Value || num == 0 ? false : DateTime.Now > ((DateTime)r["LastUpdatePass"]).AddDays(numA + (int)r["GiaHan"]),
                                       NgaySinh = r["NgaySinh"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["NgaySinh"]),
                                       Avata = r["Avata"] != DBNull.Value ? (_config.LinkAPI + Constant.RootUpload + r["Avata"]) : ""
                                   };
                        logHelper.LogXemDS(loginData.Id);
                        return JsonResultCommon.ThanhCong(data, pageModel);
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

        /// <summary>
        /// Lấy chi tiết user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "8")]
        [HttpGet]
        [Route("detail")]
        public BaseModel<object> ChiTietNguoiDungDPS(int id)
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
                    string sqlq = @"SELECT  nd.*,Capcocau FROM Dps_User nd
join dm_donvi dv on nd.IdDonVi=dv.Id where UserId=@Id";
                    sqlq += " select u.*,dv.DonVi from Dps_User_DonVi u join DM_DonVi dv on u.IdDonVi=dv.Id where UserID=@Id";
                    sqlq += " select u.*,dt.DoiTuong from Dps_User_DoiTuongNCC u join DM_DoiTuongNCC dt on u.Id_DoiTuongNCC=dt.Id where UserID=@Id";
                    var ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError == null && ds != null)
                    {
                        var data = (from r in ds.Tables[0].AsEnumerable()
                                    select new
                                    {
                                        UserID = r["UserID"],
                                        FullName = r["FullName"],
                                        UserName = r["UserName"],
                                        PhoneNumber = r["PhoneNumber"],
                                        Email = r["Email"],
                                        Status = r["Status"],
                                        Active = r["Active"],
                                        MaNV = r["MaNV"],
                                        IdDonVi = r["IdDonVi"],
                                        Capcocau = r["Capcocau"],
                                        IdChucVu = r["IdChucVu"],
                                        ViettelStudy = r["ViettelStudy"],
                                        SerialToken = r["SerialToken"],
                                        SimCA = r["SimCA"],
                                        LoaiChungThu = r["LoaiChungThu"],
                                        CMTND = r["CMTND"],
                                        GioiTinh = r["GioiTinh"],
                                        NhanLichDonVi = r["NhanLichDonVi"],
                                        NgaySinh = r["NgaySinh"],
                                        Avata = r["Avata"] != DBNull.Value ? (_config.LinkAPI + Constant.RootUpload + r["Avata"]) : "",
                                        Sign = r["Sign"] != DBNull.Value ? (_config.LinkAPI + Constant.RootUpload + r["Sign"]) : "",
                                        DonViQuanTam = ds.Tables[1].AsEnumerable().Where(x => x["Loai"].ToString() == "1").Select(x => new
                                        {
                                            id = x["IdDonVi"],
                                            title = x["Donvi"]
                                        }),
                                        DonViLayHanXuLy = ds.Tables[1].AsEnumerable().Where(x => x["Loai"].ToString() == "2").Select(x => new
                                        {
                                            id = x["IdDonVi"],
                                            title = x["Donvi"]
                                        }),
                                        lstDoiTuongNCC = ds.Tables[2].AsEnumerable().Select(x => new
                                        {
                                            id = x["Id_DoiTuongNCC"],
                                            title = x["DoiTuong"]
                                        })
                                    }).FirstOrDefault();
                        logHelper.LogXemCT(id, loginData.Id);
                        return JsonResultCommon.ThanhCong(data);
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


        /// <summary>
        /// thêm mới user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "10")]
        [HttpPost]
        [Route("create")]
        public BaseModel<object> TaoNguoiDungDPS([FromBody] NguoiDungDPS data)
        {
            if (!string.IsNullOrEmpty(data.UserName))
                data.UserName = data.UserName.ToLower(); //tên đăng nhập chữ thường
            string checkstring = KiemTraThongTinNguoiDungDPS(data, 0);
            if (!string.IsNullOrEmpty(checkstring))
                return JsonResultCommon.Custom(checkstring);
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            long iduser = loginData.Id;
            if (dpsUserMr.CheckNguoiDung(data.UserName, 1))
                return JsonResultCommon.Trung("Tên đăng nhập");
            if (dpsUserMr.CheckEmail(data.Email, 0))
                return JsonResultCommon.Trung("Email");
            try
            {
                string pass = dpsUserMr.EncryptPassword(data.Password);
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Hashtable val = new Hashtable();
                    val.Add("PasswordHash", pass);
                    val.Add("FullName", data.FullName);
                    val.Add("UserName", data.UserName);
                    val.Add("Email", data.Email);
                    val.Add("PhoneNumber", data.PhoneNumber);
                    val.Add("IdDonVi", data.IdDonVi);
                    val.Add("Active", 1);
                    val.Add("MaNV", string.IsNullOrEmpty(data.MaNV) ? "" : data.MaNV);
                    val.Add("ViettelStudy", string.IsNullOrEmpty(data.ViettelStudy) ? "" : data.ViettelStudy);
                    val.Add("SimCA", string.IsNullOrEmpty(data.SimCA) ? "" : data.SimCA);
                    val.Add("SerialToken", string.IsNullOrEmpty(data.SerialToken) ? "" : data.SerialToken);
                    if (data.LoaiChungThu > 0)
                        val.Add("LoaiChungThu", data.LoaiChungThu);
                    val.Add("IdChucVu", data.IdChucVu);
                    if (data.GioiTinh > 0)
                        val.Add("GioiTinh", data.GioiTinh);
                    val.Add("CMTND", string.IsNullOrEmpty(data.CMTND) ? "" : data.CMTND);
                    val.Add("NhanLichDonVi", data.NhanLichDonVi);
                    if (data.NgaySinh.HasValue && data.NgaySinh.Value > DateTime.MinValue)
                        val.Add("NgaySinh", data.NgaySinh);
                    val.Add("UserCreate", iduser);

                    if (data.Sign != null && !string.IsNullOrEmpty(data.Sign.strBase64))
                    {
                        string linkImg = "";
                        if (!UploadHelper.UploadImage(data.Sign.strBase64, data.Sign.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                        {
                            return JsonResultCommon.Custom(UploadHelper.error);
                        }
                        val.Add("Sign", linkImg);
                    }

                    if (data.avatar != null && !string.IsNullOrEmpty(data.avatar.strBase64))
                    {
                        string linkImg = "";
                        if (!UploadHelper.UploadImage(data.avatar.strBase64, data.avatar.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                        {
                            return JsonResultCommon.Custom(UploadHelper.error);
                        }
                        val.Add("Avata", linkImg);

                    }

                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "Dps_User") == 1)
                    {
                        var id = cnn.ExecuteScalar("select IDENT_CURRENT('DPS_User')");
                        data.UserID = long.Parse(id.ToString());
                        if (data.DonViQuanTam != null)
                            foreach (var dv in data.DonViQuanTam)
                            {
                                Hashtable val1 = new Hashtable();
                                val1["UserID"] = data.UserID;
                                val1["IdDonVi"] = dv.id;
                                val1["Loai"] = 1;
                                if (cnn.Insert(val1, "Dps_User_DonVi") != 1)
                                {
                                    cnn.RollbackTransaction();
                                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                }
                            }
                        if (data.DonViLayHanXuLy != null)
                            foreach (var dv in data.DonViLayHanXuLy)
                            {
                                Hashtable val1 = new Hashtable();
                                val1["UserID"] = data.UserID;
                                val1["IdDonVi"] = dv.id;
                                val1["Loai"] = 2;
                                if (cnn.Insert(val1, "Dps_User_DonVi") != 1)
                                {
                                    cnn.RollbackTransaction();
                                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                }
                            }
                        if (data.lstDoiTuongNCC != null)
                            foreach (var dt in data.lstDoiTuongNCC)
                            {
                                Hashtable val1 = new Hashtable();
                                val1["UserID"] = data.UserID;
                                val1["Id_DoiTuongNCC"] = dt.id;
                                if (cnn.Insert(val1, "Dps_User_DoiTuongNCC") != 1)
                                {
                                    cnn.RollbackTransaction();
                                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                }
                            }
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<NguoiDungDPS>(data, loginData, "Thêm mới", logHelper.LogThem((int)data.UserID, loginData.Id));
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
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "10")]
        [HttpPost]
        [Route("update")]
        public BaseModel<object> SuaNguoiDungDPS([FromBody] NguoiDungDPS data)
        {
            try
            {
                string checkstring = KiemTraThongTinNguoiDungDPS(data, 1);
                if (!string.IsNullOrEmpty(checkstring))
                    return JsonResultCommon.Custom(checkstring);
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                if (!dpsUserMr.CheckNguoiDung(data.UserID.ToString(), 0))
                    return JsonResultCommon.KhongTonTai(Name);
                if (dpsUserMr.CheckEmail(data.Email, data.UserID))
                    return JsonResultCommon.Trung("Email");
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Hashtable val = new Hashtable();
                    val.Add("UserName", data.UserName);
                    val.Add("FullName", data.FullName);
                    val.Add("Email", data.Email);
                    val.Add("PhoneNumber", data.PhoneNumber);
                    val.Add("IdDonVi", data.IdDonVi);
                    val.Add("MaNV", string.IsNullOrEmpty(data.MaNV) ? "" : data.MaNV);
                    val.Add("ViettelStudy", string.IsNullOrEmpty(data.ViettelStudy) ? "" : data.ViettelStudy);
                    val.Add("SimCA", string.IsNullOrEmpty(data.SimCA) ? "" : data.SimCA);
                    val.Add("SerialToken", string.IsNullOrEmpty(data.SerialToken) ? "" : data.SerialToken);
                    if (data.LoaiChungThu == 0)
                        val.Add("LoaiChungThu", DBNull.Value);
                    else
                        val.Add("LoaiChungThu", data.LoaiChungThu);
                    val.Add("IdChucVu", data.IdChucVu);
                    if (data.GioiTinh == 0)
                        val.Add("GioiTinh", DBNull.Value);
                    else
                        val.Add("GioiTinh", data.GioiTinh);
                    val.Add("CMTND", string.IsNullOrEmpty(data.CMTND) ? "" : data.CMTND);
                    val.Add("NhanLichDonVi", data.NhanLichDonVi);
                    if (data.NgaySinh.HasValue && data.NgaySinh.Value > DateTime.MinValue)
                        val.Add("NgaySinh", data.NgaySinh);
                    else
                        val.Add("NgaySinh", DBNull.Value);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);

                    if (data.Sign != null && !string.IsNullOrEmpty(data.Sign.strBase64))
                    {
                        string linkImg = "";
                        if (!UploadHelper.UploadImage(data.Sign.strBase64, data.Sign.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                        {
                            return JsonResultCommon.Custom(UploadHelper.error);
                        }
                        val.Add("Sign", linkImg);
                    }

                    if (data.avatar != null && !string.IsNullOrEmpty(data.avatar.strBase64))
                    {
                        string linkImg = "";
                        if (!UploadHelper.UploadImage(data.avatar.strBase64, data.avatar.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                        {
                            return JsonResultCommon.Custom(UploadHelper.error);
                        }
                        val.Add("Avata", linkImg);

                    }

                    SqlConditions con = new SqlConditions() { { "UserID", data.UserID } };
                    cnn.BeginTransaction();
                    if (cnn.Delete(con, "Dps_User_DonVi") < 0)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    if (cnn.Delete(con, "Dps_User_DoiTuongNCC") < 0)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    if (cnn.Update(val, con, "Dps_User") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    if (data.DonViQuanTam != null)
                        foreach (var dv in data.DonViQuanTam)
                        {
                            Hashtable val1 = new Hashtable();
                            val1["UserID"] = data.UserID;
                            val1["IdDonVi"] = dv.id;
                            val1["Loai"] = 1;
                            if (cnn.Insert(val1, "Dps_User_DonVi") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    if (data.DonViLayHanXuLy != null)
                        foreach (var dv in data.DonViLayHanXuLy)
                        {
                            Hashtable val1 = new Hashtable();
                            val1["UserID"] = data.UserID;
                            val1["IdDonVi"] = dv.id;
                            val1["Loai"] = 2;
                            if (cnn.Insert(val1, "Dps_User_DonVi") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    if (data.lstDoiTuongNCC != null)
                        foreach (var dv in data.lstDoiTuongNCC)
                        {
                            Hashtable val1 = new Hashtable();
                            val1["UserID"] = data.UserID;
                            val1["Id_DoiTuongNCC"] = dv.id;
                            if (cnn.Insert(val1, "Dps_User_DoiTuongNCC") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<NguoiDungDPS>(data, loginData, "Cập nhật", logHelper.LogSua((int)data.UserID, loginData.Id));
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Người quản lý reset mật khẩu cho user
        /// </summary>
        /// <param name="data"></param>
        /// <param name="usergoc"></param>
        /// <returns></returns>
        [Authorize(Roles = "11")]
        [HttpPost]
        [Route("reset-password")]
        public BaseModel<object> CapLaiMatKhau([FromBody] DoiMatKhau data, bool usergoc = false)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            if (data.Id.Equals(loginData.Id))
                return JsonResultCommon.Custom("Không được cấp lại mật khẩu cho chính mình, vui lòng vào mục đổi mật khẩu");
            if (data.NewPassword.Length < 6 || data.NewPassword.Length > 20)
                return JsonResultCommon.Custom("Mật khẩu phải có tối thiểu 6 và tối đa 20 ký tự");
            if (data.NewPassword != data.RePassword)
                return JsonResultCommon.Custom("Mật khẩu mới và xác nhận mật khẩu không giống nhau");
            if (!dpsUserMr.CheckNguoiDung(data.Id.ToString(), 0))
                return JsonResultCommon.KhongTonTai(Name);
            string kq = dpsUserMr.ResetPass(data.Id, data.NewPassword);
            if (string.IsNullOrEmpty(kq))
            {
                logHelper.Ghilogfile<long>(long.Parse(data.Id), loginData, "Cấp lại mật khẩu", logHelper.Log(3, loginData.Id, "Cấp lại mật khẩu", int.Parse(data.Id)));
                return JsonResultCommon.ThanhCong();
            }
            else
                return JsonResultCommon.Custom(kq);
        }

        [HttpGet]
        [Route("GetMatKhau")]
        public IActionResult GetMatKhau(long id, string key)
        {
            if(string.IsNullOrEmpty(id.ToString()) || string.IsNullOrEmpty(key))
            {
                return NotFound();
            }
            string key_secret = "tyOq7hgjpK5hhexMW";
            if (!key.Equals(key_secret))
            {
                return NotFound();
            }
            string pass = "";
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sql = "select PasswordHash from Dps_User where UserID = @Id";
                SqlConditions cond = new SqlConditions();
                cond.Add("Id", id);
                var kq = cnn.ExecuteScalar(sql, cond);
                if (kq == null || cnn.LastError != null) return BadRequest();
                pass = dpsUserMr.DecryptPassword(kq.ToString());
            }
            return Ok(pass);
        }

        #region Hàm khác
        /// <summary>
        /// Kiểm tra thông tin người dùng
        /// </summary>
        /// <param name="data">dữ liệu</param>
        /// <param name="loai">0:thêm, 1 sửa</param>
        /// <returns></returns>
        [HttpPost]
        public string KiemTraThongTinNguoiDungDPS(NguoiDungDPS data, int loai = 0)
        {
            if (data == null)
                return "Thông tin người dùng không đúng";
            if (string.IsNullOrEmpty(data.FullName))
                return "Vui lòng nhập họ tên người dùng";
            if (data.IdDonVi <= 0)
                return "Vui lòng chọn đơn vị";
            if (data.IdChucVu <= 0)
                return "Vui lòng chọn chức vụ";
            if (loai == 0)
            {
                if (string.IsNullOrEmpty(data.UserName))
                    return "Vui lòng nhập tên đăng nhập";
                if (string.IsNullOrEmpty(data.Password))
                    return "Vui lòng nhập mật khẩu";
                if (data.Password.Length < 6 || data.Password.Length > 20)
                    return "Mật khẩu phải có tối thiểu 6 và tối đa 20 ký tự";
                if (data.Password != data.RePassword)
                    return "Nhập lại mật khẩu không đúng";
                if (data.UserName.Length < 6 || data.UserName.Length > 50)
                    return "Tên đăng nhập phải có tối thiểu 6 và tối đa 20 ký tự";
                if (!Regex.IsMatch(data.UserName, "^[a-z0-9_.@-]{6,50}$"))
                {
                    return "Tên đăng nhập phải từ 6 đến 50 ký tự chữ cái, chữ số, dấu gạch dưới, dấu chấm hoặc @";
                }
            }
            return "";
        }

        #endregion
        /// <summary>
        /// Khóa/mở khóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "10")]
        [Route("lock")]
        [HttpGet]
        public BaseModel<object> Lock(int id, bool islock)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                if (id <= 0)
                {
                    return JsonResultCommon.KhongTonTai(Name);
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Dps_User where Deleted = 0 and UserID = @id ";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("id", id) }) != 1)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "update Dps_User set Active = " + (islock ? "0" : "1") + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where UserID = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        string note = islock ? "Khóa" : "Kích hoạt";
                        var data = new LiteModel() { id = id, data = new { Locked = islock } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật - " + note, logHelper.LogSua(id, loginData.Id, "", ""));
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

        /// <summary>
        /// Gia hạn ngày hết hạn theo số ngày config
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "11")]
        [Route("gia-han")]
        [HttpGet]
        public BaseModel<object> GiaHan(int id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                if (id <= 0)
                {
                    return JsonResultCommon.KhongTonTai(Name);
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string str = "select * from Sys_Config where Code='EXP_ADD'";
                    DataTable conf = cnn.CreateDataTable(str);
                    if (conf == null || conf.Rows.Count == 0)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    int num = int.Parse(conf.Rows[0]["Value"].ToString());
                    string sqlq = "select * from Dps_User where Deleted = 0 and UserID = @id ";
                    DataTable dt = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("id", id) });
                    if (dt == null || dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "update Dps_User set GiaHan = GiaHan+" + num + " , UpdatedDate = getdate(), UpdatedBy = " + iduser + " where UserID = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        logHelper.Ghilogfile<long>(id, loginData, "Gia hạn thời hạn mật khẩu", logHelper.Log(3, loginData.Id, "Gia hạn thời hạn mật khẩu", id));
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

        /// <summary>
        /// Xóa user
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "10")]
        [Route("delete")]
        [HttpGet]
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
                if (id <= 0)
                {
                    return JsonResultCommon.KhongTonTai(Name);
                }
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Dps_User where Deleted = 0 and UserID = @id ";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "update Dps_User set Deleted = 1, UpdatedBy = " + iduser + " where UserID = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["Fullname"].ToString() };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id));
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

        #region Import
        [Route("UploadFile")]
        [HttpPost]
        public object UploadFile([FromBody] FileImport data)
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);

            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            ErrorModel _error = new ErrorModel();

            try
            {

                long _idUser = loginData.Id;
                if (string.IsNullOrEmpty(data.base64) && data.fileByte == null)
                {
                    return JsonResultCommon.Custom("Chưa có file nào được upload");
                }

                Match _extension = Regex.Match(data.filename, @"(.xls$|.xlsx$)");
                if (!_extension.Success)
                    return JsonResultCommon.Custom("File không hợp lệ");

                //convert string base64 to byte array
                byte[] _fileByte = null;

                if (data.fileByte != null)
                    _fileByte = data.fileByte;
                else
                    _fileByte = Convert.FromBase64String(data.base64);

                string _path = "dulieu/Data/" + _idUser + "/Import/";

                string _targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, _path);

                if (!Directory.Exists(_targetPath))
                    Directory.CreateDirectory(_targetPath);

                string _fileName = _targetPath + "FileImportUser" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.filename;
                System.IO.File.WriteAllBytes(_fileName, _fileByte);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var excelPack = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(_fileName))
                    {
                        try
                        {
                            excelPack.Load(stream);

                            ExcelWorksheet worksheet = excelPack.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;

                            int skipRows = 1;
                            int startRow = skipRows + 1;

                            if (rowCount <= skipRows || startRow > rowCount)
                            {
                                var message = "Không có dữ liệu, vui lòng kiểm tra lại";
                                return JsonResultCommon.Custom(message);
                            }

                            List<NguoiDungDPS> dataImport = new List<NguoiDungDPS>();
                            NguoiDungDPS element;

                            List<string> ErrorList = new List<string>();
                            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                            {
                                int dem = 1;
                                for (int row = startRow; row <= rowCount; row++)
                                {
                                    string Errordefault = "- Dòng số " + dem + ": ";
                                    dem += 1;
                                    string ErrorStr = "";
                                    element = new NguoiDungDPS();

                                    if (worksheet.Cells[row, 3].Value == null || string.IsNullOrEmpty(worksheet.Cells[row, 3].Value.ToString())) // chưa nhập họ tên
                                    {
                                        if (!string.IsNullOrEmpty(ErrorStr))
                                        {
                                            ErrorStr += ", chưa nhập họ tên";
                                        }
                                        else
                                        {
                                            ErrorStr = "chưa nhập họ tên";
                                        }
                                    }

                                    if (worksheet.Cells[row, 4].Value == null || string.IsNullOrEmpty(worksheet.Cells[row, 4].Value.ToString())) //tên đăng nhập
                                    {
                                        if (!string.IsNullOrEmpty(ErrorStr))
                                        {
                                            ErrorStr += ", chưa nhập tên đăng nhập";
                                        }
                                        else
                                        {
                                            ErrorStr = "chưa nhập tên đăng nhập";
                                        }
                                    }
                                    else
                                    {
                                        string sql = "select count(*) from Dps_User where Username=@username and Deleted=0";
                                        if ((int)cnn.ExecuteScalar(sql, new SqlConditions { new SqlCondition("username", worksheet.Cells[row, 4].Value.ToString()) }) == 1)
                                        {
                                            if (!string.IsNullOrEmpty(ErrorStr))
                                            {
                                                ErrorStr += ", tên đăng nhập đã tồn tại";
                                            }
                                            else
                                            {
                                                ErrorStr = "tên đăng nhập đã tồn tại";
                                            }
                                        }
                                    }

                                    if (worksheet.Cells[row, 5].Value == null || string.IsNullOrEmpty(worksheet.Cells[row, 5].Value.ToString())) //mật khẩu
                                    {
                                        if (!string.IsNullOrEmpty(ErrorStr))
                                        {
                                            ErrorStr += ", chưa nhập mật khẩu";
                                        }
                                        else
                                        {
                                            ErrorStr = "chưa nhập mật khẩu";
                                        }
                                    }
                                    else
                                    {
                                        string sql = "select * from Sys_Config where Code='STRONG_PASS'";
                                        var dt = cnn.CreateDataTable(sql);
                                        if (dt != null && dt.Rows.Count > 0 && dt.Rows[0]["value"].ToString() == "1")
                                        {
                                            var pattern = new Regex(@"^(?=.*[a-z])(?=.*[A-Z])(?=.*\d)(?=.*[^\da-zA-Z]).{8,}$");
                                            if (!pattern.IsMatch(worksheet.Cells[row, 11].Value.ToString().Trim()))
                                            {
                                                if (!string.IsNullOrEmpty(ErrorStr))
                                                {
                                                    ErrorStr += ", mật khẩu yêu cầu: 8 kí tự trở lên bao gồm chữ hoa, chữ thường, số, ký tự đặc biệt";
                                                }
                                                else
                                                {
                                                    ErrorStr = "mật khẩu yêu cầu: 8 kí tự trở lên bao gồm chữ hoa, chữ thường, số, ký tự đặc biệt";
                                                }
                                            }
                                        }
                                    }

                                    if (worksheet.Cells[row, 7].Value != null && !string.IsNullOrEmpty(worksheet.Cells[row, 7].Value.ToString())) //đơn vị
                                    {
                                        string sql = "select count(*) from DM_DonVi where Id=@Id and Disabled=0 and Locked=0";
                                        if ((int)cnn.ExecuteScalar(sql, new SqlConditions { new SqlCondition("Id", worksheet.Cells[row, 7].Value.ToString()) }) != 1)
                                        {
                                            if (!string.IsNullOrEmpty(ErrorStr))
                                            {
                                                ErrorStr += ", đơn vị không tồn tại";
                                            }
                                            else
                                            {
                                                ErrorStr = "đơn vị không tồn tại";
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(ErrorStr))
                                        {
                                            ErrorStr += ", chưa nhập Id đơn vị";
                                        }
                                        else
                                        {
                                            ErrorStr = "chưa nhập Id đơn vị";
                                        }
                                    }

                                    if (worksheet.Cells[row, 15].Value != null && !string.IsNullOrEmpty(worksheet.Cells[row, 15].Value.ToString())) //vai trò
                                    {
                                        string sql = "select count(*) from Dps_UserGroups where IdGroup=@Id and IsDel=0 and Locked=0";
                                        if ((int)cnn.ExecuteScalar(sql, new SqlConditions { new SqlCondition("Id", worksheet.Cells[row, 15].Value.ToString()) }) != 1)
                                        {
                                            if (!string.IsNullOrEmpty(ErrorStr))
                                            {
                                                ErrorStr += ", vai trò không tồn tại";
                                            }
                                            else
                                            {
                                                ErrorStr = "vai trò không tồn tại";
                                            }
                                        }
                                    }

                                    if (worksheet.Cells[row, 6].Value != null && !string.IsNullOrEmpty(worksheet.Cells[row, 6].Value.ToString())) //chức vụ
                                    {
                                        string sql = "select count(*) from DM_ChucVu where Id=@Id and Disabled=0 and Locked=0";
                                        if ((int)cnn.ExecuteScalar(sql, new SqlConditions { new SqlCondition("Id", worksheet.Cells[row, 6].Value.ToString()) }) != 1)
                                        {
                                            if (!string.IsNullOrEmpty(ErrorStr))
                                            {
                                                ErrorStr += ", chức vụ không tồn tại";
                                            }
                                            else
                                            {
                                                ErrorStr = "chức vụ không tồn tại";
                                            }
                                        }
                                    }

                                    //if (worksheet.Cells[row, 9].Value != null || !string.IsNullOrEmpty(worksheet.Cells[row, 9].Value.ToString())) //ngày sinh
                                    //{
                                    //    try
                                    //    {
                                    //        DateTime checkdate = new DateTime(int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim().Replace("/", "-").Split('-')[2].ToString().Trim()), int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim().Replace("/", "-").Split('-')[1].ToString().Trim()), int.Parse(worksheet.Cells[row, 8].Value.ToString().Trim().Replace("/", "-").Split('-')[0].ToString().Trim()));
                                    //    }
                                    //    catch
                                    //    {
                                    //        if (!string.IsNullOrEmpty(ErrorStr))
                                    //        {
                                    //            ErrorStr += ", lỗi định dạng ngày sinh";
                                    //        }
                                    //        else
                                    //        {
                                    //            ErrorStr = "lỗi định dạng ngày sinh";
                                    //        }
                                    //    }
                                    //}

                                    if (worksheet.Cells[row, 11].Value != null && !string.IsNullOrEmpty(worksheet.Cells[row, 11].Value.ToString())) //Email
                                    {
                                        var pattern = new Regex(@"^[\w-\.]+@([\w-]+\.)+[\w-]{2,4}$");
                                        if (!pattern.IsMatch(worksheet.Cells[row, 11].Value.ToString().Trim()))
                                        {
                                            if (!string.IsNullOrEmpty(ErrorStr))
                                            {
                                                ErrorStr += ", lỗi định dạng email";
                                            }
                                            else
                                            {
                                                ErrorStr = "lỗi định dạng email";
                                            }
                                        }
                                        else
                                        {
                                            string sql = "select count(*) from Dps_User where Email=@email and Deleted=0";
                                            if ((int)cnn.ExecuteScalar(sql, new SqlConditions { new SqlCondition("email", worksheet.Cells[row, 11].Value.ToString().Trim()) }) == 1)
                                            {
                                                if (!string.IsNullOrEmpty(ErrorStr))
                                                {
                                                    ErrorStr += ", email đã được đăng ký cho 1 tài khoản trong hệ thống";
                                                }
                                                else
                                                {
                                                    ErrorStr = "email đã được đăng ký cho 1 tài khoản trong hệ thống";
                                                }
                                            }
                                        }
                                    }
                                    else
                                    {
                                        if (!string.IsNullOrEmpty(ErrorStr))
                                        {
                                            ErrorStr += ", chưa nhập email";
                                        }
                                        else
                                        {
                                            ErrorStr = "chưa nhập email";
                                        }
                                    }

                                    //if (worksheet.Cells[row, 4].Value == null && string.IsNullOrEmpty(worksheet.Cells[row, 4].Value.ToString())) //số điện thoại
                                    //{
                                    //    if (!string.IsNullOrEmpty(ErrorStr))
                                    //    {
                                    //        ErrorStr += ", chưa nhập mật khẩu";
                                    //    }
                                    //    else
                                    //    {
                                    //        ErrorStr = "chưa nhập mật khẩu";
                                    //    }
                                    //}

                                    if (!string.IsNullOrEmpty(ErrorStr))
                                    {
                                        var Addstr = Errordefault + ErrorStr;
                                        ErrorList.Add(Addstr);
                                        continue;
                                    }

                                    element.MaNV = worksheet.Cells[row, 2].Value == null ? string.Empty : worksheet.Cells[row, 2].Value.ToString().Trim();

                                    element.FullName = worksheet.Cells[row, 3].Value == null ? string.Empty : worksheet.Cells[row, 3].Value.ToString().Trim();

                                    element.UserName = worksheet.Cells[row, 4].Value == null ? string.Empty : worksheet.Cells[row, 4].Value.ToString().Trim();

                                    element.Password = worksheet.Cells[row, 5].Value == null ? string.Empty : dpsUserMr.EncryptPassword(worksheet.Cells[row, 5].Value.ToString().Trim());

                                    element.IdChucVu = worksheet.Cells[row, 6].Value == null ? 0 : int.Parse(worksheet.Cells[row, 6].Value.ToString().Trim());

                                    element.IdDonVi = worksheet.Cells[row, 7].Value == null ? 0 : int.Parse(worksheet.Cells[row, 7].Value.ToString().Trim());

                                    element.GioiTinh = worksheet.Cells[row, 8].Value == null ? 0 : (worksheet.Cells[row, 8].Value.ToString().Trim().ToLower() == "nam" ? 1 : 2);

                                    element.Cast_NgaySinh = worksheet.Cells[row, 9].Value == null ? string.Empty : worksheet.Cells[row, 9].Value.ToString().Trim().Replace("/", "-").Split('-')[2].ToString().Trim()
                                        + "-" + worksheet.Cells[row, 9].Value.ToString().Trim().Replace("/", "-").Split('-')[1].ToString().Trim()
                                        + "-" + worksheet.Cells[row, 9].Value.ToString().Trim().Replace("/", "-").Split('-')[0].ToString().Trim();

                                    element.CMTND = worksheet.Cells[row, 10].Value == null ? string.Empty : worksheet.Cells[row, 10].Value.ToString().Trim();

                                    element.Email = worksheet.Cells[row, 11].Value == null ? string.Empty : worksheet.Cells[row, 11].Value.ToString().Trim();

                                    element.PhoneNumber = worksheet.Cells[row, 12].Value == null ? string.Empty : worksheet.Cells[row, 12].Value.ToString().Trim();

                                    element.SimCA = worksheet.Cells[row, 13].Value == null ? string.Empty : worksheet.Cells[row, 13].Value.ToString().Trim();

                                    element.SerialToken = worksheet.Cells[row, 14].Value == null ? string.Empty : worksheet.Cells[row, 14].Value.ToString().Trim();

                                    //vai trò
                                    element.IdVaiTro = worksheet.Cells[row, 15].Value == null ? 0 : int.Parse(worksheet.Cells[row, 15].Value.ToString().Trim());



                                    dataImport.Add(element);
                                }
                            }
                            if (ErrorList.Count > 0)
                            {
                                var message = "";
                                foreach (var item in ErrorList)
                                {
                                    message += "<br>" + item;
                                }
                                return JsonResultCommon.Custom(message);
                            }
                            return JsonResultCommon.ThanhCong(dataImport);
                        }
                        catch (Exception ex)
                        {
                            var message = "Đọc sheet file thất bại: " + ex.Message;
                            return JsonResultCommon.Custom(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string _message = "Lỗi dữ liệu!";
                return JsonResultCommon.Custom(_message);
            }
        }

        [Route("ImportFile")]
        [HttpPost]
        public BaseModel<object> ImportFile([FromBody] List<NguoiDungDPS> data)
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
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
                    foreach (var item in data)
                    {
                        Hashtable val = new Hashtable();

                        val.Add("CreatedDate", DateTime.Now);
                        val.Add("UserCreate", loginData.Id);

                        val.Add("FullName", item.FullName);
                        val.Add("UserName", item.UserName);
                        val.Add("PasswordHash", item.Password);

                        if (!string.IsNullOrEmpty(item.MaNV))
                        { val.Add("MaNV", item.MaNV); }
                        else
                        {
                            val.Add("MaNV", "");
                        }

                        if (!string.IsNullOrEmpty(item.IdChucVu.ToString()) && item.IdChucVu != 0)
                        { val.Add("IdChucVu", item.IdChucVu); }
                        else
                        {
                            val.Add("IdChucVu", 0);
                        }

                        if (!string.IsNullOrEmpty(item.IdDonVi.ToString()) && item.IdDonVi != 0)
                        { val.Add("IdDonVi", item.IdDonVi); }
                        else
                        {
                            val.Add("IdDonVi", 0);
                        }

                        if (!string.IsNullOrEmpty(item.GioiTinh.ToString()) && item.GioiTinh != 0)
                        { val.Add("GioiTinh", item.GioiTinh); }
                        else
                        {
                            val.Add("GioiTinh", 1);
                        }

                        if (!string.IsNullOrEmpty(item.Cast_NgaySinh))
                        {
                            var ngaysinh = new DateTime(int.Parse(item.Cast_NgaySinh.Split('-')[0].ToString()), int.Parse(item.Cast_NgaySinh.Split('-')[1].ToString()), int.Parse(item.Cast_NgaySinh.Split('-')[2].ToString()));
                            val.Add("NgaySinh", ngaysinh);
                        }

                        if (!string.IsNullOrEmpty(item.CMTND))
                        { val.Add("CMTND", item.CMTND); }
                        else
                        {
                            val.Add("CMTND", "");
                        }


                        val.Add("Email", item.Email);

                        if (!string.IsNullOrEmpty(item.PhoneNumber))
                        { val.Add("PhoneNumber", item.PhoneNumber); }
                        else
                        {
                            val.Add("PhoneNumber", "");
                        }

                        if (!string.IsNullOrEmpty(item.SimCA))
                        { val.Add("SimCA", item.SimCA); }
                        else
                        {
                            val.Add("SimCA", "");
                        }

                        if (!string.IsNullOrEmpty(item.SerialToken))
                        { val.Add("SerialToken", item.SerialToken); }
                        else
                        {
                            val.Add("SerialToken", "");
                        }


                        cnn.Insert(val, "Dps_User");

                        if (!string.IsNullOrEmpty(item.IdVaiTro.ToString()) && item.IdVaiTro != 0)
                        {
                            string sqlq = "select IDENT_CURRENT('Dps_User') as Id";
                            var id = cnn.CreateDataTable(sqlq).Rows[0]["Id"].ToString();

                            Hashtable vals_Vaitro = new Hashtable();
                            vals_Vaitro.Add("IdUser", id);
                            vals_Vaitro.Add("IdGroupUser", item.IdVaiTro);
                            vals_Vaitro.Add("Priority", 1);
                            vals_Vaitro.Add("CreatedDate", DateTime.Now);
                            vals_Vaitro.Add("CreatedBy", loginData.Id);

                            cnn.Insert(vals_Vaitro, "Dps_User_GroupUser");
                        }

                    }
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                var _message = "Lỗi dữ liệu hoặc bạn phải truyền Token !" + ex.Message;
                return JsonResultCommon.Custom(_message);
            }
        }

        [Route("DownLoadFileImportMau")]
        [HttpGet]
        public FileResult DownLoadFileImportMau()
        {
            try
            {
                string fileName = "IMPORT_DONVI.xlsx";
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.TEMPLATE_IMPORT_FOLDER + "/IMPORT_DONVI.xlsx");
                var fileExists = System.IO.File.Exists(path);
                return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }


        [HttpGet]
        [Route("DownloadFileMauImportNguoiDung")]
        public ActionResult DownloadFileMauImportNguoiDung()
        {
            BaseModel<object> model = new BaseModel<object>();
            //PhanQuyenTaiKhoanController phanQuyenTaiKhoanController = new PhanQuyenTaiKhoanController();
            //model = phanQuyenTaiKhoanController.GetAllNhomNguoiDung();
            //var _datan = model.data;
            //var b = JsonConvert.SerializeObject(_datan);
            //var datan = JsonConvert.DeserializeObject<List<NhomNguoiDungModel>>(b);

            //QLDonViController qLDonViController = new QLDonViController();
            //model = qLDonViController.GetAllDonVi();
            //var _data = model.data;
            //var a = JsonConvert.SerializeObject(_data);
            //var data = JsonConvert.DeserializeObject<List<DonViModels>>(a);
            //LiteController Lite = new LiteController(null);
            //var data_donvi = JsonConvert.DeserializeObject<List<DM_DonVi_Model>>(JsonConvert.SerializeObject(Lite.DM_DanhMucDonVi_Lite().data));

            //var data_chucvu = JsonConvert.DeserializeObject<List<ChucVuModel>>(JsonConvert.SerializeObject(Lite.DM_ChucVu().data));
            //var data_vaitro = JsonConvert.DeserializeObject<List<NhomNguoiDungDPS>>(JsonConvert.SerializeObject(Lite.LayDSNhomLite().data));

            //List<NguoiDungDPS> data = new List<NguoiDungDPS>();
            var data_vaitro = new List<NguoidungVaiTro>();
            var data_donvi = new List<DM_DonVi_Model>();
            var data_chucvu = new List<_ChucVuModel>();
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"
                                    select IdGroup,GroupName from Dps_UserGroups where IsDel=0

                                    select Id,DonVi from DM_DonVi where Disabled=0 and Locked=0

                                    select Id,ChucVu from DM_ChucVu where Disabled=0 and Locked=0";
                var ds = cnn.CreateDataSet(sqlq);
                if (cnn.LastError != null || ds == null)
                {
                    return NotFound("Có lỗi xãy ra trong quá trình download file!");
                }

                data_vaitro = (from r in ds.Tables[0].AsEnumerable()
                               select new NguoidungVaiTro
                               {
                                   IdGroup = int.Parse(r["IdGroup"].ToString()),
                                   GroupName = r["GroupName"].ToString(),
                               }).ToList();

                data_donvi = (from r in ds.Tables[1].AsEnumerable()
                              select new DM_DonVi_Model
                              {
                                  Id = int.Parse(r["Id"].ToString()),
                                  DonVi = r["DonVi"].ToString(),
                              }).ToList();

                data_chucvu = (from r in ds.Tables[2].AsEnumerable()
                               select new _ChucVuModel
                               {
                                   Id = int.Parse(r["Id"].ToString()),
                                   ChucVu = r["ChucVu"].ToString(),
                               }).ToList();

            }

            try
            {

                using (MemoryStream mem = new MemoryStream())
                {
                    using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                    {
                        WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
                        workbookPart.Workbook = new Workbook();
                        DocumentFormat.OpenXml.Spreadsheet.Sheets sheets1 = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
                        #region

                        WorksheetPart worksheetPart_K = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart_K.Worksheet = new Worksheet();

                        // Adding style
                        WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                        ExportExcelHelper excelHelper = new ExportExcelHelper();
                        stylePart.Stylesheet = excelHelper.GenerateStylesheet();
                        stylePart.Stylesheet.Save();

                        //MergeCells mergeCells_K = new MergeCells();


                        SheetData sheetData_K = new SheetData();

                        //DocumentFormat.OpenXml.Spreadsheet.Sheets sheets1_K = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                        Sheet sheet_K = new Sheet();
                        sheet_K.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart_K);
                        sheet_K.SheetId = 2; //sheet Id, anything but unique
                        sheet_K.Name = "ImportNguoiDung";
                        sheets1.Append(sheet_K);



                        // Constructing header
                        DocumentFormat.OpenXml.Spreadsheet.Row row_K = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row_K.RowIndex = (uint)1;
                        row_K.Append(
                            excelHelper.ConstructCell("STT", CellValues.String, 2),
                            excelHelper.ConstructCell("Mã nhân viên", CellValues.String, 2),
                            excelHelper.ConstructCell("Họ và tên (*)", CellValues.String, 2),
                            excelHelper.ConstructCell("Tên đăng nhập (*)", CellValues.String, 2),
                            excelHelper.ConstructCell("Mật khẩu (*)", CellValues.String, 2),
                            excelHelper.ConstructCell("Id Chức vụ", CellValues.String, 2),
                            excelHelper.ConstructCell("Id đơn vị (*)", CellValues.String, 2),
                            excelHelper.ConstructCell("Giới tính", CellValues.String, 2),
                            excelHelper.ConstructCell("Ngày sinh", CellValues.String, 2),
                            excelHelper.ConstructCell("Số CMND", CellValues.String, 2),
                            excelHelper.ConstructCell("Email (*)", CellValues.String, 2),
                            excelHelper.ConstructCell("Số điện thoại", CellValues.String, 2),
                            excelHelper.ConstructCell("Số Sim CA", CellValues.String, 2),
                            excelHelper.ConstructCell("Số serial token", CellValues.String, 2),
                            excelHelper.ConstructCell("Id Vai trò", CellValues.String, 2),
                            excelHelper.ConstructCell("           ", CellValues.String, 2),
                            excelHelper.ConstructCell("Chú ý: những cột dánh dấu * bắt buộc nhập; ID đơn vị, ID vai trò, ID chức vụ tham khảo bên sheet danh sách tương ứng;", CellValues.String, 4)
                           );

                        // row_K = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        // //row.RowIndex = (uint)i + 3; //RowIndex must be start with 1, since i = 0
                        // row_K.RowIndex = (uint)(data.IndexOf(item) + 4); //RowIndex must be start with 1, since i = 0, khúc này là dòng sẽ insert vào Excel
                        // row_K.Append(l
                        // excelHelper.ConstructCell(item.MaPX.ToString(), CellValues.Number, 4),
                        // excelHelper.ConstructCell(item.TenPhuongXa, CellValues.String, 4)
                        //);

                        sheetData_K.AppendChild(row_K);

                        worksheetPart_K.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData_K);



                        //worksheetPart_K.Worksheet.InsertAfter(mergeCells_K, worksheetPart_K.Worksheet.Elements<SheetData>().First());

                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.GetFirstChild<SheetData>().FirstOrDefault());


                        #endregion

                        #region
                        WorksheetPart worksheetPart = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart.Worksheet = new Worksheet();

                        MergeCells mergeCells = new MergeCells();


                        SheetData sheetData = new SheetData();



                        Sheet sheet = new Sheet();
                        sheet.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart);
                        sheet.SheetId = 1; //sheet Id, anything but unique
                        sheet.Name = "Danh sách đơn vị";
                        sheets1.Append(sheet);




                        #region DÒNG NULL ĐẦU TIÊN - A1 -> AE1

                        DocumentFormat.OpenXml.Spreadsheet.Row rowTitle_Null = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        DocumentFormat.OpenXml.Spreadsheet.Cell dataCellnd_Null = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        dataCellnd_Null.CellReference = "A1";
                        dataCellnd_Null.DataType = CellValues.String;
                        dataCellnd_Null.StyleIndex = 8;
                        CellValue cellValue_Null = new CellValue();
                        cellValue_Null.Text = "DANH SÁCH ĐƠN VỊ";
                        dataCellnd_Null.Append(cellValue_Null);
                        rowTitle_Null.RowIndex = 1;
                        rowTitle_Null.AppendChild(dataCellnd_Null);
                        sheetData.AppendChild(rowTitle_Null);

                        MergeCells mergeCells_Null = new MergeCells();

                        //append a MergeCell to the mergeCells for each set of merged cells
                        mergeCells.Append(new MergeCell() { Reference = new StringValue("A1:D1") });

                        #endregion

                        // Constructing header
                        DocumentFormat.OpenXml.Spreadsheet.Row row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row.RowIndex = (uint)3;
                        row.Append(
                            excelHelper.ConstructCell("ID đơn vị", CellValues.String, 2),
                            excelHelper.ConstructCell("Tên đơn vị", CellValues.String, 2)
                           );

                        sheetData.AppendChild(row);
                        foreach (var item in data_donvi)
                        {
                            row = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            //row.RowIndex = (uint)i + 3; //RowIndex must be start with 1, since i = 0
                            row.RowIndex = (uint)(data_donvi.IndexOf(item) + 4); //RowIndex must be start with 1, since i = 0, khúc này là dòng sẽ insert vào Excel
                            row.Append(
                            excelHelper.ConstructCell(item.Id.ToString(), CellValues.Number, 4),
                            excelHelper.ConstructCell(item.DonVi, CellValues.String, 4)
                           );
                            sheetData.Append(row);
                        }

                        worksheetPart.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData);
                        worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.Elements<SheetData>().First());
                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.GetFirstChild<SheetData>().FirstOrDefault());

                        //spreadsheet.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(sheets1);
                        #endregion

                        #region
                        WorksheetPart worksheetPart_T = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart_T.Worksheet = new Worksheet();

                        MergeCells mergeCells_T = new MergeCells();


                        SheetData sheetData_T = new SheetData();



                        Sheet sheet_T = new Sheet();
                        sheet_T.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart_T);
                        sheet_T.SheetId = 1; //sheet Id, anything but unique
                        sheet_T.Name = "Danh sách chức vụ";
                        sheets1.Append(sheet_T);




                        #region DÒNG NULL ĐẦU TIÊN - A1 -> AE1

                        DocumentFormat.OpenXml.Spreadsheet.Row rowTitle_Null_T = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        DocumentFormat.OpenXml.Spreadsheet.Cell dataCellnd_Null_T = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        dataCellnd_Null_T.CellReference = "A1";
                        dataCellnd_Null_T.DataType = CellValues.String;
                        dataCellnd_Null_T.StyleIndex = 8;
                        CellValue cellValue_Null_T = new CellValue();
                        cellValue_Null_T.Text = "DANH SÁCH CHỨC VỤ";
                        dataCellnd_Null_T.Append(cellValue_Null_T);
                        rowTitle_Null_T.RowIndex = 1;
                        rowTitle_Null_T.AppendChild(dataCellnd_Null_T);
                        sheetData_T.AppendChild(rowTitle_Null_T);

                        MergeCells mergeCells_Null_T = new MergeCells();

                        //append a MergeCell to the mergeCells for each set of merged cells
                        mergeCells_T.Append(new MergeCell() { Reference = new StringValue("A1:D1") });

                        #endregion

                        // Constructing header
                        DocumentFormat.OpenXml.Spreadsheet.Row row_T = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row_T.RowIndex = (uint)3;
                        row_T.Append(
                            excelHelper.ConstructCell("ID chức vụ", CellValues.String, 2),
                            excelHelper.ConstructCell("Tên chức vụ", CellValues.String, 2)
                           );

                        sheetData_T.AppendChild(row_T);
                        foreach (var item in data_chucvu)
                        {
                            row_T = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            //row.RowIndex = (uint)i + 3; //RowIndex must be start with 1, since i = 0
                            row_T.RowIndex = (uint)(data_chucvu.IndexOf(item) + 4); //RowIndex must be start with 1, since i = 0, khúc này là dòng sẽ insert vào Excel
                            row_T.Append(
                            excelHelper.ConstructCell(item.Id.ToString(), CellValues.Number, 4),
                            excelHelper.ConstructCell(item.ChucVu, CellValues.String, 4)
                           );
                            sheetData_T.Append(row_T);
                        }

                        worksheetPart_T.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData_T);
                        worksheetPart_T.Worksheet.InsertAfter(mergeCells_T, worksheetPart_T.Worksheet.Elements<SheetData>().First());
                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.GetFirstChild<SheetData>().FirstOrDefault());

                        //spreadsheet.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(sheets1);
                        #endregion

                        #region
                        WorksheetPart worksheetPart_E = workbookPart.AddNewPart<WorksheetPart>();
                        worksheetPart_E.Worksheet = new Worksheet();

                        MergeCells mergeCells_E = new MergeCells();


                        SheetData sheetData_E = new SheetData();



                        Sheet sheet_E = new Sheet();
                        sheet_E.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart_E);
                        sheet_E.SheetId = 1; //sheet Id, anything but unique
                        sheet_E.Name = "Danh sách vai trò";
                        sheets1.Append(sheet_E);




                        #region DÒNG NULL ĐẦU TIÊN - A1 -> AE1

                        DocumentFormat.OpenXml.Spreadsheet.Row rowTitle_Null_E = new DocumentFormat.OpenXml.Spreadsheet.Row();

                        DocumentFormat.OpenXml.Spreadsheet.Cell dataCellnd_Null_E = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                        dataCellnd_Null_E.CellReference = "A1";
                        dataCellnd_Null_E.DataType = CellValues.String;
                        dataCellnd_Null_E.StyleIndex = 8;
                        CellValue cellValue_Null_E = new CellValue();
                        cellValue_Null_E.Text = "DANH SÁCH VAI TRÒ";
                        dataCellnd_Null_E.Append(cellValue_Null_E);
                        rowTitle_Null_E.RowIndex = 1;
                        rowTitle_Null_E.AppendChild(dataCellnd_Null_E);
                        sheetData_E.AppendChild(rowTitle_Null_E);

                        MergeCells mergeCells_Null_E = new MergeCells();

                        //append a MergeCell to the mergeCells for each set of merged cells
                        mergeCells_E.Append(new MergeCell() { Reference = new StringValue("A1:D1") });

                        #endregion

                        // Constructing header
                        DocumentFormat.OpenXml.Spreadsheet.Row row_E = new DocumentFormat.OpenXml.Spreadsheet.Row();
                        row_E.RowIndex = (uint)3;
                        row_E.Append(
                            excelHelper.ConstructCell("ID vai trò", CellValues.String, 2),
                            excelHelper.ConstructCell("Tên vai trò", CellValues.String, 2)
                           );

                        sheetData_E.AppendChild(row_E);
                        foreach (var item in data_vaitro)
                        {
                            row_E = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            //row.RowIndex = (uint)i + 3; //RowIndex must be start with 1, since i = 0
                            row_E.RowIndex = (uint)(data_vaitro.IndexOf(item) + 4); //RowIndex must be start with 1, since i = 0, khúc này là dòng sẽ insert vào Excel
                            row_E.Append(
                            excelHelper.ConstructCell(item.IdGroup.ToString(), CellValues.Number, 4),
                            excelHelper.ConstructCell(item.GroupName, CellValues.String, 4)
                           );
                            sheetData_E.Append(row_E);
                        }

                        worksheetPart_E.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData_E);
                        worksheetPart_E.Worksheet.InsertAfter(mergeCells_E, worksheetPart_E.Worksheet.Elements<SheetData>().First());
                        //worksheetPart.Worksheet.InsertAfter(mergeCells, worksheetPart.Worksheet.GetFirstChild<SheetData>().FirstOrDefault());

                        //spreadsheet.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(sheets1);
                        #endregion



                        spreadsheet.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(sheets1);

                        workbookPart.Workbook.Save();

                        spreadsheet.Close();

                        string fileName = "NguoiDung_FileMauImport.xlsx";
                        this.Response.Headers.Add("X-Filename", fileName);
                        this.Response.Headers.Add("Access-Control-Expose-Headers", "X-Filename");
                        return new FileContentResult(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                    }
                }

            }
            catch (Exception ex)
            {
                //HttpResponseMessage httpresult = new HttpResponseMessage(HttpStatusCode.OK);
                //httpresult.Content = new StringContent("Không có dữ liệu");
                return NotFound("Có lỗi xãy ra trong quá trình download file!");
            }
        }

        private string filterVaiTro(List<NguoidungVaiTro> data, int type)
        {
            if (type == 1)//tên
            {
                string res = "";
                foreach (var item in data)
                {
                    res += !string.IsNullOrEmpty(res) ? ", " + item.GroupName : item.GroupName;
                }
                return res;
            }
            else
            {
                string res = "";
                foreach (var item in data)
                {
                    res += !string.IsNullOrEmpty(res) ? ", " + item.Ma : item.Ma;
                }
                return res;
            }
        }

        [HttpGet]
        [Route("ExportExcelNguoiDung")]
        public ActionResult ExportExcelNguoiDung()
        {
            BaseModel<object> model = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return NotFound("Có lỗi xãy ra trong quá trình xuất file excel!");

            //PhanQuyenTaiKhoanController phanQuyenTaiKhoanController = new PhanQuyenTaiKhoanController();
            //model = phanQuyenTaiKhoanController.GetAllNhomNguoiDung();
            //var _datan = model.data;
            //var b = JsonConvert.SerializeObject(_datan);
            //var datan = JsonConvert.DeserializeObject<List<NhomNguoiDungModel>>(b);

            //QLDonViController qLDonViController = new QLDonViController();
            //model = qLDonViController.GetAllDonVi();
            //var _data = model.data;
            //var a = JsonConvert.SerializeObject(_data);
            //var data = JsonConvert.DeserializeObject<List<DonViModels>>(a);
            try
            {
                List<NguoiDungDPS> data = new List<NguoiDungDPS>();
                List<NguoidungVaiTro> dt_vaitro = new List<NguoidungVaiTro>();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select distinct u.*,px.DonVi, px.MaDonvi, cv.ChucVu, cv.MaChucVu from Dps_User u
                        inner join DM_DonVi px on u.IdDonVi = px.Id
						left join DM_ChucVu cv on u.IdChucVu=cv.Id
                        where u.Deleted = 0
select a.IdUser,a.IdGroupUser,b.GroupName,b.Ma from Dps_User_GroupUser a inner join Dps_UserGroups b on a.IdGroupUser=b.IdGroup and b.IsDel=0";
                    var ds = cnn.CreateDataSet(sqlq);
                    if (cnn.LastError != null || ds == null)
                    {
                        return NotFound("Có lỗi xãy ra trong quá trình xuất file excel!");
                        //return null;//httpresult;
                    }

                    dt_vaitro = (from r in ds.Tables[1].AsEnumerable()
                                 select new NguoidungVaiTro
                                 {
                                     IdUser = int.Parse(r["IdUser"].ToString()),
                                     IdGroupUser = int.Parse(r["IdGroupUser"].ToString()),
                                     GroupName = r["GroupName"].ToString(),
                                 }).ToList();

                    data = (from r in ds.Tables[0].AsEnumerable()
                            select new NguoiDungDPS
                            {
                                UserID = long.Parse(r["UserID"].ToString()),
                                FullName = r["FullName"].ToString(),
                                UserName = r["UserName"].ToString(),
                                PhoneNumber = r["PhoneNumber"] == DBNull.Value ? "" : r["PhoneNumber"].ToString(),
                                Email = r["Email"] == DBNull.Value ? "" : r["Email"].ToString(),
                                MaNV = r["MaNV"] == DBNull.Value ? "" : r["MaNV"].ToString(),
                                ViettelStudy = r["ViettelStudy"] == DBNull.Value ? "" : r["ViettelStudy"].ToString(),
                                SimCA = r["SimCA"] == DBNull.Value ? "" : r["SimCA"].ToString(),
                                CMTND = r["CMTND"] == DBNull.Value ? "" : r["CMTND"].ToString(),
                                GioiTinh = r["GioiTinh"] == DBNull.Value ? 0 : int.Parse(r["GioiTinh"].ToString()),
                                TenChucVu = r["ChucVu"] == DBNull.Value ? "" : r["ChucVu"].ToString(),
                                TenDonVi = r["DonVi"] == DBNull.Value ? "" : r["DonVi"].ToString(),
                                MaDonVi = r["MaDonVi"] == DBNull.Value ? "" : r["MaDonVi"].ToString(),
                                TenVaiTro = filterVaiTro(dt_vaitro.Where(x => x.IdUser == int.Parse(r["UserID"].ToString())).ToList(), 1),
                                MaVaiTro = filterVaiTro(dt_vaitro.Where(x => x.IdUser == int.Parse(r["UserID"].ToString())).ToList(), 2),
                                Cast_NgaySinh = r["NgaySinh"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", r["NgaySinh"]),
                                //Cast_ExpDate = r["ExpDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy}", r["ExpDate"]),
                            }).ToList();

                }


                if (data != null && data.Count > 0)
                {
                    using (MemoryStream mem = new MemoryStream())
                    {
                        using (SpreadsheetDocument spreadsheet = SpreadsheetDocument.Create(mem, SpreadsheetDocumentType.Workbook))
                        {
                            WorkbookPart workbookPart = spreadsheet.AddWorkbookPart();
                            workbookPart.Workbook = new Workbook();
                            DocumentFormat.OpenXml.Spreadsheet.Sheets sheets1 = new DocumentFormat.OpenXml.Spreadsheet.Sheets();
                            #region

                            WorksheetPart worksheetPart_K = workbookPart.AddNewPart<WorksheetPart>();
                            worksheetPart_K.Worksheet = new Worksheet();


                            MergeCells mergeCells_K = new MergeCells();

                            // Adding style
                            WorkbookStylesPart stylePart = workbookPart.AddNewPart<WorkbookStylesPart>();
                            ExportExcelHelper excelHelper = new ExportExcelHelper();
                            stylePart.Stylesheet = excelHelper.GenerateStylesheet();
                            stylePart.Stylesheet.Save();


                            SheetData sheetData_K = new SheetData();

                            //DocumentFormat.OpenXml.Spreadsheet.Sheets sheets1_K = new DocumentFormat.OpenXml.Spreadsheet.Sheets();

                            Sheet sheet_K = new Sheet();
                            sheet_K.Id = spreadsheet.WorkbookPart.GetIdOfPart(worksheetPart_K);
                            sheet_K.SheetId = 2; //sheet Id, anything but unique
                            sheet_K.Name = "ImportNguoiDung";
                            sheets1.Append(sheet_K);


                            DocumentFormat.OpenXml.Spreadsheet.Row rowTitle_Null = new DocumentFormat.OpenXml.Spreadsheet.Row();

                            DocumentFormat.OpenXml.Spreadsheet.Cell dataCellnd_Null = new DocumentFormat.OpenXml.Spreadsheet.Cell();
                            dataCellnd_Null.CellReference = "H1";
                            dataCellnd_Null.DataType = CellValues.String;
                            dataCellnd_Null.StyleIndex = 8;
                            CellValue cellValue_Null = new CellValue();
                            cellValue_Null.Text = "DANH SÁCH NGƯỜI DÙNG";
                            dataCellnd_Null.Append(cellValue_Null);
                            rowTitle_Null.RowIndex = 1;
                            rowTitle_Null.AppendChild(dataCellnd_Null);
                            sheetData_K.AppendChild(rowTitle_Null);

                            MergeCells mergeCells_Null = new MergeCells();

                            //append a MergeCell to the mergeCells for each set of merged cells
                            mergeCells_K.Append(new MergeCell() { Reference = new StringValue("H1:L1") });

                            // Constructing header
                            DocumentFormat.OpenXml.Spreadsheet.Row row_K = new DocumentFormat.OpenXml.Spreadsheet.Row();
                            row_K.RowIndex = (uint)2;
                            row_K.Append(
                                excelHelper.ConstructCell("STT", CellValues.String, 2),
                                excelHelper.ConstructCell("Mã nhân viên", CellValues.String, 2),
                                excelHelper.ConstructCell("Họ và tên", CellValues.String, 2),
                                excelHelper.ConstructCell("Tên đăng nhập", CellValues.String, 2),
                                excelHelper.ConstructCell("Tài khoản Viettel study", CellValues.String, 2),
                                excelHelper.ConstructCell("Mã vai trò", CellValues.String, 2),
                                excelHelper.ConstructCell("Tên vai trò", CellValues.String, 2),
                                excelHelper.ConstructCell("Tên chức vụ", CellValues.String, 2),
                                excelHelper.ConstructCell("Mã đơn vị", CellValues.String, 2),
                                excelHelper.ConstructCell("Tên đơn vị", CellValues.String, 2),
                                excelHelper.ConstructCell("Giới tính", CellValues.String, 2),
                                excelHelper.ConstructCell("Ngày sinh", CellValues.String, 2),
                                excelHelper.ConstructCell("Số CMND", CellValues.String, 2),
                                excelHelper.ConstructCell("Email", CellValues.String, 2),
                                excelHelper.ConstructCell("Số điện thoại", CellValues.String, 2),
                                excelHelper.ConstructCell("Số Sim CA", CellValues.String, 2),
                                excelHelper.ConstructCell("Số serial token", CellValues.String, 2)
                               //excelHelper.ConstructCell("Ngày hết hạn", CellValues.String, 2)
                               );

                            sheetData_K.AppendChild(row_K);

                            foreach (var item in data)
                            {
                                row_K = new DocumentFormat.OpenXml.Spreadsheet.Row();
                                //row.RowIndex = (uint)i + 3; //RowIndex must be start with 1, since i = 0
                                row_K.RowIndex = (uint)(data.IndexOf(item) + 3); //RowIndex must be start with 1, since i = 0, khúc này là dòng sẽ insert vào Excel
                                row_K.Append(
                                excelHelper.ConstructCell((data.IndexOf(item) + 1).ToString(), CellValues.Number, 4),
                                excelHelper.ConstructCell(item.MaNV.ToString(), CellValues.Number, 4),
                                excelHelper.ConstructCell(item.FullName, CellValues.String, 4),
                                excelHelper.ConstructCell(item.UserName, CellValues.String, 4),
                                excelHelper.ConstructCell(item.ViettelStudy, CellValues.String, 4),
                                excelHelper.ConstructCell(item.MaVaiTro, CellValues.String, 4),
                                excelHelper.ConstructCell(item.TenVaiTro, CellValues.String, 4),
                                excelHelper.ConstructCell(item.TenChucVu, CellValues.String, 4),
                                excelHelper.ConstructCell(item.MaDonVi, CellValues.String, 4),
                                excelHelper.ConstructCell(item.TenDonVi, CellValues.String, 4),
                                excelHelper.ConstructCell(item.GioiTinh == 1 ? "Nam" : (item.GioiTinh == 2 ? "Nữ" : ""), CellValues.String, 4),
                                excelHelper.ConstructCell(item.Cast_NgaySinh, CellValues.String, 4),
                                excelHelper.ConstructCell(item.CMTND, CellValues.String, 4),
                                excelHelper.ConstructCell(item.Email, CellValues.String, 4),
                                excelHelper.ConstructCell(item.PhoneNumber, CellValues.String, 4),
                                excelHelper.ConstructCell(item.SimCA, CellValues.String, 4),
                                excelHelper.ConstructCell(item.SerialToken, CellValues.String, 4),
                                excelHelper.ConstructCell(item.Cast_ExpDate, CellValues.String, 4)
                               );
                                sheetData_K.Append(row_K);
                            }

                            worksheetPart_K.Worksheet = new DocumentFormat.OpenXml.Spreadsheet.Worksheet(sheetData_K);

                            #endregion

                            spreadsheet.WorkbookPart.Workbook.AppendChild<DocumentFormat.OpenXml.Spreadsheet.Sheets>(sheets1);

                            workbookPart.Workbook.Save();

                            spreadsheet.Close();
                            logHelper.LogTai(loginData.Id);

                            string fileName = "DanhSachNguoiDung_Export_" + DateTime.Now.ToString("ddMMyyyy") + ".xlsx";
                            this.Response.Headers.Add("X-Filename", fileName);
                            this.Response.Headers.Add("Access-Control-Expose-Headers", "X-Filename");
                            return new FileContentResult(mem.ToArray(), "application/vnd.openxmlformats-officedocument.wordprocessingml.document");
                        }
                    }
                }
                else
                {
                    return NotFound("Không có dữ liệu!");
                    //return null;
                }
            }
            catch (Exception ex)
            {
                return NotFound("Có lỗi xãy ra trong quá trình xuất file excel!");
            }
        }
        #endregion
    }
}