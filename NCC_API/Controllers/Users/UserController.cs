using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Options;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using System.Collections;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Hosting;
using Newtonsoft.Json;
using System.Net.Http;
using System.Net.Mail;
using System.IO;
using System.Globalization;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Timensit_API.Controllers.Users
{
    [ApiController]
    [Route("api/user")]
    [EnableCors("TimensitPolicy")]
    public class UserController : ControllerBase
    {
        private IHttpContextAccessor _accessor;
        private readonly IHostingEnvironment _hostingEnvironment;
        LoginController lc;
        private NCCConfig _config;
        UserManager dpsUserMr;
        LogHelper logHelper;
        private IOptions<NCCConfig> MailConfig;
        private IHubContext<ThongBaoHub> _hub_context;
        public UserController(IOptions<NCCConfig> config, IConfiguration configLogin, IHostingEnvironment hostingEnvironment, IHttpContextAccessor accessor, IHubContext<ThongBaoHub> hub_context)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config.Value;
            lc = new LoginController(config, configLogin);
            dpsUserMr = new UserManager(config);
            dpsUserMr = new UserManager(config);
            logHelper = new LogHelper(_config, accessor, hostingEnvironment, 1);
            MailConfig = config;
            _hub_context = hub_context;
        }

        [AllowAnonymous]
        [HttpPost]
        [Route("Login")]
        public async Task<BaseModel<object>> Login([FromBody] UserModel login)
        {
            BaseModel<object> model = new BaseModel<object>();
            if (login.checkReCaptCha)
            {
                var IsValid_capcha = await validateCaptchaAsync(login.GReCaptCha ?? "");
                if (!IsValid_capcha)
                {
                    return JsonResultCommon.Custom("Mã Captcha không hợp lệ!");
                }
            }
            long vt = 0;
            if (login.cur_vaitro.HasValue)
                vt = login.cur_vaitro.Value;
            var user = lc.AuthenticateUser(login.username, login.password, vt);

            if (user != null)
            {
                if (user.Active != 1)
                {
                    return JsonResultCommon.Custom("Tài khoản này đã bị khoá, vui lòng liên hệ quản trị viên.");
                }
                if (user.ExpDate < DateTime.Now)
                {
                    return JsonResultCommon.Custom("Tài khoản này đã bị khoá do hết thời hạn đăng nhập, vui lòng liên hệ quản trị viên.");
                }
                logHelper.Log(6, user.Id, "Đăng nhập");
                return JsonResultCommon.ThanhCong(user);
            }
            return JsonResultCommon.Custom("Tài khoản hoặc mật khẩu không chính xác.");
        }

        [Authorize]
        [HttpGet]
        [Route("Logout")]
        public async Task<BaseModel<object>> Logout()
        {
            string Token = lc.GetHeader(Request);
            var user = lc._GetInfoUser(Token);

            if (user == null)
            {
                return JsonResultCommon.DangNhap();
            }
            logHelper.Log(5, user.Id, "Đăng xuất");
            return JsonResultCommon.ThanhCong();
        }

        [HttpPost]
        [Authorize()]
        [Route("ResetSession")]
        public BaseModel<object> ResetSession()
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            var user = lc._GetInfoUser(Token);

            if (user == null)
            {
                return JsonResultCommon.DangNhap();
            }
            var reset = lc.RefreshJSONWebToken(ref user);
            user.ResetToken = reset;
            return JsonResultCommon.ThanhCong(user);
        }

        [HttpGet]
        //[Authorize()]
        [Route("PermissionUrl")]
        public BaseModel<object> PermissionUrl(string currentUrl)
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            var user = lc._GetInfoUser(Token);
            if (user == null)
                return JsonResultCommon.DangNhap();
            var rules = lc._GetAllRuleUser(Token);
            try
            {
                string temp = currentUrl;
                int index = currentUrl.IndexOf("?");
                if (index >= 0)
                    currentUrl = currentUrl.Remove(index);
                if (currentUrl == "/")
                {
                    return JsonResultCommon.ThanhCong("/");
                }
                //string href = currentUrl;
                //if (href[0] == '/')
                //{
                //    href = href.Remove(0, 1);
                //}
                string[] url = currentUrl.Split('/');
                string href = "";
                if (url.Length > 0)
                    href = url[1];


                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    // Lấysubmenu
                    string sql = @"  select distinct ug.IdUser,r.IdRole,r.Href
                                from Dps_User_GroupUser ug
                                inner join Dps_UserGroupRoles gr on gr.IDGroupUser=ug.IdGroupUser
                                inner join Dps_Menu r on r.IdRole=gr.IDGroupRole
                                where ug.IdUser=@IdUser and ug.IdGroupUser=@vaitro and r.Disabled=0 and r.Href<>'0' and r.Href=@Href";
                    sql += " select * from Dps_Menu where Disabled=0 and Href<>'0' and Href=@Href and IdRole=0";
                    DataSet ds = cnn.CreateDataSet(sql, new SqlConditions { { "IdUser", user.Id }, { "vaitro", user.VaiTro }, { "Href", href } });
                    if (ds == null || (ds.Tables[0].Rows.Count == 0 && ds.Tables[1].Rows.Count == 0))
                        return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");

                    return JsonResultCommon.ThanhCong(true);
                }
            }
            catch (Exception ex)
            {
                _baseModel.status = 0;
                _baseModel.data = false;
                return _baseModel;
            }
        }

        /// <summary>
        /// Lấy menu chức năng theo phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("LayMenuChucNang")]
        public BaseModel<object> LayMenuChucNang()
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
                string sqlq = "select * from Dps_Menu where Disabled=0 and IdParent is null order by Priority";
                sqlq += @" select * from Dps_Menu
                                where (IdRole in (select IDGroupRole as Role from Dps_UserGroupRoles ugr
					                                inner join Dps_User_GroupUser u_gr on u_gr.IdGroupUser=ugr.IDGroupUser
					                                inner join Dps_User u on u.UserID=u_gr.IdUser
					                                where  u.UserID=@IDUser and u_gr.IdGroupUser=@vaitro and u_gr.Locked=0 and u_gr.Disabled=0) or IdRole=0) and Disabled=0";
                if (loginData.Capcocau == 3)
                    sqlq += " and Id not in (42,53,60, 66, 69, 70)";//bỏ menu duyệt của cấp xa
                if (loginData.Capcocau != 3)
                    sqlq += " and Id not in (61)";//bỏ menu nhập đề xuất khi k phải cấp xã
                sqlq += " order by Priority";
                SqlConditions sqlcond = new SqlConditions();
                sqlcond.Add("IDUser", iduser);
                sqlcond.Add("vaitro", loginData.VaiTro);
                DataSet ds = cnn.CreateDataSet(sqlq, sqlcond);
                if (ds == null || ds.Tables.Count == 0)
                    return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                try
                {
                    var data = from r in ds.Tables[0].AsEnumerable()
                               select new
                               {
                                   #region map menu.config.ts
                                   id = r["Id"],
                                   title = r["Title"],
                                   //bullet = "dot",
                                   //icon = r["Icon"],
                                   root = true,
                                   alignment = "left",
                                   submenu = from c in ds.Tables[1].AsEnumerable()
                                             where c["IdParent"].ToString() == r["Id"].ToString()
                                             select new
                                             {
                                                 title = c["Title"],
                                                 icon = c["Icon"],
                                                 page = !string.IsNullOrEmpty(c["Href"].ToString()) ? "/" + c["Href"] : null
                                             },
                                   #endregion
                               };
                    var menu = new BaseModel<object>
                    {
                        status = 1,
                        //data = data
                        data = data.Where(x => x.submenu.Count() > 0)
                    };
                    return menu;
                }
                catch (Exception ex)
                {
                    return JsonResultCommon.Exception(ex, ControllerContext);
                }
            }
        }

        /// <summary>
        /// Lấy menu chức năng theo phân quyền
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("getSubMenu")]
        public BaseModel<object> getSubMenu([FromQuery] QueryParams query)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            long iduser = loginData.Id;
            int active = !string.IsNullOrEmpty(query.filter["active"]) ? int.Parse(query.filter["active"]) : 0;
            if (active > 0)
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    var sqlcond = new SqlConditions();
                    var where = "";


                    string sqlq = @"select * from Dps_Menu
                                    where (IdRole in (select IDGroupRole as Role from Dps_UserGroupRoles ugr
					                                inner join Dps_User_GroupUser u_gr on u_gr.IdGroupUser=ugr.IDGroupUser
					                                inner join Dps_User u on u.UserID=u_gr.IdUser
					                                where  u.UserID=@IDUser and u_gr.IdGroupUser=@vaitro and u_gr.Locked=0 and u_gr.Disabled=0 ) or IdRole=0) and Disabled=0 and IdParent=@Active";
                    if (loginData.Capcocau == 3)
                        sqlq += " and Id not in (42,53,60, 66, 69, 70)";//bỏ menu duyệt của cấp xa
                    if (loginData.Capcocau != 3)
                        sqlq += " and Id not in (61)";//bỏ menu nhập đề xuất khi k phải cấp xã
                    sqlq += " order by Priority";
                    sqlcond.Add("IDUser", iduser);
                    sqlcond.Add("vaitro", loginData.VaiTro);
                    sqlcond.Add("Active", active);
                    DataSet ds = cnn.CreateDataSet(sqlq, sqlcond);
                    if (ds == null || ds.Tables.Count == 0)
                        return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                    try
                    {
                        var data = (from c in ds.Tables[0].AsEnumerable()
                                    select new MenuTabModel
                                    {
                                        title = c["Title"].ToString(),
                                        icon = c["Icon"].ToString(),
                                        page = !string.IsNullOrEmpty(c["Href"].ToString()) ? "/" + c["Href"] : null,
                                        num = 0
                                    }).ToList();

                        var menu = new BaseModel<object>
                        {
                            status = 1,
                            //data = data
                            data = data
                        };
                        return menu;
                    }
                    catch (Exception ex)
                    {
                        return JsonResultCommon.Exception(ex, ControllerContext);
                    }
                }
            }
            else
            {
                var menu = new BaseModel<object>
                {
                    status = 1,
                    //data = data
                    data = new List<object>()
                    {
                        new
                        {
                            title= "Màn hình chính",
                            icon= "fa fa-desktop",
                            page= "/",
                            num = 0
                         }
                    }

                };
                return menu;
            }

        }

        /// <summary>
        /// User tự thay đổi mật khẩu
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize()]
        [HttpPost]
        [Route("change-password")]
        public BaseModel<object> DoiMatKhau([FromBody] DoiMatKhau data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            long iduser = loginData.Id;
            string strRe = "";
            if (string.IsNullOrEmpty(data.OldPassword))
                strRe += "mật khẩu hiện tại";
            if (string.IsNullOrEmpty(data.NewPassword))
            {
                strRe += strRe == "" ? "" : ", ";
                strRe += "mật khẩu mới";
            }
            if (string.IsNullOrEmpty(data.RePassword))
            {
                strRe += strRe == "" ? "" : ", ";
                strRe += "xác nhận mật khẩu";
            }
            if (strRe != "")
                return JsonResultCommon.BatBuoc(strRe);
            if (data.NewPassword.Length < 6 || data.NewPassword.Length > 20)
                return JsonResultCommon.Custom("Mật khẩu phải có tối thiểu 6 và tối đa 20 ký tự");
            if (data.NewPassword != data.RePassword)
                return JsonResultCommon.Custom("Mật khẩu mới và xác nhận mật khẩu không giống nhau");
            if (data.OldPassword.Equals(data.NewPassword))
                return JsonResultCommon.Custom("Mật khẩu mới không được giống mật khẩu cũ");
            BaseModel<object> kq = dpsUserMr.ChangePass(iduser.ToString(), data.OldPassword, data.NewPassword);
            if (kq.status == 1)
            {
                int reload = 0;
                if (data.Logout)
                {
                    //string id = User.Identity.GetUserId();
                    //var t = dpsUserMr.UpdateSecurityStampAsync(id);
                    //AuthenticationManager.SignOut(DefaultAuthenticationTypes.ApplicationCookie);
                    reload = 1;
                }
                return JsonResultCommon.ThanhCong(reload);
            }
            return kq;
        }

        /// <summary>
        /// Cập nhật thông tin user
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize()]
        [HttpPost]
        [Route("update")]
        public BaseModel<object> Update_ThongTin([FromBody] NguoiDungDPS data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string filepath = "";
                if (!string.IsNullOrEmpty(data.avatar.strBase64))
                {
                    string folder = "/user/" + loginData.Id;
                    if (!UploadHelper.UploadImage(data.avatar.strBase64, data.avatar.filename, folder, _hostingEnvironment.ContentRootPath, ref filepath, true))
                        return JsonResultCommon.Custom(UploadHelper.error);
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Hashtable val = new Hashtable();
                    if (!string.IsNullOrEmpty(filepath))
                        val.Add("Avata", filepath);
                    val.Add("Email", data.Email);
                    val.Add("PhoneNumber", data.PhoneNumber);
                    SqlConditions con = new SqlConditions() { { "UserID", loginData.Id } };
                    if (cnn.Update(val, con, "Dps_User") == 1)
                    {
                        return JsonResultCommon.ThanhCong(data);
                    }
                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
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
        [Authorize()]
        [HttpPost]
        [Route("change-avatar")]
        public BaseModel<object> Change_Avatar([FromBody] FileModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                return JsonResultCommon.Custom(UploadHelper.error);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpGet]
        [Route("test")]
        public BaseModel<object> test()
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return new BaseModel<object>
                {
                    data = null,
                    status = 0,
                    error = new ErrorModel
                    {
                        // code = Constant.ERRORDATA,
                        message = "Phiên đăng nhập hết hạn vui lòng đăng nhập lại."
                    }
                };
            }
            return new BaseModel<object>
            {
                data = null,
                status = 1
            };
        }
        private async Task<bool> validateCaptchaAsync(string captchares)
        {
            ErrorModel error = new ErrorModel();
            object jres = new object();
            if (String.IsNullOrEmpty(captchares))
            {
                return false;
            }
            //var data = WebAPI_TayNinh.Classs.Common.getConfig();
            string secret_key = _config.SecretKey;
            if (string.IsNullOrEmpty(secret_key))
            {
                error.message = "Captcha không hợp lệ";
                return false;
            }
            var content = new FormUrlEncodedContent(new[]
              {
                new KeyValuePair<string, string>("secret",  secret_key),
                new KeyValuePair<string, string>("response", captchares)
              });
            HttpClient client = new HttpClient();
            var res = await client.PostAsync("https://www.google.com/recaptcha/api/siteverify", content);
            captchaResult captchaRes = JsonConvert.DeserializeObject<captchaResult>(res.Content.ReadAsStringAsync().Result);

            if (!captchaRes.success)
            {
                error.message = "Captcha không hợp lệ";
                return false;
            }


            error.message = "Xác thực Capcha Thành công";
            return true;
        }
        #region vai trò

        [HttpGet]
        [Authorize()]
        [Route("doi-vai-tro")]
        public BaseModel<object> DoiVaiTro(int VaiTro)
        {
            string Token = lc.GetHeader(Request);
            var user = lc._GetInfoUser(Token);
            if (user == null)
            {
                return JsonResultCommon.DangNhap();
            }
            string sql = "select top(1)IdGroup, GroupName from Dps_User_GroupUser ug join Dps_UserGroups g on ug.IdGroupUser=g.IdGroup where IdGroupUser=@vt and IdUser=@id and ug.Disabled=0 and ug.Locked=0 order by Priority ";

            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "vt", VaiTro }, { "id", user.Id } });
                if (dt != null && dt.Rows.Count > 0)
                {
                    user.VaiTro = int.Parse(dt.Rows[0]["IdGroup"].ToString());
                    user.TenVaiTro = dt.Rows[0]["GroupName"].ToString();
                }
                else
                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
            }
            var reset = lc.RefreshJSONWebToken(ref user);
            user.ResetToken = reset;
            return JsonResultCommon.ThanhCong(user);
        }

        [HttpGet]
        [Authorize()]
        [Route("ds-vai-tro")]
        public BaseModel<object> ListVaiTro()
        {
            string Token = lc.GetHeader(Request);
            var user = lc._GetInfoUser(Token);
            if (user == null)
            {
                return JsonResultCommon.DangNhap();
            }
            string sql = @"select g.*, dv.DonVi as TenDonVi from Dps_User_GroupUser ug 
join Dps_UserGroups g on ug.IdGroupUser = g.IdGroup
join DM_DonVi dv on g.DonVi = dv.Id
where ug.Locked = 0 and ug.Disabled = 0 and g.Locked = 0 and g.IsDel = 0 and IdUser = @Id order by ug.Priority";
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", user.Id } });
                if (dt == null || cnn.LastError != null)
                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                var data = (from r in dt.AsEnumerable()
                            select new
                            {
                                IdGroup = r["IdGroup"],
                                GroupName = r["GroupName"],
                                TenDonVi = r["TenDonVi"],
                                InUse = user.VaiTro.ToString() == r["IdGroup"].ToString()
                            }).ToList();
                return JsonResultCommon.ThanhCong(data);
            }
        }

        #endregion

        [Authorize]
        [HttpGet]
        [Route("GetInfoUser")]
        public BaseModel<object> GetInfoUser()
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
                    string sqlq = @"SELECT  * FROM Dps_User nd where UserId=@Id";
                    var ds = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", iduser } });
                    if (cnn.LastError == null && ds != null)
                    {
                        var data = (from r in ds.AsEnumerable()
                                    select new
                                    {
                                        UserID = r["UserID"],
                                        FullName = r["FullName"] == DBNull.Value ? "" : r["FullName"],
                                        UserName = r["UserName"],
                                        PhoneNumber = r["PhoneNumber"] == DBNull.Value ? "" : r["PhoneNumber"],
                                        Email = r["Email"] == DBNull.Value ? "" : r["Email"],
                                        CMTND = r["CMTND"] == DBNull.Value ? "" : r["CMTND"],
                                        NgaySinh = r["NgaySinh"] == DBNull.Value ? "" : r["NgaySinh"],
                                        Avata = r["Avata"] == DBNull.Value ? "" : _config.LinkAPI + Constant.RootUpload + r["Avata"],
                                        Sign = r["Sign"] == DBNull.Value ? "" : _config.LinkAPI + Constant.RootUpload + r["Sign"]
                                    }).FirstOrDefault();
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

        [Authorize]
        [HttpPost]
        [Route("UpdateInfoUser")]
        public BaseModel<object> UpdateInfoUser([FromBody] NguoiDungDPS data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            long iduser = loginData.Id;
            string strRe = "";
            if (string.IsNullOrEmpty(data.FullName))
                strRe += (strRe == "" ? "" : ", ") + "Họ tên";
            if (!string.IsNullOrEmpty(strRe))
                return JsonResultCommon.BatBuoc(strRe);
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                Hashtable val = new Hashtable();
                val.Add("FullName", data.FullName);
                val.Add("CMTND", data.CMTND);
                val.Add("Email", data.Email);
                val.Add("PhoneNumber", data.PhoneNumber);


                val.Add("UpdatedDate", DateTime.Now);
                val.Add("UpdatedBy", loginData.Id);

                if (!string.IsNullOrEmpty(data.Cast_NgaySinh))
                {
                    var ngaysinh = new DateTime(int.Parse(data.Cast_NgaySinh.Split('-')[0].ToString()), int.Parse(data.Cast_NgaySinh.Split('-')[1].ToString()), int.Parse(data.Cast_NgaySinh.Split('-')[2].ToString()));

                    val.Add("NgaySinh", ngaysinh);
                }
                else
                {
                    val.Add("NgaySinh", DBNull.Value);
                }


                if (!string.IsNullOrEmpty(data.Sign.strBase64))
                {
                    string linkImg = "";
                    if (!UploadHelper.UploadImage(data.Sign.strBase64, data.Sign.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                    {
                        return JsonResultCommon.Custom(UploadHelper.error);
                    }
                    val.Add("Sign", linkImg);
                }

                if (!string.IsNullOrEmpty(data.avatar.strBase64))
                {
                    string linkImg = "";
                    if (!UploadHelper.UploadImage(data.avatar.strBase64, data.avatar.filename, "/images/NguoiDung/", _hostingEnvironment.ContentRootPath, ref linkImg, true))
                    {
                        return JsonResultCommon.Custom(UploadHelper.error);
                    }
                    val.Add("Avata", linkImg);

                }

                if (cnn.Update(val, new SqlConditions { { "UserID", iduser } }, "Dps_User") == 1)
                {
                    return JsonResultCommon.ThanhCong(data);
                }
                else
                {
                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }
            }
        }

        [AllowAnonymous]
        [HttpGet]
        [Route("ForgotPassword")]
        public BaseModel<object> UpdateInfoUser(string username)
        {
            BaseModel<object> model = new BaseModel<object>();
            if (!string.IsNullOrEmpty(username))
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string guid = Guid.NewGuid().ToString();
                    //var cus = db.Sys_Users.Where(x => x.Username == username).FirstOrDefault();
                    var cus = cnn.CreateDataTable("select * from Dps_User where Username=@username", new SqlConditions { { "username", username } });
                    if (cnn.LastError != null || cus == null)
                    {
                        return JsonResultCommon.Custom("Lỗi hệ thống");
                    }
                    else
                    {
                        if (cus.Rows.Count <= 0)
                        {
                            return JsonResultCommon.Custom("Người dùng không tồn tại");
                        }
                        else
                        {
                            if (string.IsNullOrEmpty(cus.Rows[0]["Email"].ToString()))
                            {
                                return JsonResultCommon.Custom("Người dùng không có thông tin Email");
                            }
                        }
                    }

                    try
                    {

                        string newPass = Constant.RandomString(8);//dpsUserMr.EncryptPassword();
                        string sql_update = "update Dps_User set PasswordHash='" + dpsUserMr.EncryptPassword(newPass) + "', LastUpdatePass=GETDATE() where Username='" + username + "'";
                        if (cnn.ExecuteNonQuery(sql_update) != 1)
                        {
                            return JsonResultCommon.Custom("Gửi mail thất bại");
                        }
                        string Error = "";

                        //string strHTML = System.IO.File.ReadAllText(_config.LinkAPI + Constant.TEMPLATE_IMPORT_FOLDER + "/User_ForgetPass.html");
                        Hashtable kval = new Hashtable();
                        kval.Add("$nguoinhan$", cus.Rows[0]["Fullname"]);
                        kval.Add("{{NewPass}}", newPass);
                        kval.Add("$SysName$", _config.SysName);

                        MailAddressCollection Lstcc = new MailAddressCollection();
                        MailInfo minfo = new MailInfo(MailConfig.Value, int.Parse(cus.Rows[0]["IdDonVi"].ToString()));
                        string fileTemp = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.TEMPLATE_IMPORT_FOLDER + "/User_ForgetPass.html");
                        var rs = SendMail.Send(fileTemp, kval, cus.Rows[0]["Email"].ToString(), "RESET MẬT KHẨU NGƯỜI DÙNG", Lstcc, Lstcc, null, false, out Error, minfo);
                        if (string.IsNullOrEmpty(Error))
                        {
                            return JsonResultCommon.ThanhCong();
                        }
                        else
                        {
                            return JsonResultCommon.Custom("Gửi mail thất bại");
                        }
                    }
                    catch (Exception ex)
                    {
                        return JsonResultCommon.Custom("Gửi mail thất bại");
                    }
                }
            }
            else
            {
                return JsonResultCommon.Custom("Chưa nhập tên đăng nhập");
            }
        }

        #region thông báo
        [Authorize]
        [HttpGet]
        [Route("GetThongBao")]
        public BaseModel<object> GetThongBao(string lastid = "")
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                long iduser = loginData.Id;
                string whereTB = "";

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = " select  * from Tbl_Notify tb where Disabled=0 and UserID=@Id  order by CreatedDate desc";
                    var ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "Id", loginData.Id } });


                    //Thong báo
                    var temptb = ds.Tables[0].AsEnumerable();
                    var dt_tb = temptb.CopyToDataTable();
                    PageModel pageModel_tb = new PageModel();
                    int total = dt_tb.Rows.Count;
                    pageModel_tb.TotalCount = total;
                    pageModel_tb.AllPage = (int)Math.Ceiling(total / (decimal)10);
                    pageModel_tb.Size = 10;
                    pageModel_tb.Page = 0;

                    if (!string.IsNullOrEmpty(lastid)) //lấy mới nhất
                    {
                        var ids = lastid.Split("|");
                        if (ids.Length == 2)
                        {
                            dt_tb = temptb.Where(x => (long)x["IdRow"] > long.Parse(!string.IsNullOrEmpty(ids[1]) ? ids[1] : "0")).OrderByDescending(x => (DateTime)x["CreatedDate"]).CopyToDataTable();
                        }
                    }
                    else
                    {
                        // Phân trang
                        if (dt_tb != null && dt_tb.Rows.Count > 0)
                        {
                            dt_tb = dt_tb.AsEnumerable().Take(10).CopyToDataTable();
                        }
                    }
                    if (cnn.LastError == null && ds != null)
                    {
                        var data = new
                        {
                            ThongBao = new
                            {
                                Name = "Thông báo",
                                List = (from r in dt_tb.AsEnumerable()
                                        select new
                                        {
                                            IdRow = r["IdRow"],
                                            ThongBao = r["NoiDung"],
                                            Link = r["Link"],
                                            Loai = r["Loai"],
                                            IsRead = r["IsRead"],
                                            CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                        }).ToList(),
                                Unread = temptb.Where(x => !(bool)x["IsRead"]).Count(),
                                Page = pageModel_tb
                            }
                        };

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

        [Authorize]
        [HttpGet]
        [Route("GetThongBaoPage")]
        public BaseModel<object> GetThongBaoPage(bool more = true, int record = 0, int page = 0)
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
                    var where = "";
                    //if (lastid > 0)
                    //{
                    //    where += " and tb.IdRow > " + lastid;
                    //}
                    string sqlq = " select * from Tbl_Notify tb where Disabled=0 and UserID=@Id " + where + " order by tb.CreatedDate desc ";
                    var ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "Id", loginData.Id } });
                    //thông báo
                    var temptb = ds.Tables[0].AsEnumerable();
                    var dt_tb = temptb.CopyToDataTable();
                    PageModel pageModel_tb = new PageModel();
                    int total = dt_tb.Rows.Count;
                    pageModel_tb.TotalCount = total;
                    pageModel_tb.AllPage = (int)Math.Ceiling(total / (decimal)record);
                    pageModel_tb.Size = record;
                    pageModel_tb.Page = page;
                    if (more)
                    {
                        page = 1;
                        record = pageModel_tb.TotalCount;
                    }
                    // Phân trang
                    dt_tb = dt_tb.AsEnumerable().Skip((page - 1) * record).Take(record).CopyToDataTable();
                    if (cnn.LastError == null && ds != null)
                    {
                        var data = new
                        {
                            Name = "Thông báo",
                            List = (from r in dt_tb.AsEnumerable()
                                    select new
                                    {
                                        IdRow = r["IdRow"],
                                        ThongBao = r["NoiDung"],
                                        Link = r["Link"],
                                        Loai = r["Loai"],
                                        IsRead = r["IsRead"],
                                        CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                    }).ToList(),
                            page = pageModel_tb,
                            //unread = temptb.Select(x=>!(bool)x["IsRead"]).ToList().Count()
                        };

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

        [Authorize]
        [HttpGet]
        [Route("ReadNotify")]
        public async Task<BaseModel<object>> ReadNotifyAsync(long Id)
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
                    string sqlF = "select * from tbl_notify where disabled=0 and IdRow=@Id";
                    SqlConditions cond = new SqlConditions { { "Id", Id }, { "User", loginData.Id } };
                    DataTable dtF = cnn.CreateDataTable(sqlF, cond);
                    if (dtF.Rows.Count > 0)
                    {
                        var data = from r in dtF.AsEnumerable()
                                   select new ThongBaoModel()
                                   {
                                       IdRow = (long)r["IdRow"],
                                       ThongBao = r["NoiDung"].ToString(),
                                       Link = r["Link"].ToString(),
                                       Loai = (short)r["Loai"],
                                       CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                       UpdatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now),
                                       IsRead = true,
                                       IsNew = false,
                                       Disabled = false
                                   };

                        string sqlq = @"Update Tbl_Notify set isRead=1, UpdatedDate=getdate() where Disabled=0 and IdRow=@Id and UserID=@User";
                        var kq = cnn.ExecuteNonQuery(sqlq, cond);
                        if (kq <= 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        await _hub_context.Clients.All.SendAsync("recieveMessaged", data);
                    }
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        #endregion
        [HttpGet]
        [Route("get-version")]
        public BaseModel<object> GetVersion()
        {
            string _Ver = new Versioning().GetAssemblyVersion();
            var d = Versioning.GetRunningVersion();
            return new BaseModel<object>
            {
                data = new
                {
                    Version = _Ver,
                    Date = d.ToString("dd/MM/yyyy")
                },
                status = 1
            };
        }
    }

    public class captchaResult
    {
        public bool success { get; set; }
        public DateTime challenge_ts { get; set; }
        public string hostname { get; set; }
        [JsonProperty("error-codes")]
        public List<string> errors { get; set; }
    }
    public class UserModel
    {
        public string username { get; set; }
        public string password { get; set; }
        public bool checkReCaptCha { get; set; }
        public string GReCaptCha { get; set; }
        public long? cur_vaitro { get; set; }
    }
    internal class ThongTinPhanMem
    {
        public string IdApp { get; set; }
        public string AppName { get; set; }
        public string HomePath { get; set; }
        public string IdCus { get; set; }
    }


}