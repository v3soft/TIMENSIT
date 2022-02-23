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
using Timensit_API.Classes;
using Microsoft.AspNetCore.Http;
using Timensit_API.Controllers.Common;
using Timensit_API.Models.DanhMuc;
using Microsoft.AspNetCore.Hosting;

namespace Timensit_API.Controllers.DanhMuc
{
    /// <summary>
    /// Quyền 3: quản lý danh mục khác
    /// </summary>
    [ApiController]
    [Route("api/loai-ho-so")]
    [EnableCors("TimensitPolicy")]
    public class LoaiHoSoController : Controller
    {
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Loại hồ sơ";
        List<CheckFKModel> FKs;
        public LoaiHoSoController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "tbl_NCC",
                PKColumn = "Id_LoaiHoSo",
                DisabledColumn = "Disabled",
                name = "Người có công"
            });
        }

        /// <summary>
        /// Danh sách Danh mục khác
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize(Roles = "3")]
        [HttpGet]
        public BaseModel<object> DanhSach([FromQuery] QueryParams query)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select hs.*, u.LoaiGiayTo as LoaiGiayTo, u1.LoaiGiayTo as LoaiGiayToCC, u2.FullName as NguoiSua, coalesce(dt.DoiTuong,'') as DoiTuong  from Const_LoaiHoSo hs
left join DM_LoaiGiayTo u on hs.Id_LoaiGiayTo=u.id
left join DM_LoaiGiayTo u1 on hs.Id_LoaiGiayTo_CC=u1.id
left join Dps_User u2 on hs.UpdatedBy=u2.UserID
left join DM_DoiTuongNCC dt on dt.Id=hs.Id_DoiTuongNCC
where hs.Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "LoaiHoSo","LoaiHoSo" },
                        { "MaLoaiHoSo","MaLoaiHoSo" },
                        { "Id_DoiTuongNCC","DoiTuong" },
                        { "LoaiGiayTo","LoaiGiayTo" },
                        { "LoaiGiayToCC","LoaiGiayToCC" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaLoaiHoSo"]))
                    {
                        string keyword = query.filter["MaLoaiHoSo"].ToLower();
                        temp = temp.Where(x => x["MaLoaiHoSo"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["LoaiHoSo"]))
                    {
                        string keyword = query.filter["LoaiHoSo"].ToLower();
                        temp = temp.Where(x => x["LoaiHoSo"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    #endregion
                    bool Visible = true;
                    if (User.IsInRole("37"))
                        Visible = false;
                    int total = temp.Count();
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, Visible);
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

                                   MaLoaiHoSo = r["MaLoaiHoSo"],

                                   LoaiHoSo = r["LoaiHoSo"],

                                   DoiTuong = r["DoiTuong"],

                                   Id_LoaiGiayTo = r["Id_LoaiGiayTo"],

                                   Id_LoaiGiayTo_CC = r["Id_LoaiGiayTo_CC"],

                                   LoaiGiayTo = r["LoaiGiayTo"],

                                   LoaiGiayToCC = r["LoaiGiayToCC"],

                                   MauCongNhan = r["MauCongNhan"],

                                   UpdatedBy = r["NguoiSua"],
                                   UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["UpdatedDate"])

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

        /// <summary>
        /// Chi tiết 
        /// </summary>
        /// <param name="id"></param>DM_LoaiGiayTo
        /// <returns></returns>
        [Authorize(Roles = "3")]
        [HttpGet("{id}")]
        public BaseModel<object> DetailItem(int id)
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
                    string sqlq = @"select * from Const_LoaiHoSo where Disabled=0 and Id = @Id";
                    sqlq += @" select l.*, gt.IsRequired from Const_LoaiHoSo_GiayTo gt
join DM_LoaiGiayTo l on l.Id = gt.Id_LoaiGiayTo
 where l.Disabled = 0 and gt.Id_LoaiHoSo = @Id";
                    //sqlq += " select c.*, bm.BieuMau from Const_LoaiHoSo_BieuMau c join Tbl_BieuMau bm on c.Id_BieuMau=bm.Id where Id_LoaiHoSo=@Id";
                    //sqlq += " select c.*, bm.DoiTuong from Const_LoaiHoSo_DoiTuong c join DM_DoiTuongNCC bm on c.Id_DoiTuong=bm.Id where c.Id_LoaiHoSo=@Id";
                    sqlq += @" select c.*, lbm.Id_BieuMau, bm.BieuMau, dt.DoiTuong from Const_LoaiHoSo_DoiTuong c 
left join Const_LoaiHoSo_BieuMau lbm on lbm.Id_LoaiHoSo_DT = c.Id
left join DM_DoiTuongNCC dt on c.Id_DoiTuong=dt.Id
left join Tbl_BieuMau bm on lbm.Id_BieuMau=bm.Id where c.Id_LoaiHoSo = @Id";
                    
                    DataSet ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "Id", id } });
                    if (cnn.LastError != null || ds == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong((from r in dt.AsEnumerable()
                                                       select new
                                                       {

                                                           Id = r["Id"],

                                                           MaLoaiHoSo = r["MaLoaiHoSo"],

                                                           LoaiHoSo = r["LoaiHoSo"],

                                                           Id_LoaiGiayTo = r["Id_LoaiGiayTo"],

                                                           Id_LoaiGiayTo_CC = r["Id_LoaiGiayTo_CC"],

                                                           Id_DoiTuongNCC = r["Id_DoiTuongNCC"],

                                                           GiayTos = from x in ds.Tables[1].AsEnumerable()
                                                                     select new
                                                                     {
                                                                         id = x["Id"],
                                                                         title = x["LoaiGiayTo"],
                                                                         IsRequired = x["IsRequired"],
                                                                     },
                                                           //BieuMaus = from x in ds.Tables[2].AsEnumerable()
                                                           //           select new
                                                           //           {
                                                           //               Id = x["Id_BieuMau"],
                                                           //               BieuMau = x["BieuMau"]
                                                           //           },
                                                           DoiTuongs = from x in ds.Tables[2].AsEnumerable()
                                                                       group x by new { a = x["Id_DoiTuong"], b = x["DoiTuong"] } into g
                                                                       select new
                                                                      {
                                                                          Id = g.Key.a,
                                                                          DoiTuong = g.Key.b,
                                                                          BieuMaus = from x in ds.Tables[2].AsEnumerable()
                                                                                     where x["Id_DoiTuong"].ToString() == g.Key.a.ToString() && x["Id_BieuMau"] != DBNull.Value
                                                                                      select new
                                                                                      {
                                                                                          Id = x["Id_BieuMau"],
                                                                                          BieuMau = x["BieuMau"]
                                                                                      },
                                                                       },

                                                           MoTa = r["MauCongNhan"],

                                                           MauCongNhan = r["MoTa"],

                                                           UpdatedBy = r["UpdatedBy"],

                                                           AllowEdit = User.IsInRole("3")

                                                       }).FirstOrDefault());
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Thêm mới 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "3")]
        [HttpPost]
        public BaseModel<object> Create([FromBody] LoaiHoSoModel data)
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
                if (string.IsNullOrEmpty(data.MaLoaiHoSo))
                    strRe += (strRe == "" ? "" : ", ") + "mã loại hồ sơ";
                if (string.IsNullOrEmpty(data.LoaiHoSo))
                    strRe += (strRe == "" ? "" : ", ") + "tên loại hồ sơ";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Const_LoaiHoSo where Disabled=0 and (LoaiHoSo = @Name or MaLoaiHoSo=@Ma)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.LoaiHoSo), { "Ma", data.MaLoaiHoSo } }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("MaLoaiHoSo", data.MaLoaiHoSo);
                    val.Add("LoaiHoSo", data.LoaiHoSo);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    if (data.Id_LoaiGiayTo > 0)
                        val.Add("Id_LoaiGiayTo", data.Id_LoaiGiayTo);
                    if (data.Id_LoaiGiayTo_CC > 0)
                        val.Add("Id_LoaiGiayTo_CC", data.Id_LoaiGiayTo_CC);
                    if (data.Id_DoiTuongNCC.HasValue && data.Id_DoiTuongNCC > 0)
                        val.Add("Id_DoiTuongNCC", data.Id_DoiTuongNCC);

                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", loginData.Id);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "Const_LoaiHoSo") == 1)
                    {
                        data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Const_LoaiHoSo') AS Current_Identity; ").ToString());
                        foreach (var gt in data.GiayTos)
                        {
                            Hashtable val1 = new Hashtable();
                            val1["Id_LoaiHoSo"] = data.Id;
                            val1["Id_LoaiGiayTo"] = gt.Id;
                            val1["IsRequired"] = gt.IsRequired;
                            if (cnn.Insert(val1, "Const_LoaiHoSo_GiayTo") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                        //if (data.BieuMaus != null)
                        //{
                        //    foreach (var bm in data.BieuMaus)
                        //    {
                        //        Hashtable val1 = new Hashtable();
                        //        val1["Id_LoaiHoSo"] = data.Id;
                        //        val1["Id_LoaiGiayTo"] = bm;
                        //        if (cnn.Insert(val1, "Const_LoaiHoSo_BieuMau") != 1)
                        //        {
                        //            cnn.RollbackTransaction();
                        //            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        //        }
                        //    }
                        //}
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<LoaiHoSoModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        cnn.RollbackTransaction();
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
        /// Update Danh mục khác
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>

        [Authorize(Roles = "3")]
        [HttpPut("{id}")]
        public BaseModel<object> Update(int id, [FromBody] LoaiHoSoModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                string strRe = "";
                if (string.IsNullOrEmpty(data.MaLoaiHoSo))
                    strRe += (strRe == "" ? "" : ", ") + "mã loại hồ sơ";
                if (string.IsNullOrEmpty(data.LoaiHoSo))
                    strRe += (strRe == "" ? "" : ", ") + "Loại hồ sơ";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);

                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Const_LoaiHoSo where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from Const_LoaiHoSo where Disabled=0 and (LoaiHoSo = @Name) and Id <> @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "Id", id }, new SqlCondition("Name", data.LoaiHoSo) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("MaLoaiHoSo", data.MaLoaiHoSo);
                    val.Add("LoaiHoSo", data.LoaiHoSo);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    if (data.Id_LoaiGiayTo > 0)
                        val.Add("Id_LoaiGiayTo", data.Id_LoaiGiayTo);
                    else
                        val.Add("Id_LoaiGiayTo", DBNull.Value);
                    if (data.Id_LoaiGiayTo_CC > 0)
                        val.Add("Id_LoaiGiayTo_CC", data.Id_LoaiGiayTo_CC);
                    else
                        val.Add("Id_LoaiGiayTo_CC", DBNull.Value);
                    if (data.Id_DoiTuongNCC.HasValue && data.Id_DoiTuongNCC > 0)
                        val.Add("Id_DoiTuongNCC", data.Id_DoiTuongNCC);
                    else
                        val.Add("Id_DoiTuongNCC", DBNull.Value);

                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);
                    cnn.BeginTransaction();
                    string strDel = "delete Const_LoaiHoSo_GiayTo where Id_LoaiHoSo=" + id;
                    if (cnn.ExecuteNonQuery(strDel) < 0)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    if (data.GiayTos != null)
                    {
                        foreach (var gt in data.GiayTos)
                        {
                            Hashtable val1 = new Hashtable();
                            val1["Id_LoaiHoSo"] = data.Id;
                            val1["Id_LoaiGiayTo"] = gt.Id;
                            val1["IsRequired"] = gt.IsRequired;
                            if (cnn.Insert(val1, "Const_LoaiHoSo_GiayTo") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    }
                    #region đối tượng áp dụng - biểu mẫu của đt
                    if (data.DoiTuongs != null)
                    {
                        foreach (var dt in data.DoiTuongs)
                        {
                            if (dt.BieuMaus != null)
                            {
                                strDel = " select Id from Const_LoaiHoSo_DoiTuong where Id_DoiTuong = @Id_dt and Id_LoaiHoSo = @Id_Loai";
                                var kq = cnn.ExecuteScalar(strDel, new SqlConditions { { "Id_dt", dt.Id }, { "Id_Loai", data.Id } });
                                if (kq == null || cnn.LastError != null)
                                {
                                    cnn.RollbackTransaction();
                                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                }
                                strDel = "delete Const_LoaiHoSo_BieuMau where Id_LoaiHoSo_DT=" + kq.ToString();
                                if (cnn.ExecuteNonQuery(strDel) < 0)
                                {
                                    cnn.RollbackTransaction();
                                    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                }
                                foreach (var bm in dt.BieuMaus)
                                {
                                    Hashtable val1 = new Hashtable();
                                    val1["Id_LoaiHoSo_DT"] = kq.ToString();
                                    val1["Id_BieuMau"] = bm.Id;

                                    if (cnn.Insert(val1, "Const_LoaiHoSo_BieuMau") != 1)
                                    {
                                        cnn.RollbackTransaction();
                                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                                    }
                                }
                            }
                            
                        }
                    }
                    #endregion
                    #region biểu mẫu (cũ)
                    //strDel = "delete Const_LoaiHoSo_BieuMau where Id_LoaiHoSo=" + id;
                    //if (cnn.ExecuteNonQuery(strDel) < 0)
                    //{
                    //    cnn.RollbackTransaction();
                    //    return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    //}
                    //if (data.BieuMaus != null)
                    //{
                    //    foreach (var bm in data.BieuMaus)
                    //    {
                    //        Hashtable val1 = new Hashtable();
                    //        val1["Id_LoaiHoSo"] = data.Id;
                    //        val1["Id_BieuMau"] = bm;
                    //        if (cnn.Insert(val1, "Const_LoaiHoSo_BieuMau") != 1)
                    //        {
                    //            cnn.RollbackTransaction();
                    //            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    //        }
                    //    }
                    //}
                    #endregion
                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "Const_LoaiHoSo") == 1)
                    {
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<LoaiHoSoModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
                        return JsonResultCommon.ThanhCong(data);
                    }
                    else
                    {
                        cnn.RollbackTransaction();
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
        /// Xóa
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "3")]
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(int id)
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
                    string sqlq = "select * from Const_LoaiHoSo where Disabled = 0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    string msg = "";
                    #region check đã được sử dụng
                    if (LiteController.InUse(cnn, FKs, id, out msg))
                    {
                        return JsonResultCommon.Custom(Name + " đang được sử dụng cho " + msg + ", không thể xóa");
                    }
                    #endregion

                    sqlq = "update Const_LoaiHoSo set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["LoaiHoSo"].ToString() };
                        logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
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
    }
}
