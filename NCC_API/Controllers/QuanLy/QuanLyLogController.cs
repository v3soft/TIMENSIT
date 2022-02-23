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
using Microsoft.AspNetCore.Hosting;
using System.IO;

// For more information on enabling Web API for empty projects, visit https://go.microsoft.com/fwlink/?LinkID=397860

namespace Timensit_API.Controllers.QuanLy
{
    [ApiController]
    [Route("api/log")]
    [EnableCors("TimensitPolicy")]
    public class QuanLyLogController : ControllerBase
    {
        LoginController lc;
        private NCCConfig _config;
        IHostingEnvironment _hostingEnvironment;
        public QuanLyLogController(IOptions<NCCConfig> configLogin, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            _hostingEnvironment = hostingEnvironment;
        }

        [Authorize(Roles = "51")]
        [HttpGet]
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
                    string sqlq = @"select l.*,lh.HanhDong,ll.LoaiLog,du.Username, du.Fullname, convert(varchar, l.CreatedDate,103) as Cast_CreatedDate 
from Tbl_Log l inner join Tbl_Log_HanhDong lh on l.IdHanhDong=lh.IdRow 
inner join Tbl_Log_Loai ll on l.IdLoaiLog=ll.IdRow  
inner join Dps_User du on l.CreatedBy = du.UserID where Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                            { "HanhDong","HanhDong" },
                            { "IP","IP" },
                            { "NoiDung","NoiDung" },
                            { "LoaiLog","LoaiLog" },
                            { "Username","Username" },
                            { "Fullname","Fullname" },
                            { "CreatedDate","CreatedDate" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }

                    if (!string.IsNullOrEmpty(query.filter["Username"]))
                    {
                        string keyword = query.filter["Username"].ToLower();
                        temp = temp.Where(x => x["Username"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Fullname"]))
                    {
                        string keyword = query.filter["Fullname"].ToLower();
                        temp = temp.Where(x => x["Fullname"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["IP"]))
                    {
                        string keyword = query.filter["IP"].ToLower();
                        temp = temp.Where(x => x["IP"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["HanhDong"]))
                    {
                        string keyword = query.filter["HanhDong"].ToLower();
                        temp = temp.Where(x => x["HanhDong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["NoiDung"]))
                    {
                        string keyword = query.filter["NoiDung"].ToLower();
                        temp = temp.Where(x => x["NoiDung"].ToString().ToLower().Contains(keyword));
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
                    if (!string.IsNullOrEmpty(query.filter["LoaiLog"]) && query.filter["LoaiLog"].ToString() != "0")
                    {
                        string keyword = query.filter["LoaiLog"].ToLower();
                        temp = temp.Where(x => x["IdLoaiLog"].ToString() == keyword);
                        if (!string.IsNullOrEmpty(query.filter["IdDoiTuong"]))
                            temp = temp.Where(x => x["IdDoiTuong"].ToString() == query.filter["IdDoiTuong"]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["LoaiHanhDong"]) && query.filter["LoaiHanhDong"].ToString() != "0")
                    {
                        string keyword = query.filter["LoaiHanhDong"].ToLower();
                        temp = temp.Where(x => x["IdHanhDong"].ToString() == keyword);
                    }
                    //if (!string.IsNullOrEmpty(query.filter["MaHoSo"]))
                    //{
                    //    string keyword = query.filter["MaHoSo"].ToLower();
                    //    temp = temp.Where(x => x["MaHoSo"].ToString().ToLower().Contains(keyword));
                    //}

                    //if (!string.IsNullOrEmpty(query.filter["TenDonvi"]))
                    //{
                    //    string keyword = query.filter["TenDonvi"].ToLower();
                    //    temp = temp.Where(x => x["TenDonvi"].ToString().ToLower().Contains(keyword));
                    //}
                    //if (!string.IsNullOrEmpty(query.filter["MaDonVi"]))
                    //{
                    //    string keyword = query.filter["MaDonVi"].ToLower();
                    //    temp = temp.Where(x => x["MaDonVi"].ToString().ToLower().Contains(keyword));
                    //}
                    //if (!string.IsNullOrEmpty(query.filter["MaDinhDanh"]))
                    //{
                    //    string keyword = query.filter["MaDinhDanh"].ToLower();
                    //    temp = temp.Where(x => x["MaDinhDanh"].ToString().ToLower().Contains(keyword));
                    //}
                    //if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    //{
                    //    var groups = query.filterGroup["Locked"].ToList();
                    //    temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    //}
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
                    var data = from r in temp1
                               select new
                               {
                                   IdRow = r["IdRow"],
                                   HanhDong = r["HanhDong"],
                                   IP = r["IP"],
                                   NoiDung = r["NoiDung"],
                                   LoaiLog = r["LoaiLog"],
                                   Username = r["Username"],
                                   Fullname = r["Fullname"],
                                   CreatedDate = DateTime.Parse(r["CreatedDate"].ToString()).ToString("dd/MM/yyyy HH:mm")
                               };
                    //logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Xóa log
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "51")]
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

                    sqlq = "update Tbl_Log set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where IdRow = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
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
        /// Xóa logs
        /// </summary>
        /// <param name="ids"></param>
        /// <returns></returns>
        [Authorize(Roles = "51")]
        [Route("deletes")]
        [HttpPost]
        public BaseModel<object> Deletes(List<string> ids)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                if (ids.Count == 0)
                    return JsonResultCommon.ThanhCong();
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string strJ = string.Join(",", ids);
                    string sqlq = "update Tbl_Log set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where IdRow in (" + strJ + ")";
                    if (cnn.ExecuteNonQuery(sqlq) <= 0)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize(Roles = "51")]
        [HttpGet]
        public object ListFileLog([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                PageModel pageModel = new PageModel();
                string folder = "theochucnang";
                if (!string.IsNullOrEmpty(query.filter["folder"]))
                    folder = query.filter["folder"];
                string domain = _config.LinkAPI;
                string folderName = Constant.RootUpload + "/Logs/" + folder + "/";
                string Base_Path = Path.Combine(_hostingEnvironment.ContentRootPath, folderName);
                var dr = new DirectoryInfo(Base_Path);
                FileInfo[] files = dr.GetFiles();
                string keyword = "";
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    keyword = query.filter["keyword"];
                Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "Name","Name" }
                    };
                var data = from f in files
                           where string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && f.Name.Contains(keyword))
                           select new
                           {
                               Name = f.Name,
                               path = domain + folderName + f.Name
                           };
                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    if ("asc".Equals(query.sortOrder))
                        data = data.OrderBy(x => LiteHelper.GetValueByName(x, sortableFields[query.sortField]));
                    else
                        data = data.OrderByDescending(x => LiteHelper.GetValueByName(x, sortableFields[query.sortField]));
                }
                int total = data.Count();
                if (total == 0)
                    return JsonResultCommon.ThanhCong(new List<string>(), pageModel, false);
                pageModel.TotalCount = total;
                pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                pageModel.Size = query.record;
                pageModel.Page = query.page;
                if (query.more)
                {
                    query.page = 1;
                    query.record = pageModel.TotalCount;
                }
                // Phân trang
                data = data.Skip((query.page - 1) * query.record).Take(query.record).ToList();
                return JsonResultCommon.ThanhCong(data, pageModel, false);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex);
            }
        }
    }
}
