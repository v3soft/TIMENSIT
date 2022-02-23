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
using Microsoft.AspNetCore.Http;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.QuanLyNguoiDung
{
    /// <summary>
    /// Quản lý vai trò người dùng<para/>
    /// Quyền 24:	Xem danh sách 
    /// Quyền 25:	Cập nhật 
    /// </summary>
    [ApiController]
    [Route("api/vai-tro-nguoi-dung")]
    [EnableCors("TimensitPolicy")]
    public class VaiTroNguoiDungController : ControllerBase
    {
        LogHelper logHelper;
        UserManager dpsUserMr;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Vai trò người dùng";
        public VaiTroNguoiDungController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            dpsUserMr = new UserManager(configLogin);
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 4);
        }

        #region vai trò người dùng
        [Authorize(Roles = "24")]
        [HttpGet]
        [Route("list")]
        public BaseModel<object> VaiTroNguoiDung(int id)
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
                    string sqlq = @"SELECT x.*,g.GroupName,dv.Id as DonVi, dv.DonVi as TenDonVi  FROM Dps_User_GroupUser x
join Dps_UserGroups g on x.IdGroupUser=g.IdGroup
join DM_DonVi dv on g.DonVi=dv.Id
where IdUser=@Id and x.Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError == null && dt != null)
                    {
                        var data = (from r in dt.AsEnumerable()
                                    select new
                                    {
                                        IdRow = r["IdRow"],
                                        UserID = r["IdUser"],
                                        IdGroup = r["IdGroupUser"],
                                        GroupName = r["GroupName"],
                                        DonVi = r["DonVi"],
                                        TenDonVi = r["TenDonVi"],
                                        NguoiKy = r["NguoiKy"],
                                        XuLyViec = r["XuLyViec"],
                                        LanhDao = r["LanhDao"],
                                        NhanVanBan = r["NhanVanBan"],
                                        Locked = r["Locked"],
                                        Priority = r["Priority"]
                                    }).ToList();
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

        [Authorize(Roles = "25")]
        [HttpPost]
        [Route("update")]
        public BaseModel<object> CapNhatVaiTro([FromBody] NguoidungVaiTro data)
        {
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
                    string strRe = "";
                    if (data.IdGroupUser <= 0)
                        strRe += "vai trò";
                    if (strRe != "")
                        return JsonResultCommon.BatBuoc(strRe);
                    string sql = "select * from Dps_User_GroupUser where disabled=0 and IdUser=@Id and IdGroupUser=@group";
                    if (data.IdRow > 0)
                        sql += " and IdRow<>" + data.IdRow;
                    DataTable dtFind = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", data.IdUser }, { "group", data.IdGroupUser } });
                    if (dtFind != null && dtFind.Rows.Count > 0)
                        return JsonResultCommon.Custom("Vai trò này đã được phân cho người dùng");
                    Hashtable val = new Hashtable();
                    val["IdUser"] = data.IdUser;
                    val["IdGroupUser"] = data.IdGroupUser;
                    val["NguoiKy"] = data.NguoiKy;
                    val["XuLyViec"] = data.XuLyViec;
                    val["LanhDao"] = data.LanhDao;
                    val["NhanVanBan"] = data.NhanVanBan;
                    val["Locked"] = data.Locked;
                    val["Priority"] = data.Priority;
                    int kq = 0;
                    if (data.IdRow > 0)
                    {
                        val["UpdatedDate"] = DateTime.Now;
                        val["UpdatedBy"] = loginData.Id;
                        kq = cnn.Update(val, new SqlConditions() { { "Idrow", data.IdRow } }, "Dps_User_GroupUser");
                    }
                    else
                    {
                        val["CreatedDate"] = DateTime.Now;
                        val["CreatedBy"] = loginData.Id;
                        kq = cnn.Insert(val, "Dps_User_GroupUser");
                    }
                    if (kq != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    string note = "Cập nhật vai trò cho người dùng";
                    logHelper.Ghilogfile<NguoidungVaiTro>(data, loginData, note, logHelper.LogSua(data.IdUser, loginData.Id, "", note));
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize(Roles = "25")]
        [Route("delete")]
        [HttpGet]
        public BaseModel<object> XoaVaiTro(int id)
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
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Dps_User_GroupUser where Disabled = 0 and IdRow = @id ";
                    DataTable dt = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("id", id) });
                    if (dt == null && dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Vai trò người dùng");
                    sqlq = "update Dps_User_GroupUser set Disabled=1, UpdatedDate = getdate(), UpdatedBy = " + loginData.Id + " where IdRow = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        int iduser = int.Parse(dt.Rows[0]["IdUser"].ToString());
                        string note = "Xóa vai trò cho người dùng";
                        var data = new LiteModel() { id = iduser, data = new { id = id, disabled = true } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, note, logHelper.LogSua(iduser, loginData.Id, "", note));
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

        [Authorize(Roles = "25")]
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
                    string sqlq = "select * from Dps_User_GroupUser where Disabled = 0 and IdRow = @id ";
                    DataTable dt = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("id", id) });
                    if (dt == null && dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Vai trò người dùng");
                    sqlq = "update Dps_User_GroupUser set Locked=" + (islock ? "1" : "0") + ", UpdatedDate = getdate(), UpdatedBy = " + iduser + " where IdRow = " + id;
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        int idU = int.Parse(dt.Rows[0]["IdUser"].ToString());
                        string note = (islock ? "Khóa" : "Mở khóa") + " vai trò cho người dùng";
                        var data = new LiteModel() { id = id, data = new { Locked = islock } };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, note, logHelper.Log(3, loginData.Id, note, idU));
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
        #endregion
    }
}