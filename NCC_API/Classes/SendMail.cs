using DpsLibs.Data;
using Timensit_API.Models.Common;
using Microsoft.Extensions.Options;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Mail;
using System.Threading.Tasks;
using Timensit_API.Models.Process;

namespace Timensit_API.Classes
{
    /// <summary>
    /// Config email lưu trữ trong sys_config group = 3 
    /// Password mail có mã hóa
    /// </summary>
    public class MailInfo
    {
        public NCCConfig _config;
        public string Email;
        public string UserName;
        public string SmptClient;
        public string Password;
        public bool EnableSSL;
        public int Port;
        public int Loai;
        public long Id;
        public MailInfo()
        { }

        public MailInfo(string _Email, string _SmptClient, string _Password, bool _EnableSSL, int _Port)
        {
            Email = _Email;
            UserName = _Email;
            SmptClient = _SmptClient;
            Password = _Password;
            EnableSSL = _EnableSSL;
            Port = _Port;
        }

        public MailInfo(NCCConfig configLogin, long DonVi, int loai = 0, long id = 0)
        {
            _config = configLogin;
            Loai = loai;
            Id = id;
            InitialData(DonVi);
        }

        private void InitialData(long DonVi)
        {
            string sql = @"select * from Tbl_CauHinh_Mail email left join Tbl_CauHinh_DonViCon dv on email.Id=dv.IdCauHinh and dv.Loai=1 and dv.Disabled=0
where email.Disabled=0 and email.Locked=0 and (email.DonVi = @DonVi or dv.DonVi=@DonVi or email.DonVi=0) order by email.DonVi desc";
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions { { "DonVi", DonVi } });
                if (dt == null || dt.Rows.Count == 0)
                {
                    Email = "";
                    return;
                }
                var item = dt.Rows[0];

                if (item["UserName"] != DBNull.Value)
                {
                    Email = item["UserName"].ToString();
                    UserName = item["UserName"].ToString();
                }
                if (item["Password"] != DBNull.Value)
                {
                    Password = item["Password"].ToString();
                }
                if (item["Server"] != DBNull.Value)
                {
                    SmptClient = item["Server"].ToString();
                }
                if (item["EnableSSL"] != DBNull.Value)
                {
                    EnableSSL = bool.Parse(item["EnableSSL"].ToString());
                }
                if (item["Port"] != DBNull.Value)
                {
                    Port = int.Parse(item["Port"].ToString());
                }
            }
        }
    }
    /// <summary>
    /// Không lưu lại email đã gửi và email không gửi được
    /// </summary>
    public class SendMail
    {
        public static bool Send(string templatefile, Hashtable Replace, string mailTo, string title, MailAddressCollection cc, MailAddressCollection bcc, string[] AttacheFile, bool SaveCannotSend, out string ErrorMessage, MailInfo MInfo)
        {
            if ((mailTo == null) || ("".Equals(mailTo.Trim())))
            {
                ErrorMessage = "Email người nhận không đúng";
                return true;
            }
            string contents = "";
            try
            {
                contents = File.ReadAllText(templatefile, System.Text.UTF8Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            foreach (DictionaryEntry Ent in Replace)
            {
                if (Ent.Value != null)
                    contents = contents.Replace(Ent.Key.ToString(), Ent.Value.ToString());
                else
                    contents = contents.Replace(Ent.Key.ToString(), "");
            }
            MailAddress email = new MailAddress(mailTo);
            MailAddressCollection To = new MailAddressCollection();
            To.Add(email);
            return Send(To, title, cc, bcc, contents, AttacheFile, SaveCannotSend, out ErrorMessage, MInfo);
        }
        public static bool Send(string TempateFile, Hashtable Replace, MailAddressCollection mailTo, string title, MailAddressCollection cc, MailAddressCollection bcc, string[] AttacheFile, bool SaveCannotSend, out string ErrorMessage, MailInfo MInfo)
        {
            string contents = "";
            try
            {
                contents = File.ReadAllText(TempateFile, System.Text.UTF8Encoding.UTF8);
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }
            foreach (DictionaryEntry Ent in Replace)
            {
                contents = contents.Replace(Ent.Key.ToString(), Ent.Value.ToString());
            }
            return Send(mailTo, title, cc, bcc, contents, AttacheFile, SaveCannotSend, out ErrorMessage, MInfo);
        }
        /// <summary>
        /// Tiến hành gửi mail
        /// </summary>
        /// <param name="mailTo"></param>
        /// <param name="title"></param>
        /// <param name="cc"></param>
        /// <param name="contents"></param>
        /// <param name="AttacheFiles"></param>
        /// <param name="SaveCannotSend"></param>
        /// <param name="ErrorMessage"></param>
        /// <param name="MInfo"></param>
        /// <returns></returns>
        public static bool Send(MailAddressCollection mailTo, string title, MailAddressCollection cc, MailAddressCollection bcc, string contents, string[] AttacheFiles, bool SaveCannotSend, out string ErrorMessage, MailInfo MInfo)
        {
            bool success = true;
            ErrorMessage = "";
            //Lưu lại email đã gửi
            Hashtable val = new Hashtable();
            if (MInfo == null || MInfo.Email == "")
            {
                ErrorMessage = "Không tìm thấy cấu hình mail";
                return false;
            }
            if (mailTo.Count == 0 && cc.Count == 0 && bcc.Count == 0)
            {
                ErrorMessage = "Không có người nhận mail";
                return false;
            }
            val.Add("Loai", MInfo.Loai);
            val.Add("Id", MInfo.Id);
            val.Add("Title", title);
            val.Add("Contents", contents);
            val.Add("SendDate", DateTime.Now);
            val.Add("SendFrom", MInfo.Email);
            MailMessage m = new MailMessage();
            string guiden = "", strcc = "", strbcc = "";
            for (int i = 0; i < mailTo.Count; i++)
            {
                m.To.Add(mailTo[i]);
                guiden += "," + mailTo[i];
            }
            m.From = new MailAddress(MInfo.Email);
            if (AttacheFiles != null)
            {
                foreach (var AttacheFile in AttacheFiles)
                {
                    if ((!"".Equals(AttacheFile)) && (File.Exists(AttacheFile)))
                    {
                        Attachment att = new Attachment(AttacheFile);
                        m.Attachments.Add(att);
                    }
                }
            }
            if (cc != null)
                for (int i = 0; i < cc.Count; i++)
                {
                    m.CC.Add((MailAddress)cc[i]);
                    strcc += "," + cc[i];
                }
            if (bcc != null)
                for (int i = 0; i < bcc.Count; i++)
                {
                    m.Bcc.Add((MailAddress)bcc[i]);
                    strbcc += "," + bcc[i];
                }
            m.IsBodyHtml = true;
            m.Subject = title;
            m.Body = contents;
            if (!"".Equals(guiden))
                guiden = guiden.Substring(1);
            val.Add("MailTo", guiden);
            if (!"".Equals(strcc))
                strcc = strcc.Substring(1);
            val.Add("CC", strcc);
            if (!"".Equals(strbcc))
                strbcc = strbcc.Substring(1);
            val.Add("BCC", strbcc);
            if ("".Equals(MInfo.Email))
            {
                ErrorMessage = "Không tìm thấy cấu hình mailserver";
                success = false;
            }
            if (!success)
            {
                val.Add("Success", success);
                val.Add("ErrorMsg", ErrorMessage);
                if (MInfo._config != null)
                {
                    DpsConnection cnn1 = new DpsConnection(MInfo._config.ConnectionString);
                    cnn1.Insert(val, "Sys_SendMail");
                }
                return false;
            }
            string str = "";
            Task.Factory.StartNew((Action)(() =>
            {
                SmtpClient s = new SmtpClient(MInfo.SmptClient, MInfo.Port);
                s.UseDefaultCredentials = false;
                s.EnableSsl = MInfo.EnableSSL;
                s.Credentials = new NetworkCredential(MInfo.UserName, MInfo.Password);
                s.DeliveryMethod = SmtpDeliveryMethod.Network;
                try
                {
                    s.Send(m);
                }
                catch (Exception ex)
                {
                    str = ex.Message;
                    success = false;
                }
                val.Add("Success", success);
                val.Add("ErrorMsg", str);
                if (MInfo._config != null)
                {
                    DpsConnection cnn1 = new DpsConnection(MInfo._config.ConnectionString);
                    cnn1.Insert(val, "Sys_SendMail");
                }
            }));
            ErrorMessage = str;
            return success;
        }

        public static bool Send(string mailTo, string title, MailAddressCollection cc, string contents, string[] AttacheFile, bool SaveCannotSend, out string ErrorMessage, MailInfo MInfo)
        {
            MailAddress email = new MailAddress(mailTo);
            MailAddressCollection to = new MailAddressCollection();
            to.Add(email);
            return Send(to, title, cc, new MailAddressCollection(), contents, AttacheFile, SaveCannotSend, out ErrorMessage, MInfo);
        }
    }
}
