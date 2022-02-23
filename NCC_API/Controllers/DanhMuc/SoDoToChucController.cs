using System;
using System.Collections;
using System.Collections.Generic;
using System.Collections.Specialized;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
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
    /// <summary>
    /// sơ đồ tổ chức: tbl_chucdanh
    /// Quyền xem: 81	Quản lý sơ đồ tổ chức
    /// Quyền sửa: 82	Cập nhật sơ đồ tổ chức
    /// </summary>
    [ApiController]
    [Route("api/so-do-to-chuc")]
    [EnableCors("TimensitPolicy")]
    public class SoDoToChucController : ControllerBase
    {
        private readonly NGUOICOCONGContext _context;
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Sơ đồ tổ chức";
        public SoDoToChucController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            DbContextOptions<NGUOICOCONGContext> options = new DbContextOptions<NGUOICOCONGContext>();
            _context = new NGUOICOCONGContext(options);
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 6);
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
        /// Load sơ đồ tổ chức---
        /// !Visible: chỉ xem, visible tất cả các button thao tác
        /// </summary>
        /// <param name="jobtitleid">Tất cả: null</param>
        /// <returns></returns>
        [Authorize(Roles = "81")]
        [HttpGet]
        public object List(string jobtitleid)
        {
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string select = "";
            SqlConditions Conds = new SqlConditions();
            bool Visible = true;
            DataTable result = new DataTable();
            result.Columns.Add("ID");
            result.Columns.Add("Name");
            result.Columns.Add("StructureID");
            result.Columns.Add("ID_ChucDanh");
            result.Columns.Add("level");
            result.Columns.Add("level_jobtitle");
            result.Columns.Add("Data_Emp", typeof(DataTable));
            result.Columns.Add("SumCo");
            result.Columns.Add("SumCan");
            result.Columns.Add("SumCB");
            result.Columns.Add("SumKD");
            result.Columns.Add("chucdanhParent");
            result.Columns.Add("children", typeof(DataTable));

            DataTable tmp = new DataTable();
            tmp.Columns.Add("ID_NV");
            tmp.Columns.Add("Name");

            StringCollection s = new StringCollection();
            DataTable dt = new DataTable();
            DataTable nv = new DataTable();
            DataTable Tb = new DataTable();

            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                #region
                if (!User.IsInRole("82"))
                    Visible = false;
                #endregion

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    select = $@"select id_row, tbl_cocautochuc.code, tenchucdanh, tbl_chucdanh.vitri, tbl_chucdanh.id_parent, visiblecap, tbl_chucdanh.isleading,tbl_chucdanh.visiblebophan, DM_Capquanly.title,tbl_chucdanh.id_cv,chucvu.cap as leveljobtitle, capcocau as cap, cocauid, tbl_cocautochuc.title as cocau,tbl_cocautochuc.rowid
                        from tbl_chucdanh left join tbl_cocautochuc on tbl_cocautochuc.rowid = tbl_chucdanh.cocauid
                        left join dm_capquanly on tbl_chucdanh.Id_Capquanly=dm_capquanly.rowid join chucvu on tbl_chucdanh.id_cv = chucvu.id_cv
                        where tbl_chucdanh.disable=0 order by vitri";
                    dt = cnn.CreateDataTable(select);

                    nv = cnn.CreateDataTable(@"select u.UserID as id_nv, u.FullName as hoten, IdChucVu as id_chucdanh from Dps_User u where u.Deleted=0");

                    SqlConditions cond = new SqlConditions();
                    cond.Add("id_nv", loginData.Id);
                    Tb = cnn.CreateDataTable("select * from Vw_ChucDanh order by vitri");
                }

                Tb.Columns.Add("SumCB", typeof(decimal));
                Tb.Columns.Add("SumKD", typeof(decimal));

                string id_chucdanh = "";
                DataRow[] row = dt.Select();
                if (!string.IsNullOrEmpty(jobtitleid))
                {
                    id_chucdanh = jobtitleid;
                    row = dt.Select("id_row in (" + id_chucdanh + ")", "vitri asc");
                }
                else
                {
                    row = dt.Select("id_parent=0");

                }
                Employee emp = new Employee(_context);
                for (int i = 0; i < row.Length; i++)
                {
                    DataRow[] slnv = nv.Select("id_chucdanh='" + row[i]["id_row"].ToString() + "'");
                    string tenchucdanh = row[i]["tenchucdanh"].ToString();
                    string tendonvi = "";
                    string tencapquanly = "";
                    if (Boolean.TrueString.Equals(row[i]["isleading"].ToString())) tendonvi += " - <span class=\"red\">" + row[i]["code"].ToString() + " - " + row[i]["cocau"].ToString() + " (" + row[i]["rowid"].ToString() + ")</span>";
                    if (Boolean.TrueString.Equals(row[i]["visiblecap"].ToString())) tenchucdanh += " " + row[i]["cap"].ToString();
                    if (!"".Equals(row[i]["title"].ToString())) tencapquanly = " - " + row[i]["title"].ToString();

                    foreach (DataRow r in slnv)
                    {
                        tmp.Rows.Add(new object[] { r["id_nv"], r["hoten"] });
                    }
                    object sumCo = 0;
                    object sumCan = 0;

                    id_chucdanh = "";
                    s = emp.GetAllPosition(row[i]["id_row"].ToString());
                    foreach (string si in s)
                    {
                        id_chucdanh += "," + si;
                    }
                    if (!id_chucdanh.Equals(""))
                    {
                        id_chucdanh = id_chucdanh.Substring(1) + "," + row[i]["id_row"].ToString();
                        sumCo = Tb.Compute("sum(NvCo)", "Id in (" + id_chucdanh + ")");
                        sumCan = Tb.Compute("sum(NvCan)", "Id in (" + id_chucdanh + ")");
                    }
                    else
                    {
                        sumCo = Tb.Compute("sum(NvCo)", "Id in (" + row[i]["id_row"] + ")");
                        sumCan = Tb.Compute("sum(NvCan)", "Id in (" + row[i]["id_row"] + ")");
                    }
                    result.Rows.Add(new object[] { row[i]["id_row"].ToString(), (!string.IsNullOrEmpty(jobtitleid) ? tenchucdanh + " (" +row[i]["id_row"].ToString() +")" : tenchucdanh + " (" +row[i]["id_row"].ToString() +")" + tendonvi  + tencapquanly),
                            row[i]["cocauid"], row[i]["id_cv"],row[i]["vitri"],row[i]["leveljobtitle"],tmp,sumCo, sumCan,0,0,tenchucdanh, addrow(row[i]["id_row"].ToString(), dt, nv, Tb, jobtitleid) });
                }
                var test = JsonConvert.SerializeObject(result);
                var data = from r in result.AsEnumerable()
                           select new
                           {
                               ID = r["ID"],
                               Name = r["Name"],
                               StructureID = r["StructureID"],
                               ID_ChucDanh = r["ID_ChucDanh"],
                               level = r["level"],
                               level_jobtitle = r["level_jobtitle"],
                               Data_Emp = r["Data_Emp"],
                               SumCo = r["SumCo"],
                               SumCan = r["SumCan"],
                               SumCB = r["SumCB"],
                               SumKD = r["SumKD"],
                               chucdanhParent = r["chucdanhParent"],
                               children = r["children"]
                           };
                logHelper.LogXemDS(loginData.Id);
                return JsonResultCommon.ThanhCong(data, pageModel, Visible);
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
        [Authorize(Roles = "81")]
        [HttpGet("{id}")]
        public object Detail(string id)
        {
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            string select = "";
            SqlConditions Conds = new SqlConditions();
            bool Visible = true;
            string macd = ""; string sonhanvien = ""; string vitri = ""; string id_chucdanh = ""; string id_chucvu = ""; string tenchucvu = ""; string tentienganh = ""; string StructureID = ""; string id_cap = ""; string nodevalue = "";
            bool hienthidonvi = false; bool dungchuyencap = false; bool hienthicap = false; bool hienthiphongban = false; bool hienthiid = false;
            DataTable tmp = new DataTable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                #region
                if (!User.IsInRole("82"))
                    Visible = false;
                #endregion

                select = "select tbl_chucdanh.*, cocauid from tbl_chucdanh where id_row=" + id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(select);
                    select = "select id_row, tenchucdanh,tbl_chucdanh.tentienganh, tbl_chucdanh.vitri, tbl_chucdanh.id_parent, visiblecap" +
                        ",visiblebophan, tbl_cocautochuc.capcocau, tbl_cocautochuc.title" +
                        ", tbl_chucdanh.isleading, DM_Capquanly.Title" +
                        ", tbl_chucdanh.Id_Capquanly,VisibleID " +
                        "from tbl_chucdanh join tbl_cocautochuc " +
                        "on tbl_chucdanh.cocauid = tbl_cocautochuc.rowid " +
                        "left join DM_Capquanly on tbl_chucdanh.Id_Capquanly=DM_Capquanly.RowID " +
                        "where tbl_chucdanh.disable=0 order by vitri";
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
                        StructureID = dt.Rows[0]["cocauid"].ToString();
                        id_chucdanh = dt.Rows[0]["id_cv"].ToString();
                        id_chucvu = dt.Rows[0]["nhom"].ToString();
                        id_cap = dt.Rows[0]["Id_Capquanly"].ToString();
                        tenchucvu = dt.Rows[0]["tenchucdanh"].ToString();
                        tentienganh = dt.Rows[0]["tentienganh"].ToString();
                        nodevalue = id;
                        hienthiid = bool.TrueString.Equals(dt.Rows[0]["VisibleID"].ToString());
                    }

                    var data = new
                    {
                        status = 1,
                        MaCD = macd,
                        SoNhanVien = sonhanvien,
                        ViTri = vitri,
                        ID_ChucDanh = id_chucdanh,
                        ID_ChucVu = id_chucvu,
                        TenChucVu = tenchucvu,
                        TenTiengAnh = tentienganh,
                        StructureID = StructureID,
                        ID_Cap = id_cap,
                        HienThiDonVi = hienthidonvi,
                        DungChuyenCap = dungchuyencap,
                        HienThiCap = hienthicap,
                        HienThiPhongBan = hienthiphongban,
                        ID = nodevalue,
                        HienThiID = hienthiid,
                        Visible = Visible,
                    };
                    logHelper.LogXemCT(long.Parse(id), loginData.Id);
                    return JsonResultCommon.ThanhCong(data);
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
        [Authorize(Roles = "82")]
        [HttpPost]
        public async Task<BaseModel<object>> Insert([FromBody] OrgChartAddData data)
        {
            BaseModel<OrgChartAddData> model = new BaseModel<OrgChartAddData>();
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
                    dt = cnn.CreateDataTable("select count(id_row) from tbl_chucdanh");
                    int macv = 0;
                    if (dt.Rows.Count > 0)
                        macv = int.Parse(dt.Rows[0][0].ToString()) + 1;
                    val.Add("macd", "CV" + macv.ToString());
                    val.Add("tenchucdanh", "Chức vụ mới");
                    val.Add("Vitri", data.ViTri);
                    val.Add("id_bp", data.ID_PhongBan);
                    val.Add("id_cv", data.ID_ChucDanh);
                    val.Add("id_parent", data.ID);
                    val.Add("sonhanviencan", 1);
                    val.Add("Lastmodified", DateTime.Now);
                    if (data.StructureID != 0) val.Add("cocauid", data.StructureID);
                    if (cnn.Insert(val, "tbl_chucdanh") == 1)
                    {
                        string LogContent = "Thêm chức vụ mới là chức vụ con của (" + data.ID + ")";
                        dt = cnn.CreateDataTable("select max(id_row) from tbl_chucdanh");
                        cnn.Disconnect();
                        if (dt.Rows.Count <= 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        data.ID = int.Parse(dt.Rows[0][0].ToString());
                    }
                }
                logHelper.Ghilogfile<OrgChartAddData>(data, loginData, "Thêm mới", logHelper.LogThem(data.ID, loginData.Id));
                return JsonResultCommon.ThanhCong(data);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cập nhật thông tin chức vụ
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [Authorize(Roles = "82")]
        [HttpPut("{id}")]
        public async Task<BaseModel<object>> Update(long id, [FromBody] OrgChartUpdateData data)
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
                    string s = "select macd,tenchucdanh,sonhanviencan,id_bp,vitri,id_cv,nhom,isleading,isstop,visiblecap,visiblebophan from tbl_chucdanh where (where)";
                    val = new Hashtable();
                    //val.Add("macd", data.MaCD.ToUpper());
                    val.Add("tenchucdanh", data.TenChucVu);
                    val.Add("Sonhanviencan", data.SoNhanVien);
                    val.Add("id_bp", data.ID_PhongBan);
                    val.Add("vitri", data.ViTri);
                    val.Add("id_cv", data.ID_ChucDanh);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("Nguoisua", loginData.Id);
                    val.Add("Nhom", data.ID_ChucVu);
                    val.Add("IsLeading", data.HienThiDonVi);
                    val.Add("IsStop", data.DungChuyenCap);
                    if (data.HienThiCap != null)
                        val.Add("VisibleCap", data.HienThiCap);
                    if (data.HienThiPhongBan != null)
                        val.Add("visiblebophan", data.HienThiPhongBan);
                    val.Add("VisibleID", data.HienThiID);
                    //val.Add("tentienganh", data.TenTiengAnh);
                    if (data.ID_Cap == 0)
                        val.Add("Id_Capquanly", DBNull.Value);
                    else
                        val.Add("Id_Capquanly", data.ID_Cap);
                    if (data.StructureID != 0) val.Add("cocauid", data.StructureID);
                    //string donviid = General.GetQuyenDonViIDTheoNhanVien(loginData.Id, cnn);
                    ////JEECS
                    //if (!donviid.Equals("0"))
                    //{
                    //    string strCheck = "select count(*) from Tbl_chucdanh where disable=0 and donviid=" + donviid + " and tenchucdanh like N'" + data.TenChucVu + "'";
                    //    if (data.ID > 0)
                    //        strCheck += " and id_row<>" + data.ID;
                    //    else
                    //    {
                    //        val.Add("donviid", donviid);
                    //    }
                    //    if (int.Parse(cnn.ExecuteScalar(strCheck).ToString()) > 0)
                    //        return JsonResultCommon.Trung(Name);
                    //}
                    SqlConditions hdk = new SqlConditions();
                    hdk.Add("id_row", id);
                    DataTable old = cnn.CreateDataTable(s, "(where)", hdk);
                    if (id == 0)
                    {
                        val.Add("id_parent", data.ID_Parent);
                        if (cnn.Insert(val, "tbl_chucdanh") == 1)
                        {
                            dt = cnn.CreateDataTable("select max(id_row) from tbl_chucdanh");
                            if (dt.Rows.Count <= 0)
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                            data.ID = int.Parse(dt.Rows[0][0].ToString());
                        }
                    }
                    else
                    {
                        int rs = cnn.Update(val, hdk, "tbl_chucdanh");
                        if (-1 == rs)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                        //Cập nhật lại id_bp của nhân viên có chức danh này
                        Hashtable Val1 = new Hashtable();
                        Val1.Add("cocauid", data.StructureID);
                        SqlConditions cond = new SqlConditions();
                        cond.Add("id_chucdanh", data.ID);
                        rs = cnn.Update(Val1, cond, "tbl_nhanvien");
                    }
                    //Update lại vị trí các node sau
                    string update = "update tbl_chucdanh set vitri=vitri+1 where (id_row<>" + data.ID + ") and (vitri>=" + data.ViTri + ") and (id_parent=" + data.ID_Parent + ")";
                    if (cnn.ExecuteNonQuery(update) == -1)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }
                logHelper.Ghilogfile<OrgChartUpdateData>(data, loginData, "Cập nhật", logHelper.LogSua(id, loginData.Id));
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
        [Authorize(Roles = "82")]
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(long id)
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
                    dt = cnn.CreateDataTable("select id_row, id_parent, tenchucdanh from tbl_chucdanh");
                    var find = dt.Select("rowid=" + id);
                    if (find.Length == 0)
                        return JsonResultCommon.KhongTonTai("Cơ cấu tổ chức");
                    DataRow drF = find[0];
                    SqlConditions cond = new SqlConditions();
                    //cond.Add("thoiviec", false);
                    cond.Add("Deleted", 0);
                    DataTable nhanvien = cnn.CreateDataTable("select UserID as id_nv, IdChucVu as id_chucdanh from Dps_user where (where)", "(where)", cond);
                    if (!checkmenucon(id.ToString(), dt, nhanvien, cnn, out loi))
                        return JsonResultCommon.Custom(loi);
                    if (!xoamenucon(id.ToString(), cnn))
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var data = new LiteModel() { id = id, title = drF["tenchucdanh"].ToString() };
                    logHelper.Ghilogfile<LiteModel>(data, loginData, "Xóa", logHelper.LogXoa(id, loginData.Id));
                    return JsonResultCommon.ThanhCong(true);

                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }

        #region
        /// <summary>
        /// Di chuyển
        /// </summary>
        /// <param name="idfrom">ID node cũ</param>
        /// <param name="namefrom">Name node cũ</param>
        /// <param name="idto">ID node mới</param>
        /// <param name="level">Số lượng con +1</param>
        /// <param name="nameto">Name node mới</param>
        /// <returns></returns>
        [Authorize(Roles = "82")]
        [Route("handleDropOrgChart")]
        [HttpPost]
        public BaseModel<object> handleDropOrgChart(long idfrom, string namefrom, long idto, long level, string nameto)
        {
            BaseModel<bool> model = new BaseModel<bool>();
            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            Employee e = new Employee(_context);
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
                    cnn.BeginTransaction();
                    Conds.Add("id_nv", loginData.Id);
                    DataTable duocquyen = cnn.CreateDataTable("select id_chucdanh from P_Capnhatsodo where (where)", "(where)", Conds);
                    foreach (DataRow dr in duocquyen.Rows)
                    {
                        StringCollection list = e.GetAllPosition(dr[0].ToString());
                        list.Add(dr[0].ToString());
                        if (list.Contains(idfrom.ToString()))
                        {
                            Conds.Add("id_chucdanh", idfrom);
                            cnn.Delete(Conds, "P_Capnhatsodo");
                        }
                    }

                    val = new Hashtable();
                    val.Add("id_parent", idto);
                    val.Add("LastModified", DateTime.Now);
                    val.Add("nguoisua", loginData.Id);
                    val.Add("vitri", level);
                    Conds = new SqlConditions();
                    Conds.Add("id_row", idfrom);
                    int result = cnn.Update(val, Conds, "tbl_chucdanh");
                    if (result <= 0)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    cnn.EndTransaction();
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
        /// <param name="IsAbove">1: phía trên; 0 phía dưới</param>
        /// <returns></returns>
        [Authorize(Roles = "82")]
        [Route("handleDropOrgChartNew")]
        [HttpPost]
        public BaseModel<object> handleDropOrgChart(long idfrom, string namefrom, long idto, string nameto, bool IsAbove)
        {
            BaseModel<bool> model = new BaseModel<bool>();
            SqlConditions Conds = new SqlConditions();
            Hashtable val = new Hashtable();
            int vitri = 0;
            int vitri1 = 0;
            Employee e = new Employee(_context);
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                if (idfrom == idto || idto == 0)
                    return JsonResultCommon.ThanhCong();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable($@"select id_parent, vitri from tbl_chucdanh where Id_row={idto}");
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);

                    cnn.BeginTransaction();
                    Conds.Add("id_nv", loginData.Id);
                    DataTable duocquyen = cnn.CreateDataTable("select id_chucdanh from P_Capnhatsodo where (where)", "(where)", Conds);
                    foreach (DataRow dr in duocquyen.Rows)
                    {
                        StringCollection list = e.GetAllPosition(dr[0].ToString());
                        list.Add(dr[0].ToString());
                        if (list.Contains(idfrom.ToString()))
                        {
                            if (dt.Rows[0]["id_parent"].ToString().Equals("0"))
                            {
                                val.Add("id_chucdanh", idfrom);
                                val.Add("id_nv", loginData.Id);
                                val.Add("LastModified", DateTime.Now);
                                val.Add("UserModified", loginData.Id);
                                cnn.Insert(val, "P_Capnhatsodo");
                            }
                            else
                            {
                                Conds.Add("id_chucdanh", idfrom);
                                cnn.Delete(Conds, "P_Capnhatsodo");
                            }
                        }
                    }

                    int.TryParse(dt.Rows[0]["vitri"].ToString(), out vitri);
                    if (IsAbove)
                    {
                        vitri1 = vitri;
                        DataTable dt_ = cnn.CreateDataTable($@"select vitri from tbl_chucdanh where Id_row<{idto} and vitri={vitri} and id_parent={dt.Rows[0]["id_parent"]}");
                        if (dt_.Rows.Count > 0) vitri1 = vitri + 1;
                        val = new Hashtable();
                        val.Add("id_parent", dt.Rows[0]["id_parent"]);
                        val.Add("LastModified", DateTime.Now);
                        val.Add("nguoisua", loginData.Id);
                        val.Add("vitri", vitri1);
                        Conds = new SqlConditions();
                        Conds.Add("Id_row", idfrom);
                        int result = cnn.Update(val, Conds, "tbl_chucdanh");
                        if (result <= 0)
                            return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        cnn.ExecuteNonQuery($"update tbl_chucdanh set vitri=vitri+{(vitri == vitri1 ? 1 : 2)} where (vitri>={vitri1}) and Id_row <> {idfrom} and Id_row <> {idto} and id_parent={dt.Rows[0]["id_parent"]}");
                        cnn.ExecuteNonQuery($"update tbl_chucdanh set vitri=vitri+{(vitri == vitri1 ? 1 : 2)} where vitri={vitri} and Id_row>={idto} and Id_row <> {idfrom} and id_parent={dt.Rows[0]["id_parent"]}");
                    }
                    else
                    {
                        vitri1 = vitri + 1;
                        val = new Hashtable();
                        val.Add("id_parent", dt.Rows[0]["id_parent"]);
                        val.Add("LastModified", DateTime.Now);
                        val.Add("nguoisua", loginData.Id);
                        val.Add("vitri", vitri1);
                        Conds = new SqlConditions();
                        Conds.Add("Id_row", idfrom);
                        int result = cnn.Update(val, Conds, "tbl_chucdanh");
                        if (result <= 0)
                            if (result <= 0)
                                return JsonResultCommon.Exception(cnn.LastError, ControllerContext);

                        cnn.ExecuteNonQuery($"update tbl_chucdanh set vitri=vitri+1 where vitri>{vitri} and Id_row>{idto} and Id_row <> {idfrom} and id_parent={dt.Rows[0]["id_parent"]}");
                        cnn.ExecuteNonQuery($"update tbl_chucdanh set vitri=vitri+2 where vitri={vitri} and Id_row>{idto} and Id_row <> {idfrom} and id_parent={dt.Rows[0]["id_parent"]}");
                    }
                    cnn.EndTransaction();
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

        /// <summary>
        /// Di chuyển nhân viên từ vị trí này sang vị trí khác/phòng ban khác
        /// </summary>
        /// <param name="id_nv">ID nhân viên</param>
        /// <param name="id_chucdanhmoi">Chức danh mới</param>
        /// <returns></returns>
        [Authorize(Roles = "82")]
        [Route("handleDropStaff")]
        [HttpPost]
        public BaseModel<object> handleDropStaff(string id_nv, string id_chucdanhmoi)
        {
            SqlConditions Conds = new SqlConditions();
            Hashtable Val = new Hashtable();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    Conds = new SqlConditions();
                    Conds.Add("UserID", id_nv);
                    Conds.Add("IdChucVu", id_chucdanhmoi, SqlOperator.NotEquals);
                    Val = new System.Collections.Hashtable();
                    Val.Add("IdChucVu", id_chucdanhmoi);
                    Val.Add("UpdatedDate", DateTime.Now);
                    Val.Add("UpdatedBy", loginData.Id);
                    if (cnn.Update(Val, Conds, "Dps_User") > 0)
                    {
                        return JsonResultCommon.ThanhCong(true);
                    }
                    else
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }

        private int CountChild(string id_node, DataTable chucvu)
        {
            DataRow[] row = chucvu.Select("id_parent=" + id_node);
            return row.Length;
        }
        private DataTable addrow(string id_parent, DataTable chucvu, DataTable nv, DataTable Tb, string jobtitleid)
        {
            Employee emp = new Employee(_context);
            StringCollection s = new StringCollection();
            DataTable result = new DataTable();
            result.Columns.Add("ID");
            result.Columns.Add("Name");
            result.Columns.Add("StructureID");
            result.Columns.Add("ID_ChucDanh");
            result.Columns.Add("level");
            result.Columns.Add("level_jobtitle");
            result.Columns.Add("Data_Emp", typeof(DataTable));
            result.Columns.Add("SumCo");
            result.Columns.Add("SumCan");
            result.Columns.Add("SumCB");
            result.Columns.Add("SumKD");
            result.Columns.Add("chucdanhParent");
            result.Columns.Add("children", typeof(DataTable));

            DataTable tmp = new DataTable();

            object sumCo = 0;
            object sumCan = 0;

            DataRow[] row = chucvu.Select("id_parent=" + id_parent, "vitri asc");
            for (int i = 0; i < row.Length; i++)
            {
                tmp = new DataTable();
                tmp.Columns.Add("ID_NV");
                tmp.Columns.Add("Name");
                long sss = long.Parse(row[i]["id_row"].ToString());
                DataRow[] slnv = nv.Select("id_chucdanh='" + sss + "'");
                int sonodecon = CountChild(row[i]["id_row"].ToString(), chucvu);
                string tenchucdanh = row[i]["tenchucdanh"].ToString();
                string tenchucdanh_tmp = row[i]["tenchucdanh"].ToString();
                string tendonvi = "";
                string tencapquanly = "";
                if (Boolean.TrueString.Equals(row[i]["isleading"].ToString())) tendonvi += " - <span class=\"red\">" + row[i]["code"].ToString() + " - " + row[i]["cocau"].ToString() + " (" + row[i]["rowid"].ToString() + ")</span>";
                if (Boolean.TrueString.Equals(row[i]["visiblecap"].ToString())) tenchucdanh += " " + row[i]["cap"].ToString();
                if (!"".Equals(row[i]["title"].ToString())) tencapquanly = " - <span class=\"blue\">" + row[i]["title"].ToString() + "</span>";

                foreach (DataRow r in slnv)
                {
                    tmp.Rows.Add(new object[] { r["id_nv"], r["hoten"] });
                }
                string id_chucdanh = "";
                s = emp.GetAllPosition(row[i]["id_row"].ToString());
                foreach (string si in s)
                {
                    id_chucdanh += "," + si;
                }
                if (!id_chucdanh.Equals(""))
                {
                    id_chucdanh = id_chucdanh.Substring(1) + "," + row[i]["id_row"].ToString();
                    sumCo = Tb.Compute("sum(NvCo)", "Id in (" + id_chucdanh + ")");
                    sumCan = Tb.Compute("sum(NvCan)", "Id in (" + id_chucdanh + ")");
                }
                else
                {
                    sumCo = Tb.Compute("sum(NvCo)", "Id in (" + row[i]["id_row"] + ")");
                    sumCan = Tb.Compute("sum(NvCan)", "Id in (" + row[i]["id_row"] + ")");
                }
                result.Rows.Add(new object[] { row[i]["id_row"].ToString(), (!string.IsNullOrEmpty(jobtitleid) ? tenchucdanh + " (" +row[i]["id_row"].ToString() +")" : tenchucdanh + " (" +row[i]["id_row"].ToString() +")" + tendonvi  + tencapquanly),
                            row[i]["cocauid"], row[i]["id_cv"],row[i]["vitri"],row[i]["leveljobtitle"],tmp,sumCo, sumCan,0,0,tenchucdanh, addrow(row[i]["id_row"].ToString(), chucvu, nv, Tb, jobtitleid) });

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    cnn.ExecuteNonQuery("update tbl_chucdanh set vitri=" + (i + 1).ToString() + " where id_row=" + row[i]["id_row"].ToString());
                }
            }
            return result;
        }
        private bool xoamenucon(string id_cv, DpsConnection cnn)
        {
            Employee emp = new Employee(_context);
            //Xóa chức vụ chọn
            SqlConditions cond = new SqlConditions();
            cond.Add("id_row", id_cv);
            Hashtable val = new Hashtable();
            val.Add("disable", 1);
            val.Add("LastModified", DateTime.Now);
            int rs = cnn.Update(val, cond, "tbl_chucdanh");
            cnn.BeginTransaction();
            if (rs <= 0)
            {
                return false;
            }
            //Xóa tất cả chức vụ bên dưới chức vụ được chọn
            StringCollection list = emp.GetAllPosition(id_cv);
            cond = new SqlConditions();
            cond.Add("id_row", list, SqlOperator.In);
            val = new Hashtable();
            val.Add("disable", 1);
            val.Add("LastModified", DateTime.Now);
            rs = cnn.Update(val, cond, "tbl_chucdanh");
            if (rs < 0)
            {
                cnn.RollbackTransaction();
                cnn.Disconnect();
                return false;
            }
            cnn.EndTransaction();
            return true;
        }
        private bool checkmenucon(string id_cv, DataTable chucvu, DataTable nhanvien, DpsConnection cnn, out string error)
        {
            string tmp_error = "";
            //Kiểm tra có nhân viên hay không
            DataRow[] rownhanvien = nhanvien.Select("id_chucdanh=" + id_cv);
            if (rownhanvien.Length > 0)
            {
                DataRow[] chucdanh = chucvu.Select("id_row=" + id_cv);
                if (chucdanh.Length > 0)
                {
                    tmp_error = "Đã có " + rownhanvien.Length.ToString() + " nhân viên có chức danh là " + chucdanh[0]["tenchucdanh"].ToString() + " nên không xóa được";
                }
                error = tmp_error;
                return false;
            }
            DataRow[] row = chucvu.Select("id_parent=" + id_cv);
            for (int i = 0; i < row.Length; i++)
            {
                if (!checkmenucon(row[i]["id_row"].ToString(), chucvu, nhanvien, cnn, out error))
                {
                    return false;
                }
            }
            error = tmp_error;
            return true;
        }
        #endregion
    }
}
