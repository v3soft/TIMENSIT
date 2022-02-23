using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using DpsLibs.Data;
using System.Data;
using System.Linq;
using System.Security.Claims;
using System.Security.Principal;
using DpsLibs;
using System.Collections;
using Microsoft.Extensions.Configuration;
using Timensit_API.Models.Common;
using Microsoft.Extensions.Options;
using Timensit_API.Models;
using System.Net.Mail;
using Timensit_API.Classes;
using System.IO;
using Microsoft.AspNetCore.Hosting;
using Timensit_API.Controllers.Common;

namespace Timensit_API.Classes
{
    /// <summary>
    /// quản lý tài khoản
    /// </summary>
    public class UserManager
    {
        private NCCConfig _config;
        private const string PASSWORD_ED = "rpNuGJebgtBEp0eQL1xKnqQG";
        private IOptions<NCCConfig> MailConfig;
        private readonly IHostingEnvironment _hostingEnvironment;
        public UserManager()
        {
        }
        public UserManager(IOptions<NCCConfig> configLogin)
        {
            _config = configLogin.Value;
        }
        public UserManager(IOptions<NCCConfig> configLogin, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            MailConfig = configLogin;
            _hostingEnvironment = hostingEnvironment;
        }
        #region Quản lý người dùng
        /// <summary>
        /// Tìm người dùng
        /// </summary>
        /// <param name="userName"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public LoginData FindAsync(string userName, string password, long cur_Vaitro = 0)
        {
            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"select u.*, dm.title as DonVi,dm.Code as MaDinhDanh, dm.Capcocau, dm.ID_Goc,cc1.ID_Goc as Id_Goc_Cha from Dps_User u 
join Tbl_Cocautochuc dm on u.IdDonVi=dm.RowID
left join Tbl_Cocautochuc cc1 on dm.ParentID=cc1.RowID
where u.Deleted = 0 and u.Username = @UserName";
                Tb = Conn.CreateDataTable(sqlq, new SqlConditions() { { "UserName", userName } });
                if (Tb == null || Tb.Rows.Count != 1)
                    return null;
                var rw = Tb.Rows[0];
                string hash = rw["PasswordHash"].ToString();
                string pass = DpsLibs.Common.EncDec.Decrypt(hash, PASSWORD_ED);
                if (password.Equals(pass))
                {
                    LoginData _Dpsuser = new LoginData();
                    _Dpsuser.Id = long.Parse(rw["UserID"].ToString());
                    _Dpsuser.UserName = rw["UserName"].ToString();
                    _Dpsuser.FullName = rw["FullName"].ToString();
                    _Dpsuser.ChucVu = rw["IdChucVu"] != DBNull.Value ? rw["IdChucVu"].ToString() : "";
                    _Dpsuser.Email = rw["Email"] != DBNull.Value ? rw["Email"].ToString() : "";
                    _Dpsuser.SDT = rw["PhoneNumber"] != DBNull.Value ? rw["PhoneNumber"].ToString() : "";
                    _Dpsuser.IdDonVi = rw["IdDonVi"] != DBNull.Value ? int.Parse(rw["IdDonVi"].ToString()) : 0;
                    _Dpsuser.Capcocau = rw["Capcocau"] != DBNull.Value ? int.Parse(rw["Capcocau"].ToString()) : 0;
                    _Dpsuser.ID_Goc = rw["ID_Goc"] != DBNull.Value ? int.Parse(rw["ID_Goc"].ToString()) : 0;
                    _Dpsuser.ID_Goc_Cha = rw["ID_Goc_Cha"] != DBNull.Value ? int.Parse(rw["ID_Goc_Cha"].ToString()) : 0;
                    _Dpsuser.DonVi = rw["DonVi"] != DBNull.Value ? rw["DonVi"].ToString() : "";
                    _Dpsuser.MaDinhDanh = rw["MaDinhDanh"] != DBNull.Value ? rw["MaDinhDanh"].ToString() : "";
                    _Dpsuser.Active = rw["Active"] != DBNull.Value ? int.Parse(rw["Active"].ToString()) : 0;
                    _Dpsuser.Avata = LiteController.genLinkAvatar(_config.LinkAPI, rw["Avata"]);
                    _Dpsuser.LastUpdatePass = (DateTime)rw["LastUpdatePass"];
                    _Dpsuser.IdTinh = long.Parse(_config.IdTinh);
                    string strVT = "select IdGroup, GroupName from Dps_User_GroupUser ug join Dps_UserGroups g on ug.IdGroupUser=g.IdGroup where IdUser=" + _Dpsuser.Id + " and ug.Disabled=0 and ug.Locked=0 order by Priority";
                    DataTable dt = Conn.CreateDataTable(strVT);
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        if (cur_Vaitro > 0)
                        {
                            var find = dt.Select("IdGroup=" + cur_Vaitro);
                            if(find.Count()>0)
                            {
                                _Dpsuser.VaiTro = int.Parse(find[0]["IdGroup"].ToString());
                                _Dpsuser.TenVaiTro = find[0]["GroupName"].ToString();
                            }    
                        }
                        if(_Dpsuser.VaiTro==0)
                        {
                            _Dpsuser.VaiTro = int.Parse(dt.Rows[0]["IdGroup"].ToString());
                            _Dpsuser.TenVaiTro = dt.Rows[0]["GroupName"].ToString();
                        }
                    }
                    else
                    {
                        _Dpsuser.VaiTro = 0;
                        _Dpsuser.TenVaiTro = "";
                    }

                    string str = "select * from Sys_Config where Code='EXP_PASS'";
                    DataTable conf = Conn.CreateDataTable(str);
                    if (conf != null && conf.Rows.Count > 0)
                    {
                        int num = int.Parse(conf.Rows[0]["Value"].ToString());
                        if (num == 0)//k xét thời hạn
                            _Dpsuser.ExpDate = null;
                        else
                        {
                            DateTime exp = _Dpsuser.LastUpdatePass.AddDays(num + (int)rw["GiaHan"]);
                            _Dpsuser.ExpDate = exp;
                        }
                    }
                    return _Dpsuser;
                }
            }
            return null;
        }
        /// <summary>
        /// Chỉ lấy tất cả quyền của user
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public List<long> GetRules(long IdUser, int VaiTro)
        {
            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"select distinct ug.IdUser,r.IdRole,r.Role
                                from Dps_User_GroupUser ug
                                inner join Dps_UserGroupRoles gr on gr.IDGroupUser=ug.IdGroupUser
                                inner join Dps_Roles r on r.IdRole=gr.IDGroupRole
                                where ug.IdUser=@UserID and r.Disabled=0";
                sqlq += " and ug.Locked=0 and ug.Disabled=0 and ug.IdGroupUser=@VaiTro";
                //string sqlq = "exec [spn_GetRoleByUser] @UserID";
                Tb = Conn.CreateDataTable(sqlq, new SqlConditions() { { "UserID", IdUser }, { "VaiTro", VaiTro } });
                if (Tb == null)
                    return null;
            }
            var slist = new List<long>();
            foreach (DataRow r in Tb.Rows)
            {
                slist.Add(long.Parse(r["IdRole"].ToString()));
            }
            return slist;
        }

        /// <summary>
        /// Lấy danh sách UserId theo quyền
        /// </summary>
        /// <param name="role"></param>
        /// <returns></returns>
        public List<long> GetUserByRole(long role)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {

                string sqlQ = @"select distinct u.UserID from Dps_User u
join Dps_User_GroupUser ug on u.UserID = ug.IdUser
join Dps_UserGroups g on g.IdGroup = ug.IdGroupUser and g.Locked = 0
join Dps_UserGroupRoles gr on gr.IDGroupUser = g.IdGroup
join Dps_Roles r on r.IdRole = gr.IDGroupRole
 where u.Deleted = 0 and ug.Disabled = 0 and g.IsDel = 0 and r.IdRole = " + role;
                DataTable dt = cnn.CreateDataTable(sqlQ);
                if (dt == null)
                    return null;
                var lst = (from r in dt.AsEnumerable() select long.Parse(r[0].ToString())).ToList();
                return lst;
            }
        }
        /// <summary>
        /// Lấy danh sách nhóm người dùng
        /// </summary>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public List<int> GetUserGroup(string IdUser)
        {
            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString)) //db QLPA
            {
                string sqlq = @"select distinct a.IdUser,b.IdGroup,b.GroupName
                                    from Tbl_User_GroupUser a
                                    inner join Dps_User u on a.IdUser=a.IdUser
                                    inner join Dps_UserGroups b on b.IdGroup=a.IdGroupUser
                                    where a.IdUser=@IdUser 
                                ";
                //string sqlq = "exec [spn_GetRoleByUser] @UserID";
                Tb = Conn.CreateDataTable(sqlq, new SqlConditions() { { "IdUser", IdUser } });
                if (Conn.LastError != null || Tb == null)
                    return null;
            }
            var slist = new List<int>();
            foreach (DataRow r in Tb.Rows)
            {
                slist.Add(int.Parse(r["IdGroup"].ToString()));
            }
            return slist;
        }
        /// <summary>
        /// Lấy danh sách nhóm quyền của nhóm người dùng 
        /// </summary>
        /// <param name="group"></param>
        /// <returns></returns>
        public Dictionary<int, List<int>> GetGroupRole_Roles(List<int> group)
        {
            Dictionary<int, List<int>> dic = new Dictionary<int, List<int>>();
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString)) //db QLPA
            {
                for (int i = 0; i < group.Count; i++)
                {
                    int idusergroup = group[i];
                    string sql = @"select g.IDGroupUser,g.IDGroupRole,r.RoleGroupName
                                from Dps_RoleGroups r
                                inner join Dps_UserGroupRoles g on g.IDGroupRole=r.GroupID
                                where IDGroupUser=@IDGroupUser";
                    DataTable dt = Conn.CreateDataTable(sql, new SqlConditions { { "IDGroupUser", idusergroup } });
                    if (Conn.LastError != null || dt == null)
                        return null;
                    var slist = new List<int>();
                    foreach (DataRow r in dt.Rows)
                    {
                        slist.Add(int.Parse(r["IDGroupRole"].ToString()));
                    }
                    dic.Add(idusergroup, slist);
                }
            }
            return dic;
        }

        /// <summary>
        /// đổi mật khẩu
        /// </summary>
        /// <param name="iduser">id người dùng</param>
        /// <param name="oldpassword">mật khẩu cũ</param>
        /// <param name="password">mật khẩu mới</param>
        /// <returns></returns>
        public BaseModel<object> ChangePass(string iduser, string oldpassword, string password)
        {
            if (string.IsNullOrEmpty(password) || password.Length < 6)
                return JsonResultCommon.Custom("Mật khẩu mới quá ngắn");
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                var Tb = Conn.CreateDataTable("select PasswordHash from Dps_User where UserID = @Id", new SqlConditions() { { "Id", iduser } });
                if (Tb == null || Tb.Rows.Count != 1)
                    return JsonResultCommon.KhongTonTai();
                if (!oldpassword.Equals(DecryptPassword(Tb.Rows[0]["PasswordHash"].ToString())))
                    return JsonResultCommon.Custom("Mật khẩu cũ không chính xác");
                string newpass = EncryptPassword(password);
                var val = new Hashtable();
                val.Add("PasswordHash", newpass);
                val.Add("LastUpdatePass", DateTime.Now);
                val.Add("GiaHan", 0);
                if (Conn.Update(val, new SqlConditions { new SqlCondition("UserID", iduser) }, "Dps_User") != 1)
                {
                    return JsonResultCommon.SQL(Conn.LastError.Message);
                }
                return JsonResultCommon.ThanhCong();
            }
        }
        /// <summary>
        /// reset mật khẩu
        /// </summary>
        /// <param name="iduser"></param>
        /// <param name="password"></param>
        /// <returns></returns>
        public string ResetPass(string iduser, string password)
        {
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                var Tb = Conn.CreateDataSet(@"select * from Dps_User where UserID = @Id 
                                                select * from Sys_Config where Code='SEND_MAIL_RESET_PASS'", new SqlConditions() { { "Id", iduser } });
                if (Tb == null || Tb.Tables[0].Rows.Count != 1)
                    return "Tài khoản không tồn tại";
                string newpass = EncryptPassword(password);
                var val = new Hashtable();
                val.Add("PasswordHash", newpass);
                val.Add("LastUpdatePass", DateTime.Now);
                val.Add("GiaHan", 0);
                Conn.BeginTransaction();
                if (Conn.Update(val, new SqlConditions { new SqlCondition("UserID", iduser) }, "Dps_User") != 1)
                {
                    Conn.RollbackTransaction();
                    return "Không thể thay đổi mật khẩu";
                }


                #region gửi mail

                try
                {
                    if (Tb.Tables[1].Rows.Count > 0)
                    {
                        if (Tb.Tables[1].Rows[0]["Value"].ToString() == "1")
                        {
                            if (string.IsNullOrEmpty(Tb.Tables[0].Rows[0]["Email"].ToString()))
                            {
                                Conn.RollbackTransaction();
                                return "Không thể thay đổi mật khẩu";// "Người dùng không có thông tin Email";
                            }
                            string Error = "";

                            //string strHTML = System.IO.File.ReadAllText(_config.LinkAPI + Constant.TEMPLATE_IMPORT_FOLDER + "/User_ForgetPass.html");
                            Hashtable kval = new Hashtable();
                            kval.Add("{{NewPass}}", password);
                            kval.Add("$nguoinhan$", Tb.Tables[0].Rows[0]["Fullname"]);
                            kval.Add("$SysName$", _config.SysName);

                            MailAddressCollection Lstcc = new MailAddressCollection();
                            MailInfo minfo = new MailInfo(MailConfig.Value, int.Parse(Tb.Tables[0].Rows[0]["IdDonVi"].ToString()));
                            if (minfo.Id > 0)
                            {
                                string fileTemp = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.TEMPLATE_IMPORT_FOLDER + "/User_ForgetPass.html");
                                var rs = SendMail.Send(fileTemp, kval, Tb.Tables[0].Rows[0]["Email"].ToString(), "RESET MẬT KHẨU NGƯỜI DÙNG", Lstcc, Lstcc, null, false, out Error, minfo);
                                if (!string.IsNullOrEmpty(Error))
                                {
                                    Conn.RollbackTransaction();
                                    return "Không thể thay đổi mật khẩu";//"Gửi mail thất bại";
                                }
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    Conn.RollbackTransaction();
                    return "Không thể thay đổi mật khẩu";// "Gửi mail thất bại";
                }

                #endregion




                Conn.EndTransaction();
                return "";
            }
        }
        /// <summary>
        /// kiểm tra người dùng có tồn tại
        /// </summary>
        /// <param name="UserNameorID">id người dùng hoặc tên đăng nhập</param>
        /// <param name="loai">0: kiểm tra bằng ID, 1: username</param>
        /// <returns></returns>
        public bool CheckNguoiDung(string UserNameorID, int loai)
        {
            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                SqlConditions sqlcond = new SqlConditions();

                string sqlq = "";
                if (loai == 1)
                {
                    sqlcond.Add("UserName", UserNameorID);
                    sqlq = "select [UserID] from Dps_User where Deleted = 0 and UserName = @UserName";
                }
                if (loai == 0)
                {
                    sqlcond.Add("Id", UserNameorID);
                    sqlq = "select [UserID] from Dps_User where Deleted = 0 and UserID = @Id";
                }
                Tb = Conn.CreateDataTable(sqlq, sqlcond);
            }
            if (Tb.Rows.Count == 1)
            {
                return true;
            }
            return false;
        }

        /// <summary>
        /// kiểm tra email người dùng có tồn tại
        /// </summary>
        /// <param name="email">email</param>
        /// <param name="UserId">0: khi insert, 1: khi update</param>
        /// <returns></returns>
        public bool CheckEmail(string email, long UserId)
        {
            DataTable Tb = null;
            using (DpsConnection Conn = new DpsConnection(_config.ConnectionString))
            {
                SqlConditions sqlcond = new SqlConditions();

                string sqlq = "";
                string idstr = "";


                sqlcond.Add("email", email);
                if (UserId > 0)
                {
                    idstr = " and UserID <> @Id";
                    sqlcond.Add("Id", UserId);
                }
                sqlq = $"select [UserID] from Dps_User where Deleted = 0 {idstr} and Email=@email";

                Tb = Conn.CreateDataTable(sqlq, sqlcond);
            }
            if (Tb.Rows.Count == 1)
            {
                return true;
            }
            return false;
        }
        #endregion

        #region mã hóa

        //mã hoá mật khẩu
        public string EncryptPassword(string password)
        {
            return DpsLibs.Common.EncDec.Encrypt(password, PASSWORD_ED);
        }
        public string DecryptPassword(string password)
        {
            return DpsLibs.Common.EncDec.Decrypt(password, PASSWORD_ED);
        }
        #endregion
    }
}