extern alias iText;

using DocumentFormat.OpenXml.VariantTypes;
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
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.Options;
using SignalRChat.Hubs;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.IO;
using System.IO.Compression;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Security.Cryptography;
using System.Security.Cryptography.X509Certificates;
using System.Text;
using System.Threading.Tasks;
using WebCore_API.Models;
using System.ComponentModel;
using Timensit_API.Controllers.DanhMuc;

namespace Timensit_API.Controllers.Common
{
    [ApiController]
    [Route("api/lite")]
    [EnableCors("TimensitPolicy")]
    public class LiteController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        UserManager dpsUserMr;
        LoginController lc;
        private NCCConfig _config;
        private IOptions<NCCConfig> _configLogin;
        private IHubContext<ThongBaoHub> _hub_context;

        public LiteController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hubcontext)
        {
            _config = configLogin.Value;
            _configLogin = configLogin;
            lc = new LoginController();
            dpsUserMr = new UserManager();
            _hostingEnvironment = hostingEnvironment;
            _hub_context = hubcontext;
        }
        #region constant
        /// <summary>
        /// Danh sách trạng thái hồ sơ ncc
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("ncc-status")]
        public BaseModel<object> NCCStatusLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.NCC_Status);
        }
        /// <summary>
        /// Danh sách trạng thái đề xuất tặng quà
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("dot-tang-qua-status")]
        public BaseModel<object> DotTangQuaStatusLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.DotTangQua_Status);
        }
        /// <summary>
        /// Danh sách Mẫu quyết dịnh phê duyệt tặng quà
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("mau-quyet-dinh-tang-qua")]
        public BaseModel<object> MauQDLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.MauQD);
        }

        /// <summary>
        /// Danh sách giới tính
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("gioi-tinh")]
        public BaseModel<object> GioiTinhLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.GioiTinh);
        }
        /// <summary>
        /// Danh sách loại biểu mẫu
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("loai-bieu-mau")]
        public BaseModel<object> LoaiBieuMauLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.LoaiBieuMau);
        }

        /// <summary>
        /// Danh sách lý do giảm
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("ly-do-giam")]
        public BaseModel<object> LyDoGiamLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.LyDoGiam);
        }
        /// <summary>
        /// Danh sách nhóm loại đối tượng ncc
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("nhom-loai-doi-tuong-ncc")]
        public BaseModel<object> NhomLoaiDoiTuongNCCLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.NhomLoaiDoiTuongNCC);
        }

        /// <summary>
        /// Danh sách cấp cơ cấu
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("cap-co-cau")]
        public BaseModel<object> CapCoCauLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.CapCoCau);
        }

        /// <summary>
        /// Danh sách loại chứng thư
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("loai-chung-thu")]
        public BaseModel<object> LoaiChungThuLite()
        {

            return JsonResultCommon.ThanhCong(LiteHelper.LoaiChungThu);
        }

        /// <summary>
        /// Danh sách trạng thái nhập số liệu
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("nhap-so-lieu-status")]
        public BaseModel<object> NhapSoLieuStatusLite()
        {
            return JsonResultCommon.ThanhCong(LiteHelper.NhapSoLieu_Status);
        }
        #endregion

        /// <summary>
        /// Cây đơn vị cha, con. cùng cấp sắp xếp theo tên DonVi
        /// Cây theo đv cha, sắp xếp theo tên Donvi
        /// dv bi khoa se  bi disabled
        /// </summary>
        /// <param name="Id">Lấy từ Đơn vị này làm cha</param>
        /// <returns></returns>
        [Route("DM_PhongBan_Tree")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DM_PhongBan_Tree(int Id = 0, bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string where = " where Disabled=0";
                    string sql = "select * from DM_DonVi";
                    if (Id > 0)
                        where += " and Id =" + Id;
                    else
                        where += " and Parent is null";
                    if (!Locked)
                    {
                        //if (IdRequired > 0)
                        //    where += " and (Locked=0 or Id=" + IdRequired + ")";
                        //else
                        where += " and Locked=0 ";
                    }
                    sql += where + " order by DonVi";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<CCLiteModel> _parents = (from pb in dt.AsEnumerable()
                                                  select new CCLiteModel()
                                                  {
                                                      id = int.Parse(pb["Id"].ToString()),
                                                      title = pb["DonVi"].ToString(),
                                                      Capcocau = (int)pb["Capcocau"],
                                                      disabled = pb["Locked"]
                                                  }).ToList();
                    var data = (from pb in _parents
                                select new CCLiteModel()
                                {
                                    id = pb.id,
                                    title = pb.title,
                                    Capcocau = pb.Capcocau,
                                    disabled = pb.disabled,
                                    data = findChild(pb.id, Locked, (bool)pb.disabled)
                                }).ToList();
                    return JsonResultCommon.ThanhCong(data);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cây đơn vị cha, con dưới cấp của đv truyền vào. cùng cấp sắp xếp theo tên DonVi
        /// dv bi khoa se  bi disabled
        /// </summary>
        /// <param name="Id">Id đơn vị</param>
        /// <returns></returns>
        [Route("DM_PhongBan_Tree_CC")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DM_PhongBan_Tree_CC(int Id, bool Locked = false)
        {
            try
            {
                int Cap = 1;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dtF = cnn.CreateDataTable("select * from dm_donvi where Id=" + Id);
                    if (dtF == null || dtF.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Đơn vị");
                    Cap = (int)dtF.Rows[0]["Capcocau"];
                    string where = " where Disabled=0";
                    string sql = "select * from DM_DonVi";
                    if (Cap == 1)
                        where += " and Parent is null";
                    if (Cap == 2)
                        where += " and (Parent=" + dtF.Rows[0]["Parent"] + " or Id=" + Id + ")";
                    if (Cap == 3)
                        where += " and (Capcocau>" + Cap + " or Id=" + Id + ")";
                    if (!Locked)
                    {
                        //if (IdRequired > 0)
                        //    where += " and (Locked=0 or Id=" + IdRequired + ")";
                        //else
                        where += " and Locked=0 ";
                    }
                    sql += where + " order by DonVi";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<CCLiteModel> _parents = (from pb in dt.AsEnumerable()
                                                  select new CCLiteModel()
                                                  {
                                                      id = int.Parse(pb["Id"].ToString()),
                                                      title = pb["DonVi"].ToString(),
                                                      Capcocau = (int)pb["Capcocau"],
                                                      disabled = pb["Locked"]
                                                  }).ToList();
                    var data = (from pb in _parents
                                select new CCLiteModel()
                                {
                                    id = pb.id,
                                    title = pb.title,
                                    Capcocau = pb.Capcocau,
                                    disabled = pb.disabled,
                                    data = findChild(pb.id, Locked, (bool)pb.disabled)
                                }).ToList();
                    return JsonResultCommon.ThanhCong(data);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private List<CCLiteModel> findChild(decimal id, bool Locked = false, bool ParentLocked = false)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sql = "select * from DM_DonVi";
                string where = " where Disabled=0 and Parent = " + id;
                if (!Locked)
                {
                    //if (IdRequired > 0)
                    //    where += " and (Locked=0 or Id=" + IdRequired + ")";
                    //else
                    where += " and Locked=0 ";
                }
                sql += where + " order by DonVi";
                DataTable dt = cnn.CreateDataTable(sql);

                List<CCLiteModel> _data = (from pb in dt.AsEnumerable()
                                           select new CCLiteModel()
                                           {
                                               id = int.Parse(pb["Id"].ToString()),
                                               title = pb["DonVi"].ToString(),
                                               Capcocau = (int)pb["Capcocau"],
                                               disabled = ParentLocked ? true : pb["Locked"]//nếu cha bị locked thì khóa luôn con
                                           }).ToList();
                if (_data.Count == 0)
                    return new List<CCLiteModel>();
                return (from pb in _data
                        select new CCLiteModel
                        {
                            id = pb.id,
                            title = pb.title,
                            Capcocau = pb.Capcocau,
                            disabled = pb.disabled,
                            data = findChild(pb.id, Locked, (bool)pb.disabled)
                        }).ToList();
            }
        }

        /// <summary>
        /// Danh sách vai trò theo đơn vị hoặc tất cả
        /// </summary>
        /// <param name="IdDonVi"></param>
        /// <returns></returns>
        [Route("vai-tro")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> VaiTroLite(int IdDonVi = 0, bool Locked = false)
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
                    string sqlq = @"select * from Dps_UserGroups where IsDel=0";
                    if (IdDonVi > 0)
                        sqlq = @"select * from Dps_UserGroups where IsDel=0 and DonVi=@Id";
                    if (!Locked)
                        sqlq += " and Locked=0 ";
                    sqlq += " order by GroupName asc";
                    var dt = cnn.CreateDataTable(sqlq, new SqlConditions { { "Id", loginData.IdDonVi } });
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var data = from r in dt.AsEnumerable()
                               select new LiteModel()
                               {
                                   id = int.Parse(r["IdGroup"].ToString()),
                                   title = r["GroupName"].ToString()
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        internal static string genLinkAvatar(string linkAPI, object v)
        {
            if (v == null)
                return "";
            return linkAPI + Constant.RootUpload + v.ToString();
        }

        /// <summary>
        /// Danh sạc vai trò (nhóm người dùng)
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("nhom-nguoi-dung")]
        public BaseModel<object> LayDSNhomLite()
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
                    string sqlq = @"SELECT [IdGroup]
                                      ,[GroupName]
                                  FROM [dbo].[Dps_UserGroups] where IsDel = 0
                                  order by GroupName";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError == null && dt != null)
                    {
                        var data = from r in dt.AsEnumerable()
                                   select new
                                   {
                                       IdGroup = r["IdGroup"],
                                       GroupName = r["GroupName"],
                                   };
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

        /// <summary>
        /// Lấy quyền dạng cây
        /// </summary>
        /// <returns></returns>
        [HttpGet]
        [Route("tree-quyen")]
        public BaseModel<object> TreeQuyen(int IdGroup)
        {
            //string Token = lc.GetHeader(Request);
            //LoginData loginData = lc._GetInfoUser(Token);
            //if (loginData == null)
            //{
            //    return JsonResultCommon.DangNhap();
            //}
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = @"select * from Dps_RoleGroups where Disabled=0 order by GroupName 
                select* from Dps_Roles where disabled = 0 order by Role";
                sqlq += " select * from Dps_UserGroupRoles where IDGroupUser=" + IdGroup;
                DataSet ds = cnn.CreateDataSet(sqlq);
                if (ds == null || ds.Tables.Count == 0)
                    return JsonResultCommon.SQL(cnn.LastError != null ? cnn.LastError.Message : "");
                try
                {
                    var data = new List<object>(){new
                    {
                        item = "Tất cả",
                        children = from p in ds.Tables[0].AsEnumerable()
                                   where p["IdParent"]==DBNull.Value
                                   select new
                                   {
                                       item = p["GroupName"],
                                       children = from r in ds.Tables[0].AsEnumerable()
                                                  where r["IdParent"].ToString() == p["IdGroup"].ToString()
                                                   select new
                                                   {
                                                       item = r["GroupName"],
                                                       children = from c in ds.Tables[1].AsEnumerable()
                                                                  where c["RoleGroup"].ToString() == r["IdGroup"].ToString()
                                                                  select new
                                                                  {
                                                                      item = c["Role"],
                                                                      selected = ds.Tables[2].AsEnumerable().Where(x => x["IDGroupRole"].ToString() == c["IdRole"].ToString()).Count() > 0,
                                                                      value=c["IdRole"],
                                                                  },
                                                   }
                                   }
                    } };
                    return JsonResultCommon.ThanhCong(data);
                }
                catch (Exception ex)
                {
                    return JsonResultCommon.Exception(ex, ControllerContext);
                }
            }
        }

        [Route("LayTreeDonVi")]
        [HttpGet]
        public object LayTreeDonVi(int idParent = 0, int idParentGoc = 0, bool isHC = false)
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
                SqlConditions conds = new SqlConditions();
                string where = " where Parent is null and Disabled=0 ";
                if (idParent != 0)
                    where = " where Parent=" + idParent + " and Disabled=0 ";
                if (idParentGoc != 0)
                    where = " where ID_Goc=" + idParentGoc + " and Disabled=0 ";
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = $@"select * from DM_DonVi {where} order by DonVi asc";
                    var dt = cnn.CreateDataTable(sqlq, conds);
                    var data = from r in dt.AsEnumerable()
                               where (isHC && r["Type"] != DBNull.Value) || !isHC
                               select new
                               {
                                   text = r["DonVi"].ToString(),
                                   data = new
                                   {
                                       IdGroup = r["Id"].ToString(),
                                       GroupName = r["DonVi"].ToString(),
                                       ID_Goc = r["ID_Goc"].ToString(),
                                       Type = r["Type"].ToString(),
                                   },
                                   state = new
                                   {
                                       selected = false
                                   },
                                   children = ChildDonVi_New(long.Parse(r["Id"].ToString()), isHC)
                               };

                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private List<DonViTree> ChildDonVi_New(long IdParent, bool isHC)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string where = " where Parent is not null and Disabled=0";
                SqlConditions conds = new SqlConditions();
                string sqlq = $@"select * from DM_DonVi {where} order by DonVi asc";
                var dt = cnn.CreateDataTable(sqlq, conds);
                var data = (from r in dt.AsEnumerable()
                            where r["Parent"].ToString() == IdParent.ToString()
                            && ((isHC && r["Type"] != DBNull.Value) || !isHC)
                            select new DonViTree
                            {

                                text = r["DonVi"].ToString(),
                                data = new
                                {
                                    IdGroup = r["Id"].ToString(),
                                    GroupName = r["DonVi"].ToString(),
                                    ID_Goc = r["ID_Goc"].ToString(),
                                    Type = r["Type"].ToString(),
                                },
                                state = new
                                {
                                    selected = false
                                },
                                children = ChildDonVi_New(long.Parse(r["Id"].ToString()), isHC)
                            }).ToList();

                return data;
            }
        }

        private List<DonViTree> ChildDonVi(DataTable dt, int IdParent)
        {
            List<object> lst = new List<object>();
            if (dt.AsEnumerable().Select(n => n.Field<long>("Parent") == Convert.ToInt64(IdParent)).Count() > 0)
            {
                var childeren = (from goc in dt.AsEnumerable()
                                 where int.Parse(goc["Parent"].ToString()) == IdParent
                                 select new DonViTree
                                 {
                                     text = goc["DonVi"].ToString(),
                                     data = new
                                     {
                                         IdGroup = goc["Id"].ToString(),
                                         GroupName = goc["DonVi"],
                                     },
                                     state = new
                                     {
                                         selected = false
                                     },
                                     children = ChildDonVi(dt, int.Parse(goc["Id"].ToString()))
                                 }).ToList();
                //lst.Append(childeren);
                return childeren;
            }
            else
            {
                return null;
            }

        }

        #region chức vụ
        [Route("DM_ChucVu")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DM_ChucVu(long Iddonvi = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_ChucVu where Disabled=0 order by ChucVu";
                    if (Iddonvi > 0)
                        sql = "select * from DM_ChucVu where Disabled=0 and DonVi = " + Iddonvi + " order by ChucVu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["ChucVu"].ToString()
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Cây chức vụ theo đơn vị
        /// </summary>
        /// <param name="Id"></param>
        /// <returns></returns>
        [HttpGet]
        [Authorize]
        [Route("tree-chuc-vu")]
        public BaseModel<object> TreeChucVu(int Donvi, bool Locked = false)
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
                SqlConditions conds = new SqlConditions();
                string where = " where IdParent is null and Disabled=0 ";
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    if (Donvi == 0)
                        return JsonResultCommon.BatBuoc("Đơn vị");
                    conds.Add("Id", Donvi);
                    where += " and DonVi=@Id ";
                    if (!Locked)
                        where += " and Locked=0";

                    string sqlq = $@"select * from DM_ChucVu {where} order by ChucVu asc";
                    var dt = cnn.CreateDataTable(sqlq, conds);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   id = r["Id"],
                                   title = r["ChucVu"],
                                   disabled = r["Locked"],
                                   data = childCV(long.Parse(r["Id"].ToString()), Locked, bool.Parse(r["Locked"].ToString()))
                               };

                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private object childCV(long id, bool Locked, bool ParentLocked)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string where = " where IdParent is not null and Disabled=0";
                if (!Locked)
                    where += " and Locked=0";
                SqlConditions conds = new SqlConditions();
                string sqlq = $@"select * from DM_ChucVu {where} order by ChucVu asc";
                var dt = cnn.CreateDataTable(sqlq, conds);
                var data = (from r in dt.AsEnumerable()
                            where r["IdParent"].ToString() == id.ToString()
                            select new
                            {
                                id = r["Id"].ToString(),
                                title = r["ChucVu"].ToString(),
                                disabled = ParentLocked ? true : bool.Parse(r["Locked"].ToString()),
                                data = childCV(int.Parse(r["Id"].ToString()), Locked, bool.Parse(r["Locked"].ToString()))
                            }).ToList();

                return data;
            }

        }
        #endregion

        [Route("Get_DSDoiTuongNhanQua")]
        [HttpGet]
        public BaseModel<object> ListDoiTuongNhanQua([FromQuery] QueryParams query)
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
                    var Conds = new SqlConditions();
                    string sqlq = @"select dm.*, u.FullName as NguoiTao, u1.FullName as NguoiSua from DM_DoiTuongNhanQua dm
join Dps_User u on dm.CreatedBy=u.UserID
left join Dps_User u1 on dm.UpdatedBy=u1.UserID
where dm.Disabled=0";
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        sqlq += " and (DoiTuong like @kw or MaDoiTuong like @kw or dm.MoTa like @kw)";
                        Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                    }
                    var dt = cnn.CreateDataTable(sqlq, Conds);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "DoiTuong","DoiTuong" },
                        { "MaDoiTuong","MaDoiTuong" },
                        { "Locked","Locked" },
                        { "Priority","Priority" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["DoiTuong"]))
                    {
                        string keyword = query.filter["DoiTuong"].ToLower();
                        temp = temp.Where(x => x["DoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaDoiTuong"]))
                    {
                        string keyword = query.filter["MaDoiTuong"].ToLower();
                        temp = temp.Where(x => x["MaDoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    }
                    #endregion
                    int i = temp.Count();
                    if (i == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);
                    dt = temp.CopyToDataTable();
                    int total = dt.Rows.Count;
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
                    var dtTemp = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (dtTemp.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);
                    dt = dtTemp.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {

                                   Id = r["Id"],

                                   DoiTuong = r["DoiTuong"],

                                   MaDoiTuong = r["MaDoiTuong"],

                                   MoTa = r["MoTa"],

                                   Locked = r["Locked"],

                                   Priority = r["Priority"],

                                   CreatedBy = r["NguoiTao"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
                                   UpdatedBy = r["NguoiSua"],
                                   UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["UpdatedDate"]),
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Route("Get_DSDoiTuongNCC")]
        [HttpGet]
        public BaseModel<object> List([FromQuery] QueryParams query)
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
                    var Conds = new SqlConditions();
                    string sqlq = @"select dm.*, u.FullName as NguoiTao, u1.FullName as NguoiSua from DM_DOITUONGNCC dm
join Dps_User u on dm.CreatedBy=u.UserID
left join Dps_User u1 on dm.UpdatedBy=u1.UserID
where dm.Disabled=0";
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        sqlq += " and (DoiTuong like @kw or MaDoiTuong like @kw or dm.MoTa like @kw)";
                        Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                    }
                    var dt = cnn.CreateDataTable(sqlq, Conds);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "DoiTuong","DoiTuong" },
                        { "MaDoiTuong","MaDoiTuong" },
                        { "Locked","Locked" },
                        { "Priority","Priority" }
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["DoiTuong"]))
                    {
                        string keyword = query.filter["DoiTuong"].ToLower();
                        temp = temp.Where(x => x["DoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MaDoiTuong"]))
                    {
                        string keyword = query.filter["MaDoiTuong"].ToLower();
                        temp = temp.Where(x => x["MaDoiTuong"].ToString().ToLower().Contains(keyword));
                    }
                    if (!string.IsNullOrEmpty(query.filter["MoTa"]))
                    {
                        string keyword = query.filter["MoTa"].ToLower();
                        temp = temp.Where(x => x["MoTa"].ToString().ToLower().Contains(keyword));
                    }
                    if (query.filterGroup != null && query.filterGroup["Locked"] != null && query.filterGroup["Locked"].Length > 0)
                    {
                        var groups = query.filterGroup["Locked"].ToList();
                        temp = temp.Where(x => groups.Contains(x["Locked"].ToString()));
                    }
                    if (query.filterGroup != null && query.filterGroup["NhomLoaiDoiTuongNCC"] != null && query.filterGroup["NhomLoaiDoiTuongNCC"].Length > 0)
                    {
                        var groups = query.filterGroup["NhomLoaiDoiTuongNCC"].ToList();
                        temp = temp.Where(x => groups.Contains(x["NhomLoaiDoiTuongNCC"].ToString()));
                    }
                    #endregion
                    int i = temp.Count();
                    if (i == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);
                    dt = temp.CopyToDataTable();
                    int total = dt.Rows.Count;
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
                    var dtTemp = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (dtTemp.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);
                    dt = dtTemp.CopyToDataTable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {

                                   Id = r["Id"],

                                   DoiTuong = r["DoiTuong"],

                                   MaDoiTuong = r["MaDoiTuong"],

                                   MoTa = r["MoTa"],


                                   Locked = r["Locked"],

                                   Priority = r["Priority"],

                                   CreatedBy = r["NguoiTao"],
                                   CreatedDate = string.Format("{0:dd/MM/yyyy}", r["CreatedDate"]),
                                   UpdatedBy = r["NguoiSua"],
                                   UpdatedDate = string.Format("{0:dd/MM/yyyy}", r["UpdatedDate"]),

                                   Loai = r["Loai"],
                                   NhomLoaiDoiTuongNCC = LiteHelper.GetLiteById(r["Loai"], LiteHelper.NhomLoaiDoiTuongNCC)
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpPost]
        [Authorize]
        [Route("danh-sach-nguoi-lite")]
        public BaseModel<object> DSNguoiDung(bool useVaiTro = true, int idDV = 0)
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
											  ,us.Username 
                                              ,[FullName]  
                                              ,Avata
											  ,g.GroupName
											  ,g.IdGroup
											  ,g.DonVi
											  ,CAST(1 AS smallint) as UserType											 
                                              ,(SELECT  DonVi + ' - ' AS [text()] 
												FROM    GetOrgTreeParent(g.DonVi,1)
												FOR XML PATH ('')) as TenDonVi
                                          FROM [Dps_User] us 
										  join Dps_User_GroupUser x on us.UserID=x.IdUser
										  left join Dps_UserGroups g on x.IdGroupUser=g.IdGroup
                                          left join DM_ChucVu cv on cv.MaChucVu = us.IdChucVu 
										  where us.Deleted=0 and x.Disabled=0";
                    if (!useVaiTro)
                        sqlq = @"SELECT  us.[UserID]
											  ,us.Username 
                                              ,Avata
                                              ,[FullName]                                           
											  ,'' as GroupName
											  ,0 as IdGroup
											  ,us.IdDonVi as DonVi
											  ,CAST(1 AS smallint) as UserType											 
                                              ,(SELECT  DonVi + ' - ' AS [text()] 
												FROM    GetOrgTreeParent(us.IdDonVi,1)
												FOR XML PATH ('')) as TenDonVi
                                          FROM [Dps_User] us 
                                          left join DM_ChucVu cv on cv.MaChucVu = us.IdChucVu 
										  where us.Deleted=0";
                    if (idDV > 0)
                        sqlq += " and us.IdDonVi=" + idDV;
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //var temp = dt.AsEnumerable();
                    //if (!string.IsNullOrEmpty(keyword))
                    //{
                    //    temp = temp.Where(x => x["FullName"].ToString().ToLower().Contains(keyword));
                    //}
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   UserID = r["UserID"],
                                   FullName = r["FullName"],
                                   UserName = r["UserName"],
                                   GroupName = r["GroupName"],
                                   IdGroup = r["IdGroup"],
                                   UserType = r["UserType"],
                                   DonVi = r["TenDonVi"] != DBNull.Value && r["TenDonVi"].ToString().Length > 0 ? r["TenDonVi"].ToString().Remove(r["TenDonVi"].ToString().LastIndexOf('-')) : "",
                                   image = genLinkAvatar(_config.LinkAPI, r["Avata"])
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Load danh sách nhân viên
        /// </summary>
        /// <returns></returns>
        [Route("Get_DSNhanVien")]
        [HttpGet]
        public object Get_DSNhanVien([FromQuery] QueryParams query)
        {
            query = query == null ? new QueryParams() : query;
            BaseModel<object> model = new BaseModel<object>();
            PageModel pageModel = new PageModel();
            ErrorModel error = new ErrorModel();
            SqlConditions Conds = new SqlConditions();
            string sql = "";
            string orderByStr = "fullname asc";
            string whereStr = "";
            try
            {

                Conds.Add("Deleted", 0);
                Conds.Add("Active", 1);
                whereStr += " nv.Deleted= @Deleted and nv.Active = @Active";
                if (!string.IsNullOrEmpty(query.filter["keyword"]))
                {
                    whereStr += " and (manv like @kw or fullname like @kw or Username like @kw)";
                    Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                }

                if (!string.IsNullOrEmpty(query.filter["IDChucVu"]))
                {
                    whereStr += " and nv.IdChucVu = @IDChucVu";
                    Conds.Add("IDChucVu", query.filter["IDChucVu"]);
                }

                if (!string.IsNullOrEmpty(query.filter["ID_NhanVien"]))
                {
                    whereStr += " and UserID not in (" + query.filter["ID_NhanVien"] + ")";
                }

                //Sort theo field trên list
                Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "HoTen", "hoten"},
                            { "MaNV", "manv"},
                            { "Structure", "tbl_cocautochuc.title"},
                            { "TenChucVu", "cd.Tenchucdanh"},
                };
                if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                {
                    orderByStr = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                }

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    sql = $@"SELECT fullname as hoten, convert(varchar(10), ngaysinh, 103) as ngaysinh,tbl_cocautochuc.title,cd.Tenchucdanh,GioiTinh as Phai, manv,UserID id_nv,Email
                        from dps_user nv join tbl_cocautochuc on nv.IdDonVi = tbl_cocautochuc.rowid left join tbl_chucdanh cd on nv.IdChucVu=cd.id_row
                        where { whereStr } order by { orderByStr}";

                    DataTable dt = cnn.CreateDataTable(sql, Conds);
                    int total = dt.Rows.Count;
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel);

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
                    var temp1 = dt.AsEnumerable().Skip((query.page - 1) * query.record).Take(query.record);
                    if (temp1.Count() == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), new PageModel());
                    dt = temp1.CopyToDataTable();

                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   HoTen = r["hoten"],
                                   NgaySinh = r["ngaysinh"],
                                   Structure = r["title"],
                                   Phai = LiteHelper.GetLiteById(r["phai"], LiteHelper.GioiTinh),
                                   TenChucVu = r["Tenchucdanh"],
                                   MaNV = r["manv"],
                                   ID_NV = r["id_nv"],
                                   Email = r["Email"]
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpPost]
        //[Authorize]
        [Route("danh-sach-don-vi-lite")]
        public BaseModel<object> DSDonVi()
        {
            try
            {
                int caidat = int.Parse(_config.IdTinh);
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                PageModel pageModel = new PageModel();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"SELECT [Id]
                                          ,[LoaiDonVi]
                                          ,[DonVi]
                                          ,[MaDonvi]
                                          ,[MaDinhDanh]
                                          ,[Parent]                                          
	                                      ,(SELECT  DonVi + ' - ' AS [text()] 
		                                    FROM    GetOrgTreeParent(Id,0)
		                                    FOR XML PATH ('')) as ParentName
                                          FROM [DM_DonVi] where Disabled=0";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    //logHelper.LogXemDS(loginData.Id, "Danh mục đơn vị");
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["Id"],
                                   LoaiDonVi = r["LoaiDonVi"],
                                   DonVi = r["DonVi"],
                                   ParentName = r["ParentName"] != DBNull.Value && r["ParentName"].ToString().Length > 0 ? r["ParentName"].ToString().Remove(r["ParentName"].ToString().LastIndexOf('-')) : "",
                                   MaDonvi = r["MaDonvi"],
                                   Parent = r["Parent"],
                                   Child = ChildDonVi1(r["Id"] != DBNull.Value ? long.Parse(r["Id"].ToString()) : 1)
                               };
                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        private object ChildDonVi1(long IdParent)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string sqlq = $@" select *, (SELECT  DonVi + ' - ' AS [text()] 
		                                    FROM    GetOrgTreeParent(dv.Id,0)
		                                    FOR XML PATH ('')) as ParentName from   GetOrgTree(@IdDV) dv where  Disabled=0";
                DataSet ds = cnn.CreateDataSet(sqlq, new SqlConditions { { "IdDV", IdParent } });
                if (cnn.LastError != null || ds == null || ds.Tables[0] == null)
                {
                    return null;
                }
                DataTable dtquyen = ds.Tables[0];
                if (dtquyen != null && dtquyen.Rows.Count >= 0)
                {
                    try
                    {
                        var equyen = dtquyen.AsEnumerable();
                        var data = (from goc in dtquyen.AsEnumerable()
                                    select new
                                    {
                                        Id = goc["Id"],
                                        LoaiDonVi = goc["LoaiDonVi"],
                                        DonVi = goc["DonVi"],
                                        ParentName = goc["ParentName"] != DBNull.Value && goc["ParentName"].ToString().Length > 0 ? goc["ParentName"].ToString().Remove(goc["ParentName"].ToString().LastIndexOf('-')) : "",
                                        MaDonvi = goc["MaDonvi"],
                                        Parent = goc["Parent"]
                                    }).ToList();
                        return data;
                    }
                    catch (Exception ex)
                    {
                        return null;
                    }
                }
                return null;
            }
        }

        [Route("tree-nguoi-dung-don-vi")]
        [HttpGet]
        public BaseModel<object> DM_PhongBan_User_Tree(int Id = 0, bool useVaiTro = true)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                string where = " where Parent is null and Disabled=0 ";
                if (Id > 0)
                    where = " where Disabled=0 and Id=" + Id;
                PageModel pageModel = new PageModel();
                SqlConditions conds = new SqlConditions();

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = $@"select * from DM_DonVi {where} order by DonVi asc";
                    var dt = cnn.CreateDataTable(sqlq, conds);
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   item = r["DonVi"].ToString(),
                                   data = new
                                   {
                                       Id = r["Id"],
                                       Name = r["DonVi"],
                                       Type = 0
                                   },
                                   children = findChildUser(long.Parse(r["Id"].ToString()), useVaiTro)
                               };

                    return JsonResultCommon.ThanhCong(data, pageModel);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("Log_HanhDong")]
        public BaseModel<object> Log_HanhDong()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                SqlConditions conds = new SqlConditions();
                string where = "";
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {

                    //if (Parent != 0)
                    //{
                    //    conds.Add("Parent", Parent);
                    //    where += " and Parent=@Parent";
                    //}

                    string sqlq = $@"select IdRow,HanhDong from Tbl_Log_HanhDong order by HanhDong";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["IdRow"],
                                   Name = r["HanhDong"],
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [HttpGet]
        [Authorize]
        [Route("Log_Loai")]
        public BaseModel<object> Log_Loai()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                {
                    return JsonResultCommon.DangNhap();
                }
                SqlConditions conds = new SqlConditions();
                string where = "";
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {

                    //if (Parent != 0)
                    //{
                    //    conds.Add("Parent", Parent);
                    //    where += " and Parent=@Parent";
                    //}

                    string sqlq = $@"select IdRow, LoaiLog from Tbl_Log_Loai order by LoaiLog";
                    var dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["IdRow"],
                                   Name = r["LoaiLog"],
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private List<NodeModel> findChildUser(long IdParent, bool useVaiTro)
        {
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string where = " where Parent is not null and Disabled=0";
                SqlConditions conds = new SqlConditions();
                string sqlq = $@"select * from DM_DonVi {where} order by DonVi asc";
                var dt = cnn.CreateDataTable(sqlq, conds);
                string sql = $@" select us.UserID,us.FullName , us.FullName +' - ' + GroupName as [Name], g.IdGroup, GroupName, DonVi, (SELECT  DonVi + ' - ' AS [text()] 
		                                    FROM    GetOrgTreeParent(DonVi,1)
		                                    FOR XML PATH ('')) as ParentName    from Dps_User_GroupUser ug 
                                  inner join Dps_UserGroups g on ug.IdGroupUser = g.IdGroup
                                    inner join Dps_User us on us.UserID = ug.IdUser
                                    where ug.Disabled = 0 and DonVi={IdParent}";
                if (!useVaiTro)
                {
                    sql = $@" select us.UserID,us.FullName , us.FullName as [Name],0 as IdGroup,'' as GroupName,IdDonVi as DonVi, (SELECT  DonVi + ' - ' AS [text()] 
		                                    FROM    GetOrgTreeParent(IdDonVi,1)
		                                    FOR XML PATH ('')) as ParentName    from  Dps_User us 
                                    where us.Deleted = 0 and IdDonVi={IdParent}";
                }
                var dt1 = cnn.CreateDataTable(sql);
                var data = new List<NodeModel>();
                var childeren = (from r in dt.AsEnumerable()
                                 where r["Parent"].ToString() == IdParent.ToString()
                                 select new NodeModel
                                 {
                                     item = r["DonVi"].ToString(),
                                     data = new
                                     {
                                         Id = r["Id"],
                                         Name = r["DonVi"],
                                         Type = 0
                                     },
                                     children = findChildUser(long.Parse(r["Id"].ToString()), useVaiTro)
                                 }).ToList();
                foreach (var item in dt1.AsEnumerable())
                {
                    var _item = new NodeModel()
                    {
                        item = item["Name"].ToString(),
                        children = null,
                        value = item["UserID"] != DBNull.Value ? long.Parse(item["UserID"].ToString()) : 0,
                        data = new
                        {
                            Id = item["UserID"] != DBNull.Value ? long.Parse(item["UserID"].ToString()) : 0,
                            IdGroup = item["IdGroup"] != DBNull.Value ? long.Parse(item["IdGroup"].ToString()) : 0,
                            DonVi = item["DonVi"] != DBNull.Value ? long.Parse(item["DonVi"].ToString()) : 0,
                            ParentName = item["ParentName"] != DBNull.Value && item["ParentName"].ToString().Length > 0 ? item["ParentName"].ToString().Remove(item["ParentName"].ToString().LastIndexOf('-')) : "",
                            Name = item["FullName"],
                            GroupName = item["GroupName"],
                            Type = 1
                        }
                    };
                    data.Add(_item);
                }
                data.AddRange(childeren);
                return data;
            };

        }

        /// <summary>
        /// Lấy danh sách Đơn vị theo đơn vị cha
        /// </summary>
        /// <param name="_query"></param>
        /// <returns></returns>
        [Authorize]
        [Route("Lite_DanhSachDonVi_Prent")]
        [HttpGet]
        public BaseModel<object> Lite_DanhSachDonVi_Prent(long Id, bool Locked = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = @"  select * from DM_DonVi where Disabled=0 and Parent=@Id";
                    if (!Locked)
                        sqlq += " and Locked=0";
                    SqlConditions cond = new SqlConditions();
                    // sqlq += " and Parent = @IdDV";

                    cond.Add("Id", Id);

                    var dt = cnn.CreateDataTable(sqlq, cond);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   Id = r["Id"],
                                   DonVi = r["DonVi"],
                                   MaDonvi = r["MaDonvi"],
                                   //MaDinhDanh = r["MaDinhDanh"],
                                   //Parent = r["Parent"],
                                   //SDT = r["SDT"],
                                   //Email = r["Email"],
                                   //DiaChi = r["DiaChi"],
                                   //Logo = r["Logo"],
                                   //DangKyLichLanhDao = r["DangKyLichLanhDao"],
                                   //KhongCoVanThu = r["KhongCoVanThu"],
                                   //LoaiDonVi = r["LoaiDonVi"],
                                   //Priority = r["Priority"],
                                   //Locked = r["Locked"],
                                   //CreatedDate = String.Format("{0:dd\\/MM\\/yyyy HH:mm}", r["CreatedDate"]),

                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }
        #region file đính kèm
        [Authorize]
        [HttpGet]
        [Route("download-dinhkem")]
        public async Task<BaseModel<object>> Download_FileDinhKem(int id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            string filename = "";
            string sql = "select * fromKem where Disabled=0 and IdRow=@Id";
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", id } });
                if (dt == null || dt.Rows.Count == 0)
                    return JsonResultCommon.KhongTonTai("Tệp đính kèm");
                filename = dt.Rows[0]["FileDinhKem"].ToString();
                int loai = int.Parse(dt.Rows[0]["Loai"].ToString());
                string table = "";
                string name = "";
                switch (loai)
                {
                    case 1:
                        {
                            name = "Đợt tặng quà";
                            table = "Tbl_DotTangQua";
                            break;
                        }
                    default:
                        {
                            return JsonResultCommon.Custom("Loại tệp đính kèm không tồn tại");
                        }
                }
                sql = "select * from " + table + " where Disabled=0 and Id=@Id";
                DataTable dt1 = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", dt.Rows[0]["Id"].ToString() } });
                if (dt1 == null || dt1.Rows.Count == 0)
                    return JsonResultCommon.KhongTonTai(name);
            }
            try
            {

                var path = Path.Combine(_hostingEnvironment.ContentRootPath, Constant.RootUpload + filename);

                var memory = new MemoryStream();
                using (var stream = new FileStream(path, FileMode.Open))
                {
                    await stream.CopyToAsync(memory);
                }
                memory.Position = 0;
                string contentType = UploadHelper.GetContentType(path);
                if (string.IsNullOrEmpty(contentType))
                {
                    return JsonResultCommon.Custom("File không đúng định dạng");
                }
                FileContentResult file = new FileContentResult(memory.ToArray(), contentType)
                {
                    FileDownloadName = UploadHelper.GetFileName(filename)
                };
                return JsonResultCommon.ThanhCong(file);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("view-dinhkem")]
        public async Task<BaseModel<object>> View_FileDinhKem(int id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            string filename = "";
            string sql = "select * from Tbl_FileDinhKem where Disabled=0 and IdRow=@Id";
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", id } });
                if (dt == null || dt.Rows.Count == 0)
                    return JsonResultCommon.KhongTonTai("Tệp đính kèm");
                filename = dt.Rows[0]["FileDinhKem"].ToString();
            }
            try
            {
                return JsonResultCommon.ThanhCong(_config.LinkAPI + Constant.RootUpload + filename);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize]
        [HttpPost]
        [Route("add-dinhkem")]
        public async Task<BaseModel<object>> Add_FileDinhKem(int loai, int id, [FromBody] List<ListImageModel> data)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
            {
                string table = "";
                string name = "";
                string folder = "";
                switch (loai)
                {
                    case 1:
                        {
                            name = "Đợt tặng quà";
                            table = "Tbl_DotTangQua";
                            folder = "DotTangQua";
                            break;
                        }
                    default:
                        {
                            return JsonResultCommon.Custom("Loại tệp đính kèm không tồn tại");
                        }
                }
                string sql = "select * from " + table + " where Disabled=0 and Id=@Id";
                DataTable dt1 = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", id } });
                if (dt1 == null || dt1.Rows.Count == 0)
                    return JsonResultCommon.KhongTonTai(name);
                foreach (var item in data)
                {
                    string x = "";
                    if (!UploadHelper.UploadFile(item.strBase64, item.filename, "/dinhkem/" + folder + "/", _hostingEnvironment.ContentRootPath, ref x))
                    {
                        return JsonResultCommon.Custom(UploadHelper.error);
                    }
                    item.src = _config.LinkAPI + x;
                    Hashtable val1 = new Hashtable();
                    val1["Loai"] = loai;
                    val1["Id"] = id;
                    val1.Add("FileDinhKem", x);
                    val1.Add("filename", UploadHelper.GetFileName(x));
                    val1.Add("type", UploadHelper.GetContentType(x));
                    val1.Add("size", 0);
                    val1.Add("UpdatedDate", DateTime.Now);
                    val1.Add("UpdatedBy", loginData.Id);
                    if (cnn.Insert(val1, "Tbl_FileDinhKem") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                }
            }
            try
            {
                return JsonResultCommon.ThanhCong(data);
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        [Authorize]
        [HttpGet]
        [Route("delete-dinhkem")]
        public async Task<BaseModel<object>> Delete_FileDinhKem(int id)
        {
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
            {
                return JsonResultCommon.DangNhap();
            }
            try
            {
                string sql = "select * from Tbl_FileDinhKem where Disabled=0 and IdRow=@Id";
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(sql, new SqlConditions() { { "Id", id } });
                    if (dt == null || dt.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Tệp đính kèm");
                    sql = "Update Tbl_FileDinhKem set Disabled=1 where IdRow=" + id;
                    if (cnn.ExecuteNonQuery(sql) < 0)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                }
                return JsonResultCommon.ThanhCong();
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region config
        /// <summary>
        /// Lấy config bằng code
        /// trang login chưa đăng nhập cần lấy config
        /// </summary>
        /// <param name="code"></param>
        /// <returns></returns>
        [AllowAnonymous]
        [Route("get-config")]
        [HttpPost]
        public BaseModel<object> GetConfig([FromBody] List<string> codes)
        {
            try
            {
                //string Token = lc.GetHeader(Request);
                //LoginData loginData = lc._GetInfoUser(Token);
                //if (loginData == null)
                //    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    var data = GetConfigs(codes, cnn);
                    if (cnn.LastError != null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }

        }

        public static Dictionary<string, string> GetConfigs(List<string> codes, DpsConnection cnn)
        {
            var data = new Dictionary<string, string>();
            if (codes == null || codes.Count == 0)
                return data;
            string str = "";
            foreach (string code in codes)
            {
                str += (str == "" ? "" : ", ") + "'" + code + "'";
            }
            string sqlq = @"  select * from Sys_Config where Code in (" + str + ")";

            var dt = cnn.CreateDataTable(sqlq);
            if (cnn.LastError != null || dt == null)
            {
                return data;
            }
            foreach (DataRow item in dt.Rows)
            {
                data.Add(item["Code"].ToString(), item["Value"].ToString());
            }
            return data;
        }
        #endregion
        /// <summary>
        /// Kiểm tra Khóa ngoại có tồn tại không
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="models"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool CheckPF(DpsConnection cnn, List<CheckFKModel> models, out string msg)
        {
            msg = "";
            if (models == null || models.Count == 0)
                return true;
            string strCheck = "";
            foreach (var model in models)
            {
                string sql = " select count(*) from {0} where {1}={2} and {3}=0";
                if (!string.IsNullOrEmpty(model.LockedColumn))
                    sql += " and {4}=0";
                if (!string.IsNullOrEmpty(model.StrWhere))
                    sql += " and " + model.StrWhere;
                strCheck += string.Format(sql, model.TableName, model.PKColumn, model.Id, model.DisabledColumn, model.LockedColumn);
            }
            DataSet ds = cnn.CreateDataSet(strCheck);
            if (ds == null || cnn.LastError != null)
                return false;
            bool kq = true;
            for (int i = 0; i < models.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                if (int.Parse(dt.Rows[0][0].ToString()) == 0)
                {
                    kq = false;
                    msg += (msg == "" ? "" : ", ") + models[i].name;
                }
            }
            return kq;
        }
        public static bool CheckPF(DataSet ds, List<CheckFKModel> models, out string msg, out List<string> names)
        {
            names = new List<string>();
            msg = "";
            if (models == null || models.Count == 0)
                return true;
            if (ds == null || ds.Tables.Count != models.Count)
                return false;
            bool kq = true;
            for (int i = 0; i < models.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                var rows = dt.Select(models[i].PKColumn + "= " + models[i].Id);
                if (rows.Count() == 0)
                {
                    kq = false;
                    msg += (msg == "" ? "" : ", ") + models[i].name;
                    names.Add("");
                }
                else
                {
                    names.Add(rows[0][models[i].TitleColumn].ToString());
                }
            }
            return kq;
        }

        /// <summary>
        /// Kiểm tra đối tượng có đang được sử dụng không
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="models"></param>
        /// <param name="id"></param>
        /// <param name="msg"></param>
        /// <returns></returns>
        public static bool InUse(DpsConnection cnn, List<CheckFKModel> models, long id, out string msg)
        {
            msg = "";
            if (models == null || models.Count == 0)
                return false;
            string strCheck = "";
            foreach (var model in models)
            {
                //select * from DM_DanhMuc where LoaiDanhMuc=3 and Disabled=0
                if (string.IsNullOrEmpty(model.DisabledColumn))
                {
                    string sql = " select count(*) from {0} where {1}={2}";
                    strCheck += string.Format(sql, model.TableName, model.PKColumn, id);
                }
                else
                {
                    string sql = " select count(*) from {0} where {1}={2} and {3}=0";
                    strCheck += string.Format(sql, model.TableName, model.PKColumn, id, model.DisabledColumn);
                }
            }
            DataSet ds = cnn.CreateDataSet(strCheck);
            if (ds == null || cnn.LastError != null)
                return true;
            bool kq = false;
            for (int i = 0; i < models.Count; i++)
            {
                DataTable dt = ds.Tables[i];
                if (int.Parse(dt.Rows[0][0].ToString()) > 0)
                {
                    kq = true;
                    msg += (msg == "" ? "" : ", ") + models[i].name;
                }
            }
            return kq;
        }

        #region chức danh, chức vụ

        /// /// <summary>
        /// Lấy danh sách chức danh
        /// </summary>
        /// <returns></returns>
        [Route("GetListPosition")]
        [HttpGet]
        public object GetListPosition()
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(@"select id_cv, tencv, nhom, cap from chucvu where disable=0 order by cap desc", Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          id_cv = r["id_cv"],
                                                          tencv = r["tencv"],
                                                          nhom = r["nhom"]
                                                      });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// Lấy danh sách chức danh theo cơ cấu tổ chức
        /// </summary>
        /// <param name="structureid">0: tất cả</param>
        /// <returns></returns>
        [Route("GetListPositionbyStructure")]
        [HttpGet]
        public object GetListPositionbyStructure(string structureid)
        {
            string sqlq = "", whereStr = " tbl_chucdanh.disable=0 and chucvu.disable=0 ", orderByStr = " tencv";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    if (!string.IsNullOrEmpty(structureid) && !"0".Equals(structureid))
                    {
                        whereStr += " and cocauid = @structureid";
                        Conds.Add("structureid", structureid);
                    }
                    sqlq = @"select distinct ChucVu.Id_CV, tencv from tbl_chucdanh inner join ChucVu on tbl_chucdanh.id_cv=chucvu.id_cv where " + whereStr + @" order by " + orderByStr;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID = r["Id_CV"],
                                                          Title = r["TenCV"],
                                                      });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// Lấy danh sách chức danh theo cơ cấu tổ chức cha và con
        /// </summary>
        /// <param name="structureid">0: tất cả</param>
        /// <returns></returns>
        [Route("GetListPositionbyStructure_All")]
        [HttpGet]
        public object GetListPositionbyStructure_All(string structureid)
        {
            string sqlq = "", whereStr = " tbl_chucdanh.disable=0 and chucvu.disable=0 ", orderByStr = " tencv";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    sqlq = @"select distinct ChucVu.Id_CV, tencv from tbl_chucdanh inner join ChucVu on tbl_chucdanh.id_cv=chucvu.id_cv where " + whereStr + @" order by " + orderByStr;
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID = r["Id_CV"],
                                                          Title = r["TenCV"],
                                                      });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Load chức vụ theo phòng ban, chức danh; Nếu id_cv=0 => Lấy danh sách chức vụ theo phòng ban
        /// </summary>
        /// <param name="id_cv"></param>
        /// <param name="id_bp"></param>
        /// <returns></returns>
        [Route("GetListJobtitleByPosition")]
        [HttpGet]
        public object GetListJobtitleByPosition(string id_cv, string id_bp)
        {
            string sqlq = "", whereStr = " disable=0", orderByStr = "tenchucdanh";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    if (!"0".Equals(id_bp))
                    {
                        whereStr += " and id_bp = @id_bp";
                        Conds.Add("id_bp", id_bp);
                    }
                    if (!"0".Equals(id_cv))
                    {
                        whereStr += " and id_cv = @id_cv";
                        Conds.Add("id_cv", id_cv);
                    }
                    sqlq = $@"select id_row, tenchucdanh from tbl_chucdanh where { whereStr } order by { orderByStr}";
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          Id_row = r["id_row"],
                                                          Tenchucdanh = r["tenchucdanh"]
                                                      });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Load chức vụ theo cơ cấu tổ chức, chức danh
        /// </summary>
        /// <param name="id_cv">0: Lấy danh sách chức vụ theo cơ cấu tổ chức</param>
        /// <param name="structureid"></param>
        /// <returns></returns>
        [Route("GetListJobtitleByStructure")]
        [HttpGet]
        public object GetListJobtitleByStructure(string id_cv, string structureid)
        {
            string sqlq = "", whereStr = " tbl_chucdanh.disable=0", orderByStr = "tenchucdanh";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    if (!"0".Equals(structureid))
                    {
                        whereStr += " and cocauid=@cocauid";
                        Conds.Add("cocauid", structureid);
                    }
                    if (!"0".Equals(id_cv))
                    {
                        whereStr += " and id_cv = @id_cv";
                        Conds.Add("id_cv", id_cv);
                    }
                    sqlq = $@"select id_row, tenchucdanh from tbl_chucdanh
                        where { whereStr } order by { orderByStr}";
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID = r["id_row"],
                                                          Title = r["tenchucdanh"]
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Load chức vụ theo cơ cấu tổ chức, chức danh
        /// </summary>
        /// <param name="id_cv">"": Lấy danh sách chức vụ theo cơ cấu tổ chức</param>
        /// <param name="structureid"></param>
        /// <returns></returns>
        [Route("GetListJobtitleByStructure_All")]
        [HttpGet]
        public object GetListJobtitleByStructure_All(string id_cv, string structureid)
        {
            string sqlq = "", whereStr = " tbl_chucdanh.disable=0", orderByStr = "tenchucdanh";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    if (!"".Equals(structureid))
                    {
                        whereStr += " and (rowid=@cocauid or parentid=@cocauid)";
                        Conds.Add("cocauid", structureid);
                    }
                    if (!string.IsNullOrEmpty(id_cv))
                    {
                        whereStr += " and id_cv = @id_cv";
                        Conds.Add("id_cv", id_cv);
                    }
                    sqlq = $@"select id_row, tenchucdanh from tbl_chucdanh join tbl_cocautochuc on cocauid = tbl_cocautochuc.rowid
                        where { whereStr } order by { orderByStr}";
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID = r["id_row"],
                                                          Title = r["tenchucdanh"]
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Lấy danh sách nhân viên theo chức danh
        /// </summary>
        /// <param name="Id_Chucdanh"></param>
        /// <returns></returns>
        [Route("GetListStaffbyPosition")]
        [HttpGet]
        public object GetListStaffbyPosition(string Id_Chucdanh)
        {
            string sqlq = "", whereStr = " thoiviec=0 and disable=0", orderByStr = "ten";
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    if (!"0".Equals(Id_Chucdanh))
                    {
                        whereStr += " and Id_Chucdanh = @Id_Chucdanh";
                        Conds.Add("Id_Chucdanh", Id_Chucdanh);
                    }
                    sqlq = $@"select Id_NV, holot + ' ' + ten as hoten from tbl_Nhanvien where { whereStr } order by { orderByStr}";
                    DataTable dt = cnn.CreateDataTable(sqlq, Conds);
                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          Id_NV = r["Id_NV"],
                                                          hoten = r["hoten"],
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);

            }
        }
        /// /// <summary>
        /// Lấy danh sách nhóm chức danh theo chức danh
        /// </summary>
        /// <returns></returns>

        [Route("GetListNhomChucDanhTheoChucDanh")]
        [HttpGet]
        public object GetListNhomChucDanhTheoChucDanh(string id_cv)
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string select = "select nhom from chucvu where (where)";
                    SqlConditions cond = new SqlConditions();
                    cond.Add("id_cv", id_cv);
                    DataTable dt = new DataTable();
                    dt = cnn.CreateDataTable(select, "(where)", cond);
                    if (dt.Rows.Count <= 0)
                    {
                        //return 0;
                    }
                    int nhom = 0;
                    if (!int.TryParse(dt.Rows[0][0].ToString(), out nhom))
                    {
                        //return 0;
                    }
                    Conds.Add("disable", 0);
                    if (!string.IsNullOrEmpty(id_cv)) Conds.Add("Id_cv", id_cv);

                    dt = cnn.CreateDataTable(@"select id_row, tenchucdanh, id_cv, tentienganh from chucdanh where (where) order by tenchucdanh", "(where)", Conds);

                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   id_row = r["id_row"],
                                   tenchucdanh = r["tenchucdanh"],
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// /// <summary>
        /// Lấy danh sách nhóm chức danh
        /// </summary>
        /// <returns></returns>
        [Route("GetListOnlyNhomChucDanh")]
        [HttpGet]
        public object GetListOnlyNhomChucDanh(string id_nhomcd)
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {

                    Conds.Add("disable", 0);
                    if (!string.IsNullOrEmpty(id_nhomcd)) Conds.Add("id_row", id_nhomcd);

                    DataTable dt = cnn.CreateDataTable(@"select id_row, tenchucdanh, id_cv, tentienganh from chucdanh where (where) order by tenchucdanh desc", "(where)", Conds);

                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   id_row = r["id_row"],
                                   tenchucdanh = r["tenchucdanh"],
                                   tentienganh = r["tentienganh"],
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// /// <summary>
        /// Lấy danh sách câp quản lý
        /// </summary>
        /// <returns></returns>
        [Route("GetListCapQuanLy")]
        [HttpGet]
        public object GetListCapQuanLy()
        {
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(@"select rowid, title, range from dm_capquanly order by range desc");
                    var data = from r in dt.AsEnumerable()
                               select new
                               {
                                   rowid = r["rowid"],
                                   title = r["title"],
                               };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// /// <summary>
        /// Lấy danh sách chế độ làm việc
        /// </summary>
        /// <returns></returns>
        [Route("GetListShift")]
        [HttpGet]
        public object GetListShift()
        {
            ErrorModel error = new ErrorModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(@"select id_row, title from Tbl_chedolamviec where disable = 0 order by title");

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID_Row = r["id_row"],
                                                          Religion = r["title"],
                                                      });
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region dv hành chính
        /// <summary>
        /// Lấy danh sách tỉnh
        /// </summary>
        /// <returns></returns>
        [Route("GetListProvinces")]
        [HttpGet]
        public object GetListProvinces()
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable(@"SELECT Id_row, ProvinceName 
                                from DM_Provinces where ((disable is null) or (disable=0)) 
                                order by ProvinceName", Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          id_row = r["Id_row"],
                                                          Province = r["ProvinceName"],
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Lấy danh sách quận huyện theo tỉnh
        /// </summary>
        /// <returns></returns>
        [Route("GetListDistrictByProvinces")]
        [HttpGet]
        public object GetListDistrictByProvinces(string id_provinces)
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                Conds.Add("ProvinceID", id_provinces);
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable("select * from DM_District where (where) and ((CustemerID is NULL)) " +
                        "order by DistrictName", "(where)", Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID_Row = r["Id_row"],
                                                          District = r["DistrictName"],
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Lấy danh sách phường xã theo quận huyện
        /// </summary>
        /// <returns></returns>
        [Route("GetListWardByDistrict")]
        [HttpGet]
        public object GetListWardByDistrict(string id_district)
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                Conds.Add("DistrictID", id_district);

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable("select * from DM_Wards where (where) order by Title", "(where)", Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          ID_Row = r["RowID"],
                                                          Ward = r["Title"],
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Lấy danh sách khóm, ấp theo phường xã
        /// </summary>
        /// <returns></returns>
        [Route("GetListKhomApByWard")]
        [HttpGet]
        public object GetListKhomApByWard(string id_ward)
        {
            SqlConditions Conds = new SqlConditions();
            ErrorModel error = new ErrorModel();
            string Token = lc.GetHeader(Request);
            LoginData loginData = lc._GetInfoUser(Token);
            if (loginData == null)
                return JsonResultCommon.DangNhap();
            try
            {
                Conds.Add("WardID", id_ward);

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    DataTable dt = cnn.CreateDataTable("select * from DM_KhomAp where (where) order by Title", "(where)", Conds);

                    return JsonResultCommon.ThanhCong(from r in dt.AsEnumerable()
                                                      select new
                                                      {
                                                          id = r["RowID"],
                                                          title = r["Title"],
                                                      });
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region danh mục
        /// <summary>
        /// dm diện chỉnh hình lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("dien-chinh-hinh-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DienChinhHinhLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_DienChinhHinh where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by DienChinhHinh";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["DienChinhHinh"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// dm dân tộc lite
        /// </summary>
        /// <returns></returns>
        [Route("dan-toc-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DanTocLite()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_DanToc";
                    sql += " order by Priority";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id_row"].ToString()),
                                                    title = pb["tendantoc"].ToString()
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// dm tôn giáo lite
        /// </summary>
        /// <returns></returns>
        [Route("ton-giao-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> TonGiaoLite()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_TonGiao";
                    sql += " order by TenTonGiao";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id_row"].ToString()),
                                                    title = pb["Tentongiao"].ToString(),
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm đối tượng ncc lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("doi-tuong-nhan-qua-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DoiTuongNhanQuaLite(bool Locked = false, bool include_muc = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select * from DM_DoiTuongNhanQua dt where dt.Disabled=0 ";
                    if (!Locked)
                        sql += " and dt.Locked=0 ";
                    sql += " order by DoiTuong";
                    if (include_muc)
                    {
                        sql += ";select Id, NguonKinhPhi from DM_NguonKinhPhi where Disabled=0";
                        sql += ";select Id, NhomLeTet from DM_NhomLeTet where Disabled = 0";
                        sql += ";select de.* from DM_DoiTuongNhanQua_Detail de where de.disabled=0";
                    }
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];
                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["DoiTuong"].ToString(),
                                                    disabled = pb["Locked"],
                                                    data = !include_muc ? null :
                                                    from nh in ds.Tables[2].AsEnumerable()
                                                    from ng in ds.Tables[1].AsEnumerable()
                                                    select new
                                                    {
                                                        Id_NhomLeTet = nh["Id"],

                                                        Id_NguonKinhPhi = ng["Id"],

                                                        SoTien = getTien(ds.Tables[3], pb["Id"], nh["id"], ng["id"])
                                                    }
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private object getTien(DataTable dataTable, object dt, object nh, object ng)
        {
            var find = dataTable.AsEnumerable().Where(x => x["Id_DoiTuongNhanQua"].ToString() == dt.ToString() && x["Id_NhomLeTet"].ToString() == nh.ToString() && x["Id_NguonKinhPhi"].ToString() == ng.ToString()).Select(x => x["SoTien"]).FirstOrDefault();
            return find;
        }

        /// <summary>
        /// dm đối tượng ncc lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <param name="Id_LoaiHoSo">Lọc theo loại hồ sơ hay k?</param>
        /// <returns></returns>
        [Route("doi-tuong-ncc-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DoiTuongNCCLite(bool Locked = false, long Id_LoaiHoSo = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select * from DM_DOITUONGNCC dt where dt.Disabled=0";
                    if (Id_LoaiHoSo > 0)
                        sql = @" select dt.* from DM_DOITUONGNCC dt
join Const_LoaiHoSo_DoiTuong l on dt.Id = l.Id_DoiTuong
 where dt.Disabled = 0 and l.Id_LoaiHoSo = " + Id_LoaiHoSo;
                    if (!Locked)
                        sql += " and dt.Locked=0 ";
                    sql += " order by DoiTuong";
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];
                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["DoiTuong"].ToString(),
                                                    disabled = pb["Locked"],
                                                    data = new
                                                    {
                                                        IsThanNhan = pb["IsThanNhan"],
                                                    }
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm đối tượng ncc lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <param name="Id_LoaiHoSo">Lọc theo loại hồ sơ hay k?</param>
        /// <returns></returns>
        [Route("doi-tuong-ncc-loai-hs-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DoiTuongNCCLoaiHSLite(bool Locked = false, string tendt = "")
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select * from DM_DOITUONGNCC dt where dt.Disabled=0";
                    if (!Locked)
                        sql += " and dt.Locked=0 ";

                    if (!string.IsNullOrEmpty(tendt))
                    {
                        sql += $"and DoiTuong like N'%{tendt}%'";
                    }
                    sql += " order by Priority";

                    sql += @" select dm.LoaiHoSo, dm.Id, l.Id_DoiTuong from Const_LoaiHoSo dm
join Const_LoaiHoSo_DoiTuong l on dm.Id = l.Id_LoaiHoSo
where dm.Disabled = 0 ";


                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];
                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["DoiTuong"].ToString(),
                                                    disabled = pb["Locked"],
                                                    data = from r in ds.Tables[1].AsEnumerable()
                                                           where r["Id_DoiTuong"].ToString() == pb["Id"].ToString()
                                                           orderby r["LoaiHoSo"]
                                                           select new LiteModel()
                                                           {
                                                               id = int.Parse(r["Id"].ToString()),
                                                               title = r["LoaiHoSo"].ToString(),
                                                           }
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private object getGiayTo(DataRow v, DataTable dtGiayTo, DataTable dtHoSoGiayTo)
        {
            if (v == null || v["Id"] == null)
                return new List<string>();
            var asGiayTo = dtGiayTo.AsEnumerable();
            var data = (from gt in asGiayTo
                        join hs_gt in dtHoSoGiayTo.AsEnumerable() on gt["Id"] equals hs_gt["Id_LoaiGiayTo"]
                        where hs_gt["Id_LoaiHoSo"].ToString() == v["Id"].ToString()
                        select new { id = gt["Id"], title = gt["LoaiGiayTo"], IsRequired = (bool)hs_gt["IsRequired"] }).ToList();
            if (v["Id_LoaiGiayTo"] != null)
            {
                var find = data.Where(x => x.id.ToString() == v["Id_LoaiGiayTo"].ToString()).FirstOrDefault();
                if (find == null)
                {
                    var temp = asGiayTo.Where(x => x["Id"].ToString() == v["Id_LoaiGiayTo"].ToString()).Select(x => new { id = x["Id"], title = x["LoaiGiayTo"], IsRequired = false }).FirstOrDefault();
                    if (temp != null)
                        data.Add(temp);
                }
            }
            if (v["Id_LoaiGiayTo_CC"] != null)
            {
                var find = data.Where(x => x.id.ToString() == v["Id_LoaiGiayTo_CC"].ToString()).FirstOrDefault();
                if (find == null)
                {
                    var temp = asGiayTo.Where(x => x["Id"].ToString() == v["Id_LoaiGiayTo_CC"].ToString()).Select(x => new { id = x["Id"], title = x["LoaiGiayTo"], IsRequired = false }).FirstOrDefault();
                    if (temp != null)
                        data.Add(temp);
                }
            }
            return data;
        }

        /// <summary>
        /// dm const loại hồ sơ
        /// </summary>
        /// <param name="include_giayto">Lấy các giấy tờ thuộc hồ sơ - nếu k thì lấy biểu mẫu</param>
        /// <returns></returns>
        [Route("const-loai-ho-so-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> ConstLoaiHoSoLite(bool include_giayto = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Const_LoaiHoSo where Disabled=0";
                    sql += " order by LoaiHoSo";

                    if (!include_giayto)
                    {
                        sql += @";select bm.*, dt.Id_LoaiHoSo, dt.Id_DoiTuong from tbl_bieumau bm 
join Const_LoaiHoSo_BieuMau c on bm.Id = c.Id_BieuMau
join Const_LoaiHoSo_DoiTuong dt on dt.Id = c.Id_LoaiHoSo_DT
 where bm.disabled = 0";
                        if (loginData.Capcocau == 1)
                            sql += " and IsTinh=1";
                        if (loginData.Capcocau == 2)
                            sql += " and IsHuyen=1";
                        if (loginData.Capcocau == 3)
                            sql += " and IsXa=1";
                    }
                    else
                    {
                        sql += " select * from DM_LoaiGiayTo where Disabled=0 and Locked=0";
                        sql += " select * from Const_LoaiHoSo_GiayTo";
                    }
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];
                    List<LiteModel> _parents = new List<LiteModel>();
                    if (!include_giayto)
                        _parents = (from lhs in dt.AsEnumerable()
                                    select new LiteModel()
                                    {
                                        id = int.Parse(lhs["Id"].ToString()),
                                        title = lhs["LoaiHoSo"].ToString(),
                                        data = from r in ds.Tables[1].AsEnumerable()
                                               where r["Id_LoaiHoSo"] != DBNull.Value && r["Id_LoaiHoSo"].ToString() == lhs["Id"].ToString()
                                               select new
                                               {
                                                   id = r["Id"],
                                                   id_dt = r["Id_DoiTuong"],
                                                   title = r["BieuMau"],
                                                   Id_CanCu = r["Id_CanCu"],
                                               },
                                    }).ToList();
                    else
                        _parents = (from pb in dt.AsEnumerable()
                                    select new LiteModel()
                                    {
                                        id = int.Parse(pb["Id"].ToString()),
                                        title = pb["LoaiHoSo"].ToString(),
                                        data = new
                                        {
                                            Id_DoiTuongNCC = pb["Id_DoiTuongNCC"],
                                            GiayTos = !include_giayto ? null : getGiayTo(pb, ds.Tables[1], ds.Tables[2])
                                        }
                                    }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }


        /// <summary>
        /// dm const loại hồ sơ - đt
        /// </summary>
        /// <returns></returns>
        [Route("const-loai-ho-so-dt-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> ConstLoaiHoSo_DTLite()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Const_LoaiHoSo where Disabled=0";
                    sql += " order by LoaiHoSo";

                    sql += @";select dt.Id, dt.DoiTuong, lhs.Id_LoaiHoSo, lhs.Id as Id_LoaiHS_DT from DM_DoiTuongNCC dt
join Const_LoaiHoSo_DoiTuong lhs on lhs.Id_DoiTuong = dt.Id;
select dm.Id, dm.BieuMau, bm.Id_LoaiHoSo_DT from Tbl_BieuMau dm
join Const_LoaiHoSo_BieuMau bm on bm.Id_BieuMau = dm.Id";
                    if (loginData.Capcocau == 1)
                        sql += " and IsTinh=1";
                    if (loginData.Capcocau == 2)
                        sql += " and IsHuyen=1";
                    if (loginData.Capcocau == 3)
                        sql += " and IsXa=1";
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];
                    var _parents = (from lhs in dt.AsEnumerable()
                                select new 
                                {
                                    id = int.Parse(lhs["Id"].ToString()),
                                    title = lhs["LoaiHoSo"].ToString(),
                                    data = from dt in ds.Tables[1].AsEnumerable()
                                           where dt["Id_LoaiHoSo"].ToString() == lhs["Id"].ToString()
                                           select new
                                           {
                                               id = dt["Id"],
                                               title = dt["DoiTuong"],
                                               biemaus = from bm in ds.Tables[2].AsEnumerable()
                                                         where bm["Id_LoaiHoSo_DT"].ToString() == dt["Id_LoaiHS_DT"].ToString()
                                                         select new
                                                         {
                                                             id = bm["Id"],
                                                             title = bm["BieuMau"]
                                                         }
                                           }
                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm const loại quyết định
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("const-loai-quyet-dinh-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> ConstLoaiQuyetDinhLite(bool includeBM = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Const_LoaiQuyetDinh where Disabled=0";
                    sql += " order by LoaiQuyetDinh";
                    if (includeBM)
                    {
                        sql += ";select * from tbl_bieumau where disabled=0";
                        if (loginData.Capcocau == 1)
                            sql += " and IsTinh=1";
                        if (loginData.Capcocau == 2)
                            sql += " and IsHuyen=1";
                        if (loginData.Capcocau == 3)
                            sql += " and IsXa=1";
                    }
                    DataSet ds = cnn.CreateDataSet(sql);
                    DataTable dt = ds.Tables[0];

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["LoaiQuyetDinh"].ToString(),
                                                    data = !includeBM ? null : from r in ds.Tables[1].AsEnumerable()
                                                                               where r["Id_LoaiQuyetDinh"] != DBNull.Value && r["Id_LoaiQuyetDinh"].ToString() == pb["Id"].ToString()
                                                                               select new
                                                                               {
                                                                                   id = r["Id"],
                                                                                   title = r["BieuMau"],
                                                                                   Id_CanCu = r["Id_CanCu"],
                                                                               }
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

//        /// <summary>
//        /// dm const loại trợ cấp
//        /// </summary>
//        /// <param name="id">id đối tượng: dm_doituongncc.id</param>
//        /// <returns></returns>
//        [Route("const-loai-tro-cap-lite")]
//        [Authorize]
//        [HttpGet]
//        public BaseModel<object> ConstLoaiTroCapLite(int id = 0, bool includeBM = false)
//        {
//            try
//            {
//                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
//                {
//                    string sql = @"select tc.*, pa.TienTroCap as TienTroCapCha from Const_LoaiTroCap tc 
//left join Const_LoaiTroCap pa on tc.Id_Parent = pa.Id where tc.Disabled=0";
//                    if (id > 0)
//                        sql += " and (tc.Id_LoaiHoSo is null or tc.Id_LoaiHoSo=" + id + ")";
//                    sql += " order by tc.TroCap";
//                    DataTable dt = cnn.CreateDataTable(sql);
//                    List<LiteModel> _parents = (from tc in dt.AsEnumerable()
//                                                select new LiteModel()
//                                                {
//                                                    id = int.Parse(tc["Id"].ToString()),
//                                                    title = tc["TroCap"].ToString(),
//                                                    data = new
//                                                    {
//                                                        TienTroCap = LoaiTroCapController.tinhtrocap(tc),
//                                                        PhuCap = tc["PhuCap"],
//                                                        TroCapNuoiDuong = tc["TroCapNuoiDuong"],
//                                                        TienMuaBao = tc["TienMuaBao"],
//                                                        Id_LoaiHoSo = tc["Id_LoaiHoSo"],
//                                                        SoThangTC = tc["SoThangTC"],
//                                                    }
//                                                }).ToList();
//                    return JsonResultCommon.ThanhCong(_parents);
//                }

//            }
//            catch (Exception ex)
//            {
//                return JsonResultCommon.Exception(ex, ControllerContext);
//            }
//        }

        /// <summary>
        /// dm const loại trợ cấp không có cha
        /// </summary>
        /// <returns></returns>
        [Route("const-loai-tro-cap-cha-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> ConstLoaiHoSoChaLite(long Id_LoaiHoSo = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Const_LoaiTroCap where Disabled=0 and Id_parent is null";
                    if (Id_LoaiHoSo > 0)
                        sql += " and Id_LoaiHoSo=" + Id_LoaiHoSo;
                    sql += " order by TroCap";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["TroCap"].ToString(),
                                                    data = new
                                                    {
                                                        TienTroCap = pb["TienTroCap"],
                                                        PhuCap = pb["PhuCap"]
                                                    }
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm dụng cụ chỉnh hình lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("dung-cu-chinh-hinh-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DungCuChinhHinhLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_DungCuChinhHinh where Disabled=0 order by DungCu";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by DungCu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["DungCu"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm loại điều dưỡng lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("loai-dieu-duong-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> LoaiDieuDuongLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_LoaiDieuDuong where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by LoaiDieuDuong";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["LoaiDieuDuong"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm nhóm lễ tết lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("nhom-le-tet-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> NhomLeTetLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_NhomLeTet where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by NhomLeTet";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["NhomLeTet"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm qh gia đình lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("quan-he-gia-dinh-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> QuanHeGiaDinhLite(bool ByQua, bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_QHGiaDinh where Disabled=0 and ByQua=" + (ByQua ? "1" : "0");
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by QHGiaDinh";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["QHGiaDinh"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm loại giấy tờ lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("loai-giay-to-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> LoaiGiayToLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_LoaiGiayTo where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by LoaiGiayTo";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["LoaiGiayTo"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm loại giấy tờ lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("giay-to-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> GiayToLite(long Id_LoaiGiayTo = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select Id, Id_NCC, Id_LoaiGiayTo, GiayTo, So, NoiCap, NgayCap, FileDinhKem, src " +
                        "from Tbl_GiayTo " +
                        "where Disabled=0";
                    if (Id_LoaiGiayTo > 0)
                        sql += " and Id_LoaiGiayTo= " + Id_LoaiGiayTo + "";
                    sql += " order by GiayTo";
                    DataTable dt = cnn.CreateDataTable(sql);
                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["GiayTo"].ToString(),
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// dm chế độ ưu đãi lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("che-do-uu-dai-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> CheDoUuDaiLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_CheDoUuDai where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by CheDoUuDai";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["CheDoUuDai"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// dm căn cứ lite
        /// </summary>
        /// <returns></returns>
        [Route("can-cu-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> CanCuLite()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Tbl_CanCu where Disabled=0";
                    sql += " order by SoCanCu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["SoCanCu"].ToString(),
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// biểu mẫu lite
        /// </summary>
        /// <param name="loai">1: hồ sơ, 2 công nhận, 3 trợ cấp, 4:khác</param>
        /// <returns></returns>
        [Route("bieu-mau-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> BieuMauLite(int loai = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Tbl_BieuMau where Disabled=0";
                    if (loai > 0)
                        sql += " and loai=" + loai;
                    sql += " order by So";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["So"].ToString() + " - " + pb["BieuMau"].ToString(),
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }


        /// <summary>
        /// Danh sách 
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [Route("Get_DSBieuMau")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> Get_DSBieuMau([FromQuery] QueryParams query)
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
                    var Conds = new SqlConditions();
                    string sqlq = @"select dm.* from Tbl_BieuMau dm
join Tbl_CanCu cc on dm.Id_CanCu=cc.Id
where dm.Disabled=0 and cc.Disabled=0";
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        sqlq += " and (dm.BieuMau like @kw or dm.So like @kw)";
                        Conds.Add("kw", "%" + query.filter["keyword"] + "%");
                    }
                    var dt = cnn.CreateDataTable(sqlq, Conds);
                    if (cnn.LastError != null || dt == null)
                    {
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    var temp = dt.AsEnumerable();
                    #region Sort/filter
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>{
                        { "BieuMau","BieuMau" },
                        { "So","So" },
                    };
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                    {
                        if ("asc".Equals(query.sortOrder))
                            temp = temp.OrderBy(x => x[sortableFields[query.sortField]]);
                        else
                            temp = temp.OrderByDescending(x => x[sortableFields[query.sortField]]);
                    }
                    var data = (from r in temp
                                select new
                                {

                                    Id = r["Id"],

                                    BieuMau = r["BieuMau"],

                                }).ToList();
                    #endregion
                    bool allowEdit = false;
                    int total = data.Count();
                    if (total == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, allowEdit);
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
                    return JsonResultCommon.ThanhCong(data, pageModel, allowEdit);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion
        #region quà tết
        /// <summary>
        /// dm mức quà lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("muc-qua-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> MucQuaLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Tbl_MucQua where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by MucQua";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["MucQua"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// dm đợt tặng quà lite
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("dot-tang-qua-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> DotTangQuaLite(bool Locked = false, int Nam = 0)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from Tbl_DotTangQua where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    if (Nam > 0)
                        sql += " and Nam= " + Nam;
                    sql += " order by Nam, DotTangQua";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = Nam > 0 ? pb["DotTangQua"].ToString() : (pb["Nam"].ToString() + "-" + pb["DotTangQua"].ToString()),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region comment, like
        /// <summary>
        /// DS emotion + ds account để replace vào comment
        /// </summary>
        /// <returns></returns>
        [Route("get-dictionary")]
        [HttpGet]
        public object getDictionary()
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                string domain = _config.LinkAPI;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from tbl_emotion";
                    sql += ";select username, fullname as hoten from DPS_User where deleted=0";
                    sql += ";select * from tbl_like_icon where disabled=0";
                    DataSet ds = cnn.CreateDataSet(sql);
                    if (cnn.LastError != null || ds == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    var emotions = new List<object>();
                    foreach (DataRow r in ds.Tables[0].Rows)
                    {
                        string folderName = Constant.RootUpload + r["path"].ToString();
                        string Base_Path = Path.Combine(_hostingEnvironment.ContentRootPath, folderName);
                        var dr = new DirectoryInfo(Base_Path);
                        FileInfo[] files = dr.GetFiles();
                        var temp = (from f in files
                                    select new
                                    {
                                        key = ":" + f.Name.Split('.')[0] + ":",
                                        value = domain + folderName + f.Name
                                    }).ToList();
                        emotions.AddRange(temp);
                    }
                    var accounts = ds.Tables[1].AsEnumerable().Select(x => new
                    {
                        key = "@" + x["username"],
                        value = x["hoten"]
                    }).ToList();
                    var data = new
                    {
                        emotions = emotions,
                        accounts = accounts,
                        icons = ds.Tables[2].AsEnumerable().Select(x => new
                        {
                            id_row = x["id_row"],
                            title = x["title"],
                            icon = "assets/media/icons/" + x["icon"],
                        })
                    };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        [Route("lite_emotion")]
        [HttpGet]
        public object LiteEmotion(int id = 0, string keyword = "")
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from tbl_emotion";
                    if (id > 0)
                        sql += " where id_row=" + id;
                    DataTable dt = cnn.CreateDataTable(sql);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    string domain = _config.LinkAPI;
                    var data = new List<object>();
                    foreach (DataRow r in dt.Rows)
                    {
                        string folderName = Constant.RootUpload + r["path"].ToString();
                        string Base_Path = Path.Combine(_hostingEnvironment.ContentRootPath, folderName);
                        var dr = new DirectoryInfo(Base_Path);
                        FileInfo[] files = dr.GetFiles();
                        data.Add(new
                        {
                            id_row = r["id_row"],
                            title = r["title"],
                            icons = from f in files
                                    where string.IsNullOrEmpty(keyword) || (!string.IsNullOrEmpty(keyword) && f.Name.Contains(keyword))
                                    select new
                                    {
                                        key = ":" + f.Name.Split('.')[0] + ":",
                                        path = domain + folderName + f.Name
                                    }
                        });
                    }
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region số liệu

        /// <summary>
        /// lite dm loại số liệu
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("loai-so-lieu-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> LoaiSoLieuLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_LoaiSoLieu where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by LoaiSoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["LoaiSoLieu"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// lite dm số liệu
        /// </summary>
        /// <param name="Id_LoaiSoLieu"></param>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("so-lieu-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> SoLieuLite(long Id_LoaiSoLieu = 0, bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_SoLieu where Disabled=0";
                    if (Id_LoaiSoLieu > 0)
                        sql += "and Id_LoaiSoLieu=" + Id_LoaiSoLieu;
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by SoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["SoLieu"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// lite filter
        /// </summary>
        /// <returns></returns>
        [Route("filter-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> FilterLite()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from we_filter where Disabled=0";
                    sql += " order by title";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["id_row"].ToString()),
                                                    title = pb["title"].ToString(),
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// lite filter operator
        /// </summary>
        /// <returns></returns>
        [Route("filter-operator-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> FilterOperatorLite()
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from we_filter_operator";
                    DataTable dt = cnn.CreateDataTable(sql);

                    var data = (from pb in dt.AsEnumerable()
                                select new
                                {
                                    id = pb["CongThuc"].ToString(),
                                    title = pb["PhepToan"].ToString(),
                                    data = new
                                    {
                                        table_name = pb["table_name"].ToString()
                                    }
                                }).ToList();

                    return JsonResultCommon.ThanhCong(data);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        /// <summary>
        /// lite dm nguồn kinh phí
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("nguon-kinh-phi-lite")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> NguonKinhPhiLite(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_NguonKinhPhi where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by NguonKinhPhi";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["NguonKinhPhi"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion
        public static string intToRoman(int num)
        {

            // create 2-dimensional array, each inner array containing
            // roman numeral representations of 1-9 in each respective
            // place (ones, tens, hundreds, etc...currently this handles
            // integers from 1-3999, but could be easily extended)
            var romanNumerals = new List<List<string>>
            {
                new List<string>(){"", "i", "ii", "iii", "iv", "v", "vi", "vii", "viii", "ix" }, // ones

                new List<string>(){"", "x", "xx", "xxx", "xl", "l", "lx", "lxx", "lxxx", "xc" }, // tens

               new List<string>(){"", "c", "cc", "ccc", "cd", "d", "dc", "dcc", "dccc", "cm" }, // hundreds

                new List<string>(){"", "m", "mm", "mmm" } // thousands
            };

            // split integer string into array and reverse array
            List<string> intArr = num.ToString().Split("").Reverse().ToList();
            int len = intArr.Count();
            string romanNumeral = "";
            int i = len - 1;

            // starting with the highest place (for 3046, it would be the thousands
            // place, or 3), get the roman numeral representation for that place
            // and append it to the final roman numeral string
            while (i >= 0)
            {
                int index = int.Parse(intArr[i]);
                romanNumeral += romanNumerals[i][index];
                i--;
            }

            return romanNumeral;

        }

        public static List<string> romanNumerals = new List<string>() { "M", "CM", "D", "CD", "C", "XC", "L", "XL", "X", "IX", "V", "IV", "I" };
        public static List<int> numerals = new List<int>() { 1000, 900, 500, 400, 100, 90, 50, 40, 10, 9, 5, 4, 1 };

        public static string ToRomanNumeral(int number)
        {
            var romanNumeral = string.Empty;
            while (number > 0)
            {
                // find biggest numeral that is less than equal to number
                var index = numerals.FindIndex(x => x <= number);
                // subtract it's value from your number
                number -= numerals[index];
                // tack it onto the end of your roman numeral
                romanNumeral += romanNumerals[index];
            }
            return romanNumeral;
        }

        public static DataTable ToDataTable<T>(IList<T> data)
        {
            PropertyDescriptorCollection properties =
                TypeDescriptor.GetProperties(typeof(T));
            DataTable table = new DataTable();
            foreach (PropertyDescriptor prop in properties)
                table.Columns.Add(prop.Name, Nullable.GetUnderlyingType(prop.PropertyType) ?? prop.PropertyType);
            foreach (T item in data)
            {
                DataRow row = table.NewRow();
                foreach (PropertyDescriptor prop in properties)
                    row[prop.Name] = prop.GetValue(item) ?? DBNull.Value;
                table.Rows.Add(row);
            }
            return table;
        }

        #region Phí số liệu
        /// <summary>
        /// lite dm phí số liệu
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("lite-phi-so-lieu")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> LitePhiSoLieu(bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = "select * from DM_PhiSoLieu where Disabled=0";
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by PhiSoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["PhiSoLieu"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region Lite Số liệu không có parent
        /// <summary>
        /// lite dm phí số liệu
        /// </summary>
        /// <param name="Locked"></param>
        /// <returns></returns>
        [Route("lite-so-lieu-parent-is-null")]
        [Authorize]
        [HttpGet]
        public BaseModel<object> LiteSoLieuParentIsNull(long Id_LoaiSoLieu = 0, bool Locked = false)
        {
            try
            {
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select * from DM_SoLieu sl where Disabled=0 and Id_Parent is null";
                    //"and sl.Id not in (
                    //    select sl2.Id_Parent from DM_SoLieu sl2 where Disabled = 0 and Id_Parent is not null
                    //)";
                    if (Id_LoaiSoLieu > 0)
                        sql += " and Id_LoaiSoLieu=" + Id_LoaiSoLieu;
                    if (!Locked)
                        sql += " and Locked=0 ";
                    sql += " order by SoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                where pb["Id_Parent"] == DBNull.Value
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id"].ToString()),
                                                    title = pb["SoLieu"].ToString(),
                                                    disabled = pb["Locked"],
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

        #region
        /// <summary>
        /// Danh sách Mẫu số liệu theo đơn vị
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("mau-so-lieu-don-vi")]
        public BaseModel<object> MauSoLieuLite(long id = 0, bool locked = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select dv.Id_MauSoLieu,msl.MauSoLieu , msl.Locked, dv.Id_DonVi, dm.DonVi from Tbl_MauSoLieu msl
join Tbl_MauSoLieu_DonVi dv on dv.Id_MauSoLieu = msl.Id
join DM_DonVi dm on dm.Id = dv.Id_DonVi
where msl.Disabled = 0";
                    if (id > 0)
                        sql += " and Id_DonVi=" + id;
                    else
                        sql += "and Id_DonVi=" + loginData.IdDonVi;

                    if (!locked)
                        sql += " and Locked=0 ";
                    sql += " order by msl.MauSoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["Id_MauSoLieu"].ToString()),
                                                    title = pb["MauSoLieu"].ToString(),
                                                    data = new
                                                    {
                                                        Id_DonVi = int.Parse(pb["Id_DonVi"].ToString()),
                                                        DonVi = pb["DonVi"]
                                                    },
                                                    disabled = pb["Locked"]
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// Danh sách Mẫu số liệu
        /// </summary>
        /// <returns></returns>
        [Authorize]
        [HttpGet]
        [Route("mau-so-lieu")]
        public BaseModel<object> MauSoLieuLite(bool locked = false, bool isMauPhong = false)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sql = @"select msl.* from Tbl_MauSoLieu msl where msl.Disabled = 0";
                    if (isMauPhong)
                        sql += " and IdParent is not null ";
                    else
                        sql += " and IdParent is null ";
                    if (!locked)
                        sql += " and Locked=0 ";
                    sql += " order by msl.MauSoLieu";
                    DataTable dt = cnn.CreateDataTable(sql);

                    List<LiteModel> _parents = (from pb in dt.AsEnumerable()
                                                select new LiteModel()
                                                {
                                                    id = int.Parse(pb["id"].ToString()),
                                                    title = pb["MauSoLieu"].ToString() + (isMauPhong ? (" - " + pb["Nam"].ToString()) : ""),
                                                    disabled = pb["Locked"]
                                                }).ToList();

                    return JsonResultCommon.ThanhCong(_parents);
                }

            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion

    }
}
