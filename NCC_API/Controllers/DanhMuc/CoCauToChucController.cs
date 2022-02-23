using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DocumentFormat.OpenXml.Wordprocessing;
using DpsLibs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Timensit_API.Controllers.Common;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Timensit_API.Models.DanhMuc;
using Timensit_API.Models.Process;
using Newtonsoft.Json;

namespace Timensit_API.Controllers.DanhMuc
{
    [ApiController]
    [Route("api/co-cau-to-chuc")]
    [EnableCors("TimensitPolicy")]
    public class CoCauToChucController : ControllerBase
    {

        private readonly NGUOICOCONGContext _context;
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Cơ cấu tổ chức";
        public CoCauToChucController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            DbContextOptions<NGUOICOCONGContext> options = new DbContextOptions<NGUOICOCONGContext>();
            _context = new NGUOICOCONGContext(options);
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 3);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "dps_user",
                PKColumn = "idchucvu",
                DisabledColumn = "deleted",
                name = "Nhân viên"
            });
        }
        /// <summary>
        /// Load cơ cấu tổ chức---
        /// !Visible: chỉ xem, visible tất cả các button thao tác
        /// </summary>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [HttpGet]
        public object List()
        {
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string select = "";
            SqlConditions Conds = new SqlConditions();

            bool Visible = false;

            DataTable result = new DataTable();
            result.Columns.Add("RowID");
            result.Columns.Add("Code");
            result.Columns.Add("Title");
            result.Columns.Add("Level");
            result.Columns.Add("ParentID");
            result.Columns.Add("Position");
            result.Columns.Add("WorkingModeID");
            result.Columns.Add("Children", typeof(DataTable));
            result.Columns.Add("Loaidonvi");
            result.Columns.Add("CapCoCau");
            result.Columns.Add("ID_Goc");
            DataTable dt = new DataTable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                #region
                if (User.IsInRole("33"))
                    Visible = true;
                #endregion

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    select = $@"select rowid, capcocau,title,parentid,vitri,code, chedolamviec, Loaidonvi, capcocau, ID_Goc from tbl_cocautochuc where disable=0 order by vitri, rowid";
                    dt = cnn.CreateDataTable(select);
                }
                var p = dt.AsEnumerable().Where(x => x["parentid"] == DBNull.Value);
                foreach (DataRow dr in p)
                {
                    result.Rows.Add(new object[] { dr["rowid"], dr["Code"], dr["title"], dr["capcocau"], dr["parentid"], dr["vitri"], dr["chedolamviec"], addrow(dr["rowid"].ToString(), dt), dr["Loaidonvi"], dr["CapCoCau"], dr["ID_Goc"] });
                }

                var data = from r in result.AsEnumerable()
                           select new
                           {
                               RowID = r["RowID"],
                               Code = r["Code"],
                               Title = r["Title"],
                               Level = r["Level"],
                               ParentID = r["ParentID"],
                               Position = r["Position"],
                               WorkingModeID = r["WorkingModeID"],
                               Children = r["Children"],
                               Loaidonvi = r["Loaidonvi"],
                               CapCoCau = r["CapCoCau"],
                               ID_Goc = r["ID_Goc"],
                           };
                logHelper.LogXemDS(loginData.Id);
                return JsonResultCommon.ThanhCong(data, null, Visible);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Selected Node Changed
        /// </summary>
        /// <param name="id"> Value Node Selected</param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [HttpGet("{id}")]
        public object Detail(string id)
        {
            string select = "";
            SqlConditions Conds = new SqlConditions();

            bool Visible = false;
            string macd = ""; string sonhanvien = ""; string vitri = ""; string id_chucdanh = ""; string id_chucvu = ""; string tenchucvu = ""; string tentienganh = ""; string id_donvi = ""; string id_phongban = ""; string id_cap = ""; string nodevalue = "";
            bool hienthidonvi = false; bool dungchuyencap = false; bool hienthicap = false; bool hienthiphongban = false; long ID_Goc = 0;
            DataTable tmp = new DataTable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                #region
                if (User.IsInRole("33"))
                    Visible = true;
                #endregion

                select = "select tbl_chucdanh.*, bophan.madv from tbl_chucdanh left join bophan on tbl_chucdanh.id_bp=bophan.id_bp where id_row=" + id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(select);
                    select = "select id_row, tenchucdanh,tbl_chucdanh.tentienganh, vitri, tbl_chucdanh.id_parent, visiblecap,visiblebophan, donvi.cap, donvi.tendonvi, tbl_chucdanh.isleading,bophan.tenbp, DM_Capquanly.Title, tbl_chucdanh.Id_Capquanly from tbl_chucdanh inner join bophan on tbl_chucdanh.id_bp=bophan.id_bp inner join donvi on bophan.madv=donvi.id_dv left join DM_Capquanly on tbl_chucdanh.Id_Capquanly=DM_Capquanly.RowID where tbl_chucdanh.disable=0 order by vitri";
                    DataTable dt_chucvu = cnn.CreateDataTable(select);
                    if (dt.Rows.Count > 0)
                    {
                        macd = dt.Rows[0]["macd"].ToString();
                        sonhanvien = dt.Rows[0]["sonhanviencan"].ToString();
                        vitri = dt.Rows[0]["vitri"].ToString();
                        hienthidonvi = (bool)dt.Rows[0]["IsLeading"];
                        if (bool.TrueString.Equals(dt.Rows[0]["IsStop"].ToString())) dungchuyencap = true;
                        else dungchuyencap = false;
                        if (bool.TrueString.Equals(dt.Rows[0]["Visiblecap"].ToString())) hienthicap = true;
                        else hienthicap = false;
                        hienthiphongban = bool.TrueString.Equals(dt.Rows[0]["Visiblebophan"].ToString());
                        id_donvi = dt.Rows[0]["madv"].ToString();
                        id_phongban = dt.Rows[0]["id_bp"].ToString();
                        id_chucdanh = dt.Rows[0]["id_cv"].ToString();
                        id_chucvu = dt.Rows[0]["nhom"].ToString();
                        id_cap = dt.Rows[0]["Id_Capquanly"].ToString();
                        tenchucvu = dt.Rows[0]["tenchucdanh"].ToString();
                        tentienganh = dt.Rows[0]["tentienganh"].ToString();
                        nodevalue = id;
                        ID_Goc = (long)dt.Rows[0]["ID_Goc"];
                    }

                    logHelper.LogXemCT(long.Parse(id), loginData.Id);
                    return JsonResultCommon.ThanhCong(new
                    {
                        status = 1,
                        MaCD = macd,
                        SoNhanVien = sonhanvien,
                        ViTri = vitri,
                        ID_ChucDanh = id_chucdanh,
                        ID_ChucVu = id_chucvu,
                        TenChucVu = tenchucvu,
                        TenTiengAnh = tentienganh,
                        ID_DonVi = id_donvi,
                        ID_PhongBan = id_phongban,
                        ID_Cap = id_cap,
                        HienThiDonVi = hienthidonvi,
                        DungChuyenCap = dungchuyencap,
                        HienThiCap = hienthicap,
                        HienThiPhongBan = hienthiphongban,
                        ID = nodevalue,
                        ID_Goc = ID_Goc,
                        Visible = Visible,
                    });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Add Node
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [HttpPost]
        public async Task<BaseModel<object>> Insert([FromBody] CoCauToChucModel data)
        {

            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            DataTable dt = new DataTable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    val.Add("capcocau", data.Level);
                    val.Add("title", data.Title);
                    val.Add("code", data.Code);
                    val.Add("vitri", data.Position);
                    if (data.ID_Goc >= 0)
                    {
                        val.Add("ID_Goc", data.ID_Goc);
                        if (data.Level == 2)
                            val.Add("Type", "H");
                        if (data.Level == 3)
                            val.Add("Type", "X");
                    }
                    if (data.WorkingModeID >= 0)
                        val.Add("chedolamviec", data.WorkingModeID);
                    string sqlq = "select ISNULL((select count(*) from tbl_cocautochuc where (code = '" + data.Code + "' or title ='" + data.Title + "') and disable=0 ),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) == 1)
                        return JsonResultCommon.Custom("Tên hoặc mã cơ cấu đã tồn tại");

                    val.Add("parentid", data.ParentID);
                    val.Add("createdby", loginData.Id);
                    val.Add("createddate", DateTime.Now);
                    if (cnn.Insert(val, "tbl_cocautochuc") != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    dt = cnn.CreateDataTable("select max(rowid) from tbl_cocautochuc");
                    data.RowID = int.Parse(dt.Rows[0][0].ToString());
                }

                logHelper.Ghilogfile<CoCauToChucModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.RowID, loginData.Id));
                return JsonResultCommon.ThanhCong(data);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Add Node
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [HttpPut("{id}")]
        public async Task<BaseModel<object>> Update(long id, [FromBody] CoCauToChucModel data)
        {

            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            DataTable dt = new DataTable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    val.Add("capcocau", data.Level);
                    val.Add("title", data.Title);
                    val.Add("code", data.Code);
                    val.Add("vitri", data.Position);
                    if (data.ID_Goc >= 0)
                        val.Add("ID_Goc", data.ID_Goc);
                    else
                    {
                        val.Add("ID_Goc", DBNull.Value);
                        val.Add("Type", DBNull.Value);
                    }
                    if (data.WorkingModeID >= 0)
                        val.Add("chedolamviec", data.WorkingModeID);
                    else
                        val.Add("chedolamviec", DBNull.Value);
                    string sqlq = "select ISNULL((select count(*) from tbl_cocautochuc where (code = '" + data.Code + "' or title = '" + data.Title + "') and disable=0  and rowid <> " + data.RowID + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) == 1)
                        return JsonResultCommon.Custom("Tên hoặc mã cơ cấu đã tồn tại");
                    string s = "select capcocau,title,vitri,parentid from tbl_cocautochuc where (where)";
                    Conds.Add("rowid", data.RowID);
                    DataTable old = cnn.CreateDataTable(s, "(where)", Conds);
                    val.Add("lastmodified", DateTime.Now);
                    val.Add("modifiedby", loginData.Id);
                    if (cnn.Update(val, Conds, "tbl_cocautochuc") != 1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }

                logHelper.Ghilogfile<CoCauToChucModel>(data, loginData, "Cập nhật", logHelper.LogSua(data.RowID, loginData.Id));
                return JsonResultCommon.ThanhCong(data);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }


        /// <summary>
        /// Delete Node
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [HttpDelete("{id}")]
        public BaseModel<object> DeleteNode(long id)
        {

            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            string loi = "";
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                DataTable dt = new DataTable();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    dt = cnn.CreateDataTable("select rowid, parentid, title from tbl_cocautochuc");
                    var find = dt.Select("rowid=" + id);
                    if (find.Length == 0)
                        return JsonResultCommon.KhongTonTai("Cơ cấu tổ chức");
                    DataRow drF = find[0];
                    SqlConditions cond = new SqlConditions();
                    cond.Add("deleted", 0);
                    DataTable nhanvien = cnn.CreateDataTable("select UserID as id_nv, IdDonVi as cocauid from Dps_User where (where)", "(where)", cond);
                    DataTable dtchucdanh = cnn.CreateDataTable("select cocauid, tenchucdanh from tbl_chucdanh where disable=0");
                    if (!checkmenucon(id.ToString(), dt, nhanvien, dtchucdanh, cnn, out loi))
                        return JsonResultCommon.Custom(loi);
                    if (!xoamenucon(id.ToString(), cnn))
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var data = new LiteModel() { id = id, title = drF["title"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id));
                    return JsonResultCommon.ThanhCong(true);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }
        #region khác
        /// <summary>
        /// Di chuyển
        /// </summary>
        /// <param name="idfrom">ID node cũ</param>
        /// <param name="namefrom">Name node cũ</param>
        /// <param name="idto">ID node mới</param>
        /// <param name="level">Số lượng con +1</param>
        /// <param name="nameto">Name node mới</param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [Route("handleDropOrgChart")]
        [HttpPost]
        public BaseModel<object> handleDropOrgChart(long idfrom, string namefrom, long idto, long level, string nameto)
        {

            BaseModel<bool> model = new BaseModel<bool>();
            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                if (idfrom == idto || idto == 0)
                    return JsonResultCommon.ThanhCong(true);
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    val.Add("parentid", idto);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("ModifiedBy", loginData.Id);
                    val.Add("vitri", level);
                    Conds = new SqlConditions();
                    Conds.Add("rowid", idfrom);
                    int result = cnn.Update(val, Conds, "tbl_cocautochuc");
                    if (result <= 0)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }
                var data = new LiteModel() { id = idfrom, data = new { parentid = idto, vitri = level } };
                logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật", logHelper.LogSua(idfrom, loginData.Id));
                return JsonResultCommon.ThanhCong(true);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }

        /// <summary>
        /// Thay đổi vị trí
        /// </summary>
        /// <param name="idfrom">ID node cũ</param>
        /// <param name="namefrom">Name node cũ</param>
        /// <param name="idto">ID node mới</param>
        /// <param name="nameto">Name node mới</param>
        /// <returns></returns>
        [Authorize(Roles = "31")]
        [Route("handleDropOrgChartNew")]
        [HttpPost]
        public BaseModel<object> handleDropOrgChart(long idfrom, string namefrom, long idto, string nameto, bool IsAbove)
        {
            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            int vitri = 0;
            int vitri1 = 0;
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                if (idfrom == idto || idto == 0)
                    return JsonResultCommon.ThanhCong(true);
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable($@"select parentid, vitri from tbl_cocautochuc where rowid={idto} ");
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    if (dt.Rows[0]["parentid"].ToString().Equals("0"))
                        return JsonResultCommon.ThanhCong(true);
                    int.TryParse(dt.Rows[0]["vitri"].ToString(), out vitri);
                    if (IsAbove)
                    {
                        vitri1 = vitri;
                        DataTable dt_ = cnn.CreateDataTable($@"select vitri from tbl_cocautochuc where rowid<{idto} and vitri={vitri}  and parentid={dt.Rows[0]["parentid"]}");
                        if (dt_.Rows.Count > 0) vitri1 = vitri + 1;

                        val.Add("parentid", dt.Rows[0]["parentid"]);
                        val.Add("LastModified", DateTime.Now);
                        val.Add("ModifiedBy", loginData.Id);
                        val.Add("vitri", vitri1);
                        Conds = new SqlConditions();
                        Conds.Add("rowid", idfrom);
                        int result = cnn.Update(val, Conds, "tbl_cocautochuc");
                        if (result <= 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        cnn.ExecuteNonQuery($"update tbl_cocautochuc set vitri=vitri+{(vitri == vitri1 ? 1 : 2)} where (vitri>={vitri1}) and rowid <> {idfrom} and rowid <> {idto}  and parentid={dt.Rows[0]["parentid"]}");
                        cnn.ExecuteNonQuery($"update tbl_cocautochuc set vitri=vitri+{(vitri == vitri1 ? 1 : 2)} where vitri={vitri} and rowid>={idto} and rowid <> {idfrom} and parentid={dt.Rows[0]["parentid"]}");
                    }
                    else
                    {
                        vitri1 = vitri + 1;

                        val.Add("parentid", dt.Rows[0]["parentid"]);
                        val.Add("LastModified", DateTime.Now);
                        val.Add("ModifiedBy", loginData.Id);
                        val.Add("vitri", vitri1);
                        Conds = new SqlConditions();
                        Conds.Add("rowid", idfrom);
                        int result = cnn.Update(val, Conds, "tbl_cocautochuc");
                        if (result <= 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        cnn.ExecuteNonQuery($"update tbl_cocautochuc set vitri=vitri+1 where vitri>{vitri} and rowid>{idto} and rowid <> {idfrom} and parentid={dt.Rows[0]["parentid"]}");
                        cnn.ExecuteNonQuery($"update tbl_cocautochuc set vitri=vitri+2 where vitri={vitri} and rowid>{idto} and rowid <> {idfrom}  and parentid={dt.Rows[0]["parentid"]}");
                    }

                }

                var data = new LiteModel() { id = idfrom, data = new { parentid = idto, vitri = vitri1 } };
                logHelper.Ghilogfile<LiteModel>(data, loginData, "Cập nhật", logHelper.LogSua(idfrom, loginData.Id));
                return JsonResultCommon.ThanhCong(true);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }

        private DataTable addrow(string id_parent, DataTable chucvu)
        {
            Employee emp = new Employee(_context);
            StringCollection s = new StringCollection();
            DataTable result = new DataTable();
            result.Columns.Add("RowID");
            result.Columns.Add("Code");
            result.Columns.Add("Title");
            result.Columns.Add("Level");
            result.Columns.Add("Parentid");
            result.Columns.Add("Position");
            result.Columns.Add("WorkingModeID");
            result.Columns.Add("Children", typeof(DataTable));
            result.Columns.Add("Loaidonvi");
            result.Columns.Add("CapCoCau");
            result.Columns.Add("ID_Goc");
            DataRow[] row = chucvu.Select("parentid=" + id_parent, "vitri asc");
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                foreach (DataRow dr in row)
                {
                    result.Rows.Add(new object[] { dr["rowid"], dr["Code"], dr["title"], dr["capcocau"], dr["parentid"], dr["vitri"], dr["chedolamviec"], addrow(dr["rowid"].ToString(), chucvu), dr["Loaidonvi"], dr["CapCoCau"], dr["ID_Goc"] });
                }
            }
            return result;
        }
        private bool xoamenucon(string id_cv, DpsConnection cnn)
        {
            //Kiểm tra có nhân viên hay không
            SqlConditions cond = new SqlConditions();
            cond.Add("rowid", id_cv);
            Hashtable val = new Hashtable();
            val.Add("disable", 1);
            val.Add("LastModified", DateTime.Now);
            int rs = cnn.Update(val, cond, "tbl_cocautochuc");
            if (rs < 0)
            {
                return false;
            }
            return true;
        }
        private bool checkmenucon(string id_cv, DataTable chucvu, DataTable nhanvien, DataTable dtchucdanh, DpsConnection cnn, out string error)
        {
            string tmp_error = "";
            //Kiểm tra có nhân viên hay không
            DataRow[] rownhanvien = nhanvien.Select("cocauid=" + id_cv);
            if (rownhanvien.Length > 0)
            {
                DataRow[] chucdanh = chucvu.Select("rowid=" + id_cv);
                if (chucdanh.Length > 0)
                {
                    tmp_error = "Đã có " + rownhanvien.Length.ToString() + " nhân viên có Phòng ban/Bộ phận là " + chucdanh[0]["title"].ToString() + " nên không xóa được";
                }
                error = tmp_error;
                return false;
            }
            //Kiểm tra có chức vụ hay không
            DataRow[] rowphongban = dtchucdanh.Select("cocauid=" + id_cv);
            if (rowphongban.Length > 0)
            {
                DataRow[] chucdanh = chucvu.Select("rowid=" + id_cv);
                if (chucdanh.Length > 0)
                {
                    tmp_error = "Đã có " + rowphongban.Length.ToString() + " chức vụ có Phòng ban/Bộ phận là " + chucdanh[0]["title"].ToString() + " nên không xóa được";
                }
                error = tmp_error;
                return false;
            }
            DataRow[] row = chucvu.Select("parentid=" + id_cv);
            for (int i = 0; i < row.Length; i++)
            {
                if (!checkmenucon(row[i]["rowid"].ToString(), chucvu, nhanvien, dtchucdanh, cnn, out error))
                {
                    return false;
                }
            }
            error = tmp_error;
            return true;
        }
        #endregion

        [Route("co-cau-map")]
        [HttpGet]
        public object CoCauMap()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                SqlConditions conds = new SqlConditions() { { "IdTinh", _config.IdTinh } };
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = $@"select Id_row as id, ProvinceName as title, cc.RowID from DM_Provinces t
left join Tbl_Cocautochuc cc on cc.ID_Goc=t.Id_row and cc.Disable=0
where t.Disable=0 and Id_row=@IdTinh

select Id_row as id, DistrictName as title, cc.RowID from DM_District h 
left join Tbl_Cocautochuc cc on cc.ID_Goc=h.Id_row and cc.Disable=0
where ProvinceID=@IdTinh

select x.RowID as id, x.Title as title, cc.RowID, h.Id_row as IdHuyen from DM_Wards x
join DM_District h on x.DistrictID=h.Id_row
left join Tbl_Cocautochuc cc on cc.ID_Goc=x.RowID and cc.Disable=0
where ProvinceID=@IdTinh";
                    var ds = cnn.CreateDataSet(sqlq, conds);
                    if (cnn.LastError != null || ds == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var data = from t in ds.Tables[0].AsEnumerable()
                               select new
                               {
                                   value = t["id"],
                                   item = t["title"],
                                   selected = t["RowID"] != DBNull.Value,
                                   data = new
                                   {
                                       RowID = t["RowID"],
                                       ID_Goc = t["id"],
                                       Title = t["title"],
                                       Type = "T"
                                   },
                                   children = from h in ds.Tables[1].AsEnumerable()
                                              select new
                                              {
                                                  value = h["id"],
                                                  item = h["title"],
                                                  selected = h["RowID"] != DBNull.Value,
                                                  data = new
                                                  {
                                                      RowID = h["RowID"],
                                                      ID_Goc = h["id"],
                                                      Title = h["title"],
                                                      Type = "H",
                                                      DVHC_Cha = t["id"],
                                                      ParentID = t["RowID"]
                                                  },
                                                  children = from x in ds.Tables[2].AsEnumerable()
                                                             where x["IdHuyen"].ToString() == h["id"].ToString()
                                                             select new
                                                             {
                                                                 value = x["id"],
                                                                 item = x["title"],
                                                                 selected = x["RowID"] != DBNull.Value,
                                                                 data = new
                                                                 {
                                                                     RowID = x["RowID"],
                                                                     Title = x["title"],
                                                                     ID_Goc = x["id"],
                                                                     Type = "X",
                                                                     DVHC_Cha = h["id"],
                                                                     ParentID = h["RowID"]
                                                                 },
                                                             }
                                              }
                               };

                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }


        [Route("map")]
        [HttpPost]
        public object Map([FromBody] List<CoCauMapModel> data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string str = "select * from tbl_CoCauToChuc where Disable=1 and Type is not null";
                    DataTable dt = cnn.CreateDataTable(str);
                    var ccs = data.Where(x => x.RowID != null && x.RowID != 0).Select(x => x.RowID).ToList();
                    cnn.BeginTransaction();
                    string sqlD = "update Tbl_CoCauToChuc set disable=1, DeletedDate=getdate(), deletedby=" + loginData.Id + " where Type is not null and Type!='T'";
                    if (ccs.Count > 0)
                        sqlD += " and RowID not in (" + string.Join(",", ccs) + ")";
                    if (cnn.ExecuteNonQuery(sqlD) < 0)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = data.Where(cc => cc.RowID == null || cc.RowID == 0).OrderBy(x => x.Type);//huyện trước
                    foreach (var cc in temp)
                    {
                        int kq = 0;
                        Hashtable val = new Hashtable();
                        string s = "Type='" + cc.Type + "' and Id_Goc=" + cc.ID_Goc;
                        var find = dt.Select(s);
                        if (find.Count() > 0)//sử dụng lại cái đã map
                        {
                            val["Disable"] = 0;
                            val["DeletedBy"] = DBNull.Value;
                            val["DeletedDate"] = DBNull.Value;
                            kq = cnn.Update(val, new SqlConditions() { { "RowID", find[0]["RowID"] } }, "Tbl_CoCautoChuc");
                        }
                        else
                        {
                            if (cc.ParentID == null)
                            {
                                string strF = "select RowID from Tbl_CoCauToChuc where Disable=0 and ID_Goc=" + cc.DVHC_Cha;
                                var rowid = cnn.ExecuteScalar(strF);
                                if (rowid != null)
                                    cc.ParentID = long.Parse(rowid.ToString());
                            }
                            val["ParentID"] = cc.ParentID;
                            val["Title"] = cc.Title;
                            val["Capcocau"] = cc.Type == "X" ? 3 : 2;
                            val["Id_Goc"] = cc.ID_Goc;
                            val["Type"] = cc.Type;
                            val["CreatedBy"] = loginData.Id;
                            val["CreatedDate"] = DateTime.Now;
                            kq = cnn.Insert(val, "Tbl_CoCauToChuc");
                            cc.RowID = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Tbl_CoCautoChuc') AS Current_Identity; ").ToString());
                        }
                        if (kq <= 0)
                        {
                            cnn.RollbackTransaction();
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        }
                    }
                    cnn.EndTransaction();
                    string note = "Map với đơn vị hành chính";
                    logHelper.Ghilogfile<CoCauMapModel>(data.ToList(), loginData, note, logHelper.LogSuas(data.Select(x => x.RowID.Value).ToList(), loginData.Id, "", note));
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
    }
}
