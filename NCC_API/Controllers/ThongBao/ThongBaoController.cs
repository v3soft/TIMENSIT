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
using Org.BouncyCastle.Bcpg;
using System.Globalization;
using Microsoft.AspNetCore.Hosting;
using Timensit_API.Classes;
using System.Net.Mail;
using System.Threading.Tasks;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Timensit_API.Controllers.ThongBao
{
    /// <summary>
    /// Quản lý thông báo<para/>
    /// </summary>
    [ApiController]
    [Route("api/thong-bao")]
    [EnableCors("TimensitPolicy")]
    public class ThongBaoController : ControllerBase
    {
        LoginController lc;
        IOptions<NCCConfig> _configLogin;
        private NCCConfig _config;
        string Name = "Thông báo";
        private readonly IHostingEnvironment _hostingEnvironment;
        string rootImg = "";
        private IHubContext<ThongBaoHub> _hub_context;
        public ThongBaoController(IOptions<NCCConfig> configLogin, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hubcontext)
        {
            _configLogin = configLogin;
            _config = configLogin.Value;
            lc = new LoginController();
            _hostingEnvironment = hostingEnvironment;
            rootImg = _config.LinkAPI + Constant.RootUpload;
            _hub_context = hubcontext;
        }

        [Authorize]
        [HttpGet]
        [Route("get-thong-bao-dashboard")]
        public BaseModel<object> getThongBaoDashboard([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                string strW = "";
                var cond = new SqlConditions() { { "UserID", loginData.Id } };
                var TuNgay = DateTime.MinValue;
                var DenNgay = DateTime.MinValue;
                if (!string.IsNullOrEmpty(query.filter["TuNgay"]))
                {
                    if (!DateTime.TryParseExact(query.filter["TuNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out TuNgay))
                        return JsonResultCommon.Custom("Thời gian không hợp lệ");
                    strW += " and nt.CreatedDate>@TuNgay";
                    cond.Add("TuNgay", TuNgay);
                }
                if (!string.IsNullOrEmpty(query.filter["DenNgay"]))
                {
                    if (!DateTime.TryParseExact(query.filter["DenNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out DenNgay))
                        return JsonResultCommon.Custom("Thời gian không hợp lệ");
                    DenNgay = DenNgay.AddDays(1);
                    strW += " and nt.CreatedDate<@DenNgay";
                    cond.Add("DenNgay", DenNgay);
                }
                if (TuNgay != DateTime.MinValue && DenNgay != DateTime.MinValue && TuNgay > DenNgay)
                    return JsonResultCommon.Custom("Khoảng thời gian không hợp lệ");
                if (!string.IsNullOrEmpty(query.filter["Loai"]))
                    strW += " and nt.Loai=" + query.filter["Loai"];
                bool includeRead = false;
                if (!string.IsNullOrEmpty(query.filter["includeRead"]) && query.filter["includeRead"] == "1")
                { }
                else
                    strW += " and nt.IsRead=0";
                long iduser = loginData.Id;
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "NoiDung","NoiDung" },
                        { "CreatedDate","CreatedDate" },
                    };
                    string sqlq = @"select nt.*, g.FullName as NguoiGui, g.Avata from Tbl_Notify nt
                                  inner join Dps_User g on nt.CreatedBy = g.UserID
                                  where nt.UserID = @UserID and Disabled = 0" + strW;
                    var _dt = cnn.CreateDataTable(sqlq, cond);
                    int total = _dt.Rows.Count;
                    if (total == 0)
                        return new BaseModel<object>
                        {
                            status = 1,
                            data = new List<string>(),
                            page = pageModel
                        };
                    if (query.more)
                    {
                        query.page = 1;
                        query.record = total;
                    }
                    pageModel.TotalCount = total;
                    pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                    pageModel.Size = query.record;
                    pageModel.Page = query.page;
                    // Phân trang
                    var temp1 = _dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp1 = temp1.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp1 = temp1.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["lastID"]))
                    {
                        temp1 = temp1.Where(x => (long)x["IdRow"] > Convert.ToInt64(query.filter["lastID"])).OrderByDescending(x => (DateTime)x["CreatedDate"]);
                        if (temp1.Count() == 0)
                            return JsonResultCommon.ThanhCong(new List<string>(), new PageModel());
                    }
                    var data = (from rw in temp1
                                select new
                                {
                                    IdRow = rw["IdRow"],
                                    UserID = rw["UserID"],
                                    NoiDung = rw["NoiDung"],
                                    Link = rw["Link"],
                                    _object = getObject(rw),
                                    Loai = rw["Loai"],
                                    CreatedBy = rw["CreatedBy"],
                                    CreatedDate = rw["CreatedDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", rw["CreatedDate"]),
                                    IsRead = (bool)rw["IsRead"],
                                    UpdatedDate = rw["UpdatedDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", rw["UpdatedDate"]),
                                    Avata = rw["Avata"] != DBNull.Value ? (_config.LinkAPI + Constant.RootUpload + rw["Avata"]) : "",
                                    NguoiGui = rw["NguoiGui"]
                                }).ToList();
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        /// <summary>
        /// Xóa thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [Route("delete")]
        [HttpGet]
        public async Task<BaseModel<object>> Delete(string id)
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
                    DataTable dtF = cnn.CreateDataTable(sqlF);
                    if (dtF.Rows.Count > 0)
                    {
                        var data = from r in dtF.AsEnumerable()
                                   select new ThongBaoModel()
                                   {
                                       IdRow = (long)r["IdRow"],
                                       ThongBao = r["NoiDung"].ToString(),
                                       Link = r["Link"].ToString(),
                                       Loai = (int)r["Loai"],
                                       CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                       UpdatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now),
                                       IsRead = (bool)r["IsRead"],
                                       IsNew = false,
                                       Disabled = true
                                   };

                        string sqlq = "update Tbl_Notify set Disabled = 1, UpdatedDate = getdate() where IdRow = " + id;
                        if (cnn.ExecuteNonQuery(sqlq) < 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        await _hub_context.Clients.All.SendAsync("recieveMessaged", data);
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
        /// Xóa nhiều thông báo
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize]
        [Route("deletes")]
        [HttpPost]
        public async Task<BaseModel<object>> Deletes(List<string> ids)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                if (ids == null || ids.Count == 0)
                    return JsonResultCommon.ThanhCong();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string str = string.Join(",", ids);
                    string sqlF = "select * from tbl_notify where disabled=0 and IdRow in (" + str + ")";
                    DataTable dtF = cnn.CreateDataTable(sqlF);
                    if (dtF.Rows.Count > 0)
                    {
                        var data = from r in dtF.AsEnumerable()
                                   select new ThongBaoModel()
                                   {
                                       IdRow = (long)r["IdRow"],
                                       ThongBao = r["NoiDung"].ToString(),
                                       Link = r["Link"].ToString(),
                                       Loai = (int)r["Loai"],
                                       CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                       UpdatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now),
                                       IsRead = (bool)r["IsRead"],
                                       IsNew = false,
                                       Disabled = true
                                   };

                        string sqlq = "update Tbl_Notify set Disabled = 1, UpdatedDate = getdate() where disabled=0 and IdRow in (" + str + ")";
                        if (cnn.ExecuteNonQuery(sqlq) < 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        await _hub_context.Clients.All.SendAsync("recieveMessaged", data);
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
        /// ĐÁnh dấu thông báo là đã đọc
        /// </summary>
        /// <param name="isDelete"></param>
        /// <returns></returns>
        [Authorize]
        [Route("markAsRead")]
        [HttpGet]
        public async Task<BaseModel<object>> markAsRead(bool isDelete = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlF = "select * from tbl_notify where Disabled = 0 and IsRead=" + (isDelete ? "1" : "0") + " and UserID =" + loginData.Id;
                    DataTable dtF = cnn.CreateDataTable(sqlF);
                    if (dtF.Rows.Count > 0)
                    {
                        var data = from r in dtF.AsEnumerable()
                                   select new ThongBaoModel()
                                   {
                                       IdRow = (long)r["IdRow"],
                                       ThongBao = r["NoiDung"].ToString(),
                                       Link = r["Link"].ToString(),
                                       Loai = (int)r["Loai"],
                                       CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                                       UpdatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", DateTime.Now),
                                       IsRead = true,
                                       IsNew = false,
                                       Disabled = isDelete
                                   };

                        string sqlq = "update Tbl_Notify set Disabled = 1, UpdatedDate = getdate() where Disabled = 0 and IsRead=1 and UserID =" + loginData.Id;
                        if (!isDelete)
                            sqlq = "update Tbl_Notify set IsRead = 1, UpdatedDate = getdate() where disabled=0 and IsRead = 0 and UserID =" + loginData.Id;
                        if (cnn.ExecuteNonQuery(sqlq) < 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        await _hub_context.Clients.All.SendAsync("recieveMessaged", data);
                    }
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        private LiteModel getObject(DataRow rw)
        {
            string Loai = rw["Loai"].ToString();
            if (Loai == "1" || Loai == "2" || Loai == "3")
            {
                string link = rw["Link"].ToString();
                var arr = link.Split("/");
                if (arr.Length > 0)
                {
                    string str = arr[arr.Length - 1];
                    long id = 0;
                    bool kq = long.TryParse(str, out id);
                    if (kq)
                        return new LiteModel()
                        {
                            id = id
                        };
                }
            }
            return null;
        }
    }
}