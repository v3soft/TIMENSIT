using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;
using DpsLibs.Data;
using Timensit_API.Classes;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using OfficeOpenXml;
using WebCore_API.Models;

namespace WebCore_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/dm_donvi")]
    [EnableCors("TimensitPolicy")]
    public class DM_DonViController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        //private readonly DPS_NFCContext _context;
        //private APIHelperClass _apiHelper;
        private NCCConfig _config;
        private LoginController lc;
        LogHelper logHelper;
        string Name = "Đơn vị";
        string rootImg = "";
        public DM_DonViController(IOptions<NCCConfig> config, IHostingEnvironment hostingEnvironment, IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config.Value;
            rootImg = _config.LinkAPI + Constant.RootUpload;
            lc = new LoginController();
            logHelper = new LogHelper(config.Value, accessor, hostingEnvironment, 3);
        }

        #region Danh sách Đơn vị
        /// <summary>
        /// Lấy danh sách Đơn vị
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        [Authorize(Roles = "5")]
        [Route("DM_DonVi_List")]
        [HttpGet]
        public BaseModel<object> DM_DonVi_List([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                bool Visible = User.IsInRole("7");
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select dv1.[Id]
                                          ,dv1.[LoaiDonVi]
                                          ,dv1.[DonVi]
                                          ,dv1.[MaDonvi]
                                          ,dv1.[MaDinhDanh]
                                          ,dv1.[Parent]
                                          ,dv1.[SDT]
                                          ,dv1.[Email]
                                          ,dv1.[DiaChi]
                                          ,dv1.[Logo]
                                          ,dv1.[Locked]
                                          ,dv1.[Priority]
                                          ,dv1.[DangKyLichLanhDao]
                                          ,dv1.[KhongCoVanThu]
                                          ,dv1.[CreatedBy]
                                          ,dv1.[CreatedDate]
                                          ,dv1.[UpdatedBy]
                                          ,dv1.[UpdatedDate]
                                          ,dv1.[Disabled]
										  ,dv2.DonVi as ParentName
										   from DM_DonVi dv1
											left join DM_DonVi dv2 on dv1.Parent = dv2.Id
										  where dv1.[Disabled]=0 and (dv1.Parent = @IdDV or dv1.Id=@IdDV)";
                    SqlConditions cond = new SqlConditions();
                    // sqlq += " and Parent = @IdDV";
                    if (!string.IsNullOrEmpty(query.filter["IdDV"]))
                    {
                        cond.Add("IdDV", long.Parse(query.filter["IdDV"]));
                    }
                    else
                    {
                        cond.Add("IdDV", long.Parse(_config.IdTinh));
                    }
                    var dt = cnn.CreateDataTable(sqlq, cond);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                         { "DonVi","DonVi" },
                         { "MaDinhDanh","MaDinhDanh" },
                         { "MaDonvi","MaDonvi" },
                         { "Locked","Locked" },
                         { "LoaiDonVi","LoaiDonVi" },
                         { "SDT","SDT" },
                         { "Email","Email" },
                         { "DiaChi","DiaChi" },
                         { "Priority","Priority" },
                         { "CreatedDate","CreatedDate" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]].ToString());
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]].ToString());
                    }
                    else
                    {
                        temp = temp.OrderBy(x => x["Priority"].ToString());
                    }
                    if (!string.IsNullOrEmpty(query.filter["LoaiDonVi"]))
                    {
                        long keyword = long.Parse(query.filter["LoaiDonVi"]);
                        temp = temp.Where(x => x["LoaiDonVi"].Equals(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["DonVi"]))
                    {
                        string keyword = query.filter["DonVi"].ToLower();
                        temp = temp.Where(x => x["DonVi"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaDinhDanh"]))
                    {
                        string keyword = query.filter["MaDinhDanh"].ToLower();
                        temp = temp.Where(x => x["MaDinhDanh"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaDonvi"]))
                    {
                        string keyword = query.filter["MaDonvi"].ToLower();
                        temp = temp.Where(x => x["MaDonvi"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["SDT"]))
                    {
                        string keyword = query.filter["SDT"].ToLower();
                        temp = temp.Where(x => x["SDT"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Email"]))
                    {
                        string keyword = query.filter["Email"].ToLower();
                        temp = temp.Where(x => x["Email"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["DiaChi"]))
                    {
                        string keyword = query.filter["DiaChi"].ToLower();
                        temp = temp.Where(x => x["DiaChi"].ToString().ToLower().Contains(keyword));
                    }
                    //if (!string.IsNullOrEmpty(query.filter["Locked"]))
                    //{
                    //    int keyword = int.Parse(query.filter["Locked"].ToString());
                    //    if (keyword >= 0)
                    //    {
                    //        temp = temp.Where(x => x["Locked"].Equals(keyword == 1 ? true : false));
                    //    }
                    //}
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains((Convert.ToInt32(x["Locked"])).ToString()));
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
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["Id"],
                                   DonVi = r["DonVi"],
                                   MaDonvi = r["MaDonvi"],
                                   MaDinhDanh = r["MaDinhDanh"],
                                   Parent = r["Parent"],
                                   SDT = r["SDT"],
                                   Email = r["Email"],
                                   DiaChi = r["DiaChi"],
                                   Logo = r["Logo"],
                                   DangKyLichLanhDao = r["DangKyLichLanhDao"],
                                   KhongCoVanThu = r["KhongCoVanThu"],
                                   LoaiDonVi = r["LoaiDonVi"],
                                   Priority = r["Priority"],
                                   Locked = r["Locked"],
                                   CreatedDate = String.Format("{0:dd\\/MM\\/yyyy HH:mm}", r["CreatedDate"]),
                                   ParentName = r["ParentName"]

                               };
                    logHelper.LogXemDS(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }
        #endregion

        #region Thêm mới Đơn vị
        /// <summary>
        /// Thêm mới Đơn vị
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_Insert")]
        [HttpPost]
        public BaseModel<object> DM_DonVi_Insert([FromBody] DM_DonVi_Model data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string strRe = "";
                if (string.IsNullOrEmpty(data.DonVi))
                    strRe += (strRe == "" ? "" : ", ") + "Tên đơn vị";
                if (string.IsNullOrEmpty(data.MaDonvi))
                    strRe += (strRe == "" ? "" : ", ") + "Mã đơn vị";

                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from DM_DonVi where Disabled=0 and MaDonvi = @MaDonvi";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("MaDonvi", data.MaDonvi) }) > 0)
                        return JsonResultCommon.Trung("Mã đơn vị");
                    else
                    {
                        Hashtable val = new Hashtable();
                        val.Add("DonVi", data.DonVi);
                        val.Add("MaDonvi", data.MaDonvi);
                        if (data.LoaiDonVi >= 0)
                            val.Add("LoaiDonVi", data.LoaiDonVi);
                        val.Add("Locked", data.Locked);
                        val.Add("DangKyLichLanhDao", data.DangKyLichLanhDao);
                        val.Add("KhongCoVanThu", data.KhongCoVanThu);
                        if (data.Priority >= 0)
                            val.Add("Priority", data.Priority);
                        val.Add("MaDinhDanh", string.IsNullOrEmpty(data.MaDinhDanh) ? "" : data.MaDinhDanh);
                        val.Add("SDT", string.IsNullOrEmpty(data.SDT) ? "" : data.SDT);
                        val.Add("Email", string.IsNullOrEmpty(data.Email) ? "" : data.Email);
                        val.Add("DiaChi", string.IsNullOrEmpty(data.DiaChi) ? "" : data.DiaChi);
                        if (data.Parent > 0)
                            val.Add("Parent", data.Parent);
                        val.Add("CreatedBy", iduser);
                        string linkImagePresent = "";
                        foreach (var item in data.listLinkImage)
                        {
                            if (item.IsAdd == true)
                            {
                                string x = "";
                                if (!UploadHelper.UploadImage(item.strBase64, item.filename, "/images/DonVi/", _hostingEnvironment.ContentRootPath, ref x, true))
                                {
                                    return JsonResultCommon.Custom(UploadHelper.error);
                                }
                                if (item.IsImagePresent)
                                {
                                    linkImagePresent = x;
                                }
                            }
                            else if (item.IsAdd == false && item.IsDel == false)
                            {
                                string _link = "";
                                if (item.src.Contains(rootImg))
                                {
                                    int _lastindex = rootImg.Length;
                                    _link = item.src.Substring(_lastindex);
                                }
                                else
                                {
                                    _link = item.src;
                                }

                                if (item.IsImagePresent)
                                {
                                    linkImagePresent = _link;
                                }
                            }
                        }
                        val.Add("Logo", linkImagePresent);
                        if (cnn.Insert(val, "DM_DonVi") == 1)
                        {
                            int id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('DM_DonVi') AS Current_Identity; ").ToString());
                            logHelper.Ghilogfile<DM_DonVi_Model>(data, loginData, "Thêm mới", logHelper.LogThem(id, loginData.Id, Name), Name);
                            return JsonResultCommon.ThanhCong(data);
                        }
                        else
                        {
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region Cập nhật Đơn vị

        /// <summary>
        /// Cập nhật Đơn vị
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_Update")]
        [HttpPost]
        public BaseModel<object> DM_DonVi_Update([FromBody] DM_DonVi_Model data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string strRe = "";
                if (string.IsNullOrEmpty(data.DonVi))
                    strRe += (strRe == "" ? "" : ", ") + "Tên đơn vị";
                if (string.IsNullOrEmpty(data.MaDonvi))
                    strRe += (strRe == "" ? "" : ", ") + "Mã đơn vị";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from DM_DonVi where Disabled=0 and Id = @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", data.Id) }) != 1)
                        return JsonResultCommon.KhongTonTai("Đơn vị");
                    sqlq = "select count(*) from DM_DonVi where Disabled=0 and Id <> @Id and  MaDonvi = @MaDonvi";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Id", data.Id), new SqlCondition("MaDonvi", data.MaDonvi) }) > 0)
                        return JsonResultCommon.Trung("Mã đơn vị");
                    SqlConditions val = new SqlConditions();
                    //if (data.Id > 0)
                    //    val.Add("Id", data.Id);
                    string setParent = " Parent=@Parent ";
                    val.Add("DonVi", data.DonVi);
                    val.Add("Id", data.Id);
                    val.Add("MaDonvi", data.MaDonvi);
                    if (data.LoaiDonVi > 0)
                        val.Add("LoaiDonVi", data.LoaiDonVi);
                    val.Add("Locked", data.Locked);
                    val.Add("DangKyLichLanhDao", data.DangKyLichLanhDao);
                    val.Add("KhongCoVanThu", data.KhongCoVanThu);
                    if (data.Priority >= 0)
                        val.Add("Priority", data.Priority);
                    val.Add("MaDinhDanh", string.IsNullOrEmpty(data.MaDinhDanh) ? "" : data.MaDinhDanh);
                    val.Add("SDT", string.IsNullOrEmpty(data.SDT) ? "" : data.SDT);
                    val.Add("Email", string.IsNullOrEmpty(data.Email) ? "" : data.Email);
                    val.Add("DiaChi", string.IsNullOrEmpty(data.DiaChi) ? "" : data.DiaChi);
                    if (data.Parent > 0)
                    {
                        val.Add("Parent", data.Parent);
                    }
                    else
                    {
                        setParent = " Parent = null ";
                    }
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);
                    string linkImagePresent = "";
                    var sql_update = $@"update DM_DonVi  set LoaiDonVi = @LoaiDonVi,DonVi=@DonVi,MaDonvi=@MaDonvi,MaDinhDanh=@MaDinhDanh, {setParent}, SDT=@SDT, Email=@Email, DiaChi=@DiaChi, Logo=@Logo, Locked=@Locked, [Priority]=@Priority,
                                    @DangKyLichLanhDao = @DangKyLichLanhDao,KhongCoVanThu = @KhongCoVanThu,UpdatedBy = @UpdatedBy, UpdatedDate = getdate() where Id = @Id";
                    foreach (var item in data.listLinkImage)
                    {
                        if (item.IsAdd == true)
                        {
                            string x = "";
                            if (!UploadHelper.UploadImage(item.strBase64, item.filename, "/images/DonVi/", _hostingEnvironment.ContentRootPath, ref x, true))
                            {
                                return JsonResultCommon.Custom(UploadHelper.error);
                            }
                            if (item.IsImagePresent)
                            {
                                linkImagePresent = x;
                            }
                        }
                        else if (item.IsAdd == false && item.IsDel == false)
                        {
                            string _link = "";
                            if (item.src.Contains(rootImg))
                            {
                                int _lastindex = rootImg.Length;
                                _link = item.src.Substring(_lastindex);
                            }
                            else
                            {
                                _link = item.src;
                            }

                            if (item.IsImagePresent)
                            {
                                linkImagePresent = _link;
                            }
                        }
                    }
                    val.Add("Logo", linkImagePresent);
                    cnn.ExecuteNonQuery(sql_update, val);
                    if (cnn.LastError == null)
                    {
                        logHelper.Ghilogfile<DM_DonVi_Model>(data, loginData, "Cập nhật", logHelper.LogSua((int)data.Id, loginData.Id, Name), Name);
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
        #endregion


        #region Xóa 1 Đơn vị

        /// <summary>
        /// Xóa 1 Đơn vị
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_Delete")]
        [HttpPost]
        public BaseModel<object> DM_DonVi_Delete(long Id)
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
                    string sqlq = "select * from DM_DonVi where Disabled = 0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", Id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Đơn vị");
                    sqlq = "";

                    sqlq = "update DM_DonVi set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + Id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) < 0)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var data = new LiteModel() { id = Id, title = dtFind.Rows[0]["DonVi"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(int.Parse(Id.ToString()), loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }
        #endregion

        #region Xóa nhiều Đơn vị
        /// <summary>
        /// Xóa nhiều Đơn vị
        /// </summary>
        /// <param name="Id"></param>   
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_Multi_Delete")]
        [HttpPost]
        public BaseModel<object> DM_DonVi_Multi_Delete([FromBody] long[] Id)
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
                    return JsonResultCommon.KhongTonTai("Đơn vị");
                }
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = $"select * from DM_DonVi where Disabled = 0 and Id in ({String.Join(",", Id)})";
                    DataTable dtFind = cnn.CreateDataTable(sqlq);
                    //if ((int)cnn.ExecuteScalar(sqlq) != 1)
                    //    return JsonResultCommon.KhongTonTai("Ý kiến");
                    sqlq = "update DM_DonVi set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id in (" + String.Join(",", Id) + ")";
                    var result = cnn.ExecuteNonQuery(sqlq);
                    if (result > 0)
                    {
                        var data = (from r in dtFind.AsEnumerable()
                                    select new LiteModel() { id = (long)r["id"], title = r["DonVi"].ToString() }).ToList();
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa nhiều", logHelper.LogXoas(Id.ToList(), loginData.Id, Name), Name);
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
        #region Lấy 1 Đơn vị
        [Route("DM_DonVi_Detail")]
        [HttpGet]
        public BaseModel<object> DM_DonVi_Detail(decimal Id)
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
                    string sqlq = @"select dv1.[Id]
                                          ,dv1.[LoaiDonVi]
                                          ,dv1.[DonVi]
                                          ,dv1.[MaDonvi]
                                          ,dv1.[MaDinhDanh]
                                          ,dv1.[Parent]
                                          ,dv1.[SDT]
                                          ,dv1.[Email]
                                          ,dv1.[DiaChi]
                                          ,dv1.[Logo]
                                          ,dv1.[Locked]
                                          ,dv1.[Priority]
                                          ,dv1.[DangKyLichLanhDao]
                                          ,dv1.[KhongCoVanThu]
                                          ,dv1.[CreatedBy]
                                          ,dv1.[CreatedDate]
                                          ,dv1.[UpdatedBy]
                                          ,dv1.[UpdatedDate]
                                          ,dv1.[Disabled]
										  ,dv2.DonVi as ParentName
										   from DM_DonVi dv1
											left join DM_DonVi dv2 on dv1.Parent = dv2.Id
										  where dv1.[Disabled]=0 and dv1.Id = @Id";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", Id } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    logHelper.LogXemCT(int.Parse(Id.ToString()), loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {
                                                           Id = r["Id"],
                                                           DonVi = r["DonVi"],
                                                           MaDonvi = r["MaDonvi"],
                                                           MaDinhDanh = r["MaDinhDanh"],
                                                           Parent = r["Parent"],
                                                           SDT = r["SDT"],
                                                           Email = r["Email"],
                                                           DiaChi = r["DiaChi"],
                                                           Logo = r["Logo"] != DBNull.Value && !string.IsNullOrEmpty(r["Logo"].ToString()) ? (rootImg + r["Logo"]) : "",
                                                           DangKyLichLanhDao = r["DangKyLichLanhDao"],
                                                           KhongCoVanThu = r["KhongCoVanThu"],
                                                           LoaiDonVi = r["LoaiDonVi"],
                                                           Priority = r["Priority"],
                                                           Locked = r["Locked"],
                                                           CreatedDate = String.Format("{0:dd\\/MM\\/yyyy HH:mm}", r["CreatedDate"]),
                                                           ParentName = r["ParentName"],
                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion
        #region Danh sách người dùng đơn vị
        /// <summary>
        /// Lấy danh sách Đơn vị
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        [Route("DM_User_DonVi")]
        [HttpGet]
        public BaseModel<object> DM_User_DonVi([FromQuery] QueryParams query)
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
                    string sqlq = @"SELECT  us.[UserID]
                                              ,[Username]
                                              ,[FullName]                                             
                                              ,[MaNV]
                                              ,us.[IdDonVi]
                                              ,[PhoneNumber]
                                              ,[IdChucVu]
                                              ,[Avata]
                                              ,[Sign]
                                              ,us.[Email]
                                              ,[ViettelStudy]
                                              ,[SimCA]
                                              ,[LoaiChungThu]
                                              ,[SerialToken]
                                              ,[CMTND]
                                              ,[GioiTinh]
                                              ,[NhanLichDonVi]
                                              ,[NgaySinh]
                                              ,[Active]
                                              ,[Status]
                                              ,[LastPing]
                                              ,[LastUpdatePass]
                                              ,[LastLogin]
                                              ,[UserCreate]     
                                              ,[Deleted]                                           
                                              ,cv.[ChucVu]
											  ,donvi.DonVi as TenDonVi
                                          FROM [Dps_User] us 
										  inner join DM_DonVi donvi on us.IdDonVi = donvi.Id
                                          left join DM_ChucVu cv on cv.MaChucVu = us.IdChucVu
										  left join Dps_User_DonVi dv on dv.UserID=us.UserID
                                          where (us.IdDonVi=@IdDV or dv.IdDonVi=@IdDV) and us.Deleted=0 ";
                    SqlConditions cond = new SqlConditions();
                    // sqlq += " and Parent = @IdDV";
                    if (!string.IsNullOrEmpty(query.filter["IdDV"]))
                    {
                        cond.Add("IdDV", long.Parse(query.filter["IdDV"]));
                    }
                    else
                    {
                        cond.Add("IdDV", long.Parse(_config.IdTinh));
                    }
                    var dt = cnn.CreateDataTable(sqlq, cond);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                         { "Username","Username" },
                         { "FullName","FullName" },
                         { "ChucVu","ChucVu" },
                         { "PhoneNumber","PhoneNumber" },
                         { "Email","Email" },
                         { "Active","Active" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]] ?? "");
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]] ?? "");
                    }


                    if (!string.IsNullOrEmpty(query.filter["Username"]))
                    {
                        string keyword = query.filter["Username"].ToLower();
                        temp = temp.Where(x => x["Username"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["FullName"]))
                    {
                        string keyword = query.filter["FullName"].ToLower();
                        temp = temp.Where(x => x["FullName"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["ChucVu"]))
                    {
                        string keyword = query.filter["ChucVu"].ToLower();
                        temp = temp.Where(x => x["ChucVu"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["PhoneNumber"]))
                    {
                        string keyword = query.filter["PhoneNumber"].ToLower();
                        temp = temp.Where(x => x["PhoneNumber"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["Email"]))
                    {
                        string keyword = query.filter["Email"].ToLower();
                        temp = temp.Where(x => x["Email"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Active"] != null)
                    {
                        var groups = query.filterGroup["Active"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Active"].ToString()));
                    }
                    #endregion
                    bool Visible = true;
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
                        Visible = Visible;
                    }
                    pageModel.AllPage = (int)Math.Ceiling(total / (decimal)query.record);
                    pageModel.Size = query.record;
                    pageModel.Page = query.page;
                    // Phân trang
                    var temp1 = temp.Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel(), Visible);
                    dt = temp1.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   UserID = r["UserID"],
                                   Username = r["Username"],
                                   FullName = r["FullName"],
                                   ChucVu = r["ChucVu"],
                                   PhoneNumber = r["PhoneNumber"],
                                   Email = r["Email"],
                                   Active = r["Active"],
                                   TenDonVi = r["TenDonVi"],
                                   IdDV = r["IdDonVi"]
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }
        #endregion
        #region Import nhóm hàng hóa
        /// <summary>
        /// Đọc file excel
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_UploadFile")]
        [HttpPost]
        public object DM_DonVi_UploadFile([FromBody] FileImport data)
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);

            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            ErrorModel _error = new ErrorModel();

            try
            {

                long _idUser = loginData.Id;
                if (string.IsNullOrEmpty(data.base64) && data.fileByte == null)
                {
                    return JsonResultCommon.Custom("Chưa có file nào được upload");
                }

                Match _extension = Regex.Match(data.filename, @"(.xls$|.xlsx$)");
                if (!_extension.Success)
                    return JsonResultCommon.Custom("File không hợp lệ");

                //convert string base64 to byte array
                byte[] _fileByte = null;

                if (data.fileByte != null)
                    _fileByte = data.fileByte;
                else
                    _fileByte = Convert.FromBase64String(data.base64);

                string _path = "dulieu/Data/" + _idUser + "/Import/";

                string _targetPath = Path.Combine(_hostingEnvironment.ContentRootPath, _path);

                if (!Directory.Exists(_targetPath))
                    Directory.CreateDirectory(_targetPath);

                string _fileName = _targetPath + "FileImportDonVi" + DateTime.Now.ToString("yyyyMMddHHmmss") + "_" + data.filename;
                System.IO.File.WriteAllBytes(_fileName, _fileByte);

                ExcelPackage.LicenseContext = LicenseContext.NonCommercial;
                using (var excelPack = new ExcelPackage())
                {
                    using (var stream = System.IO.File.OpenRead(_fileName))
                    {
                        try
                        {
                            excelPack.Load(stream);

                            ExcelWorksheet worksheet = excelPack.Workbook.Worksheets[0];
                            var rowCount = worksheet.Dimension.Rows;

                            int skipRows = 1;
                            int startRow = skipRows + 1;

                            if (rowCount <= skipRows || startRow > rowCount)
                            {
                                var message = "Không có dữ liệu, vui lòng kiểm tra lại";
                                return JsonResultCommon.Custom(message);
                            }

                            List<DM_DonVi_Model> dataImport = new List<DM_DonVi_Model>();
                            DM_DonVi_Model element;

                            for (int row = startRow; row <= rowCount; row++)
                            {
                                element = new DM_DonVi_Model();

                                try { element.MaDonvi = worksheet.Cells[row, 2].Value.ToString().Trim(); element.messageError = "null,"; }
                                catch (Exception ex) { element.isError = true; element.messageError = "Lỗi mã đơn vị,"; }

                                try { element.DonVi = worksheet.Cells[row, 3].Value.ToString().Trim(); element.messageError += "null,"; }
                                catch (Exception ex) { element.isError = true; element.messageError += "Lỗi tên đơn vị,"; }

                                element.MaDinhDanh = worksheet.Cells[row, 4].Value == null ? string.Empty : worksheet.Cells[row, 4].Value.ToString().Trim(); element.messageError += "null,";

                                element.Parent = worksheet.Cells[row, 5].Value == null ? 0 : decimal.Parse(worksheet.Cells[row, 5].Value.ToString().Trim()); element.messageError += "null,";

                                try { element.LoaiDonVi = decimal.Parse(worksheet.Cells[row, 6].Value.ToString().Trim()); element.messageError += "null,"; }
                                catch (Exception ex) { element.isError = true; element.messageError += "Lỗi loại đơn vị,"; }

                                element.SDT = worksheet.Cells[row, 7].Value == null ? string.Empty : worksheet.Cells[row, 7].Value.ToString().Trim(); element.messageError += "null,";

                                element.Email = worksheet.Cells[row, 8].Value == null ? string.Empty : worksheet.Cells[row, 8].Value.ToString().Trim(); element.messageError += "null,";

                                element.DiaChi = worksheet.Cells[row, 9].Value == null ? string.Empty : worksheet.Cells[row, 9].Value.ToString().Trim(); element.messageError += "null,";

                                try { element.Priority = decimal.Parse(worksheet.Cells[row, 10].Value.ToString().Trim()); element.messageError += "null,"; }
                                catch (Exception ex) { element.isError = true; element.messageError += "Lỗi thứ tự,"; }

                                element.DangKyLichLanhDao = worksheet.Cells[row, 11].Value == null ? false : decimal.Parse(worksheet.Cells[row, 11].Value.ToString().Trim()) == 1 ? true : false; element.messageError += "null,";

                                dataImport.Add(element);
                            }

                            return JsonResultCommon.ThanhCong(dataImport);
                        }
                        catch (Exception ex)
                        {
                            var message = "Đọc sheet file thất bại: " + ex.Message;
                            return JsonResultCommon.Custom(message);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                string _message = "Lỗi dữ liệu!";
                return JsonResultCommon.Custom(_message);
            }
        }

        /// <summary>
        /// Import đơn vị vào database
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "7")]
        [Route("DM_DonVi_Import")]
        [HttpPost]
        public BaseModel<object> DM_DonVi_Import([FromBody] List<DM_DonVi_Model> data)
        {
            BaseModel<object> _baseModel = new BaseModel<object>();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    foreach (DM_DonVi_Model dv in data)
                    {
                        if (!string.IsNullOrEmpty(dv.MaDonvi) && !string.IsNullOrEmpty(dv.DonVi) && !dv.isError)
                        {
                            var sqlq = "select count(*) from DM_DonVi where Disabled=0  and  MaDonvi = @MaDonvi and LoaiDonVi=@LoaiDonVi";
                            if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("MaDonvi", dv.MaDonvi), new SqlCondition("LoaiDonVi", dv.LoaiDonVi) }) > 0)
                                continue;
                            Hashtable val = new Hashtable();
                            val.Add("DonVi", dv.DonVi);
                            val.Add("MaDonvi", dv.MaDonvi);
                            if (dv.LoaiDonVi > 0)
                                val.Add("LoaiDonVi", dv.LoaiDonVi);
                            val.Add("Locked", dv.Locked);
                            val.Add("DangKyLichLanhDao", dv.DangKyLichLanhDao);
                            val.Add("KhongCoVanThu", dv.KhongCoVanThu);
                            if (dv.Priority >= 0)
                                val.Add("Priority", dv.Priority);
                            if (!string.IsNullOrEmpty(dv.MaDinhDanh))
                                val.Add("MaDinhDanh", dv.MaDinhDanh);
                            if (!string.IsNullOrEmpty(dv.SDT))
                                val.Add("SDT", dv.SDT);
                            if (!string.IsNullOrEmpty(dv.Email))
                                val.Add("Email", dv.Email);

                            if (dv.Parent > 0)
                                val.Add("Parent", dv.Parent);
                            val.Add("CreatedBy", loginData.Id);
                            cnn.Insert(val, "DM_DonVi");
                        }
                    }
                    logHelper.LogImport(loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                var _message = "Lỗi dữ liệu hoặc bạn phải truyền Token !" + ex.Message;
                return JsonResultCommon.Custom(_message);
            }
        }
        [AllowAnonymous]
        [Route("DownLoadFileImportMau")]
        [HttpGet]
        public FileResult DownLoadFileImportMau()
        {
            try
            {
                string fileName = "IMPORT_DONVI.xlsx";
                string path = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.TEMPLATE_IMPORT_FOLDER + "/IMPORT_DONVI.xlsx");
                var fileExists = System.IO.File.Exists(path);
                return PhysicalFile(path, "application/vnd.openxmlformats-officedocument.spreadsheetml.sheet", fileName);
            }
            catch (Exception ex)
            {
                return null;
            }
        }
        #endregion
    }
}
