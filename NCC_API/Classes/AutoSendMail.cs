using DpsLibs.Data;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Configuration;
using Timensit_API.Controllers.Common;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using SignalRChat.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net.Mail;
using System.Threading.Tasks;

namespace Timensit_API.Classes
{
    public class AutoSendMail
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private readonly IHubContext<ThongBaoHub> _hub_context;
        public AutoSendMail(IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context)
        {
            _hostingEnvironment = hostingEnvironment;
            _hub_context = hub_context;
            //
            // TODO: Add constructor logic here
            //
            //1p chạy 1 lần (chỉ áp dụng những chức năng cần chạy sớm và phải chạy nhanh)
            Timer1Minute = new System.Timers.Timer(60000);
            Timer1Minute.Elapsed += new System.Timers.ElapsedEventHandler(Timer1Minute_Elapsed);
            //5p chạy 1 lần
            Timer5Minute = new System.Timers.Timer(300000);
            Timer5Minute.Elapsed += new System.Timers.ElapsedEventHandler(Timer5Minute_Elapsed);
            //10p chạy 1 lần
            TimerSendNotify = new System.Timers.Timer(100000);
            TimerSendNotify.Elapsed += new System.Timers.ElapsedEventHandler(TimerSendNotify_Elapsed);
            //60p chạy 1 lần
            TimerAutoUpdate = new System.Timers.Timer(3600000);
            TimerAutoUpdate.Elapsed += new System.Timers.ElapsedEventHandler(TimerAutoUpdate_Elapsed);
        }
        public string MsgError;
        private string _basePath;
        public string BasePath
        {
            get
            {
                return _basePath;
            }
            set
            {
                _basePath = value;
            }
        }
        System.Timers.Timer TimerAutoUpdate;
        System.Timers.Timer TimerSendNotify;
        System.Timers.Timer Timer1Minute;
        System.Timers.Timer Timer5Minute;

        string CurrentMachineID = "";
        public Hashtable logtowrite = new Hashtable();
        string listerror = "";
        public static int dadocduoc = 0;
        public string NotSendmail;
        public void Start()
        {
            TimerAutoUpdate.Start();
            TimerSendNotify.Start();
            Timer1Minute.Start();
            Timer5Minute.Start();
        }
        public void Stop()
        {
            TimerAutoUpdate.Stop();
            TimerSendNotify.Stop();
            Timer1Minute.Stop();
            Timer1Minute.Stop();
        }

        protected void Timer1Minute_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }
        private void Timer5Minute_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }
        protected void TimerSendNotify_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
                IConfigurationRoot configuration = Constant.getConfig();
                string ConnectionString = configuration["NCCConfig:ConnectionString"];
                NCCConfig _config = new NCCConfig();
                _config.ConnectionString = ConnectionString;
                _config.SysName = configuration["NCCConfig:SysName"];
                _config.LinkBackend = configuration["NCCConfig:LinkBackend"];

                using (DpsConnection cnn = new DpsConnection(ConnectionString))
                {
                    //MailInfo MInfo = new MailInfo(cnn);
                    //generate task from repeated

                    EveryDayReminder(cnn, _config);
                }
            }
            catch (Exception ex)
            {
            }

        }
        void TimerAutoUpdate_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            try
            {
            }
            catch (Exception ex)
            {
            }
        }

        private void EveryDayReminder(DpsConnection cnn, NCCConfig _config)
        {
            TimeSpan thoigiannhacnho = new TimeSpan(7, 0, 0);
            SqlConditions cond = new SqlConditions();
            cond.Add("Id_row", 1);
            string select = "select giatri from tbl_thamso where (where)";
            DataTable dt = cnn.CreateDataTable(select, "(where)", cond);
            if (dt.Rows.Count > 0)
            {
                bool IsNhacnho = false;
                DateTime Gionhacnho = new DateTime();
                if ("".Equals(dt.Rows[0][0].ToString())) IsNhacnho = true;
                else
                {
                    if (DateTime.TryParse(dt.Rows[0][0].ToString(), out Gionhacnho))
                    {
                        if (Gionhacnho <= DateTime.Now)
                            IsNhacnho = true;
                    }
                }
                if (IsNhacnho)
                {
                    DateTime Gionhactieptheo = Gionhacnho.AddDays(1);
                    if (Gionhactieptheo < DateTime.Now) Gionhactieptheo = DateTime.Today.AddDays(1).Add(thoigiannhacnho);
                    //Cập nhật lại giờ nhắc tiếp theo
                    Hashtable val = new Hashtable();
                    val.Add("giatri", Gionhactieptheo.ToString());
                    cnn.Update(val, cond, "tbl_thamso");
                    //Chạy các hàm nhắc nhỡ hàng ngày
                    this.NhacNho(cnn, _config);//Nhắc nhở sắp đến hạn, trễ hạn

                    //Nhắc nhở lễ tết sắp diễn ra
                    string sql = "select * from DM_NhomLeTet where Disabled=0 and NgayKhaiBao=FORMAT (GETDATE(),'dd/MM')";
                    sql += @";select distinct ug.IdUser,r.IdRole,r.Role,u.UserID,u.Username,px.Id_Goc,px.DonVi, px.MaDonvi, px.Capcocau
                                                from Dps_User_GroupUser ug
                                                inner join Dps_UserGroupRoles gr on gr.IDGroupUser=ug.IdGroupUser
                                                inner join Dps_Roles r on r.IdRole=gr.IDGroupRole
                                                join Dps_User u on u.UserID = ug.IdUser
                                                inner join DM_DonVi px on u.IdDonVi = px.Id
                                                where r.Disabled=0 and ug.Locked=0 and ug.Disabled=0 and r.IdRole = 61 and Capcocau = 1";
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dtLe = ds.Tables[0];
                    var lst = ds.Tables[1].AsEnumerable().Select(x => long.Parse(x["IdUser"].ToString())).ToList();
                    if (dtLe != null && dtLe.Rows.Count > 0)
                    {
                        foreach (DataRow dr in dtLe.Rows)
                        {
                            string NotifyMailtitle = "Nhóm lễ tết '" + dr["NhomLeTet"].ToString() + "' sắp diễn ra";
                            DataThongBaoModel datatb = new DataThongBaoModel()
                            {
                                Loai = 0,
                                IdNguoiGui = 0,
                                CC = lst,
                                To = 0,
                                title = NotifyMailtitle,
                                url = "/dot-tang-qua",
                                content = "",
                                Email = true,
                                SMS = true
                            };
                            string fileTemp = NotifyMailtitle;
                            datatb.content = fileTemp;
                            ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                        }
                    }
                }
            }
            else
            {
                Hashtable val = new Hashtable();
                val.Add("Id_row", 1);
                val.Add("giatri", DateTime.Now.AddDays(-1));
                val.Add("mota", "Thời gian nhắc nhở tiếp theo");
                val.Add("nhom", "other");
                val.Add("id_nhom", 3);
                val.Add("Allowedit", 0);
                cnn.Insert(val, "tbl_thamso");
            }
        }
        private void NhacNho(DpsConnection cnn, NCCConfig _config)
        {
            int ngay_nhacnho = 0, ngay_nhacnho_tre = 0;
            var vals = LiteController.GetConfigs(new List<string>() { "NGAY_NHACNHO", "NGAY_NHACNHO_TRE" }, cnn);
            if (vals != null)
            {
                if (vals.ContainsKey("NGAY_NHACNHO"))
                    ngay_nhacnho = int.Parse(vals["NGAY_NHACNHO"].ToString());
                if (vals.ContainsKey("NGAY_NHACNHO_TRE"))
                    ngay_nhacnho_tre = int.Parse(vals["NGAY_NHACNHO_TRE"].ToString());
            }
            string sql = "select * from quytrinh_quatrinhduyet where valid is null";
            sql += "; select UserID, Fullname, Email, PhoneNumber from Dps_User where Deleted=0 and Active=1";
            DataSet ds = cnn.CreateDataSet(sql);
            var dtAs = ds.Tables[0].AsEnumerable();
            var groups = from r in dtAs
                         group r by new { a = r["id_phieu"], b = r["loai"] } into g
                         select new
                         {
                             id_phieu = g.Key.a,
                             loai = (int)g.Key.b,
                             quytrinh = g.OrderBy(x => x["priority"]).FirstOrDefault()
                         };
            foreach (DataRow dr in ds.Tables[1].Rows)
            {
                string idUser = dr["UserID"].ToString();
                List<int> count1, count2;
                count1 = count2 = new List<int>() { 0, 0, 0 };
                foreach (var item in groups)
                {
                    if (item.quytrinh != null && item.quytrinh["Deadline"] != DBNull.Value)
                    {
                        string checker = item.quytrinh["checker"] == DBNull.Value ? "" : item.quytrinh["checker"].ToString();
                        string checkers = item.quytrinh["checkers"] == DBNull.Value ? "" : item.quytrinh["checkers"].ToString();
                        if (checker == "" && checkers == "")
                            continue;
                        List<string> users = checkers.Split(",").ToList();
                        if (checker != "")
                            users.Add(checker);
                        if (users.Contains(idUser))
                        {
                            DateTime d = DateTime.Now.Date;
                            DateTime deadline = (DateTime)item.quytrinh["Deadline"];
                            if (deadline.Date < d)//nhắc nhở đến hạn
                            {
                                double ngay = (d - deadline.Date).TotalDays;
                                if (ngay <= ngay_nhacnho)
                                {
                                    if (item.loai == 1)//đợt tặng quà
                                        count1[0]++;
                                    if (item.loai == 2)//hồ sơ
                                        count1[1]++;
                                    if (item.loai == 3)//số liệu
                                        count1[2]++;
                                }
                            }
                            if (deadline.Date >= d)//nhắc nhở trễ hạn
                            {
                                double ngay = (deadline.Date - d).TotalDays;
                                if (ngay > ngay_nhacnho_tre)
                                {
                                    if (item.loai == 1)//đợt tặng quà
                                        count2[0]++;
                                    if (item.loai == 2)//hồ sơ
                                        count2[1]++;
                                    if (item.loai == 3)//số liệu
                                        count2[2]++;
                                }
                            }
                        }
                    }
                }
                DataThongBaoModel datatb = new DataThongBaoModel()
                {
                    Loai = 0,
                    IdNguoiGui = 0,
                    CC = new List<long>(),
                    To = long.Parse(idUser),
                    title = "",
                    url = "",
                    content = "",
                    Email = true,
                    SMS = true
                };
                string NotifyMailtitle = "Nhắc nhở quá trình duyệt sắp đến hạn";
                datatb.title = NotifyMailtitle;
                if (count1[0] > 0)
                {
                    string link = "/duyet-de-xuat";
                    string fileTemp = "Bạn có <b>" + count1[0].ToString() + " đề xuất</b> sắp hết hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
                if (count1[1] > 0)
                {
                    string link = "/duyet-ho-so";
                    string fileTemp = "Bạn có <b>" + count1[1].ToString() + " hồ sơ</b> sắp hết hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
                if (count1[2] > 0)
                {
                    string link = "/duyet-so-lieu";
                    string fileTemp = "Bạn có <b>" + count1[2].ToString() + " mẫu số liệu</b> sắp hết hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
                NotifyMailtitle = "Nhắc nhở quá trình duyệt trễ hạn";
                datatb.title = NotifyMailtitle;
                if (count2[0] > 0)
                {
                    string link = "/duyet-de-xuat";
                    string fileTemp = "Bạn có <b>" + count1[0].ToString() + " đề xuất</b> bị trễ hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
                if (count2[1] > 0)
                {
                    string link = "/duyet-ho-so";
                    string fileTemp = "Bạn có <b>" + count1[1].ToString() + " mẫu số liệu</b> bị trễ hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
                if (count2[2] > 0)
                {
                    string link = "/duyet-so-lieu";
                    string fileTemp = "Bạn có <b>" + count1[2].ToString() + " mẫu số liệu</b> bị trễ hạn xử lý";
                    datatb.url = link;
                    datatb.content = fileTemp;
                    ThongBaoHelper.sendThongBao(datatb, _config, _hostingEnvironment, _hub_context);
                }
            }
        }

        public static void SendErrorReport(string errormsg)
        {
            try
            {
                var config = Constant.getConfig();
                string mailto = config["Error:mailto"];
                string mcc = config["Error:cc"];
                if (!string.IsNullOrEmpty(mailto))
                {
                    string connectionstring = config["NCCConfig:ConnectionString"];
                    NCCConfig _config = new NCCConfig();
                    _config.ConnectionString = connectionstring;
                    _config.SysName = config["NCCConfig:SysName"];
                    _config.LinkBackend = config["NCCConfig:LinkBackend"];
                    using (DpsConnection cnn = new DpsConnection(connectionstring))
                    {
                        MailInfo MInfo = new MailInfo("info.vts.hr@gmail.com", "smtp.gmail.com", "wqdaujziaqgoatsg",true, 587);
                        MailAddressCollection cc = new MailAddressCollection();
                        if (!string.IsNullOrEmpty(mcc))
                            cc.Add(mcc);
                        string[] file = new string[0];
                        SendMail.Send(mailto, "Lỗi NCC", cc, "Nội dung lỗi: " + errormsg, file, false, out errormsg, MInfo);
                    }
                }
            }
            catch (Exception ex)
            {

            }
        }
    }
}
