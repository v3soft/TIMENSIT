using DpsLibs.Data;
using DpsLibs.Web;
using Timensit_API.Controllers;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SignalRChat.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Timensit_API.Classes
{
    public class ThongBaoHelper
    {
        public string Error = "";
        public NCCConfig _config;
        IHostingEnvironment _hostingEnvironment;
        private IHubContext<ThongBaoHub> _hub_context;
        public ThongBaoHelper()
        {
        }
        public ThongBaoHelper(NCCConfig configLogin, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = configLogin;
            _hub_context = hub_context;
        }
        /// <summary>
        /// Send FCM notify
        /// Send SMS nếu true
        /// Send Email nếu true
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        public bool sendThongBaoAsync(int loai, DataThongBaoModel data)
        {
            DataTable dtF = null;
            using (var cnn = new DpsConnection(_config.ConnectionString))
            {
                string strU = "select avata from dPs_User where UserID=" + data.IdNguoiGui;
                var avt = cnn.ExecuteScalar(strU);
                string avatar = avt != null ? (_config.LinkAPI + Constant.RootUpload + avt.ToString()) : "";
                var lst = new List<long>();
                if (data.Users != null && data.Users.Count > 0)
                    lst.AddRange(data.Users);
                if (data.CC != null && data.CC.Count > 0)
                    lst.AddRange(data.CC);
                if (data.To > 0)
                    lst.Add(data.To);
                lst = lst.Distinct().ToList();
                Hashtable val = new Hashtable();
                val["NoiDung"] = data.title;
                val["Link"] = data.url;
                val["Loai"] = data.Loai;
                val["CreatedBy"] = data.IdNguoiGui;
                List<ThongBaoModel> _data = new List<ThongBaoModel>();
                foreach (long id in lst)
                {
                    val["UserID"] = id;
                    int kq = cnn.Insert(val, "Tbl_Notify");
                    int IdRow = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_Notify') AS Current_Identity; ").ToString());
                    _data.Add(new ThongBaoModel()
                    {
                        IdRow = IdRow,
                        UserID = id,
                        ThongBao = data.title,
                        Link = data.url,
                        Loai = data.Loai,
                        CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now),
                        CreatedBy = data.IdNguoiGui,
                        NguoiGui = data.NguoiGui,
                        Avata = avatar,
                        UpdatedDate = null,
                        IsRead = false,
                        IsNew = true,
                        Disabled = false
                    });
                }
                Task.Run(() => _hub_context.Clients.All.SendAsync("recieveMessaged", _data));
                string sql = " select * from FCM where endpoint is not null and IdUser in (" + string.Join(",", lst) + ") ";
                DataSet ds = cnn.CreateDataSet(sql);
                if (cnn.LastError != null)
                    return false;
                dtF = ds.Tables[0];
                if (data.SMS)
                {
                    SMSModel dataSMS = new SMSModel();
                    dataSMS.Users = data.Users;
                    dataSMS.CreatedBy = data.IdNguoiGui;
                    dataSMS.Message = data.message;
                    SMSHelper.SendSMS(dataSMS, cnn, data.Loai, data.Id);
                }
            }
            if (data.Email)
            {
                string nguoinhan = "";
                MailAddressCollection bcc = new MailAddressCollection();
                MailAddressCollection cc = new MailAddressCollection();
                MailAddressCollection to = new MailAddressCollection();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    if (data.Users != null && data.Users.Count > 0)
                    {
                        //BCC
                        string sql = "select distinct UserID,Fullname, email from Dps_User where UserID in (" + string.Join(",", data.Users) + ")";
                        DataTable dt = cnn.CreateDataTable(sql);
                        foreach (DataRow user in dt.Rows)
                        {
                            string str = "";
                            if (user["email"] != null)
                                str = user["email"].ToString();
                            bcc.Add(new MailAddress(str, user["Fullname"].ToString()));
                        }
                    }
                    if (data.CC != null && data.CC.Count > 0)
                    {
                        //cc
                        string sql = " select distinct UserID,Fullname, email from Dps_User where UserID in (" + string.Join(",", data.CC) + ")";
                        DataTable dt = cnn.CreateDataTable(sql);
                        foreach (DataRow user in dt.Rows)
                        {
                            string str = "";
                            if (user["email"] != null)
                                str = user["email"].ToString();
                            cc.Add(new MailAddress(str, user["Fullname"].ToString()));
                        }
                    }
                    if (data.To > 0)
                    {
                        string sql = " select distinct UserID,Fullname, email from Dps_User where UserID =" + data.To;
                        DataTable dt = cnn.CreateDataTable(sql);
                        string str = "";
                        if (dt.Rows[0]["email"] != null)
                            str = dt.Rows[0]["email"].ToString();
                        to.Add(new MailAddress(str, dt.Rows[0]["Fullname"].ToString()));
                        nguoinhan = dt.Rows[0]["Fullname"].ToString();
                    }
                }
                string ErrorMessage = "";
                string MailTitle = string.Format("[{0}] {1}", _config.SysName, data.title);
                Hashtable replace = new Hashtable();
                replace.Add("$nguoinhan$", nguoinhan);
                replace.Add("$title$", data.title);
                replace.Add("$sender$", data.NguoiGui == null ? "" : data.NguoiGui);
                replace.Add("$senddate$", string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now));
                replace.Add("$content$", string.IsNullOrEmpty(data.content) ? "" : data.content);
                replace.Add("$link$", _config.LinkBackend + data.url);
                replace.Add("$System$", _config.SysName);
                string TemplateMail = _hostingEnvironment.ContentRootPath + "/" + Constant.TEMPLATE_IMPORT_FOLDER + "/thong-bao.html";
                var Attachefiles = new string[0];
                if (data.FileDinhKems != null)
                    Attachefiles = (from f in data.FileDinhKems select f.src).ToArray();
                MailInfo MInfo = new MailInfo(_config, data.IdDonVi, data.Loai, data.Id);
                Task.Run(() => SendMail.Send(TemplateMail, replace, to, MailTitle, cc, bcc, Attachefiles, true, out ErrorMessage, MInfo));
            }
            if (dtF != null)
            {
                foreach (DataRow dr in dtF.Rows)
                {
                    NofUserDevice dev = new NofUserDevice()
                    {
                        IdRow = long.Parse(dr["IdRow"].ToString()),
                        IdUser = long.Parse(dr["IdUser"].ToString()),
                        Token = dr["Token"].ToString(),
                        Auth = dr["auth"].ToString(),
                        EndPoint = dr["endpoint"].ToString(),
                        P256dh = dr["p256dh"].ToString(),
                        PrivateKey = dr["PrivateKey"].ToString(),
                        PublicKey = dr["PublicKey"].ToString()
                    };
                    string icon = _config.LinkBackend + "/assets/media/logos/favicon.ico";
                    Task.Run(() => FCMController.push(dev, new NofModel(data.title, data.url, data.content, icon)));
                }
            }
            return true;
        }

        public static bool sendThongBao(int loai, long user, List<long> lst, string title, string url, NCCConfig configLogin, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context, bool email = true, bool sms = true)
        {
            DataThongBaoModel data = new DataThongBaoModel()
            {
                Loai = loai,
                IdNguoiGui = user,
                CC = lst,
                title = title,
                url = url,
                Email = email,
                SMS = sms
            };
            ThongBaoHelper helper = new ThongBaoHelper(configLogin, hostingEnvironment, hub_context);
            return helper.sendThongBaoAsync(loai, data);
        }

        public static bool sendThongBao(DataThongBaoModel data, NCCConfig configLogin, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context)
        {
            ThongBaoHelper helper = new ThongBaoHelper(configLogin, hostingEnvironment, hub_context);
            return helper.sendThongBaoAsync(0, data);
        }
    }

    public class DataThongBaoModel
    {
        /// <summary>
        /// Sys_SMS.Loai 1: văn bản đến, 2: văn bản đi, 3: hồ sơ, 4: công việc, 5 thông báo, 6 lịch, 7 ý kiến, 8 lịch sử, 9 comment,10 đề xuất duyệt,11 thông báo đợt tặng quà
        /// Map với Tbl_YKienXuLy
        /// </summary>
        public int Loai { get; set; }
        /// <summary>
        /// Id tương ứng vs loại
        /// </summary>
        public long Id { get; set; }
        public long IdNguoiGui { get; set; }
        public string NguoiGui { get; set; }
        public long IdDonVi { get; set; }
        public bool Email { get; set; }
        public bool SMS { get; set; }
        public string message { get; set; }
        public string url { get; set; }
        public string title { get; set; }
        public string content { get; set; }
        //public string template { get; set; }
        public long To { get; set; }
        public List<long> Users { get; set; }//BCC
        public List<long> CC { get; set; }
        public List<ListImageModel> FileDinhKems { get; set; }
    }
    public class ThongBaoModel
    {
        public long IdRow { get; set; }
        public long UserID { get; set; }
        public string ThongBao { get; set; }
        public string NoiDung
        {
            get { return ThongBao; }
        }
        public string Link { get; set; }
        public int Loai { get; set; }
        public bool IsRead { get; set; } = false;
        public bool IsNew { get; set; } = true;
        public bool Disabled { get; set; } = false;
        public long CreatedBy { get; set; }
        public string NguoiGui { get; set; }
        public string Avata { get; set; }
        public string CreatedDate { get; set; }
        public string UpdatedDate { get; set; }
    }
}
