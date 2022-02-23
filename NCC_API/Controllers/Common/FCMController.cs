using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using WebPush;

namespace Timensit_API.Controllers
{
    [ApiController]
    [Route("api/fcm")]
    [EnableCors("TimensitPolicy")]
    //firebase cloud message
    public class FCMController : ControllerBase
    {
        private NCCConfig _Config;
        private LoginController _account;

        public FCMController(IOptions<NCCConfig> config)
        {
            _Config = config.Value;
            _account = new LoginController();
        }

        [Route("CreateFCM")]
        [HttpGet]
        public object CreateFCM()
        {
            try
            {
                string Token = _account.GetHeader(Request);
                LoginData loginData = _account._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                using (DpsConnection cnn = new DpsConnection(_Config.ConnectionString))
                {
                    string sql = "select * from FCM where IdUser=@Id and Token=@token";
                    DataTable dt = cnn.CreateDataTable(sql, new SqlConditions { { "Id", loginData.Id }, { "token", GetLastCode_JWT(Token) } });
                    if (dt != null && dt.Rows.Count > 0)
                    {
                        return JsonResultCommon.ThanhCong(dt.Rows[0]["PublicKey"]);
                    }

                    VapidDetails vapidKeys = VapidHelper.GenerateVapidKeys();
                    Hashtable val = new Hashtable();
                    val.Add("IdUser", loginData.Id);
                    val.Add("Token", GetLastCode_JWT(Token));
                    val.Add("PublicKey", vapidKeys.PublicKey);
                    val.Add("PrivateKey", vapidKeys.PrivateKey);
                    if (cnn.Insert(val, "FCM") == 1)
                        return JsonResultCommon.ThanhCong(vapidKeys.PublicKey);
                    else
                        return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Route("UpdateFCM")]
        [HttpPost]
        public object UpdateFCM([FromBody] FMCModel data)
        {
            try
            {
                string Token = _account.GetHeader(Request);
                LoginData loginData = _account._GetInfoUser(Token);

                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                using (DpsConnection cnn = new DpsConnection(_Config.ConnectionString))
                {
                    string sql = "select * from FCM where IdUser=@Id and Token=@token";
                    DataTable dt = cnn.CreateDataTable(sql, new SqlConditions { { "Id", loginData.Id }, { "token", GetLastCode_JWT(Token) } });
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return JsonResultCommon.Custom("User device không tồn tại");
                    }

                    Hashtable val = new Hashtable();
                    val.Add("EndPoint", data.endpoint);
                    val.Add("P256dh", data.keys.p256dh);
                    val.Add("Auth", data.keys.auth);
                    if (cnn.Update(val, new SqlConditions { { "IdRow", dt.Rows[0]["IdRow"] } }, "FCM") == 1)
                        return JsonResultCommon.ThanhCong(dt.Rows[0]["PublicKey"]);
                    else
                        return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Route("DeleteFCM")]
        [HttpPost]
        public object DeleteFCM([FromBody] FMCModel data)
        {
            try
            {
                string Token = _account.GetHeader(Request);
                LoginData loginData = _account._GetInfoUser(Token);

                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                using (DpsConnection cnn = new DpsConnection(_Config.ConnectionString))
                {
                    int re = cnn.Delete(new SqlConditions { { "EndPoint", data.endpoint }, { "P256dh", data.keys.p256dh }, { "Auth", data.keys.auth } }, "FCM");
                    if (re <= 0)
                    {
                        return JsonResultCommon.Custom("User device không tồn tại");
                    }

                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Push thông báo bằng id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        [Route("pushMessage")]
        [HttpGet]
        public bool pushMessageById(string id, NofModel data)
        {
            if (string.IsNullOrEmpty(data.icon))
                data.icon = _Config.LinkBackend + "/assets/media/logos/favicon.ico";
            using (DpsConnection cnn = new DpsConnection(_Config.ConnectionString))
            {
                string sql = "select * from FCM where IdUser=@Id and EndPoint is not null";
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions { { "Id", id } });
                if (dt == null && dt.Rows.Count == 0)
                {
                    return false;
                }
                foreach (DataRow dr in dt.Rows)
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
                    Task.Run(() => push(dev, data));
                }
                return true;
            }
        }

        /// <summary>
        /// Push thông báo bằng token
        /// </summary>
        /// <param name="token"></param>
        /// <param name="data"></param>
        /// <returns></returns> 
        public static bool push(NofUserDevice device, NofModel data)
        {
            var pushEndpoint = device.EndPoint;
            var p256dh = device.P256dh;
            var auth = device.Auth;

            var subject = "mailto: example@example.com";
            var publicKey = device.PublicKey;
            var privateKey = device.PrivateKey;

            var subscription = new PushSubscription(pushEndpoint, p256dh, auth);
            var vapidDetails = new VapidDetails(subject, publicKey, privateKey);
            //var gcmAPIKey = token;

            var webPushClient = new WebPushClient();
            string text = JsonConvert.SerializeObject(data);
            try
            {
                webPushClient.SendNotificationAsync(subscription, text, vapidDetails);
                return true;
                //webPushClient.SendNotification(subscription, "payload", gcmAPIKey);
            }
            catch (WebPushException exception)
            {
                return false;
            }
        }

        private string GetLastCode_JWT(string Token)
        {
            return Token.Substring(Token.LastIndexOf('.') + 1);
        }

        [Route("ThongBao")]
        [HttpPost]
        public void ThongBao([FromBody]NofThongBaoModel model)
        {
            int Loai = model.Loai;
            long NguoiGui = model.NguoiGui;
            List<string> users = model.users;
            NofModel data = model.data;
            foreach (var user in users)
            {
                pushMessageById(user, data);
            }
            //NofThongBaoNhacNho tb = new NofThongBaoNhacNho();
            //tb.Loai = Loai;
            //tb.NgayGui = DateTime.Now;
            //tb.NguoiGui = NguoiGui;
            //tb.NoiDung = msg;
            //tb.LinkPhieu = link;
            //_context.NofThongBaoNhacNho.Add(tb);
            //_context.SaveChanges();
            //List<NofThongBaoNhacNhoCt> ct = new List<NofThongBaoNhacNhoCt>();
            //foreach (var u in users)
            //{
            //    ct.Add(new NofThongBaoNhacNhoCt()
            //    {
            //        ThongBaoNhacNhoId = tb.Id,
            //        NguoiNhan = u
            //    });
            //}
            //_context.NofThongBaoNhacNhoCt.AddRange(ct);
            //_context.SaveChanges();
        }
    }
    public class KeyModel
    {
        public string auth { get; set; }
        public string p256dh { get; set; }

    }
    public class FMCModel
    {
        public string token { get; set; }
        public string endpoint { get; set; }
        public KeyModel keys { get; set; }
    }
    public class NofModel
    {
        public NofModel() { }
        public NofModel(string _title)
        {
            title = _title;
            message = "";
        }
        public NofModel(string _title, string _message, string _link)
        {
            title = _title;
            message = _message;
            link = _link;
        }
        public NofModel(string _title, string _message, string _link, string _icon)
        {
            title = _title;
            message = _message;
            link = _link;
            icon = _icon;
        }
        public string message { get; set; }
        public string link { get; set; }
        public string title { get; set; }
        public string icon { get; set; }
    }

    public partial class NofUserDevice
    {
        public long IdRow { get; set; }
        public decimal IdUser { get; set; }
        public string Token { get; set; }
        public string PublicKey { get; set; }
        public string PrivateKey { get; set; }
        public string EndPoint { get; set; }
        public string Auth { get; set; }
        public string P256dh { get; set; }
        public DateTime CreatedDate { get; set; }
    }
    public class NofThongBaoModel
    {
        public int Loai { get; set; }
        public long NguoiGui { get; set; }
        public List<string> users { get; set; }
        public NofModel data { get; set; }
    }
}
