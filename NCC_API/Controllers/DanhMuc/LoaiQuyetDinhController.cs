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
    /// </summary>
    [ApiController]
    [Route("api/loai-quyet-dinh")]
    [EnableCors("TimensitPolicy")]
    public class LoaiQuyetDinhController : Controller
    {
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Nhóm quyết định";
        List<CheckFKModel> FKs;
        public LoaiQuyetDinhController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment)
        {
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            FKs.Add(new CheckFKModel
            {
                TableName = "tbl_bieumau",
                PKColumn = "Id_LoaiQuyetDinh",
                DisabledColumn = "Disabled",
                name = "Biểu mẫu"
            });
        }

        /// <summary>
        /// Danh sách Danh mục khác
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        public BaseModel<object> DanhSach([FromQuery] QueryParams query)
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
                    string sqlq = @"select hs.*, u2.FullName as NguoiSua  from Const_LoaiQuyetDinh hs
left join Dps_User u2 on hs.UpdatedBy=u2.UserID
where hs.Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "LoaiQuyetDinh","LoaiQuyetDinh" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["LoaiQuyetDinh"]))
                    {
                        string keyword = query.filter["LoaiQuyetDinh"].ToLower();
                        temp = temp.Where(x => x["LoaiQuyetDinh"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    #endregion
                    bool Visible = true;
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

                                   LoaiQuyetDinh = r["LoaiQuyetDinh"],

                                   MoTa = r["MoTa"],

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
        [Authorize]
        [HttpGet("{id}")]
        public BaseModel<object> DetailItem(int id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"select * from Const_LoaiQuyetDinh where Disabled=0 and Id = @Id";
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

                                                           LoaiQuyetDinh = r["LoaiQuyetDinh"],

                                                           UpdatedBy = r["UpdatedBy"],

                                                           AllowEdit = true

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
        [Authorize]
        [HttpPost]
        public BaseModel<object> Create([FromBody] LoaiQuyetDinhModel data)
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
                if (string.IsNullOrEmpty(data.LoaiQuyetDinh))
                    strRe += (strRe == "" ? "" : ", ") + "tên nhóm quyết định";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select count(*) from Const_LoaiQuyetDinh where Disabled=0 and (LoaiQuyetDinh = @Name)";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { new SqlCondition("Name", data.LoaiQuyetDinh) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("LoaiQuyetDinh", data.LoaiQuyetDinh);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", loginData.Id);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "Const_LoaiQuyetDinh") == 1)
                    {
                        data.Id = int.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('Const_LoaiQuyetDinh') AS Current_Identity; ").ToString());
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<LoaiQuyetDinhModel>(data, loginData, "Thêm mới", logHelper.LogThem(data.Id, loginData.Id, Name), Name);
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

        [Authorize]
        [HttpPut("{id}")]
        public BaseModel<object> Update(int id, [FromBody] LoaiQuyetDinhModel data)
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
                if (string.IsNullOrEmpty(data.LoaiQuyetDinh))
                    strRe += (strRe == "" ? "" : ", ") + "Nhóm quyết định";
                if (!string.IsNullOrEmpty(strRe))
                    return JsonResultCommon.BatBuoc(strRe);

                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select * from Const_LoaiQuyetDinh where Disabled=0 and Id = @Id";
                    DataTable dtFind = cnn.CreateDataTable(sqlq, new SqlConditions { new SqlCondition("Id", id) });
                    if (dtFind == null || dtFind.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai(Name);
                    sqlq = "select count(*) from Const_LoaiQuyetDinh where Disabled=0 and (LoaiQuyetDinh = @Name) and Id <> @Id";
                    if ((int)cnn.ExecuteScalar(sqlq, new SqlConditions { { "Id", id }, new SqlCondition("Name", data.LoaiQuyetDinh) }) > 0)
                        return JsonResultCommon.Trung(Name);

                    Hashtable val = new Hashtable();
                    val.Add("LoaiQuyetDinh", data.LoaiQuyetDinh);
                    val.Add("MoTa", !string.IsNullOrEmpty(data.MoTa) ? data.MoTa : "");
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", loginData.Id);
                    cnn.BeginTransaction();
                    if (cnn.Update(val, new SqlConditions { { "Id", id } }, "Const_LoaiQuyetDinh") == 1)
                    {
                        cnn.EndTransaction();
                        logHelper.Ghilogfile<LoaiQuyetDinhModel>(data, loginData, "Cập nhật", logHelper.LogSua((int)id, loginData.Id, Name), Name);
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
        [Authorize]
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
                    string sqlq = "select * from Const_LoaiQuyetDinh where Disabled = 0 and Id = @Id";
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

                    sqlq = "update Const_LoaiQuyetDinh set Disabled = 1, UpdatedDate = getdate(), UpdatedBy = " + iduser + " where Id = '" + id + "'";
                    if (cnn.ExecuteNonQuery(sqlq) == 1)
                    {
                        var data = new LiteModel() { id = id, title = dtFind.Rows[0]["LoaiQuyetDinh"].ToString() };
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

    public class LoaiQuyetDinhModel
    {
        public long Id { get; set; }
        public string LoaiQuyetDinh { get; set; }
        public string MoTa { get; set; }
    }
}
