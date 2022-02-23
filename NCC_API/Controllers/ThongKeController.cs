using System;
using System.Collections.Generic;
using System.Data;
using System.Globalization;
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

namespace Timensit_API.Controllers.QuanTri
{
    [ApiController]
    [Route("api/thong-ke")]
    [EnableCors("TimensitPolicy")]
    public class ThongKeController : ControllerBase
    {
        LoginController lc;
        private NCCConfig _config;
        public ThongKeController(IOptions<NCCConfig> configLogin)
        {
            _config = configLogin.Value;
            lc = new LoginController();
        }
        /// <summary>
        /// Thống kê tổng hợp số tài khoản truy cập hệ thống
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("thong-ke-2")]
        public BaseModel<object> ThongKe2([FromQuery] QueryParams query)
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
                    string str = "";
                    if (!string.IsNullOrEmpty(query.filter["DonVi"]))
                    {
                        str = string.Format(" and (Parent={0} or dv.Id={0})", query.filter["DonVi"]);
                    }
                    if (string.IsNullOrEmpty(query.filter["TuNgay"]) || string.IsNullOrEmpty(query.filter["DenNgay"]))
                    {
                        return JsonResultCommon.Custom("Khoảng thời gian không hợp lệ");
                    }
                    DateTime from = DateTime.Now;
                    DateTime to = DateTime.Now;
                    bool from1 = DateTime.TryParseExact(query.filter["TuNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out from);
                    bool to1 = DateTime.TryParseExact(query.filter["DenNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out to);
                    if (!from1 || !to1)
                    {
                        return JsonResultCommon.Custom("Khoảng thời gian không hợp lệ");
                    }
                    to = to.AddDays(1);
                    double ck = (to - from).TotalDays;
                    DateTime to2 = from;
                    DateTime from2 = to2.AddDays(-ck);
                    string sqlq = @"select dv.Id, dv.DonVi, dv.MaDonvi, count(distinct u.Username) as SoLuong from DPS_User u join DM_DonVi dv on u.IdDonVi=dv.Id
                                    join Tbl_Log log on u.UserID=log.CreatedBy and log.IdLoaiLog=1 and log.IdHanhDong=6
                                    where u.Deleted=0 and log.CreatedDate>=@from and log.CreatedDate<@to
                                    and dv.Disabled=0 " + str + @"
                                    group by dv.Id, dv.DonVi, dv.MaDonvi";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions() { { "from", from }, { "to", to } });
                    var dt2 = cnn.CreateDataTable(sqlq, new SqlConditions() { { "from", from2 }, { "to", to2 } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                    {
                        { "DonVi","DonVi" },
                        { "MaDonVi","MaDonVi" },
                        { "SoLuong","SoLuong" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
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
                                   Id = r["Id"],
                                   DonVi = r["DonVi"],
                                   MaDonVi = r["MaDonVi"],
                                   SoLuongPrev = (from r2 in dt2.AsEnumerable()
                                                  where r2["Id"].ToString() == r["Id"].ToString()
                                                  select r2["SoLuong"]).FirstOrDefault(),
                                   SoLuong = r["SoLuong"]
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
        /// <summary>
        /// Danh sách tài khoản truy cập hệ thống theo đơn vị
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpPost]
        [Route("thong-ke-3")]
        public BaseModel<object> ThongKe3([FromQuery] QueryParams query)
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
                    string str = "";
                    if (!string.IsNullOrEmpty(query.filter["DonVi"]))
                    {
                        str = string.Format(" and dv.Id={0}", query.filter["DonVi"]);
                    }
                    if (string.IsNullOrEmpty(query.filter["TuNgay"]) || string.IsNullOrEmpty(query.filter["DenNgay"]))
                    {
                        return JsonResultCommon.Custom("Khoảng thời gian không hợp lệ");
                    }
                    DateTime from = DateTime.Now;
                    DateTime to = DateTime.Now;
                    bool from1 = DateTime.TryParseExact(query.filter["TuNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out from);
                    bool to1 = DateTime.TryParseExact(query.filter["DenNgay"], "dd/MM/yyyy", CultureInfo.InvariantCulture, DateTimeStyles.None, out to);
                    if (!from1 || !to1)
                    {
                        return JsonResultCommon.Custom("Khoảng thời gian không hợp lệ");
                    }
                    to = to.AddDays(1);
                    string sqlq = @"select u.UserID, u.Username, u.FullName,log.CreatedDate, dv.DonVi, dv.MaDonvi from DPS_User u join DM_DonVi dv on u.IdDonVi=dv.Id
                                    join Tbl_Log log on u.UserID=log.CreatedBy and log.IdLoaiLog=1 and log.IdHanhDong=6
                                    where u.Deleted=0 and log.CreatedDate>=@from and log.CreatedDate<@to
                                    and dv.Disabled=0 " + str;
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions() { { "from", from }, { "to", to } });
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
                        { "CreatedDate","CreatedDate" },
                        { "DonVi","DonVi" },
                        { "MaDonvi","MaDonvi" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if (!"desc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
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
                                   UserID = r["UserID"],
                                   Username = r["Username"],
                                   FullName = r["FullName"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm:ss}", r["CreatedDate"]),
                                   DonVi = r["DonVi"],
                                   MaDonVi = r["MaDonVi"],
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }

        [Route("thong-ke-dasboard")]
        [HttpGet]
        public async Task<BaseModel<object>> ThongKeDashboard()
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
                    string strW = "", strW1 = "";
                    if (loginData.Capcocau == 2)//cấp huyện
                    {
                        strW += $@" and (Id_Xa in ( select RowId from Tbl_Cocautochuc where disable=0 and ParentID= {loginData.IdDonVi} ) 
or DistrictID = {loginData.ID_Goc_Cha})";
                        strW1 += $@"and (dv.Id_DonVi in ( select RowId from Tbl_Cocautochuc where disable=0 and ParentID= {loginData.IdDonVi}  )
or (dv.Id_DonVi in (select c.RowId from Tbl_Cocautochuc c
join Tbl_Cocautochuc pa on c.ParentID=pa.RowID where pa.disable=0 and pa.Type='H' and pa.ID_Goc = {loginData.ID_Goc_Cha})))";
                    }
                    if (loginData.Capcocau == 3)//cấp xã
                    {
                        strW += " and Id_Xa=" + loginData.IdDonVi;
                        strW1 += " and dv.Id_DonVi=" + loginData.IdDonVi;
                    }
                    //hồ sơ
                    string sqlq = $@"select st.Status, Description, ISNULL(Quantity,0) as Quantity from (
select [Status],  count(ID) as Quantity from Tbl_NCC vb
join DM_Wards xa on xa.RowID=vb.Id_Xa
join DM_District huyen on huyen.Id_Row=xa.DistrictID where Disabled=0 " + strW + @" group by vb.Status
) vb  right join DF_Status st on vb.Status = st.Status where st.Status<>0";
                    //đề xuất
                    sqlq += @";select st.Status, Description, ISNULL(Quantity,0) as Quantity from (
select [Status],  count(ID) as Quantity from Tbl_DeXuatTangQua vb
join DM_Wards xa on xa.RowID=vb.Id_Xa
join DM_District huyen on huyen.Id_Row=xa.DistrictID where Disabled=0 " + strW + @" group by vb.Status
) vb  right join DF_Status st on vb.Status = st.Status where st.Status<>0";
                    //nhập số liệu
                    sqlq += @";select st.Status, Description, ISNULL(Quantity,0) as Quantity from (
select [Status],  count(dm.ID) as Quantity from Tbl_NhapSoLieu dm
join Tbl_MauSoLieu_DonVi dv on dv.Id = dm.Id_MauSoLieu_DonVi where dm.Disabled=0 and dv.disabled=0 " + strW1 + @" group by dm.Status
) vb  right join DF_Status st on vb.Status = st.Status where st.Status<>0";
                    var ds = cnn.CreateDataSet(sqlq);
                    var HS = new List<ThongKeDasboardModel>();
                    var tang = new List<ThongKeDasboardModel>();
                    var solieu = new List<ThongKeDasboardModel>();

                    if (ds != null && ds.Tables.Count > 0)
                    {
                        if (ds.Tables[0].Rows.Count > 0)
                        {
                            HS = (from r in ds.Tables[0].AsEnumerable()
                                  select new ThongKeDasboardModel
                                  {
                                      Status = (short)r["Status"],
                                      Description = r["Description"] != DBNull.Value ? r["Description"].ToString() : "",
                                      Quantity = (int)r["Quantity"],
                                  }).ToList();
                        }
                        if (ds.Tables[1].Rows.Count > 0)
                        {
                            tang = (from r in ds.Tables[1].AsEnumerable()
                                    select new ThongKeDasboardModel
                                    {
                                        Status = (short)r["Status"],
                                        Description = r["Description"] != DBNull.Value ? r["Description"].ToString() : "",
                                        Quantity = (int)r["Quantity"],
                                    }).ToList();
                        }
                        if (ds.Tables[2].Rows.Count > 0)
                        {
                            solieu = (from r in ds.Tables[2].AsEnumerable()
                                      select new ThongKeDasboardModel
                                      {
                                          Status = (short)r["Status"],
                                          Description = r["Description"] != DBNull.Value ? r["Description"].ToString() : "",
                                          Quantity = (int)r["Quantity"],
                                      }).ToList();
                        }
                    }
                    var data = new List<object>(){
                            new{
                                name = "Hồ sơ NCC",
                                list = HS,
                                color = "success",
                                icon = "fa fa-file"
                            },
                             new{
                                name = "Đề xuất tặng quà",
                                list = tang,
                                color = "primary",
                                icon = "fa fa-file"
                            },
                              new{
                                name = "Số liệu năm",
                                list = solieu,
                                color = "danger",
                                icon = "fa fa-calendar"
                            }
                    };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }


        [Route("bieudo-vanban")]
        [HttpGet]
        public BaseModel<object> BieuDoThongKeVanBan()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string strW = "", strW1 = "";
                    if (loginData.Capcocau == 2)//cấp huyện
                    {
                        strW += $@" and (Id_Xa in ( select RowId from Tbl_Cocautochuc where disable=0 and ParentID= {loginData.IdDonVi} ) 
or DistrictID = {loginData.ID_Goc_Cha})";
                        strW1 += $@"and (dv.Id_DonVi in ( select RowId from Tbl_Cocautochuc where disable=0 and ParentID= {loginData.IdDonVi}  )
or (dv.Id_DonVi in (select c.RowId from Tbl_Cocautochuc c
join Tbl_Cocautochuc pa on c.ParentID=pa.RowID where pa.disable=0 and pa.Type='H' and pa.ID_Goc = {loginData.ID_Goc_Cha})))";
                    }
                    if (loginData.Capcocau == 3)//cấp xã
                    {
                        strW += " and Id_Xa=" + loginData.IdDonVi;
                        strW1 += " and dv.Id_DonVi=" + loginData.IdDonVi;
                    }
                    string sql = @"select Deadline, Status, CheckDate from Tbl_NCC dm
join DM_Wards xa on xa.RowID=dm.Id_Xa
join DM_District huyen on huyen.Id_Row=xa.DistrictID
where dm.Disabled=0 and dm.status<>0  " + strW;

                    sql += @";select Deadline, Status, CheckDate from Tbl_DeXuatTangQua dm
join DM_Wards xa on xa.RowID=dm.Id_Xa
join DM_District huyen on huyen.Id_Row=xa.DistrictID
where dm.Disabled=0 and dm.status<>0 " + strW;

                    sql += @" ;select Deadline, Status, CheckDate from Tbl_NhapSoLieu dm
                    join Tbl_MauSoLieu_DonVi dv on dv.Id = dm.Id_MauSoLieu_DonVi
                    where dm.Disabled = 0 and dv.disabled=0 and dm.status <> 0 " + strW1;
                    DataSet ds = cnn.CreateDataSet(sql);
                    var dtA = ds.Tables[0].AsEnumerable();
                    var dtB = ds.Tables[1].AsEnumerable();
                    var dtC = ds.Tables[2].AsEnumerable();
                    var data = new
                    {
                        VanBanDen = new
                        {
                            HTDungHan = dtA.Where(x => x["status"].ToString() == "2" && (x["deadline"] == DBNull.Value || x["CheckDate"] == DBNull.Value || (DateTime)x["deadline"] >= (DateTime)x["CheckDate"])).Count(),
                            HTTreHan = dtA.Where(x => x["status"].ToString() == "2" && x["deadline"] != DBNull.Value && x["CheckDate"] != DBNull.Value && (DateTime)x["deadline"] < (DateTime)x["CheckDate"]).Count(),
                            DangLam = dtA.Where(x => x["status"].ToString() == "1" && (x["deadline"] == DBNull.Value || (DateTime)x["deadline"] >= DateTime.Now)).Count(),
                            TreHan = dtA.Where(x => x["status"].ToString() == "1" && x["deadline"] != DBNull.Value && (DateTime)x["deadline"] < DateTime.Now).Count(),
                        },
                        VanBanDi = new
                        {
                            HTDungHan = dtB.Where(x => x["status"].ToString() == "2" && (x["deadline"] == DBNull.Value || x["CheckDate"] == DBNull.Value || (DateTime)x["deadline"] >= (DateTime)x["CheckDate"])).Count(),
                            HTTreHan = dtB.Where(x => x["status"].ToString() == "2" && x["deadline"] != DBNull.Value && x["CheckDate"] != DBNull.Value && (DateTime)x["deadline"] < (DateTime)x["CheckDate"]).Count(),
                            DangLam = dtB.Where(x => x["status"].ToString() == "1" && (x["deadline"] == DBNull.Value || (DateTime)x["deadline"] >= DateTime.Now)).Count(),
                            TreHan = dtB.Where(x => x["status"].ToString() == "1" && x["deadline"] != DBNull.Value && (DateTime)x["deadline"] < DateTime.Now).Count(),
                        },
                        SoLieu = new
                        {
                            HTDungHan = dtC.Where(x => x["status"].ToString() == "2" && (x["deadline"] == DBNull.Value || x["CheckDate"] == DBNull.Value || (DateTime)x["deadline"] >= (DateTime)x["CheckDate"])).Count(),
                            HTTreHan = dtC.Where(x => x["status"].ToString() == "2" && x["deadline"] != DBNull.Value && x["CheckDate"] != DBNull.Value && (DateTime)x["deadline"] < (DateTime)x["CheckDate"]).Count(),
                            DangLam = dtC.Where(x => x["status"].ToString() == "1" && (x["deadline"] == DBNull.Value || (DateTime)x["deadline"] >= DateTime.Now)).Count(),
                            TreHan = dtC.Where(x => x["status"].ToString() == "1" && x["deadline"] != DBNull.Value && (DateTime)x["deadline"] < DateTime.Now).Count(),
                        }
                    };

                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex,ControllerContext);
            }
        }
    }
}
