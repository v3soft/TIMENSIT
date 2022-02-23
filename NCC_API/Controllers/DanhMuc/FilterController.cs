using DpsLibs.Data;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Timensit_API.Classes;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Timensit_API.Models;
using Microsoft.Extensions.Options;
using Timensit_API.Models.Common;
using Timensit_API.Controllers.Users;
using Timensit_API.Models.DanhMuc;
using Timensit_API.Controllers.Common;
using Microsoft.AspNetCore.Http;

namespace Timensit_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/filter")]
    [EnableCors("TimensitPolicy")]
    /// <summary>
    /// Danh mục bộ lọc phục vụ cho số liệu
    /// Quyền xem: 42	Quản lý danh mục số liệu đơn vị
    /// Quyền sửa: 43	Cập nhật danh mục số liệu đơn vị
    /// </summary>
    public class FilterController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private NCCConfig _config;
        LoginController lc;
        LogHelper logHelper;
        string Name = "Bộ trích xuất";

        public FilterController(IOptions<NCCConfig> config, IHostingEnvironment hostingEnvironment, IHttpContextAccessor accessor)
        {
            _hostingEnvironment = hostingEnvironment;
            _config = config.Value;
            lc = new LoginController();
            logHelper = new LogHelper(config.Value, accessor, hostingEnvironment, 2);
        }
        /// <summary>
        /// list custom filter of current user
        /// </summary>
        /// <returns></returns>
        [Route("List")]
        [HttpGet]
        public object List([FromQuery] QueryParams query)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                bool Visible = User.IsInRole("43");
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select  dm.*, u.FullName as NguoiTao, u1.FullName as NguoiSua from we_filter dm
join Dps_User u on dm.CreatedBy=u.UserID
left join Dps_User u1 on dm.UpdatedBy=u1.UserID where dm.disabled=0 ";
                    DataTable dt = cnn.CreateDataTable(sql);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                    #region Sort/filter
                    var temp = dt.AsEnumerable();
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "title","title" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["title"]))
                    {
                        string keyword = query.filter["title"].ToLower();
                        temp = temp.Where(x => x["title"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["bang"] != null && query.filterGroup["bang"].Length > 0)
                    {
                        var groups = query.filterGroup["bang"].ToList();
                        temp = temp.Where(x => groups.Contains(x["bang"].ToString()));
                    }
                    #endregion
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
                                   id_row = r["id_row"],
                                   title = r["title"],
                                   bang = r["bang"],
                                   color = r["color"],

                                   CreatedBy = r["NguoiTao"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
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
        /// list filter key in system: we_filter_key
        /// </summary>
        /// <returns></returns>
        [Route("list_filterkey")]
        [HttpGet]
        public object Lite_FilterKey()
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select id_row, title, loai,sql, table_name from we_filter_key";
                    DataTable dt = cnn.CreateDataTable(sql);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    cnn.Disconnect();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   id_row = r["id_row"],
                                   title = r["title"],
                                   loai = r["loai"],
                                   table_name = r["table_name"],
                                   operators = getOperator(r["loai"].ToString()),
                                   options = getOption(r["sql"].ToString())
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Detail of custom filter by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Detail")]
        [HttpGet]
        public object Detail(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    string sqlq = @"select * from we_filter where disabled=0 and createdby=" + loginData.Id + " and id_row=" + id;
                    sqlq += @";select * from we_filter_detail where id_filter = " + id;
                    DataSet ds = cnn.CreateDataSet(sqlq);
                    if (cnn.LastError != null || ds == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    if (ds.Tables[0] == null || ds.Tables[0].Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai();
                    var data = (from r in ds.Tables[0].AsEnumerable()
                                select new
                                {
                                    id_row = r["id_row"],
                                    title = r["title"],
                                    color = r["color"],
                                    pheptoan = r["pheptoan"],
                                    bang = r["bang"],
                                    details = from w in ds.Tables[1].AsEnumerable()
                                              select new
                                              {
                                                  id_row = w["id_row"],
                                                  id_key = w["id_key"],
                                                  @operator = w["operator"],
                                                  value = w["value"]
                                              }
                                }).FirstOrDefault();
                    logHelper.LogXemCT(id, loginData.Id, Name);
                    return JsonResultCommon.ThanhCong(data);
                }
                #endregion
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Insert filter
        /// </summary>
        /// <param name="data">FilterModel: title and details are required</param>
        /// <returns></returns>
        [Route("Insert")]
        [HttpPost]
        public async Task<object> Insert(FilterSoLieuModel data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                string strRe = "";
                if (string.IsNullOrEmpty(data.title))
                    strRe += (strRe == "" ? "" : ",") + "tên bộ trích xuất";
                if (string.IsNullOrEmpty(data.bang))
                    strRe += (strRe == "" ? "" : ",") + "nguồn dữ liệu";
                if (string.IsNullOrEmpty(data.pheptoan))
                    strRe += (strRe == "" ? "" : ",") + "phép toán";
                //if (data.details == null || data.details.Count == 0)
                //    strRe += (strRe == "" ? "" : ",") + "trường thông tin filter";
                if (strRe != "")
                    return JsonResultCommon.BatBuoc(strRe);

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("title", data.title);
                    val.Add("bang", data.bang);
                    val.Add("pheptoan", data.pheptoan);
                    if (string.IsNullOrEmpty(data.color))
                        val.Add("color", "");
                    else
                        val.Add("color", data.color);
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", iduser);
                    string strCheck = "select count(*) from we_filter where Disabled=0 and (CreatedBy=@id_user) and title=@name";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "id_user", loginData.Id }, { "name", data.title } }).ToString()) > 0)
                    {
                        return JsonResultCommon.Custom("Filter đã tồn tại");
                    }
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "we_filter") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string idc = cnn.ExecuteScalar("select IDENT_CURRENT('we_filter')").ToString();
                    Hashtable val1 = new Hashtable();
                    val1["id_filter"] = idc;
                    foreach (var key in data.details)
                    {
                        val1["id_key"] = key.id_key;
                        val1["value"] = key.value;
                        val1["operator"] = key.@operator;
                        if (cnn.Insert(val1, "we_filter_detail") != 1)
                        {
                            cnn.RollbackTransaction();
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        }
                    }
                    //string LogContent = "", LogEditContent = "";
                    //LogContent = LogEditContent = "Thêm mới dữ liệu filter: title=" + data.title + ", id_user=" + loginData.Id;
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    cnn.EndTransaction();
                    data.id_row = int.Parse(idc);
                    logHelper.Ghilogfile<FilterSoLieuModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Update filter
        /// </summary>
        /// <param name="data">FilterModel: title and details are required; old detail not in details will be deleted and then inserted detail.idrow=0</param>
        /// <returns></returns>
        [Route("Update")]
        [HttpPost]
        public async Task<object> Update(FilterSoLieuModel data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                string strRe = "";
                if (string.IsNullOrEmpty(data.title))
                    strRe += (strRe == "" ? "" : ",") + "tên filter";
                if (string.IsNullOrEmpty(data.bang))
                    strRe += (strRe == "" ? "" : ",") + "nguồn dữ liệu";
                if (string.IsNullOrEmpty(data.pheptoan))
                    strRe += (strRe == "" ? "" : ",") + "phép toán";
                //if (data.details == null || data.details.Count == 0)
                //    strRe += (strRe == "" ? "" : ",") + "trường thông tin filter";
                if (strRe != "")
                    return JsonResultCommon.BatBuoc(strRe);

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id_row", data.id_row);
                    sqlcond.Add("CreatedBy", iduser);
                    string s = "select * from we_filter where createdby=@CreatedBy and disabled=0 and id_row=@id_row";
                    DataTable old = cnn.CreateDataTable(s, sqlcond);
                    if (cnn.LastError != null || old == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    if (old.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Filter");
                    Hashtable val = new Hashtable();
                    val.Add("title", data.title);
                    val.Add("bang", data.bang);
                    val.Add("pheptoan", data.pheptoan);
                    if (string.IsNullOrEmpty(data.color))
                        val.Add("color", "");
                    else
                        val.Add("color", data.color);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", iduser);
                    string strCheck = "select count(*) from we_filter where Disabled=0 and (CreatedBy=@id_user) and title=@name and id_row<>@id_row";
                    if (int.Parse(cnn.ExecuteScalar(strCheck, new SqlConditions() { { "id_user", loginData.Id }, { "name", data.title }, { "id_row", data.id_row } }).ToString()) > 0)
                    {
                        return JsonResultCommon.Custom("Filter đã tồn tại");
                    }
                    cnn.BeginTransaction();
                    if (cnn.Update(val, sqlcond, "we_filter") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    string ids = string.Join(",", data.details.Where(x => x.id_row > 0).Select(x => x.id_row));
                    string strDel = "delete we_filter_detail where id_filter=" + data.id_row;
                    if (ids != "")//xóa
                        strDel += " and id_row not in (" + ids + ")";
                    if (cnn.ExecuteNonQuery(strDel) < 0)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    Hashtable val1 = new Hashtable();
                    val1["id_filter"] = data.id_row;
                    foreach (var key in data.details)
                    {
                        if (key.id_row == 0)
                        {
                            val1["id_key"] = key.id_key;
                            val1["value"] = key.value;
                            val1["operator"] = key.@operator;
                            if (cnn.Insert(val1, "we_filter_detail") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            }
                        }
                    }
                    DataTable dt = cnn.CreateDataTable(s, sqlcond);
                    //string LogContent = "", LogEditContent = "";
                    //LogEditContent = DpsPage.GetEditLogContent(old, dt);
                    //if (!LogEditContent.Equals(""))
                    //{
                    //    LogEditContent = "Chỉnh sửa dữ liệu (" + data.id_row + ") : " + LogEditContent;
                    //    LogContent = "Chỉnh sửa dữ liệu filter (" + data.id_row + "), Chi tiết xem trong log chỉnh sửa chức năng";
                    //}
                    //DpsPage.Ghilogfile(loginData.IDKHDPS.ToString(), LogEditContent, LogContent, loginData.UserName);
                    cnn.EndTransaction();
                    logHelper.Ghilogfile<FilterSoLieuModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.id_row, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// delete filter by id
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Route("Delete")]
        [HttpGet]
        public BaseModel<object> Delete(long id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from we_filter where Disabled = 0 and id_row = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Filter");
                    //if (Common.TestDuplicate("", id.ToString(), "-1", "v_wework", "id_milestone", "", "-1", cnn, "", true) == false)
                    //{
                    //    return JsonResultCommon.Custom("Đang có công việc thuộc mục tiêu này nên không thể xóa");
                    //}
                    sqlq = "update we_filter set Disabled=1, UpdatedDate=getdate(), UpdatedBy=" + iduser + " where id_row = " + id;
                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    var data = new LiteModel() { id = id, title = dtFind.Rows[0]["Title"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id, Name), Name);
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpGet("test")]
        public double Cal(string id_filter)
        {
            double val = 0;
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                var dsNguon = getDataNguon(cnn);
                val = CalFilter(dsNguon, cnn, id_filter);
            }
            return val;
        }

        public static double CalFilter(DataSet dsNguon, DpsConnection cnn, string id_filter)
        {
            string sql = "select * from we_filter where id_row=" + id_filter;
            DataTable dtFind = cnn.CreateDataTable(sql);
            if (dtFind.Rows.Count == 0)
                return 0;
            string dieukien_where = "1=1" + genStringWhere(cnn, id_filter);
            DataTable dt = null;
            switch (dtFind.Rows[0]["bang"].ToString())
            {
                case "tbl_ncc": dt = dsNguon.Tables[0]; break;
                case "tbl_doituongnhanqua":
                    if (dieukien_where.Contains("Id_DotTangQua = "))
                        dieukien_where = dieukien_where.Replace("Id_DotTangQua = ", "Id_DotTangQua <= ");
                    var temp = (from de in dsNguon.Tables[1].AsEnumerable()
                                join dot in dsNguon.Tables[2].AsEnumerable() on de["Id_DotTangQua"].ToString() equals dot["Id"].ToString()
                                select new
                                {
                                    Id = (int)de["Id"], //long to int
                                    SoTien = (decimal)de["SoTien"],
                                    Id_NguonKinhPhi = de["Id_NguonKinhPhi"],
                                    Id_DotTangQua = de["Id_DotTangQua"],
                                    Id_DoiTuongNCC = de["Id_DoiTuongNCC"],
                                    Id_Xa = de["Id_Xa"],
                                    Id_KhomAp = de["Id_KhomAp"],
                                    DistrictID = de["DistrictID"],
                                    Id_NhomLeTet = dot["Id_NhomLeTet"],
                                    Nam = dot["Nam"]
                                }).ToList();
                    dt = LiteController.ToDataTable(temp);
                    break;
            }
            var value = dt.Compute(dtFind.Rows[0]["pheptoan"].ToString(), dieukien_where);
            if (value != null)
                return double.Parse(value.ToString());
            return 0;
        }

        public static DataSet getDataNguon(DpsConnection cnn)
        {
            string sql = @"select ncc.*, xa.DistrictID,cast( coalesce(IsChet,0) as bit ) as IsChet, hd.TuNgay as NgayChet, Id_LoaiTroCap, coalesce(tc.TroCap,0) as TienTroCap, tc.TuNgay as NgayTroCap, NgayCat, IsHangThang from tbl_ncc ncc
left join Tbl_HoatDong hd on ncc.Id=hd.Id_NCC and IsChet=1 and hd.Disabled=0
left join Tbl_TroCap tc on ncc.Id=tc.Id_NCC and tc.Disabled=0
left join Const_LoaiTroCap ltc on tc.Id_LoaiTroCap=ltc.Id
join DM_Wards xa on xa.RowID=ncc.Id_Xa
where ncc.disabled=0
;select de.Id_NCC as Id, SoTien,de.Id_NguonKinhPhi, Id_DotTangQua  , ncc.Id_DoiTuongNCC, ncc.Id_Xa, ncc.Id_KhomAp, xa.DistrictID
from Vw_Detail de
join Tbl_DoiTuongNhanQua ncc on de.Id_NCC=ncc.Id
join DM_Wards xa on xa.RowID=ncc.Id_Xa
join Tbl_DotTangQua_Detail de1 on de1.Disabled=0 and de1.Id_DotTangQua=de.Id_Dot_Tang and de1.Id_DoiTuongNCC= ncc.Id_DoiTuongNCC
join Tbl_DotTangQua_Detail_Muc dem on dem.Id_Detail=de1.Id and dem.Id_NguonKinhPhi=de.Id_NguonKinhPhi
where (IdGiam is null or (IdGiam is not null and Id_Dot_Giam > Id_Dot_Tang)) and SoTien is not null and SoTien>0 
and Id_DeXuat not in (select Id from Tbl_DeXuatTangQua where Status<=0)
; select * from Tbl_DotTangQua where disabled=0";
            return cnn.CreateDataSet(sql);
        }

        /// <summary>
        /// get condition string from custom filter
        /// </summary>
        /// <param name="cnn">current connection</param>
        /// <param name="id_filter">id_row of filter</param>
        /// <returns>condition string or empty string if filter is not exist</returns>
        public static string genStringWhere(DpsConnection cnn, string id_filter)
        {
            string re = "";
            string sql = @"select k.*, d.value as fvalue, d.operator from we_filter f join we_filter_detail d  on f.id_row=d.id_filter
join we_filter_key k on k.id_row=d.id_key
where f.disabled=0 and f.id_row = " + id_filter;
            DataTable dtKey = cnn.CreateDataTable(sql);
            if (dtKey != null && dtKey.Rows.Count > 0)
            {
                foreach (DataRow dr in dtKey.Rows)
                {
                    string id_row = dr["id_row"].ToString();
                    //                    if (id_row == "1")//loại công việc
                    //                    {
                    //                        if (dr["fvalue"].ToString() == "1")//tôi được giao
                    //                            re += " and w.id_nv=@iduser";
                    //                        if (dr["fvalue"].ToString() == "2")//cv tôi tạo
                    //                            re += " and w.createdby=@iduser";
                    //                        if (dr["fvalue"].ToString() == "3")//cv trong project/team tôi quản lý
                    //                            re += " and w.id_project_team in (select id_project_team from we_project_team_user where disabled=0 and admin=1 and id_user=@iduser)";
                    //                        continue;
                    //                    }
                    //                    if (id_row == "12")//tag
                    //                    {
                    //                        re += @" and w.id_row in (select distinct id_work from we_filter_tag wt 
                    //join we_tag t on t.id_row = wt.id_tag
                    //join we_project_team_user u on t.id_project_team = u.id_project_team
                    //where wt.disabled = 0 and t.disabled = 0 and id_user = @iduser and t.title like '%" + dr["fvalue"].ToString() + "%')";
                    //                        continue;
                    //                    }

                    if (dr["loai"].ToString() == "2")
                        re += string.Format(" and {0} like '%{1}%'", dr["value"], dr["fvalue"]);
                    else
                    {
                        if (dr["loai"].ToString() == "4")//dr["value"] là chuỗi format
                            re += string.Format(dr["value"].ToString(), dr["fvalue"]);
                        else
                        {
                            //3,5,7 kiểu datetime
                            if (id_row == "3" || id_row == "5" || id_row == "7")
                                re += string.Format(" and {0} {1} '{2}'", dr["value"], dr["operator"], dr["fvalue"]);
                            else
                                re += string.Format(" and {0} {1} {2}", dr["value"], dr["operator"], dr["fvalue"]);
                        }
                    }
                }
            }
            return re;
        }

        private object getOption(string v)
        {
            if (string.IsNullOrEmpty(v))
                return null;
            v = v.Replace("@idTinh", _config.IdTinh);
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(v);
                if (cnn.LastError != null || dt == null)
                    return null;
                cnn.Disconnect();
                return from r in dt.AsEnumerable()
                       orderby r["title"]
                       select new
                       {
                           id = r["id"],
                           title = r["title"],
                       };
            }
        }

        private object getOperator(string v)
        {
            List<object> re = new List<object>();
            switch (v)
            {
                case "2":
                    re.Add(new { id = "like", title = "Chứa" });
                    break;
                case "3":
                    re.Add(new { id = "=", title = "Bằng" });
                    re.Add(new { id = "<>", title = "Khác" });
                    re.Add(new { id = ">", title = "Lớn hơn" });
                    re.Add(new { id = "<", title = "Nhỏ hơn" });
                    break;
                default:
                    re.Add(new { id = "=", title = "Bằng" });
                    break;
            }
            return re;
        }
    }
}
