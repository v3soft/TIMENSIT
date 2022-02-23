using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timensit_API.Controllers.QuanLy
{
    [ApiController]
    [Route("api/smshistory")]
    [EnableCors("TimensitPolicy")]
    public class SMSHistoryController : Controller
    {
        LoginController lc;
        private NCCConfig _config;
        string Name = "Log";
        public SMSHistoryController(IOptions<NCCConfig> configLogin)
        {
            _config = configLogin.Value;
            lc = new LoginController();
        }

        [Authorize(Roles = "77")]
        [HttpPost]
        [Route("list")]
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
                    //string sqlq = @"select a.*, case when Status=0 then N'Thất bại' else N'Thành công' end as TrangThai,b.Loai, case when b.Loai=1 then N'Thông báo' else N'Loại' end as LoaiSMS,b.Message,b.Brandname,b.IdBrandname,b.Username,b.Password,b.CreatedDate from Sys_SMS_Detail a join Sys_SMS b on a.IdSMS=b.IdRow and b.Disabled=0";
                    string sqlq = @"select IdRow,Loai,Message,IdBrandname,Brandname,Username, Password,CreatedDate
                                    ,case when Loai=1 then N'Thông báo' else N'Loại' end as LoaiSMS
                                    ,convert(varchar,success)+'/'+convert(varchar,total) as ThanhCong,CreatedDate
                                     from Sys_SMS where Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                            { "SDT","SDT" },
                            { "NoiDungLoi","NoiDungLoi" },
                            { "TrangThai","TrangThai" },
                            { "Loai","Loai" },
                            { "Message","Message" },
                            { "Brandname","Brandname" },
                            { "Username","Username" },
                            { "Password","Password" },
                            { "CreatedDate","CreatedDate" },
                    };
                    temp = temp.OrderBy(x => x["CreatedDate"]);
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }

                    if (!string.IsNullOrEmpty(query.filter["SDT"]))
                    {
                        string keyword = query.filter["SDT"].ToLower();
                        temp = temp.Where(x => x["SDT"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["NoiDungLoi"]))
                    {
                        string keyword = query.filter["NoiDungLoi"].ToLower();
                        temp = temp.Where(x => x["NoiDungLoi"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Message"]))
                    {
                        string keyword = query.filter["Message"].ToLower();
                        temp = temp.Where(x => x["Message"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Brandname"]))
                    {
                        string keyword = query.filter["Brandname"].ToLower();
                        temp = temp.Where(x => x["Brandname"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Username"]))
                    {
                        string keyword = query.filter["Username"].ToLower();
                        temp = temp.Where(x => x["Username"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["tungay"]))
                    {
                        try
                        {
                            var keyword = new DateTime(int.Parse(query.filter["tungay"].ToString().Split('-')[0].ToString()), int.Parse(query.filter["tungay"].ToString().Split('-')[1].ToString()), int.Parse(query.filter["tungay"].ToString().Split('-')[2].ToString()));
                            //conds.Add("BatDau_tungay", keyword);
                            //where += " and BDNghi>= @BatDau_tungay";
                            temp = temp.Where(x => DateTime.Parse(x["CreatedDate"].ToString()) >= keyword);
                        }
                        catch
                        {
                            return JsonResultCommon.Custom("Thời gian từ ngày không đúng");
                        }
                    }
                    if (!string.IsNullOrEmpty(query.filter["denngay"]))
                    {
                        try
                        {
                            var keyword = new DateTime(int.Parse(query.filter["denngay"].ToString().Split('-')[0].ToString()), int.Parse(query.filter["denngay"].ToString().Split('-')[1].ToString()), int.Parse(query.filter["denngay"].ToString().Split('-')[2].ToString()));
                            //conds.Add("BatDau_denngay", keyword.AddDays(1));
                            //where += " and BDNghi < @BatDau_denngay";
                            temp = temp.Where(x => DateTime.Parse(x["CreatedDate"].ToString()) < keyword.AddDays(1));
                        }
                        catch
                        {
                            return JsonResultCommon.Custom("Thời gian đến ngày không đúng");
                        }
                    }
                    if (!string.IsNullOrEmpty(query.filter["Loai"]) && query.filter["Loai"].ToString() != "0")
                    {
                        string keyword = query.filter["Loai"].ToLower();
                        temp = temp.Where(x => x["Loai"].ToString() == keyword);
                    }
                    if (query.filterGroup["TrangThai"] != null && query.filterGroup["TrangThai"].Count() > 0)
                    {
                        string[] keyword = query.filterGroup["TrangThai"];
                        temp = temp.Where(x => keyword.Contains(x["Status"].ToString().ToLower()));
                    }
                    if (!string.IsNullOrEmpty(query.filter["IdDonVi"]))
                    {
                        var dt_dv = cnn.CreateDataTable("select * from Dps_User where DonVi=@Id", new SqlConditions { { "Id", query.filter["IdDonVi"] } });
                        List<long> LstLoai = new List<long>();
                        foreach (DataRow item in dt_dv.Rows)
                        {
                            long id = long.Parse(item["UserID"].ToString());
                            LstLoai.Add(id);
                        }
                        temp = temp.Where(x => LstLoai.Contains(long.Parse(x["UserID"].ToString())));
                    }
                    #endregion
                    int total = temp.Count();
                    if (total == 0)
                        return new BaseModel<object>
                        {
                            status = 1,
                            data = new List<string>(),
                            page = pageModel
                        };

                    pageModel.TotalCount = total;
                    if (query.more)
                    {
                        query.page = 1;
                        query.record = pageModel.TotalCount;
                    }
                    pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                    pageModel.Size = query.record;
                    pageModel.Page = query.page;
                    // Phân trang
                    var temp1 = temp.Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel());
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   IdSMS = r["IdRow"],
                                   //SDT = r["SDT"],
                                   //NoiDungLoi = r["NoiDungLoi"],
                                   //TrangThai = r["TrangThai"],
                                   Loai = r["Loai"],
                                   LoaiSMS = r["LoaiSMS"],
                                   Message = r["Message"],
                                   Brandname = r["Brandname"],
                                   IdBrandname = r["IdBrandname"],
                                   Username = r["Username"],
                                   Password = r["Password"],
                                   ThanhCong = r["ThanhCong"],
                                   CreatedDate = DateTime.Parse(r["CreatedDate"].ToString()).ToString("dd/MM/yyyy HH:mm")
                               };
                    //logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Route("detail")]
        [HttpGet]
        public BaseModel<object> detail(int id)
        {
            try
            {
                BaseModel<object> model = new BaseModel<object>();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select *,case when Status=0 then N'Thất bại' else N'Thành công' end as TrangThai from Sys_SMS_Detail where IdSMS=@Id";

                    var dt = cnn.CreateDataTable(sql,new SqlConditions { { "Id", id } });

                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id=r["IdRow"],
                                   IdSMS = r["IdSMS"],
                                   UserID = r["UserID"],
                                   Status = r["Status"],
                                   NoiDungLoi = r["NoiDungLoi"],
                                   SDT = r["SDT"],
                               };

                    
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch(Exception ex)
            { return JsonResultCommon.Exception(ex,ControllerContext); }
        }

        [Authorize(Roles = "77")]
        [Route("delete")]
        [HttpGet]
        public BaseModel<object> Delete(string id)
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
                    //string sqlq = "select * from DM_DanhMuc where Disabled = 0 and Id = @Id";
                    //if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", id) }) != 1)
                    //    return JsonResultCommon.KhongTonTai(Name);
                    string sqlq = "";

                    sqlq = "update Sys_SMS set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where IdRow = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        //logHelper.LogXoa(int.Parse(id), loginData.Id, Name);
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
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Authorize(Roles = "77")]
        [Route("Multi_Delete")]
        [HttpPost]
        public BaseModel<object> Multi_Delete([FromBody] decimal[] Id)
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
                if (Id.Count() == 0)
                {
                    return JsonResultCommon.KhongTonTai("Tin nhắn");
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    //string sqlq = $"select count(*) from DM_DonVi where Disabled = 0 and Id in ({String.Join(",", Id)})";
                    //if ((int)cnn.ExecuteScalar(sqlq) != 1)
                    //    return JsonResultCommon.KhongTonTai("Ý kiến");
                    string sqlq = "update Sys_SMS set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where IdRow in (" + String.Join(",", Id) + ")";
                    var result = cnn.ExecuteNonQuery(sqlq);
                    if (result > 0)
                    {
                        //foreach (var item in Id)
                        //{
                        //    logHelper.LogXoa(int.Parse(item.ToString()), loginData.Id, Name);
                        //}
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
                return JsonResultCommon.Exception(ex,ControllerContext);
            }

        }
    }
}
