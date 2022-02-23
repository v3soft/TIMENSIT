using System;
using System.Collections;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Threading.Tasks;
using DpsLibs.Data;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Cors;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.Extensions.Options;
using Timensit_API.Classes;
using Timensit_API.Controllers.Common;
using Timensit_API.Controllers.Users;
using Timensit_API.Models;
using Timensit_API.Models.Common;
using Timensit_API.Models.DanhMuc;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Timensit_API.Controllers.DanhMuc
{
    /// <summary>
    /// Danh mục Đợt tặng quà
    /// Quyền Xem: 22- Quản lý
    /// Quyền sửa: 20-Cập nhật
    /// </summary>
    [ApiController]
    [Route("api/comment")]
    [EnableCors("TimensitPolicy")]
    public class CommentController : ControllerBase
    {
        private readonly IHostingEnvironment _hostingEnvironment;
        private IHubContext<ThongBaoHub> _hub_context;
        List<CheckFKModel> FKs;
        LogHelper logHelper;
        LoginController lc;
        private NCCConfig _config;
        string Name = "Thảo luận";
        public CommentController(IOptions<NCCConfig> configLogin, IHttpContextAccessor accessor, IHostingEnvironment hostingEnvironment, IHubContext<ThongBaoHub> hub_context)
        {
            _hostingEnvironment = hostingEnvironment;
            _hub_context = hub_context;
            _config = configLogin.Value;
            lc = new LoginController();
            logHelper = new LogHelper(configLogin.Value, accessor, hostingEnvironment, 2);
            FKs = new List<CheckFKModel>();
            //FKs.Add(new CheckFKModel
            //{
            //    TableName = "tbl_chucdanh",
            //    PKColumn = "Id_Capquanly",
            //    DisabledColumn = "Disable",
            //    name = "Chức danh trong sơ đồ tổ chức"
            //});
        }

        /// <summary>
        /// ds comment
        /// </summary>
        /// <param name="query"></param>
        /// <returns></returns>
        [HttpGet]
        public object List([FromQuery] QueryParams query)
        {
            if (query == null)
                query = new QueryParams();
            bool Visible = true;
            PageModel pageModel = new PageModel();
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    SqlConditions Conds = new SqlConditions();
                    string dieukienSort = "CreatedDate", dieukien_where = " ";
                    if (string.IsNullOrEmpty(query.filter["object_type"]) || string.IsNullOrEmpty(query.filter["object_id"]))
                        return JsonResultCommon.Custom("Đối tượng bắt buộc nhập");
                    dieukien_where += " and object_type=@object_type and object_id=@object_id";
                    Conds.Add("object_type", query.filter["object_type"]);
                    Conds.Add("object_id", query.filter["object_id"]);

                    if (!string.IsNullOrEmpty(query.filter["LastID"]))
                    {

                    }
                    bool include_cmt = true;
                    if (!string.IsNullOrEmpty(query.filter["include_cmt"]))
                        include_cmt = query.filter["include_cmt"] == "1";

                    if (!string.IsNullOrEmpty(query.filter["id_parent"]))
                    {
                        dieukien_where += " and (id_parent = @id_parent)";
                        dieukien_where = dieukien_where.Replace("@id_parent", query.filter["id_parent"]);
                    }
                    if (!string.IsNullOrEmpty(query.filter["keyword"]))
                    {
                        dieukien_where += " and (comment like '%@keyword%')";
                        dieukien_where = dieukien_where.Replace("@keyword", query.filter["keyword"]);
                    }
                    #region Sort data theo các dữ liệu bên dưới
                    Dictionary<string, string> sortableFields = new Dictionary<string, string>
                        {
                            { "comment", "comment"},
                            { "CreatedBy", "NguoiTao"},
                            { "CreatedDate", "CreatedDate"},
                            { "UpdatedBy", "NguoiSua"},
                            {"UpdatedDate","UpdatedDate" }
                        };
                    #endregion
                    if (!string.IsNullOrEmpty(query.sortField) && sortableFields.ContainsKey(query.sortField))
                        dieukienSort = sortableFields[query.sortField] + ("desc".Equals(query.sortOrder) ? " desc" : " asc");
                    else
                        dieukienSort = "CreatedDate desc";
                    #region Trả dữ liệu về backend để hiển thị lên giao diện
                    string _table = "Tbl_NCC", _objectType = "2";
                    //1: Đợt tặng, 2:hồ sơ, 3: số liệu
                    if (query.filter["object_type"] == "1")
                    { _table = "Tbl_DeXuatTangQua"; _objectType = "1"; }
                    if (query.filter["object_type"] == "3")
                    { _table = "Tbl_NhapSoLieu"; _objectType = "3"; }
                    #region Sql Cũ
//                    string sqlq = $@"select * from (select NULL as Deadline,1 as cmt, c.*,acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, coalesce(tong,0) as reply, NULL as FileDinhKem, NULL as src, 0 as IsTraLai, NULL as nguoi_nhan from tbl_comment c join Dps_User acc on c.CreatedBy=acc.UserID
//left join (select count(*) as tong, id_parent from tbl_comment where disabled=0 group by id_parent) child on child.id_parent=c.id_row where c.disabled=0
//union
//select qt.deadline as Deadline,0 as cmt,Id as id_row,{ _objectType } as object_type, Id as object_id, N'(Gửi duyệt)' as comment, NULL as id_parent,ngay_tao as CreatedDate, nguoi_tao as CreatedBy,0 as Disabled,NULL as UpdatedDate, NULL as UpdatedBy,
//acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, 0 as reply, NULL as FileDinhKem, NULL as src, 0 as IsTraLai, qt.nguoi_nhan from Tbl_NCC ncc
//join quytrinh_begin qt on loai=@object_type and qt.id_phieu=ncc.Id
//join Dps_User acc on qt.nguoi_tao = acc.UserID
//where Id = @object_id
//union
//select ls.Deadline,0 as cmt,ls.id_row, qt.loai as object_type, qt.id_phieu as object_id, concat(cast(IIf(ls.is_final=1,IIF(ls.approved is null,IIF(ls.Status='-1',N'(Trả lại xã)',N'(Thu hồi) '), N'(Kết thúc) '),'') as nvarchar(50)) , cast(IIf(ls.approved=1,N'(Duyệt) ',IIF(ls.approved=0,N'(Trả lại) ','')) as nvarchar(50)) , cast(note as nvarchar(2000))) as comment, NULL as id_parent,ngay_tao as CreatedDate, nguoi_tao as CreatedBy,0 as Disabled,NULL as UpdatedDate, NULL as UpdatedBy,
//acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, 0 as reply, FileDinhKem, src,IIF(ls.approved=1,0,1) as IsTraLai, iif(nguoi_nhan is null and id_quatrinh_return <>0, qt2.checkers, nguoi_nhan) as nguoi_nhan from quytrinh_lichsu ls
//left join quytrinh_quatrinhduyet qt on ls.Id_quatrinh = qt.id_row
//left join quytrinh_quatrinhduyet qt2 on ls.id_quatrinh_return = qt.id_row --lấy thêm checker ở bước ko duyệt từ id return
//join Dps_User acc on ls.nguoi_tao = acc.UserID
//where qt.loai = { _objectType } and qt.id_phieu = @object_id) as u where 1=1" + dieukien_where + "  order by " + dieukienSort;
                    #endregion
                    string sqlq = $@"select * from (select NULL as Deadline,1 as cmt, c.*,acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, coalesce(tong,0) as reply, NULL as FileDinhKem, NULL as src, 0 as IsTraLai, NULL as nguoi_nhan from tbl_comment c join Dps_User acc on c.CreatedBy=acc.UserID
left join (select count(*) as tong, id_parent from tbl_comment where disabled=0 group by id_parent) child on child.id_parent=c.id_row where c.disabled=0
union
select qt.deadline as Deadline,0 as cmt,Id as id_row,{ _objectType } as object_type, Id as object_id, N'(Gửi duyệt)' as comment, NULL as id_parent,ngay_tao as CreatedDate, nguoi_tao as CreatedBy,0 as Disabled,NULL as UpdatedDate, NULL as UpdatedBy,
acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, 0 as reply, NULL as FileDinhKem, NULL as src, 0 as IsTraLai, qt.nguoi_nhan from Tbl_NCC ncc
join quytrinh_begin qt on loai=@object_type and qt.id_phieu=ncc.Id
join Dps_User acc on qt.nguoi_tao = acc.UserID
where Id = @object_id
union
select ls.Deadline ,0 as cmt,ls.id_row, qt.loai as object_type, qt.id_phieu as object_id, concat(cast(IIf(ls.is_final=1,IIF(ls.approved is null,IIF(ls.Status='-1',N'(Trả lại xã)',N'(Thu hồi) '), N'(Kết thúc) '),'') as nvarchar(50)) , cast(IIf(ls.approved=1,N'(Duyệt) ',IIF(ls.approved=0,N'(Trả lại) ','')) as nvarchar(50)) , cast(ls.note as nvarchar(2000))) as comment, NULL as id_parent, ls.ngay_tao as CreatedDate, ls.nguoi_tao as CreatedBy,0 as Disabled,NULL as UpdatedDate, NULL as UpdatedBy,
acc.Avata,acc.UserID,acc.Fullname, acc.PhoneNumber, acc.UserName, 0 as reply, ls.FileDinhKem, ls.src, IIF(ls.approved=1,0,1) as IsTraLai, iif(ls.nguoi_nhan is null and ls.id_quatrinh_return <>0, qt2.checkers, ls.nguoi_nhan) as nguoi_nhan from quytrinh_lichsu ls
left join quytrinh_quatrinhduyet qt on ls.Id_quatrinh = qt.id_row
left join quytrinh_quatrinhduyet qt2 on ls.id_quatrinh_return = qt2.id_row --lấy thêm checker ở bước ko duyệt từ id return
join Dps_User acc on ls.nguoi_tao = acc.UserID
where qt.loai = { _objectType } and qt.id_phieu = @object_id) as u where 1=1" + dieukien_where + "  order by " + dieukienSort;
                    sqlq += ";select att.*, acc.username from Tbl_FileDinhKem att join Dps_User acc on att.UpdatedBy=acc.UserID where disabled=0 and Loai = 3";
                    sqlq += @";select l.*, ico.title, ico.icon, nv.fullname as hoten from tbl_comment_like l 
join tbl_like_icon ico on ico.id_row = l.type
join Dps_User nv on l.createdby = nv.UserID where l.disabled = 0 and ico.disabled = 0";
                    sqlq += ";select UserId, Fullname from Dps_User where deleted=0";
                    sqlq += @";select top(1)cap.title, qt.* from quytrinh_quatrinhduyet qt
join quytrinh_capquanlyduyet cap on qt.id_quytrinh_capquanly = cap.rowid
where id_phieu=@object_id and loai=@object_type and valid  is null
order by priority";
                    DataSet ds = cnn.CreateDataSet(sqlq, Conds);
                    if (cnn.LastError != null || ds == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    DataTable dt = ds.Tables[0];
                    if (dt.Rows.Count == 0)
                        return JsonResultCommon.ThanhCong(new List<string>(), pageModel, Visible);
                    var temp = dt.AsEnumerable();
                    if (!include_cmt)//k lấy bình luận
                        temp = temp.Where(x => x["cmt"].ToString() == "0");
                    int total = temp.Count();
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
                    var ttttt = temp.Where(x => x["id_parent"] == DBNull.Value).Skip((query.page - 1) * query.record).Take(query.record);
                    var asChilds = temp.Where(x => x["id_parent"] != DBNull.Value);
                    ttttt = asChilds.Concat(ttttt);
                    var asU = ds.Tables[3].AsEnumerable();
                    var data = getChild(temp, loginData, ds.Tables[1].AsEnumerable(), ds.Tables[2].AsEnumerable(), asU, null);
                    if (!include_cmt && ds.Tables[4].Rows.Count > 0)
                    {
                        var r = ds.Tables[4].Rows[0];
                        List<string> l = new List<string>();
                        if (r["checkers"] != DBNull.Value)
                            l = r["checkers"].ToString().Split(",").ToList();
                        if (r["checker"] != DBNull.Value)
                            l.Add(r["checker"].ToString());
                        string nguoinhan = "";
                        var find = asU.Where(x => l.Contains(x["UserID"].ToString())).Select(x => x["fullname"].ToString());
                        if (find.Count() > 0)
                            nguoinhan = string.Join(", ", find);
                        var curr = new
                        {
                            title =r["title"],
                            NguoiNhans = new List<string>(),
                            strNguoiNhan = nguoinhan,
                            Deadline = r["Deadline"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["Deadline"]),
                            IsTre = r["Deadline"] == DBNull.Value ? false : (DateTime)r["Deadline"] < DateTime.Now,
                        };
                        return JsonResultCommon.ThanhCong(data, curr, pageModel);
                    }
                    return JsonResultCommon.ThanhCong(data, pageModel, Visible);
                }
                #endregion
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        private object getChild(IEnumerable<DataRow> data, LoginData loginData, IEnumerable<DataRow> asAttachment, IEnumerable<DataRow> asLike, IEnumerable<DataRow> asU, object parent = null)
        {
            string domain = _config.LinkAPI;
            if (parent == null)
                parent = DBNull.Value;
            return from r in data
                   where r["id_parent"].Equals(parent)
                   select parseObject(r, loginData, asAttachment, asLike, asU, data);
        }

        private object parseObject(DataRow r, LoginData loginData, IEnumerable<DataRow> asAttachment, IEnumerable<DataRow> asLike = null, IEnumerable<DataRow> asU = null, IEnumerable<DataRow> data = null)
        {
            string domain = _config.LinkAPI;
            string nguoinhan = "";
            if (asU != null && r["nguoi_nhan"] != DBNull.Value)
            {
                List<string> l = r["nguoi_nhan"].ToString().Split(",").ToList();
                var find = asU.Where(x => l.Contains(x["UserID"].ToString())).Select(x => x["fullname"].ToString());
                if (find.Count() > 0)
                    nguoinhan = string.Join(", ", find);
            }
            return new
            {
                id_row = r["id_row"],
                IsTraLai = r["IsTraLai"],
                cmt = r["cmt"].ToString(),//có phải là comment hay k
                comment = r["comment"],
                ls_att = r["FileDinhKem"] == DBNull.Value ? null : new
                {//quytrinh_lichsu attachment
                    filename = r["FileDinhKem"],
                    path = _config.LinkAPI + Constant.RootUpload + r["src"],
                    icon = UploadHelper.GetIcon(UploadHelper.GetContentType(r["FileDinhKem"].ToString()))
                },
                CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", r["CreatedDate"]),
                CreatedBy = r["CreatedBy"],
                NguoiNhans = new List<string>(),
                strNguoiNhan = nguoinhan,
                AllowEdit = r["cmt"].ToString() == "1" && r["CreatedBy"].ToString() == loginData.Id.ToString(),
                Deadline = r["Deadline"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["Deadline"]),
                IsTre = r["Deadline"] == DBNull.Value ? false : (DateTime)r["Deadline"] < (DateTime)r["CreatedDate"],
                NguoiTao = new
                {
                    id_nv = r["UserID"],
                    hoten = r["fullname"],
                    username = r["username"],
                    mobile = r["PhoneNumber"],
                    image = LiteController.genLinkAvatar(_config.LinkAPI, r["Avata"])
                },
                UpdatedDate = r["UpdatedDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["UpdatedDate"]),
                Attachment = r["cmt"].ToString() == "0" ? null : from dr in asAttachment
                                                                 where r["id_row"].ToString() == dr["id"].ToString()
                                                                 select new
                                                                 {
                                                                     id_row = dr["idrow"],
                                                                     path = _config.LinkAPI + Constant.RootUpload + dr["FileDinhKem"],
                                                                     filename = dr["FileName"],
                                                                     type = dr["type"],
                                                                     isImage = UploadHelper.IsImage(dr["type"].ToString()),
                                                                     icon = UploadHelper.GetIcon(dr["type"].ToString()),
                                                                     size = dr["size"],
                                                                     NguoiTao = dr["username"],
                                                                     CreatedBy = dr["UpdatedBy"],
                                                                     CreatedDate = string.Format("{0:dd/MM/yyyy HH:mm}", dr["UpdatedDate"])
                                                                 },
                Like = asLike == null ? null : (from dr in asLike
                                                where dr["id_comment"].Equals(r["id_row"]) && dr["CreatedBy"].ToString() == loginData.Id.ToString()
                                                select new
                                                {
                                                    type = dr["type"],
                                                    title = dr["title"],
                                                    icon = "assets/media/icons/" + dr["icon"],
                                                }).FirstOrDefault(),
                Likes = asLike == null ? null : from dr in asLike
                                                where dr["id_comment"].Equals(r["id_row"])
                                                group dr by new { a = dr["type"], b = dr["icon"], c = dr["title"] } into g
                                                select new
                                                {
                                                    type = g.Key.a,
                                                    title = g.Key.c,
                                                    icon = "assets/media/icons/" + g.Key.b,
                                                    tong = g.Count(),
                                                    Users = string.Join(Environment.NewLine, from u in g select u["hoten"]),

                                                },
                Children = data == null ? new List<string>() : getChild(data, loginData, asAttachment, asLike, asU, r["id_row"])
            };
        }



        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPost]
        public async Task<object> Insert([FromBody] CommentModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                string strRe = "";
                if (string.IsNullOrEmpty(data.comment))
                    strRe += (strRe == "" ? "" : ",") + "nội dung";
                if (data.object_type <= 0 || data.object_id <= 0)
                    strRe += (strRe == "" ? "" : ",") + "đối tượng";
                if (strRe != "")
                    return JsonResultCommon.BatBuoc(strRe);
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    Hashtable val = new Hashtable();
                    val.Add("comment", data.comment);
                    if (data.id_parent > 0)
                        val.Add("id_parent", data.id_parent);
                    val.Add("object_type", data.object_type);
                    val.Add("object_id", data.object_id);
                    val.Add("CreatedDate", DateTime.Now);
                    val.Add("CreatedBy", iduser);
                    cnn.BeginTransaction();
                    if (cnn.Insert(val, "tbl_comment") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    long idc = long.Parse(cnn.ExecuteScalar("select IDENT_CURRENT('tbl_comment')").ToString());
                    if (data.Attachments != null)
                    {
                        foreach (var item in data.Attachments)
                        {
                            string folder = "Comment";
                            string x = "";
                            if (!UploadHelper.UploadFile(item.strBase64, item.filename, "/dinhkem/" + folder + "/", _hostingEnvironment.ContentRootPath, ref x))
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Custom(UploadHelper.error);
                            }
                            item.src = _config.LinkAPI + x;
                            Hashtable val1 = new Hashtable();
                            val1["Loai"] = 3;
                            val1["Id"] = idc;
                            val1.Add("FileDinhKem", x);
                            val1.Add("filename", UploadHelper.GetFileName(x));
                            val1.Add("type", UploadHelper.GetContentType(x));
                            val1.Add("size", 0);
                            val1.Add("UpdatedDate", DateTime.Now);
                            val1.Add("UpdatedBy", iduser);
                            if (cnn.Insert(val1, "Tbl_FileDinhKem") != 1)
                            {
                                cnn.RollbackTransaction();
                                return JsonResultCommon.Exception(cnn.LastError,ControllerContext);
                            }
                        }
                    }
                    cnn.EndTransaction();
                    data.id_row = idc;
                    string sql = @"select NULL as Deadline,1 as cmt, c.*,acc.UserID,acc.Avata,acc.Fullname, acc.UserName, acc.PhoneNumber, coalesce(tong,0) as reply, NULL as FileDinhKem, NULL as src, 0 as IsTraLai, NULL as nguoi_nhan from tbl_comment c join Dps_User acc on c.CreatedBy=acc.UserID
left join(select count(*) as tong, id_parent from tbl_comment where disabled = 0 group by id_parent) child on child.id_parent = c.id_row where c.disabled = 0 and c.id_row=" + idc;
                    sql += ";select att.*, acc.username from Tbl_FileDinhKem att join Dps_User acc on att.UpdatedBy=acc.UserID where disabled=0 and Loai = 3 and Id=" + idc;
                    DataSet ds = cnn.CreateDataSet(sql);
                    var re = parseObject(ds.Tables[0].Rows[0], loginData, ds.Tables[1].AsEnumerable());


                    var linkto = "";
                    #region setup link dựa vào loại cmt - 1: đề xuất,2 hồ sơ, 3: số liệu,4-quy trình duyệt
                    if (data.object_type == 1)
                    {
                        linkto = "/duyet-de-xuat?showcmt=" + data.object_id;
                    }
                    else if (data.object_type == 2)
                    {
                        linkto = "/chi-tiet-ho-so/" + data.object_id;
                    }
                    else if (data.object_type == 3)
                    {
                        linkto = "/duyet-so-lieu?showcmt=" + data.object_id;
                    }
                    else if (data.object_type == 4)
                    {
                        linkto = "/theo-doi-quy-trinh?showcmt=" + data.object_id;
                    }

                    #endregion

                    #region Notify replay comment 

                    if (data.id_parent > 0)
                    {
                        // get user comment

                        //string query = @"SELECT CreatedBy  FROM we_comment  WHERE (id_row = )" + data.id_parent ;
                        //DataSet user = cnn.CreateDataSet(sql);
                        object nguoitao = cnn.ExecuteScalar(" SELECT CreatedBy  FROM Tbl_Comment  WHERE id_row = " + data.id_parent).ToString();
                        if (nguoitao != null && nguoitao.ToString() != loginData.Id.ToString())
                        {
                            var list = new List<long>();
                            list.Add(long.Parse(nguoitao.ToString()));

                            string NotifyMailtitle1 = loginData.FullName + " đã trả lời bình luận của bạn";
                            string link1 = linkto;
                            ThongBaoHelper.sendThongBao(9, loginData.Id, list, NotifyMailtitle1, link1, _config, _hostingEnvironment, _hub_context);
                        }

                    }
                    #endregion

                    #region Notify nhắc tên người được tag

                    if (data.Users != null)
                    {
                        var lst = data.Users.Select(x => x.id_nv).Where(x => x != loginData.Id).ToList();

                        string NotifyMailtitle = loginData.FullName + " đã nhắc đến bạn trong 1 bình luận";
                        string link = linkto;
                        ThongBaoHelper.sendThongBao(9, loginData.Id, lst, NotifyMailtitle, link, _config, _hostingEnvironment, _hub_context);
                    }

                    #endregion


                    return JsonResultCommon.ThanhCong(re);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="data"></param>
        /// <returns></returns>
        [HttpPut("{id}")]
        public async Task<object> Update(long id, [FromBody] CommentModel data)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                string strRe = "";
                if (string.IsNullOrEmpty(data.comment))
                    strRe += (strRe == "" ? "" : ",") + "nội dung";
                if (strRe != "")
                    return JsonResultCommon.BatBuoc(strRe);

                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    long iduser = loginData.Id;
                    SqlConditions sqlcond = new SqlConditions();
                    sqlcond.Add("id_row", id);
                    sqlcond.Add("disabled", 0);
                    string s = "select * from tbl_comment where (where)";
                    DataTable old = cnn.CreateDataTable(s, "(where)", sqlcond);
                    if (old == null || old.Rows.Count == 0)
                        return JsonResultCommon.KhongTonTai("Bình luận");

                    Hashtable val = new Hashtable();
                    val.Add("comment", data.comment);
                    val.Add("UpdatedDate", DateTime.Now);
                    val.Add("UpdatedBy", iduser);
                    cnn.BeginTransaction();
                    if (cnn.Update(val, sqlcond, "tbl_comment") != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    }
                    cnn.EndTransaction();
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <returns></returns>
        [HttpDelete("{id}")]
        public BaseModel<object> Delete(long id)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                long iduser = loginData.Id;
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select ISNULL((select count(*) from tbl_comment where Disabled=0 and  id_row = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai("Bình luận");
                    sqlq = "update tbl_comment set Disabled=1, UpdatedDate=getdate(), UpdatedBy=" + iduser + " where id_row = " + id;
                    cnn.BeginTransaction();
                    if (cnn.ExecuteNonQuery(sqlq) != 1)
                    {
                        cnn.RollbackTransaction();
                        return JsonResultCommon.Exception(cnn.LastError,ControllerContext);
                    }
                    cnn.EndTransaction();
                    return JsonResultCommon.ThanhCong();
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }

        #region like
        /// <summary>
        /// 
        /// </summary>
        /// <param name="id"></param>
        /// <param name="type">0:unline, tbl_like_icon: 1: like,2 love,3: haha, 4 wow, 5 sad, 6 care, 7 ảngy</param>
        /// <returns></returns>
        [Route("like")]
        [HttpGet]
        public object Like(long id, int type)
        {
            try
            {
                string Token = lc.GetHeader(Request);
                LoginData loginData = lc._GetInfoUser(Token);
                if (loginData == null)
                    return JsonResultCommon.DangNhap();
                using (DpsConnection cnn = new DpsConnection(_config.ConnectionString))
                {
                    string sqlq = "select ISNULL((select count(*) from tbl_comment where id_row = " + id + "),0)";
                    if (long.Parse(cnn.ExecuteScalar(sqlq).ToString()) != 1)
                        return JsonResultCommon.KhongTonTai("Bình luận");
                    sqlq = "select * from tbl_comment_like l join tbl_like_icon ico on l.type=ico.id_row where CreatedBy=" + loginData.Id + " and id_comment=" + id;
                    DataTable dt = cnn.CreateDataTable(sqlq);
                    if (cnn.LastError != null || dt == null)
                        return JsonResultCommon.Exception(cnn.LastError, ControllerContext);
                    bool value = true;
                    int re = 0;
                    Hashtable val = new Hashtable();
                    if (dt.Rows.Count == 0)
                    {
                        val["id_comment"] = id;
                        val["type"] = type;
                        val["CreatedBy"] = loginData.Id;
                        val["CreatedDate"] = DateTime.Now;
                        re = cnn.Insert(val, "tbl_comment_like");
                    }
                    else
                    {
                        value = type == 0;// !(bool)dt.Rows[0]["disabled"];
                        val["disabled"] = value;
                        if (type > 0)
                            val["type"] = type;
                        val["UpdatedBy"] = loginData.Id;
                        val["UpdatedDate"] = DateTime.Now;
                        re = cnn.Update(val, new SqlConditions() { { "id_row", dt.Rows[0]["id_row"] } }, "tbl_comment_like");
                    }
                    if (re <= 0)
                        return JsonResultCommon.Exception(cnn.LastError,ControllerContext);
                    sqlq += @";select l.*, ico.title, ico.icon, nv.fullname as hoten from tbl_comment_like l 
join tbl_like_icon ico on ico.id_row = l.type
join Dps_User nv on l.createdby = nv.UserID where l.disabled = 0 and ico.disabled = 0 and l.id_comment=" + id;
                    DataSet ds = cnn.CreateDataSet(sqlq);
                    DataRow r = ds.Tables[0].Rows[0];
                    var data = new
                    {
                        Like = (bool)r["disabled"] ? null : new
                        {
                            type = r["type"],
                            title = r["title"],
                            icon = "assets/media/icons/" + r["icon"],
                        },
                        Likes = from dr in ds.Tables[1].AsEnumerable()
                                group dr by new { a = dr["type"], b = dr["icon"], c = dr["title"] } into g
                                select new
                                {
                                    type = g.Key.a,
                                    title = g.Key.c,
                                    icon = "assets/media/icons/" + g.Key.b,
                                    tong = g.Count(),
                                    Users = string.Join(Environment.NewLine, from u in g select u["hoten"]),

                                },
                    };
                    return JsonResultCommon.ThanhCong(data);
                }
            }
            catch (Exception ex)
            {
                return JsonResultCommon.Exception(ex, ControllerContext);
            }
        }
        #endregion
    }
    public class CommentModel
    {
        public long id_row { get; set; }
        /// <summary>
        /// 1: work,2 topic
        /// </summary>
        public int object_type { get; set; }
        public int cmt { get; set; } = 1;
        public long object_id { get; set; }
        public string comment { get; set; }
        public long id_parent { get; set; }
        public List<CommentUserModel> Users { get; set; }
        public List<ListImageModel> Attachments { get; set; }
    }

    public class CommentUserModel
    {
        public long id_nv { get; set; } = 0;
        public string hoten { get; set; }
        public string username { get; set; }
    }
}
