using System;
using System.Data;
using DpsLibs.Data;
using System.Collections.Generic;
using System.Collections;
using System.Net.Mail;
using System.IO;
using System.Text.RegularExpressions;
using Microsoft.AspNetCore.Hosting;
using WebCore_API.Models;
using System.Linq;
using Microsoft.EntityFrameworkCore;
using Timensit_API.Models.Common;
using Timensit_API.Models.Process;
using System.Data.SqlClient;
using Timensit_API.Models.DanhMuc;
using Timensit_API.Models;
using System.Transactions;
using Microsoft.EntityFrameworkCore.Storage;
using Timensit_API.Controllers.Common;
using SignalRChat.Hubs;
using Microsoft.AspNetCore.SignalR;

namespace Timensit_API.Classes
{
    /// <summary>
    /// Quy trinh duyet cac don xin: xin nghi phep (1), xin vao tre/ra som (2)
    /// Mổi khi muốn thêm 1 loại đơn xin thì vào thêm 1 dòng với các thông tin trong bảng dm_loaiphieuduyet
    /// và thêm code xét trường hợp đơn đó trong hàm GetReplaceResultMailtemplate, GetReplaceNotifyMailtemplate
    /// </summary>
    public class ApproveProcess
    {
        private EnumerableRowCollection<DMCapQuanLyModel> DmCapquanly;
        private EnumerableRowCollection<TblChucDanhModel> TblChucdanh;
        private EnumerableRowCollection<NguoiDungDPS> TblNhanvien;
        private readonly NGUOICOCONGContext _context;
        private readonly IHostingEnvironment _hostingEnvironment;
        private IHubContext<ThongBaoHub> _hub_context;
        private NCCConfig _config;
        public string ErrorMessage;
        #region
        /// <summary>
        /// Tên field người duyệt trong bảng phiếu cần duyệt
        /// </summary>
        private string CheckerField = "";
        /// <summary>
        /// Tên field duyệt trong bảng phiếu cần duyệt, có thể là chuỗi gán giá trị vd "TinhTrang=1"
        /// </summary>
        private string ValidField = "";
        /// <summary>
        /// Tên field không duyệt trong bảng phiếu cần duyệt, kiểu boolean thì trùng với ValidField, có thể là chuỗi gán giá trị vd "TinhTrang=2"
        /// </summary>
        private string InValidField = "";
        /// <summary>
        /// Tên field ngày duyệt trong bảng phiếu cần duyệt
        /// </summary>
        private string CheckedDateField = "";
        /// <summary>
        /// Tên field ghi chú của người duyệt trong bảng phiếu cần duyệt
        /// </summary>
        private string CheckNoteField = "";
        /// <summary>
        /// Tiêu đề mail thông báo cho người duyệt
        /// </summary>
        public string NotifyMailtitle = "";
        /// <summary>
        /// File tempate gửi mail thông báo cho người duyệt
        /// </summary>
        public string NotifyMailtemplate = "";
        /// <summary>
        /// Tiêu đề mail phiếu đã được duyệt
        /// </summary>
        private string AcceptMailtitle = "";
        /// <summary>
        /// Tiêu đền mail phiếu không được duyệt
        /// </summary>
        private string RejectMailtitle = "";
        /// <summary>
        /// File template mail thông báo phiếu đã được duyệt/không được duyệt
        /// </summary>
        private string CompleteMailtemplate = "";
        /// <summary>
        /// Tiêu đề mail thông báo output
        /// </summary>
        public string OutputMailtitle = "";
        /// <summary>
        /// File tempate gửi mail thông báo output
        /// </summary>
        public string OutputMailtemplate = "";
        /// <summary>
        /// Tên bảng phiếu duyệt (ví dụ Xnp_Requests đối với đơn xin phép, đơn xin vào trễ - ra sớm)
        /// </summary>
        private string TableName = "";
        /// <summary>
        /// Tên field người gửi trong bảng phiếu duyệt
        /// </summary>
        private string SenderField = "";
        /// <summary>
        /// Field khóa chính của bảng phiếu cần duyệt (vd xnp_requests là Id_rq)
        /// </summary>
        private string PrimaryKeyField = "";
        /// <summary>
        /// Tên view chứa dữ liệu để lấy thông tin đưa vào các mail thông báo
        /// </summary>
        private string DataViewName = "";
        /// <summary>
        /// Loại phiếu (dữ liệu lưu trữ trong dm_loaiphieuduyet)
        /// </summary>
        private int Loai;
        /// <summary>
        /// Cập nhật quá trình duyệt vào 1 field text
        /// </summary>
        private bool IsUpdateProcessText;
        /// <summary>
        /// Tên field text sẽ lưu quá trình duyệt kiểu text
        /// </summary>
        public string ProcessTextField;
        /// <summary>
        /// Field status của bảng phiếu duyệt. Quy định chung: 0: Chưa gửi, 1: đã gửi. Áp dụng cho các phiếu không duyệt thì trả lại cấp trước đó cập nhật lại.
        /// Có thể không khai báo
        /// </summary>
        public string StatusField;
        /// <summary>
        /// 0: 1 cấp không duyệt thì xem như phiếu không được duyệt<para/>
        /// 1: 1 cấp không duyệt thì trả về cấp trước đó cho đến khi đến cấp đầu tiên.
        /// 2: theo thiết lập của bước (id_back trong dm_quytrinhduyet)
        /// </summary>
        public string ProcessMethodQT;

        /// <summary>
        /// Loại quy trình xử lý
        ///0: quy trình duyệt 1 nút
        ///1: quy trình duyệt nhiều nút - loại này k cho phép người duyệt hiện tại auto duyệt bước tiếp theo
        /// </summary>
        public string ProcessMethod;

        /// <summary>
        /// phiếu xác định checker
        /// </summary>
        public bool AllowDevChecker;

        /// <summary>
        /// Cho biết bc duyệt hiện tại hoàn tất hay chưa
        /// </summary>
        public bool ssfinal = true;

        /// <summary>
        /// data detail để duyệt ss theo chi tiết, =null thì user duyệt tất cả ss có quyền
        /// </summary>
        public List<checkDetail> CheckDetails = null;

        /// <summary>
        /// Sử dụng SenderField ở table hay view
        /// </summary>
        public bool useTableName = true;

        public string ls_nguoi;
        public string ls_ngay;
        public string ls_ghichu;
        public string ls_tinhtrang;
        public string ls_khoangoai;
        public string ls_table;
        public string Message;
        public long User;
        public MailInfo MInfo;

        #endregion
        public ApproveProcess(int Id_Loai, long NguoiDuyet, IHostingEnvironment hostingEnvironment, NCCConfig configLogin, NGUOICOCONGContext context, IHubContext<ThongBaoHub> hub_context)
        {
            _context = context;
            _hostingEnvironment = hostingEnvironment;
            _hub_context = hub_context;
            _config = configLogin;
            Loai = Id_Loai;
            User = NguoiDuyet;
            var data = _context.QuytrinhLoaiphieuduyet.Where(x => x.Rowid == Id_Loai).FirstOrDefault();
            if (data != null)
            {
                CheckedDateField = data.Checkeddatefield;
                CheckerField = data.Checkerfield;
                ValidField = data.Validfield;
                InValidField = data.Invalidfield;
                CheckNoteField = data.Checknotefield;
                NotifyMailtitle = data.Notifymailtitle;
                NotifyMailtemplate = data.Notifymailtemplate;
                OutputMailtitle = data.Outputmailtitle;
                OutputMailtemplate = data.Outputmailtemplate;
                AcceptMailtitle = data.Acceptmailtitle;
                RejectMailtitle = data.Rejectmailtitle;
                CompleteMailtemplate = data.Completemailtemplate;
                TableName = data.Tablename;
                PrimaryKeyField = data.Primarykeyfield;
                IsUpdateProcessText = data.Isupdateprocesstext.HasValue && data.Isupdateprocesstext.Value;
                ProcessTextField = data.Processtextfield;
                StatusField = data.Statusfield;
                SenderField = data.Senderfield;
                DataViewName = data.Dataviewname;
                ProcessMethod = data.Processmethod.ToString();
                AllowDevChecker = data.AllowDevChecker;

                ls_nguoi = data.LsNguoi;
                ls_ngay = data.LsNgay;
                ls_ghichu = data.LsGhichu;
                ls_tinhtrang = data.LsTinhtrang;
                ls_khoangoai = data.LsKhoangoai;
                ls_table = data.LsTable;
                #region lấy dữ liệu người gửi băng Senderfield và TableName hoặc DataViewName
                string check = @"SELECT column_name FROM information_schema.columns WHERE table_name='{0}' and column_name='{1}';";
                check += "select * from dm_capquanly;";
                check += "select * from tbl_chucdanh where disable=0;";
                check += @"select dps_user.*, cd.Tenchucdanh, cc.Capcocau from dps_user 
join tbl_chucdanh cd on cd.Id_row = dps_user.IdChucVu
join Tbl_Cocautochuc cc on cc.RowID = dps_user.IdDonVi
 where deleted = 0 and active = 1; ";
                check += " select * from Dps_User_DoiTuongNCC";
                DataTable dt = null;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = string.Format(check, TableName, SenderField);
                    IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                    dbDataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    dbDataAdapter.Fill(dataSet);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        dt = dataSet.Tables[0];
                        DmCapquanly = dataSet.Tables[1].AsEnumerable().Select(x => new DMCapQuanLyModel
                        {
                            Rowid = (long)x["RowId"],
                            Range = (int)x["Range"],
                            Summary = x["Summary"].ToString(),
                            Title = x["Title"].ToString()
                        });
                        TblChucdanh = dataSet.Tables[2].AsEnumerable().Select(x => new TblChucDanhModel
                        {
                            IdRow = (long)x["id_row"],
                            StructureID = (long)x["CoCauID"],
                            ID_ChucDanh = x["id_cv"] == DBNull.Value ? 0 : (long)x["id_cv"],
                            TenChucDanh = x["TenChucDanh"].ToString(),
                            IdCapquanly = x["Id_Capquanly"] == DBNull.Value ? 0 : (long)x["Id_Capquanly"]
                        });
                        TblNhanvien = dataSet.Tables[3].AsEnumerable().Select(x => new NguoiDungDPS
                        {
                            UserID = (long)x["UserID"],
                            IdDonVi = long.Parse(x["IdDonVi"].ToString()),
                            FullName = x["Fullname"].ToString(),
                            Email = x["Email"].ToString(),
                            PhoneNumber = x["PhoneNumber"].ToString(),
                            IdChucVu = int.Parse(x["IdChucVu"].ToString()),
                            TenChucVu = x["TenChucDanh"].ToString(),
                            CapCoCau = int.Parse(x["CapCoCau"].ToString()),
                            lstDoiTuongNCC = dataSet.Tables[4].AsEnumerable().Where(dt => dt["UserID"].ToString() == x["UserID"].ToString()).Select(dt => new LiteModel() { id = (long)dt["Id_DoiTuongNCC"] }).ToList()
                        });
                        var UserDV = TblNhanvien.Where(x => x.UserID == User).FirstOrDefault();
                        MInfo = new MailInfo(_config, UserDV.IdDonVi, Loai);
                    }
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        useTableName = false;
                    }
                }
                #endregion
            }
        }

        public enum ApprovalState
        {
            Canceled,
            InProcess,
            Approved
        }

        /// <summary>
        /// Bắt đầu quy trình duyệt
        /// </summary>
        /// <param name="Id_Phieu">Phiếu cần duyệt</param>
        /// <param name="Id_Quytrinh">Quy trình áp dụng</param>
        /// <param name="Id_chucdanh">Nhân viên/đối tượng trong phiếu cần duyệt</param>
        /// <param name="devChecker">dev checker cho bước đầu tiên</param>
        /// <returns></returns>
        public int BeginApproval(long Id_Phieu, int Id_Quytrinh, string Id_chucdanh, List<Checker4Detail> devChecker = null, bool offNoti = false, string messageNotifice = "")
        {
            try
            {
                MInfo.Id = Id_Phieu;
                //Kiểm tra đã tạo quá trình duyệt trước đó chưa
                string FChecker = "";
                var quytrinh = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid == Id_Quytrinh).FirstOrDefault();
                if (quytrinh == null)
                {
                    ErrorMessage = "Quy trình không tồn tại";
                    return 0;
                }
                ProcessMethodQT = quytrinh.Processmethod.ToString();
                //string select = "";
                var Quatrinhduyet = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai).OrderBy(x => x.Priority).FirstOrDefault();
                int rs = 0;
                if (Quatrinhduyet == null)
                {
                    //Trường hợp chưa khởi tạo trước đó (bắt đầu quy trình mới
                    //Lấy cấp quản lý của người gửi
                    int range = 0;
                    var dt = (from chucdanh in TblChucdanh
                              join cap in DmCapquanly on chucdanh.IdCapquanly equals cap.Rowid into item
                              from cap in item.DefaultIfEmpty()
                              where chucdanh.IdRow.ToString() == Id_chucdanh
                              select new
                              {
                                  IdRow = chucdanh.IdRow,
                                  Range = cap == null ? 0 : cap.Range
                              }).ToList();
                    if (dt.Count > 0)
                        int.TryParse(dt[0].Range.ToString(), out range);

                    var TbFirstGroup = (from x in _context.QuytrinhCapquanlyduyet
                                        join cap in DmCapquanly on x.IdCapquanly equals cap.Rowid into item
                                        from cap in item.DefaultIfEmpty()
                                        where x.IdQuytrinh == Id_Quytrinh && x.Disable == false
                                        orderby x.Priority
                                        select new
                                        {
                                            IdQuytrinh = x.IdQuytrinh,
                                            IdCapquanly = x.IdCapquanly,
                                            DuyetSS = x.DuyetSs,
                                            Priority = x.Priority,
                                            Notifyto = x.Notifyto,
                                            Rowid = x.Rowid,
                                            Range = cap.Range,
                                            SoNgayXuLy = getSoNgay(x, null),
                                        }).ToList();
                    if (TbFirstGroup == null || TbFirstGroup.Count == 0)
                        return -1;
                    //BLayer.Employee Emp = new BLayer.Employee(_context);
                    int thutubatdau = 1;
                    bool IsBatdau = false;
                    string listcc = "";
                    for (int i = 0; i < TbFirstGroup.Count; i++)
                    {
                        var Val = new QuytrinhQuatrinhduyet();
                        Val.IdPhieu = Id_Phieu;
                        Val.Loai = Loai;
                        var FRow = TbFirstGroup[i];
                        int Range_capduyet = 0;
                        int.TryParse(FRow.Range.ToString(), out Range_capduyet);
                        if (!IsBatdau)
                            int.TryParse(FRow.Priority.ToString(), out thutubatdau);
                        //Lấy danh sách người cần gửi thông báo trước để lưu lại danh sách tất cả những người nhận mail cc
                        //để sử dụng cho trường các cấp duyệt trong quy trình đều nhỏ hơn cấp của người duyệt
                        if (FRow.Notifyto != null && !"".Equals(FRow.Notifyto.ToString()))
                        {
                            var lst = FRow.Notifyto.Split(",").ToList();
                            var tmp = TblNhanvien.Where(x => lst.Contains(x.UserID.ToString())).Select(x => new
                            {
                                IdNv = x.UserID,
                                Email = x.Email
                            }).ToList();
                            string cc = "";
                            for (int j = 0; j < tmp.Count; j++)
                            {
                                if (!string.IsNullOrEmpty(tmp[j].Email)) cc += ";" + tmp[j].Email;
                            }
                            if (!"".Equals(cc))
                            {
                                cc = cc.Substring(1);
                                Val.Notifyto = cc;
                                listcc += ";" + cc;
                            }
                        }
                        Val.IdQuytrinhCapquanly = FRow.Rowid;
                        Val.Priority = FRow.Priority;
                        int priority = 0;
                        int.TryParse(FRow.Priority.ToString(), out priority);
                        if (priority == thutubatdau)
                        {
                            string Checkers = "";
                            List<Checker4Detail> lstChecker = new List<Checker4Detail>();
                            //Kiểm tra cấp duyệt có > cấp của nhân viên cần duyệt hay không
                            //nếu k thì lấy quản lý trực tiếp của nhân viên làm ng duyệt
                            if ((range == 0) || (Range_capduyet == 0) || Range_capduyet > range)
                            {
                                if (FRow.IdCapquanly == -2 || FRow.IdCapquanly == -3 || FRow.IdCapquanly == -4 || FRow.IdCapquanly == -5)
                                {
                                    if (FRow.IdCapquanly == -4 || FRow.IdCapquanly == -5)
                                    {
                                        //Trường hợp Phiếu xác định người duyệt
                                        lstChecker = devChecker;
                                    }
                                    else
                                    {
                                        //Trường hợp quyền hoặc chức danh cụ thể, lấy mảng checker theo quyền
                                        List<string> temp = GetCheckers2(FRow.Rowid, FRow.IdCapquanly);
                                        lstChecker = (from r in temp
                                                      select new Checker4Detail() { Checkers = new List<string>() { r } }).ToList();
                                    }
                                    if (lstChecker != null && lstChecker.Count > 0)
                                    {
                                        var tt = new List<string>();
                                        foreach (var c in lstChecker)
                                            tt.AddRange(c.Checkers);
                                        var str = string.Join(",", tt);
                                        Checkers = str;
                                        Val.Checker = null;
                                        Val.Checkers = Checkers;
                                    }
                                }
                                else
                                {
                                    Checkers = GetCheckers(FRow.Rowid.ToString(), Id_chucdanh);
                                    if (!"".Equals(Checkers))
                                    {
                                        Val.Checkers = Checkers;
                                        Val.Checker = null;
                                        if (!IsBatdau) FChecker = Checkers;
                                    }
                                }
                            }
                            else
                            {
                                Employee Emp = new Employee(_context);
                                var mans = Emp.GetIDManager(Id_chucdanh);
                                if (mans != null)
                                {
                                    Checkers = string.Join(",", mans);
                                    Val.Checkers = Checkers;
                                    Val.Checker = null;
                                    if (!IsBatdau) FChecker = Checkers;
                                }
                            }
                            if (FRow.DuyetSS)
                                Val.Ss = true;
                            if (FRow.SoNgayXuLy > 0)
                                Val.Deadline = DateTime.Now.AddDays(FRow.SoNgayXuLy);
                            else
                                Val.Deadline = null;
                            _context.QuytrinhQuatrinhduyet.Add(Val);
                            _context.SaveChanges();
                            if (FRow.DuyetSS)//trường hợp có nhiều người duyệt và chọn duyệt ss
                            {
                                //duyệt song song thì lưu tách checker
                                foreach (var user in lstChecker)
                                {
                                    QuytrinhQuatrinhduyetSs val1 = new QuytrinhQuatrinhduyetSs();
                                    val1.Checkers = user.strCheckers;
                                    val1.IdQuatrinh = Val.IdRow;
                                    if (user.IdCt.HasValue)
                                        val1.IdCt = user.IdCt;
                                    if (FRow.SoNgayXuLy > 0)
                                        val1.Deadline = DateTime.Now.AddDays(FRow.SoNgayXuLy);
                                    else
                                        val1.Deadline = null;
                                    _context.QuytrinhQuatrinhduyetSs.Add(val1);
                                    _context.SaveChanges();
                                }
                            }
                            if (string.IsNullOrEmpty(Checkers))
                            {
                                var user = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid == Id_Quytrinh).Select(x => x.Checkernotfoundsendto).FirstOrDefault();
                                //Gửi mail cho người nhận thông báo khi không tìm thấy người duyệt
                                SendThongbaoChonnguoiduyet(user, Id_Phieu);
                            }
                            IsBatdau = true;
                            string s = "insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai, deadline, nguoi_nhan) values (@id_phieu, @nguoi_tao, @ngay_tao, @loai, @deadline, @nguoi)";
                            _context.Database.ExecuteSqlCommand(s,
                new SqlParameter("id_phieu", Id_Phieu),
                new SqlParameter("nguoi_tao", User),
                new SqlParameter("ngay_tao", DateTime.Now),
                new SqlParameter("loai", Loai),
                new SqlParameter("deadline", Val.Deadline.HasValue ? Val.Deadline : (object)DBNull.Value),
                new SqlParameter("nguoi", string.IsNullOrEmpty(Val.Checkers) ? (object)DBNull.Value : Val.Checkers));
                            _context.SaveChanges();
                        }
                        else
                        {
                            if (FRow.DuyetSS)
                                Val.Ss = true;
                            Val.Checker = null;
                            Val.Checkers = null;
                            //if (FRow.SoNgayXuLy > 0)
                            //    Val.Deadline = DateTime.Now.AddDays(FRow.SoNgayXuLy);
                            //else
                            //    Val.Deadline = null;
                            _context.QuytrinhQuatrinhduyet.Add(Val);
                            _context.SaveChanges();
                        }

                        rs++;
                    }
                }
                else
                {
                    var val = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && x.IdQuytrinhCapquanly == Quatrinhduyet.IdQuytrinhCapquanly).FirstOrDefault();
                    if (val == null)
                    {
                        ErrorMessage = "Không tìm thấy quá trình duyệt";
                        return 0;
                    }
                    //Trường hợp đã khởi tạo rồi, áp dụng đối với quy trình cấp quản lý trả lại cho người gửi lúc đầu
                    string Checkers = GetCheckers(Quatrinhduyet.IdQuytrinhCapquanly.ToString(), Id_chucdanh);
                    if ("".Equals(Checkers))
                    {
                        var user = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid == Id_Quytrinh).Select(x => x.Checkernotfoundsendto).FirstOrDefault();
                        //Gửi mail cho người nhận thông báo khi không tìm thấy người duyệt
                        SendThongbaoChonnguoiduyet(user, Id_Phieu);
                        val.Checker = null;
                        val.Checkers = null;
                        //ErrorMessage = "Không tìm thấy người duyệt";
                        //return 0;
                    }
                    else
                    {
                        val.Checker = null;
                        val.Checkers = Checkers;
                        var FRow = _context.QuytrinhCapquanlyduyet.Where(x => x.Rowid == val.IdQuytrinhCapquanly).FirstOrDefault();
                        if (FRow != null)
                        {
                            if (FRow.SoNgayXuLy > 0)
                                val.Deadline = DateTime.Now.AddDays(FRow.SoNgayXuLy);
                            else
                                val.Deadline = null;
                        }
                    }
                    _context.QuytrinhQuatrinhduyet.Update(val);
                    _context.SaveChanges();

                    string s = "insert into quytrinh_begin ( id_phieu, nguoi_tao, ngay_tao, loai, deadline, nguoi_nhan) values (@id_phieu, @nguoi_tao, @ngay_tao, @loai, @deadline, @nguoi)";
                    _context.Database.ExecuteSqlCommand(s,
        new SqlParameter("id_phieu", Id_Phieu),
        new SqlParameter("nguoi_tao", User),
        new SqlParameter("ngay_tao", DateTime.Now),
        new SqlParameter("loai", Loai),
        new SqlParameter("deadline", val.Deadline.HasValue ? val.Deadline : (object)DBNull.Value),
        new SqlParameter("nguoi", string.IsNullOrEmpty(val.Checkers) ? (object)DBNull.Value : val.Checkers));
                    _context.SaveChanges();
                    rs = 1;
                }
                if (rs > 0 && !offNoti)
                {
                    NotifyByEmail(Id_Phieu, messageNotifice);
                }

                return rs;
            }
            catch (Exception ex)
            {
                ErrorMessage = "Có gì đó không đúng, vui lòng thử lại sau";
                return 0;
            }
        }

        public string GetCurrentApprovingUser(int Id_Phieu)
        {
            var nv = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && !x.Valid.HasValue && (x.Checker.HasValue || !string.IsNullOrEmpty(x.Checkers))).OrderBy(x => x.Priority).FirstOrDefault();
            if (nv != null)
                return nv.Checker.HasValue ? nv.Checker.ToString() : nv.Checkers;
            return "";
        }

        /// <summary>
        /// Gửi email thông báo
        /// </summary>
        /// <param name="Id_Phieu">Phiếu cần thông báo</param>
        /// <returns></returns>
        public void NotifyByEmail(long Id_Phieu, string messageNotifice = "")
        {
            if (CheckState(Id_Phieu) != ApprovalState.InProcess)
                return;
            var dt = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && (x.Checker.HasValue || x.ListChecker.Count > 0) && !x.Valid.HasValue).OrderBy(x => x.Priority).FirstOrDefault();
            if (dt != null)
            {
                if (!string.IsNullOrEmpty(NotifyMailtemplate))
                {
                    string TemplateMail = _hostingEnvironment.ContentRootPath + @"\MailTemplate\" + NotifyMailtemplate;
                    Hashtable replace = GetReplaceMail_Notify(Id_Phieu, 1, true);
                    replace.Add("$your$", "");
                    replace.Add("$cuanhanvien_en$", "from " + replace["$FullName$"] ?? "");
                    replace.Add("$cuanhanvien$", "của " + replace["$FullName$"] ?? "");
                    replace.Add("$nguoinhan$", "");
                    MailAddressCollection emailcc = new MailAddressCollection();
                    emailcc = new MailAddressCollection();
                    List<string> listemail = new List<string>();
                    if (!string.IsNullOrEmpty(dt.Notifyto))
                    {
                        listemail = dt.Notifyto.ToString().Split(';').ToList();
                    }
                    for (int j = 0; j < listemail.Count; j++)
                    {
                        if (!"".Equals(listemail[j])) emailcc.Add(listemail[j]);
                    }
                    Employee employee = new Employee(_context);
                    MailAddressCollection To = new MailAddressCollection();

                    string nguoinhan = "";
                    var formatLink = replace["$link$"].ToString().Replace("$id$", Id_Phieu.ToString());

                    if (dt.Checker.HasValue)
                    {
                        List<long> lst = new List<long>();
                        lst.Add(dt.Checker.Value);
                        var nhanvien = TblNhanvien.Where(x => x.UserID.Equals(dt.Checker)).FirstOrDefault();
                        if (nhanvien != null && !string.IsNullOrEmpty(nhanvien.Email))
                        {
                            nguoinhan += (nguoinhan == "" ? "" : ", ") + nhanvien.FullName;
                            To.Add(nhanvien.Email);
                        }

                        ThongBaoHelper.sendThongBao(Loai, User, lst, string.IsNullOrEmpty(messageNotifice) ? NotifyMailtitle : messageNotifice, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);
                        replace["$nguoinhan$"] = nguoinhan;
                    }
                    if (dt.ListChecker.Count > 0)
                    {//gửi nhiều ng thì replace $nguoinhan$=""
                        List<long> lst = new List<long>();
                        foreach (var id_nv in dt.ListChecker)
                        {
                            lst.Add(long.Parse(id_nv));
                            var nhanvien = TblNhanvien.Where(x => x.UserID.ToString() == id_nv).FirstOrDefault();
                            if(nhanvien == null) continue;
                            if (!string.IsNullOrEmpty(nhanvien.Email))
                            {
                                var find = To.Where(x => x.Address == nhanvien.Email).Count();
                                if (find == 0)
                                {
                                    nguoinhan += (nguoinhan == "" ? "" : ", ") + nhanvien.FullName;
                                    To.Add(nhanvien.Email);
                                }
                            }
                        }
                        ThongBaoHelper.sendThongBao(Loai, User, lst, string.IsNullOrEmpty(messageNotifice) ? NotifyMailtitle : messageNotifice, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);
                    }
                    replace["$SysName$"] = _config.SysName;
                    if (dt.Deadline.HasValue)
                        replace["$Deadline$"] = dt.Deadline.Value.ToString("dd/MM/yyyy HH:mm");
                    else
                        replace["$Deadline$"] = "";

                    SendMail.Send(TemplateMail, replace, To, NotifyMailtitle, emailcc, null, null, true, out ErrorMessage, MInfo);
                }
            }
        }

        public ApprovalState CheckState(long Id_Phieu)
        {
            var dt = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && x.Valid.HasValue && x.Valid == false).FirstOrDefault();
            if (dt != null)
                return ApprovalState.Canceled;
            dt = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && !x.Valid.HasValue).FirstOrDefault();
            if (dt != null)
                return ApprovalState.InProcess;

            return ApprovalState.Approved;
        }

        //private string Getthoigian(string hinhthuc, object giovao, object giora)
        //{
        //    string result = "";
        //    if (("1".Equals(hinhthuc)) || ("3".Equals(hinhthuc))) result += ", Giờ vào: " + string.Format("{0:HH:mm}", giovao);
        //    if (("2".Equals(hinhthuc)) || ("3".Equals(hinhthuc))) result += ", Giờ ra: " + string.Format("{0:HH:mm}", giora);
        //    if (!"".Equals(result)) result = result.Substring(2);
        //    return result;
        //}

        /// <summary>
        /// Lấy người duyệt của 1 bước khi chưa bắt đầu quy trình
        /// </summary>
        /// <param name="Id_quytrinh_capquanly">Bước duyệt</param>
        /// <returns>ID người duyệt, trống nếu không có người duyệt</returns>
        public string GetCheckers(string Id_quytrinh_capquanly, string id_chucdanh)
        {
            List<string> ids = new List<string>();
            var FRow = _context.QuytrinhCapquanlyduyet.Where(x => x.Rowid.ToString() == Id_quytrinh_capquanly).FirstOrDefault();
            if (FRow == null)
                return "";
            int id = 0;
            if ((FRow.IdChucdanh != null && !"".Equals(FRow.IdChucdanh.ToString())) && (int.TryParse(FRow.IdChucdanh.ToString(), out id)))
            {
                //dùng hàm GetCheckers để lấy nhiều nhân viên
                var dt = TblNhanvien.Where(x => x.IdChucVu == id).ToList();
                if (dt.Count > 0)
                    ids = dt.Select(x => x.UserID.ToString()).ToList();
            }
            else
            {
                Employee Emp = new Employee(_context);
                List<NguoiDungDPS> quanlys = new List<NguoiDungDPS>();
                if ((FRow.IdCapquanly != null && !"0".Equals(FRow.IdCapquanly.ToString())) && (int.TryParse(FRow.IdCapquanly.ToString(), out id)))
                {
                    //Không phải quản lý trực tiếp
                    quanlys = Emp.GetQuanlyByCap(id, id_chucdanh);
                    if (quanlys != null)
                    {
                        ids.AddRange(quanlys.Select(x => x.UserID.ToString()).ToList());
                    }
                }
                else
                {
                    //Quản lý trực tiếp
                    ids.AddRange(Emp.GetIDManager(id_chucdanh));
                    //if(!"".Equals(id_manager))
                    //quanly = Emp.GetManager(Id_nv, cnn);
                }
            }
            //string uyquyen = GetUyquyen(id_checker, DateTime.Today);
            //if (!uyquyen.Equals("")) return uyquyen;
            List<string> re = new List<string>();
            if (Loai == 2)//loại hồ sơ, người duyệt cấp tỉnh xử lý đối tượng ncc được phân
            {
                foreach (var id_checker in ids)
                    if (checkUserDoiTuong(id_checker))
                        re.Add(id_checker);
            }
            else
                re = ids;
            return string.Join(",", re);
        }

        public string GetCheckers1(long Id_Phieu, string Id_quytrinh_capquanly, long PreviousApprover, IDbContextTransaction transaction = null)
        {
            var dt = _context.QuytrinhCapquanlyduyet.Where(x => x.Rowid.ToString() == Id_quytrinh_capquanly).ToList();
            if (dt.Count <= 0) return "";
            var FRow = dt[0];
            //Xác định người duyệt cho bước đầu tiên
            int id = 0;//chức danh
            List<string> ids = new List<string>();
            if (FRow.IdChucdanh != null && !"".Equals(FRow.IdChucdanh.ToString()) && int.TryParse(FRow.IdChucdanh.ToString(), out id))
            {
                //dùng hàm GetCheckers để lấy nhiều nhân viên
                //SqlConditions Conds = new SqlConditions();
                //Conds.Add("id_chucdanh", id);
                //select = "select id_nv from tbl_nhanvien where (id_chucdanh=@id_chucdanh) and ((thoiviec=0) or (ngaythoiviec>getdate()))";
                //dt = cnn.CreateDataTable(select, Conds);
                var nv = TblNhanvien.Where(x => x.IdChucVu == id).ToList();
                if (nv.Count > 0)
                    ids = nv.Select(x => x.UserID.ToString()).ToList();
            }
            else
            {
                string Id_Nv = "";
                string loai = SenderField;
                string idnv = null;//người gửi
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = string.Format("select top(1)\"{0}\" from \"{1}\" where \"{2}\"={3}", loai, (useTableName ? TableName : DataViewName), PrimaryKeyField, Id_Phieu);
                    if (transaction != null)
                        command.Transaction = transaction.GetDbTransaction();
                    IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                    dbDataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    dbDataAdapter.Fill(dataSet);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                    {
                        if (dataSet.Tables[0].Rows.Count > 0)
                            idnv = dataSet.Tables[0].Rows[0][0].ToString();
                    }
                }
                var nhanvien = TblNhanvien.Where(x => x.UserID.ToString() == idnv).ToList();
                if (nhanvien.Count <= 0) return "";
                string id_chucdanh = nhanvien[0].IdChucVu.ToString();

                Employee Emp = new Employee(_context);
                List<NguoiDungDPS> quanlys = new List<NguoiDungDPS>();
                if ((!"".Equals(FRow.IdCapquanly.ToString())) && (int.TryParse(FRow.IdCapquanly.ToString(), out id)))
                {
                    if (id > 0)
                    {
                        //Không phải quản lý trực tiếp
                        quanlys = Emp.GetQuanlyByCap(id, id_chucdanh, transaction);
                    }
                    else
                    {
                        //Quản lý trực tiếp của cấp duyệt trước đó
                        quanlys = Emp.GetManager(PreviousApprover.ToString(), transaction);
                    }
                }
                else
                {
                    //Quản lý trực tiếp
                    quanlys = Emp.GetManager(Id_Nv, transaction);
                }
                if (quanlys != null)
                    ids = quanlys.Select(x => x.UserID.ToString()).ToList();
            }
            //string uyquyen = GetUyquyen(id_checker, DateTime.Today);
            //if (!uyquyen.Equals("")) return uyquyen;
            List<string> re = new List<string>();
            if (Loai == 2)//loại hồ sơ, người duyệt cấp tỉnh xử lý đối tượng ncc được phân
            {
                foreach (var id_checker in ids)
                    if (checkUserDoiTuong(id_checker, transaction))
                        re.Add(id_checker);
            }
            else
                re = ids;
            return string.Join(",", re);
        }

        /// <summary>
        /// Trường hợp quyền hoặc chức danh cụ thể, trả list checkers
        /// </summary>
        /// <param name="Id_quytrinh_capquanly"></param>
        /// <param name="IdCapquanly">-2 chức danh hoặc -3 quyền</param>
        /// <returns></returns>
        public List<string> GetCheckers2(long Id_quytrinh_capquanly, long? IdCapquanly)
        {
            List<string> lst = new List<string>();
            if (IdCapquanly != -2 && IdCapquanly != -3)
                return lst;
            var FRow = _context.QuytrinhCapquanlyduyet.Where(x => x.IdCapquanly == IdCapquanly && x.Rowid == Id_quytrinh_capquanly).FirstOrDefault();
            if (FRow == null)
                return lst;

            if (IdCapquanly == -3)
            {
                string code = FRow.Code;
                lst = getUserByRole(FRow.Code);
            }
            else
            {
                long? chucdanh = FRow.IdChucdanh;
                lst = TblNhanvien.Where(x => x.IdChucVu == chucdanh).Select(x => x.UserID.ToString()).ToList();
            }
            List<string> re = new List<string>();
            foreach (string id_checker in lst)
            {
                string uyquyen = GetUyquyen(id_checker, DateTime.Today);
                if (string.IsNullOrEmpty(uyquyen))
                    re.Add(id_checker);
                else
                    re.Add(uyquyen);
            }
            if (Loai == 2)//loại hồ sơ, người duyệt cấp tỉnh xử lý đối tượng ncc được phân
            {
                List<string> kq = new List<string>();
                foreach (string id in re)
                {
                    if (checkUserDoiTuong(id))
                        kq.Add(id);
                }
                return kq;
            }
            return re;
        }

        private List<string> getUserByRole(string code)
        {
            string sql = @"select distinct g.IdUser from Dps_UserGroupRoles p 
                                    join Dps_User_GroupUser g on p.IDGroupUser = g.IDGroupUser
                                    join Dps_Roles r on r.RoleGroup = p.IDGroupRole where r.Disabled = 0 and g.Disabled = 0 and IdRole = " + code;

            DataTable dt = null;
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = sql;
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    dt = dataSet.Tables[0];
                }
            }
            if (dt == null)
                return new List<string>();
            return dt.AsEnumerable().Select(x => x["IdUser"].ToString()).ToList();
        }

        private bool checkUserDoiTuong(string id, IDbContextTransaction transaction = null)
        {
            if (string.IsNullOrEmpty(id))
                return false;
            var find = TblNhanvien.Where(x => x.UserID.ToString() == id).FirstOrDefault();
            if (find == null)
                return false;
            if (find.CapCoCau != 1)//k phải cấp tỉnh
                return true;
            string doituong = "";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = "select top(1) Id_DoiTuongNCC from Tbl_NCC where disabled=0 and Id=" + MInfo.Id;
                if (transaction != null)
                    command.Transaction = transaction.GetDbTransaction();
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0)
                        doituong = dataSet.Tables[0].Rows[0][0].ToString();
                }
            }
            var dt = find.lstDoiTuongNCC.Where(x => x.id.ToString() == doituong).FirstOrDefault();
            return dt != null;
        }

        public List<string> GetOutput(long Id_quytrinh, string User)
        {
            string id_chucdanh = GetIDChucdanh(User);
            List<string> re = new List<string>();
            var FRow = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid == Id_quytrinh).FirstOrDefault();
            if (FRow == null)
                return re;
            long? IdCapquanly = FRow.IdCapquanly;
            if (IdCapquanly != -2 && IdCapquanly != -3)
            {
                List<string> ids = new List<string>();
                int id = 0;
                if ((FRow.IdChucdanh != null && !"".Equals(FRow.IdChucdanh.ToString())) && (int.TryParse(FRow.IdChucdanh.ToString(), out id)))
                {
                    //dùng hàm GetCheckers để lấy nhiều nhân viên
                    var dt = TblNhanvien.Where(x => x.IdChucVu == id).ToList();
                    if (dt.Count > 0)
                        ids = dt.Select(x => x.UserID.ToString()).ToList();
                }
                else
                {
                    Employee Emp = new Employee(_context);
                    List<NguoiDungDPS> quanlys = new List<NguoiDungDPS>();
                    if ((FRow.IdCapquanly != null && !"0".Equals(FRow.IdCapquanly.ToString())) && (int.TryParse(FRow.IdCapquanly.ToString(), out id)))
                    {
                        //Không phải quản lý trực tiếp
                        quanlys = Emp.GetQuanlyByCap(id, id_chucdanh);
                        if (quanlys != null)
                        {
                            ids = quanlys.Select(x => x.UserID.ToString()).ToList();
                        }
                    }
                    else
                    {
                        //Quản lý trực tiếp
                        ids = Emp.GetIDManager(id_chucdanh);
                        //if(!"".Equals(id_manager))
                        //quanly = Emp.GetManager(Id_nv, cnn);
                    }
                }
                //string uyquyen = GetUyquyen(id_checker, DateTime.Today);
                //if (!uyquyen.Equals(""))
                //    re.Add(uyquyen);
            }
            else
            {
                List<string> lst = new List<string>();
                if (IdCapquanly == -3)
                {
                    string code = FRow.Code;
                    lst = getUserByRole(FRow.Code);
                }
                else
                {
                    long? chucdanh = FRow.IdChucdanh;
                    lst = TblNhanvien.Where(x => x.IdChucVu == chucdanh).Select(x => x.UserID.ToString()).ToList();
                }
                foreach (string id_checker in lst)
                {
                    string uyquyen = GetUyquyen(id_checker, DateTime.Today);
                    if (string.IsNullOrEmpty(uyquyen))
                        re.Add(id_checker);
                    else
                        re.Add(uyquyen);
                }
            }
            return re;
        }

        /// <summary>
        /// Duyệt phiếu theo checker là User truyền vào, xác định quy trình có IsFinal hay không<para/>
        /// Dựa vào thứ tự duyệt hiện tại, cập nhật duyệt<para/>
        /// Xac dinh nguoi duyet tiep theo
        /// </summary>
        /// <param name="Id_Phieu"></param>
        /// <param name="Approved"></param>
        /// <param name="devChecker">dev checker cho bước kế bc hiện tại</param>
        /// <returns></returns>
        public bool Approve(DuyetDataModel data, string trangthai_new = "", List<Checker4Detail> devChecker = null, bool saveMaster = true, string messageNotifice = "", string messageOutput = "")
        {
            long Id_Phieu = data.id;
            bool Approved = data.value;
            string msg = string.IsNullOrEmpty(data.note) ? "" : data.note;
            MInfo.Id = Id_Phieu;
            Message = msg;
            #region kiểm tra phiếu tồn tại và lấy trạng thái hiện tại
            string _str = "select * from \"{0}\" where \"{1}\" = {2} ";
            DataTable dtPhieu = null;
            string trangthai = "";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = string.Format(_str, TableName, PrimaryKeyField, Id_Phieu);
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                    dtPhieu = dataSet.Tables[0];
                if (dtPhieu == null || dtPhieu.Rows.Count == 0)
                {
                    ErrorMessage = "Phiếu không tồn tại";
                    return false;
                }
                if (saveMaster)
                {
                    string tinhtrang_field = ValidField;
                    int index = tinhtrang_field.IndexOf("=");
                    if (index >= 0)
                        tinhtrang_field = tinhtrang_field.Remove(index).Replace("\"", "");
                    trangthai = dtPhieu.Rows[0][tinhtrang_field].ToString();
                }

            }
            #endregion

            bool IsFinal = false;
            var Data = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && !x.Valid.HasValue && (x.Checker == User || x.ListChecker.Contains(User.ToString()))).OrderBy(x => x.Priority).ToList();
            if (Data == null || Data.Count == 0)
            {
                ErrorMessage = "Người dùng không được phép duyệt phiếu";
                return false;
            }
            var Rw = Data[0];
            var temp = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai).OrderBy(x => x.Priority).ToList();
            int first_priority = temp[0].Priority.Value;
            int last_priority = temp[temp.Count - 1].Priority.Value;
            //Lấy thứ tự duyệt hiện tại
            int priority = 1;
            int.TryParse(Data[0].Priority.ToString(), out priority);

            var Val = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && x.IdQuytrinhCapquanly == Rw.IdQuytrinhCapquanly).FirstOrDefault();
            if (Val == null)
            {
                ErrorMessage = "Không tìm thấy quy trình quá trình duyệt";
                return false;
            }
            var quytrinh = (from dm in _context.QuytrinhQuytrinhduyet
                            join dm1 in _context.QuytrinhCapquanlyduyet on dm.Rowid equals dm1.IdQuytrinh
                            where dm1.Rowid == Val.IdQuytrinhCapquanly
                            select new
                            {
                                Processmethod = dm.Processmethod,
                                Rowid = dm.Rowid,
                                IdBack = dm1.IdBack,
                                SoNgayXuLy = getSoNgay(dm1, null)
                            }).ToList();
            string id_quytrinh = "";
            if (quytrinh.Count > 0)
                id_quytrinh = quytrinh[0].Rowid.ToString();
            else
            {
                ErrorMessage = "Không tìm thấy quá trình duyệt";
                return false;
            }
            ProcessMethodQT = quytrinh[0].Processmethod.ToString();
            string ProcessText = "";
            string Hotennguoiduyet = "";
            if (ProcessTextField != null && !"".Equals(ProcessTextField))
            {
                //Lấy text quá trình duyệt hiện tại
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = string.Format("select {0} from {1} where {2}={3}", ProcessTextField, TableName, PrimaryKeyField, Id_Phieu);
                    _context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        if (result != null)
                            ProcessText += result.GetString(0);
                    }
                }
                var dt = TblNhanvien.Where(x => x.UserID == User).ToList();
                if (dt.Count > 0) Hotennguoiduyet = dt[0].FullName.ToString();
            }
            bool IsSendNotify = true;
            using (var transaction = _context.Database.BeginTransaction())
            {
                //duyệt song song thì không update duyệt liên tiếp, ng duyệt ss cuối cùng hoặc ng k duyệt đc xem là checker của bước đó
                if (Val.Ss)
                {
                    List<QuytrinhQuatrinhduyetSs> sss = null;
                    if (CheckDetails != null)
                        sss = (from x in _context.QuytrinhQuatrinhduyetSs
                               join ct in CheckDetails on x.IdCt equals ct.IdCt
                               where x.IdQuatrinh == Val.IdRow && x.Passed != true && !x.Valid.HasValue && x.ListChecker.Contains(User.ToString())
                               select new QuytrinhQuatrinhduyetSs()
                               {
                                   IdRow = x.IdRow,
                                   IdQuatrinh = x.IdQuatrinh,
                                   Checkers = x.Checkers,
                                   IdCt = x.IdCt,
                                   Passed = x.Passed,
                                   Checker = User,
                                   Valid = Approved,
                                   Checknote = ct.checknote ?? "",
                                   Checkeddate = DateTime.Now
                               }).ToList();
                    else
                        sss = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == Val.IdRow && x.Passed != true && !x.Valid.HasValue && x.ListChecker.Contains(User.ToString())).ToList();
                    if (sss == null || sss.Count == 0)
                    {
                        transaction.Rollback();
                        ErrorMessage = "Người dùng không có quyền duyệt";
                        return false;
                    }
                    if (CheckDetails == null)
                        foreach (var ss in sss)
                        {
                            ss.Checker = User;
                            ss.Valid = Approved;
                            ss.Checknote = Message ?? "";
                            ss.Checkeddate = DateTime.Now;
                        }
                    //Cập nhật duyệt
                    _context.QuytrinhQuatrinhduyetSs.UpdateRange(sss);
                    _context.SaveChanges();
                    ssfinal = false;
                    if (Approved)
                    {
                        var kt = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == Val.IdRow && x.Passed != true && !x.Valid.HasValue).ToList();
                        if (kt.Count == 0)
                            ssfinal = true;
                    }
                    else
                    {
                        ssfinal = true;
                    }
                    //khu kết thúc, cập nhật các dòng passed=đã duyệt qua(trường hợp quay lui dùng cờ này để check)
                    if (ssfinal)
                    {
                        var kt = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == Val.IdRow && x.Passed != true).ToList();
                        foreach (var ss1 in kt)
                        {
                            ss1.Passed = true;
                        }
                        _context.QuytrinhQuatrinhduyetSs.UpdateRange(kt);
                        _context.SaveChanges();
                    }
                }
                long id_quatrinh_return = 0;
                if (Approved)
                {
                    if (!Val.Ss || (Val.Ss && ssfinal))
                    {
                        if (Rw.ListChecker.Contains(User.ToString()))
                            Val.Checker = User;
                        Val.Valid = Approved;
                        Val.Checknote = Message ?? "";
                        Val.Checkeddate = DateTime.Now;
                        ProcessText += "<br />" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - " + Hotennguoiduyet + ": Accept - Note: " + Message;
                        //Cập nhật duyệt
                        _context.QuytrinhQuatrinhduyet.Update(Val);
                        _context.SaveChanges();
                        //Cập nhật lại tình trạng khi duyệt lần đầu
                        if (priority == first_priority)
                        {
                            if (!string.IsNullOrEmpty(StatusField))
                            {
                                string str = string.Format("update \"{0}\" set \"{1}\"=1 where \"{2}\"={3}", TableName, StatusField, PrimaryKeyField, Id_Phieu);
                                int rs = _context.Database.ExecuteSqlCommand(str);
                                if (rs <= 0)
                                {
                                    transaction.Rollback();
                                    ErrorMessage = "Cập nhật không thành công";
                                    return false;
                                }
                            }
                        }
                    }
                    //Xóa lịch sử chỉnh sửa dữ liệu khi đồng ý duyệt
                    //Kiểm tra thứ tự duyệt hiện tại đã duyệt hết chưa
                    var dt = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && !x.Valid.HasValue && x.Priority == priority).ToList();
                    if (dt == null)
                    {
                        transaction.Rollback();
                        ErrorMessage = "Lỗi cập nhật cơ sở dữ liệu";
                        return false;
                    }
                    if (dt.Count == 0) //Trường thứ tự duyệt hiện tại đã được duyệt thì xác định người duyệt tiếp theo
                    {
                        //Xac dinh nguoi duyet tiep theo
                        var dt1 = (from qt in _context.QuytrinhQuatrinhduyet
                                   join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                                   join dm1 in _context.QuytrinhQuytrinhduyet on dm.IdQuytrinh equals dm1.Rowid
                                   where qt.IdPhieu == Id_Phieu && qt.Loai == Loai && !qt.Valid.HasValue && !qt.Checker.HasValue
                                   orderby qt.Priority
                                   select new
                                   {
                                       IdRow = qt.IdRow,
                                       IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                                       IdCapquanly = dm.IdCapquanly,
                                       DuyetSS = dm.DuyetSs,
                                       Checkernotfoundsendto = dm1.Checkernotfoundsendto,
                                       Maxlevel = dm.Maxlevel,
                                       SoNgayXuLy = getSoNgay(dm, transaction),
                                       Deadline = qt.Deadline,
                                       Checker = qt.Checker,
                                       ListChecker = qt.ListChecker
                                   }).ToList();
                        if (dt1 == null)
                        {
                            transaction.Rollback();
                            ErrorMessage = "Lỗi cập nhật cơ sở dữ liệu";
                            return false;
                        }
                        //Xóa các bước có cấp tối đa nhỏ hơn hoặc bằng cấp của người vừa duyệt
                        int capnguoiduyethientai = -1;
                        for (int k = 0; k < dt1.Count; k++)
                        {
                            string Id_capquanly = "";
                            int maxlevel = 0;
                            if ((!"".Equals(dt1[k].Maxlevel.ToString())) && (int.TryParse(dt1[k].Maxlevel.ToString(), out maxlevel)))
                            {
                                //Trường hợp có quy định cấp tối đa
                                if (capnguoiduyethientai == -1)
                                {
                                    //Trường hợp chưa xác định cấp người duyệt hiện tại
                                    capnguoiduyethientai = GetCapNhanvien(User.ToString(), out Id_capquanly);
                                }
                                var capquanly = DmCapquanly.Where(x => x.Rowid == maxlevel).ToList();
                                if (capquanly.Count > 0)
                                {
                                    int capcuavitri = 0;
                                    if (int.TryParse(capquanly[0].Range.ToString(), out capcuavitri))
                                    {
                                        if (capcuavitri <= capnguoiduyethientai)
                                        {
                                            //Xóa cấp này
                                            var Val1 = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && x.IdQuytrinhCapquanly == dt1[k].IdQuytrinhCapquanly).FirstOrDefault();
                                            if (Val1 == null)
                                            {
                                                transaction.Rollback();
                                                ErrorMessage = "Lỗi cập nhật cơ sở dữ liệu";
                                                return false;
                                            }
                                            //nếu duyệt ss thì xóa ss
                                            if (Val1.Ss)
                                            {
                                                var kt = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == Val1.IdRow).ToList();
                                                _context.QuytrinhQuatrinhduyetSs.RemoveRange(kt);
                                            }
                                            _context.QuytrinhQuatrinhduyet.Remove(Val1);
                                            _context.SaveChanges();
                                        }
                                    }
                                }
                            }
                        }
                        //Sau khi xóa thì lấy lại danh sách các cấp duyệt còn lại
                        dt1 = (from qt in _context.QuytrinhQuatrinhduyet
                               join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                               join dm1 in _context.QuytrinhQuytrinhduyet on dm.IdQuytrinh equals dm1.Rowid
                               where qt.IdPhieu == Id_Phieu && qt.Loai == Loai && !qt.Valid.HasValue && !qt.Checker.HasValue
                               orderby qt.Priority
                               select new
                               {
                                   IdRow = qt.IdRow,
                                   IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                                   IdCapquanly = dm.IdCapquanly,
                                   DuyetSS = dm.DuyetSs,
                                   Checkernotfoundsendto = dm1.Checkernotfoundsendto,
                                   Maxlevel = dm.Maxlevel,
                                   SoNgayXuLy = getSoNgay(dm, transaction),
                                   Deadline = qt.Deadline,
                                   Checker = qt.Checker,
                                   ListChecker = qt.ListChecker
                               }).ToList();
                        if (dt1.Count <= 0)
                        {
                            IsFinal = true;
                        }
                        else
                        {
                            if (!Val.Ss || (Val.Ss && ssfinal))//k phải duyệt ss hoặc duyệt ss đã kết thúc
                            {
                                List<string> lst = new List<string>();
                                int i = 0;
                                int cDevChecker = 0;
                                bool loop = false;
                                do
                                {
                                    loop = false;
                                    lst = new List<string>();
                                    //Update note này đã được duyệt luôn
                                    var find = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.IdQuytrinhCapquanly == dt1[i].IdQuytrinhCapquanly && x.Loai == Loai).FirstOrDefault();
                                    if (find == null)//k tìm thấy bước kế tiếp
                                    {
                                        transaction.Rollback();
                                        ErrorMessage = "Cập nhật duyệt không thành công";
                                        return false;
                                    }
                                    if (find.Ss)//bc kế tiếp duyệt ss thì break
                                    {
                                        i++;
                                        break;
                                    }
                                    if (dt1[i].IdCapquanly == -4 || dt1[i].IdCapquanly == -5)
                                    {
                                        if (cDevChecker > 0)//devchecker chỉ đc sử dụng cho 1 bước kế tiếp
                                        {
                                            i++;
                                            break;
                                        }
                                        cDevChecker++;
                                        //bc kế tiếp cần devchecker
                                        foreach (var c in devChecker)
                                        {
                                            lst.AddRange(c.Checkers);
                                        }
                                    }
                                    else
                                    {
                                        if (dt1[i].IdCapquanly == -2 || dt1[i].IdCapquanly == -3)
                                        {
                                            lst = GetCheckers2(dt1[i].IdQuytrinhCapquanly, dt1[i].IdCapquanly);
                                        }
                                        else
                                        {
                                            string NextCheckers = GetCheckers1(Id_Phieu, dt1[i].IdQuytrinhCapquanly.ToString(), User, transaction);
                                            if (NextCheckers != "")
                                                lst = NextCheckers.Split(",").ToList();
                                        }
                                    }
                                    data.NguoiNhans = lst;
                                    if (lst.Count == 0)
                                    {
                                        data.Deadline = dt1[i].Deadline;
                                        data.Checkers = dt1[i].ListChecker;
                                        if (dt1[i].Checker.HasValue && !data.Checkers.Contains(dt1[i].Checker.Value.ToString()))
                                            data.Checkers.Add(dt1[i].Checker.Value.ToString());
                                        saveHistory(User, dt1[i].IdRow, IsFinal, trangthai, trangthai_new, data);
                                        transaction.Commit();
                                        //Gửi mail cho người nhận thông báo khi không tìm thấy người duyệt tiếp theo
                                        SendThongbaoChonnguoiduyet(dt1[i].Checkernotfoundsendto.ToString(), Id_Phieu);
                                        return true;
                                    }
                                    if (ProcessMethod == "0" && lst.Contains(User.ToString()))
                                    {
                                        loop = true;
                                        find.Valid = Approved;
                                        find.Checknote = Message ?? "";
                                        find.Checkeddate = DateTime.Now;
                                        find.Checker = User;
                                        if (lst.Contains(User.ToString()))
                                            find.Checkers = string.Join(",", lst);
                                        _context.QuytrinhQuatrinhduyet.Update(find);
                                        _context.SaveChanges();
                                    }
                                    i++;
                                } while (loop && i < dt1.Count);
                                if (ProcessMethod != "0" || !lst.Contains(User.ToString()))
                                {
                                    //Cập nhật người duyệt cho bước kế tiếp
                                    //Lấy thứ tự duyệt cấp người duyệt tiếp theo
                                    var lstPriority = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && x.IdQuytrinhCapquanly == dt1[i - 1].IdQuytrinhCapquanly).Select(x => x.Priority).ToList();
                                    var dt2 = (from qt in _context.QuytrinhQuatrinhduyet
                                               join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                                               join dm1 in _context.QuytrinhQuytrinhduyet on dm.IdQuytrinh equals dm1.Rowid
                                               join cap in DmCapquanly on dm.IdCapquanly equals cap.Rowid into QT
                                               from cap in QT.DefaultIfEmpty()
                                               where !qt.Checker.HasValue && !qt.Valid.HasValue && qt.IdPhieu == Id_Phieu && lstPriority.Contains(qt.Priority) && qt.Loai == Loai
                                               select new
                                               {
                                                   IdRow = qt.IdRow,
                                                   IdCapquanly = dm.IdCapquanly,
                                                   DuyetSS = dm.DuyetSs,
                                                   IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                                                   Checkernotfoundsendto = dm1.Checkernotfoundsendto,
                                                   Range = cap.Range,
                                                   SoNgayXuLy = getSoNgay(dm, transaction),
                                                   Deadline = qt.Deadline,
                                                   Checker = qt.Checker,
                                                   ListChecker = qt.ListChecker
                                               }).ToList();
                                    if (dt2 == null || dt2.Count == 0)
                                    {
                                        transaction.Rollback();
                                        ErrorMessage = "Không thể cập nhật người duyệt kế tiếp";
                                        return false;
                                    }
                                    for (int j = 0; j < dt2.Count; j++)
                                    {
                                        var find = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.IdQuytrinhCapquanly == dt2[j].IdQuytrinhCapquanly && x.Loai == Loai).FirstOrDefault();
                                        if (find == null)
                                        {
                                            transaction.Rollback();
                                            ErrorMessage = "Không thể cập nhật người duyệt kế tiếp";
                                            return false;
                                        }
                                        List<Checker4Detail> lstChecker = new List<Checker4Detail>();
                                        int Range_capduyet = dt2[j].Range.HasValue ? dt2[j].Range.Value : 0;
                                        if (dt2[j].IdCapquanly == -2 || dt2[j].IdCapquanly == -3 || dt2[j].IdCapquanly == -4 || dt2[j].IdCapquanly == -5)
                                        {
                                            if (dt2[j].IdCapquanly == -4 || dt2[j].IdCapquanly == -5)
                                            {
                                                //Trường hợp Phiếu xác định người duyệt
                                                if (devChecker != null)
                                                    lstChecker = devChecker;
                                            }
                                            else
                                                lstChecker = GetCheckers2(dt2[j].IdQuytrinhCapquanly, dt2[j].IdCapquanly).Select(x => new Checker4Detail()
                                                {
                                                    Checkers = new List<string>() { x }
                                                }).ToList();
                                            if (dt2[j].DuyetSS)
                                            {
                                                find.Ss = true;
                                                List<QuytrinhQuatrinhduyetSs> sss = new List<QuytrinhQuatrinhduyetSs>();
                                                //duyệt song song thì lưu tách checker
                                                foreach (var user in lstChecker)
                                                {
                                                    QuytrinhQuatrinhduyetSs val1 = new QuytrinhQuatrinhduyetSs();
                                                    val1.Checkers = user.strCheckers;
                                                    val1.IdQuatrinh = dt2[j].IdRow;
                                                    if (user.IdCt.HasValue)
                                                        val1.IdCt = user.IdCt;
                                                    if (dt2[j].SoNgayXuLy > 0)
                                                        val1.Deadline = DateTime.Now.AddDays(dt2[j].SoNgayXuLy);
                                                    else
                                                        val1.Deadline = null;
                                                    sss.Add(val1);
                                                }
                                                _context.QuytrinhQuatrinhduyetSs.AddRange(sss);
                                                _context.SaveChanges();
                                            }

                                            var tt = new List<string>();
                                            foreach (var c in lstChecker)
                                                tt.AddRange(c.Checkers);
                                            var str = string.Join(",", tt);
                                            find.Checkers = string.Join(",", str);
                                        }
                                        else
                                        {
                                            string NextChecker = GetCheckers1(Id_Phieu, dt2[j].IdQuytrinhCapquanly.ToString(), User, transaction);
                                            if (NextChecker != "")
                                            {
                                                lstChecker = NextChecker.Split(",").Select(x => new Checker4Detail()
                                                {
                                                    Checkers = new List<string>() { x }
                                                }).ToList();

                                                find.Checkers = NextChecker;
                                            }
                                        }
                                        if (dt2[j].SoNgayXuLy > 0)
                                            find.Deadline = DateTime.Now.AddDays(dt2[j].SoNgayXuLy);
                                        else
                                            find.Deadline = null;
                                        _context.QuytrinhQuatrinhduyet.Update(find);
                                        _context.SaveChanges();
                                        data.NguoiNhans = find.ListChecker;
                                        if (lstChecker.Count == 0)
                                        {
                                            data.Deadline = dt2[j].Deadline;
                                            data.Checkers = dt2[j].ListChecker;
                                            if (dt2[j].Checker.HasValue && !data.Checkers.Contains(dt2[j].Checker.Value.ToString()))
                                                data.Checkers.Add(dt2[j].Checker.Value.ToString());
                                            saveHistory(User, dt2[j].IdRow, IsFinal, trangthai, trangthai_new, data);
                                            transaction.Commit();
                                            SendThongbaoChonnguoiduyet(dt2[j].Checkernotfoundsendto.ToString(), Id_Phieu);
                                            return true;
                                        }
                                    }
                                }
                                else IsFinal = true;
                            }
                        }
                    }
                }
                else
                {
                    //khi duyệt song song, 1 ss k duyệt xem như bước đó k duyệt
                    //Nếu Quy duyệt theo cách không duyệt thì chấm dứt
                    if ("0".Equals(ProcessMethodQT) || "".Equals(ProcessMethodQT))
                    {
                        if (Rw.ListChecker.Contains(User.ToString()))
                            Val.Checker = User;
                        Val.Valid = Approved;
                        Val.Checknote = Message ?? "";
                        Val.Checkeddate = DateTime.Now;
                        IsFinal = true;
                        ProcessText += "<br />" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - " + Hotennguoiduyet + ": Reject - Note: " + Message + " - End process.";
                        //Cập nhật duyệt
                        _context.QuytrinhQuatrinhduyet.Update(Val);
                        _context.SaveChanges();
                    }
                    else
                    {
                        //Trường hợp trả về cấp trước đó thì notify riêng, ko chạy hàm NotifyByEmail
                        IsSendNotify = false;
                        var Val1 = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai && (x.Checker == User || x.ListChecker.Contains(User.ToString())) && x.Priority == priority).FirstOrDefault();
                        if (Val1 == null)
                        {
                            transaction.Rollback();
                            ErrorMessage = "Cập nhật không thành công";
                            return false;
                        }
                        Val1.Checker = null;
                        Val1.Checkers = null;
                        Val1.Checknote = Message;
                        Val1.Deadline = null;
                        //Cập nhật người duyệt tại giai đoạn này
                        _context.QuytrinhQuatrinhduyet.Update(Val1);
                        _context.SaveChanges();
                        //Nếu quy trình duyệt theo cách không duyệt thì trả về câp trước đó
                        if (priority == first_priority || ("2".Equals(ProcessMethodQT) && quytrinh[0].IdBack.ToString() == "0"))
                        {
                            //Trả về cho nhân viên
                            ProcessText += "<br />" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - " + Hotennguoiduyet + ": Return to sender - Note: " + Message;
                            IsFinal = true;
                        }
                        else
                        {
                            long idBack = 0;
                            List<QuytrinhQuatrinhduyet> finds = new List<QuytrinhQuatrinhduyet>();
                            int PreviousPriority = priority - 1;
                            if ("2".Equals(ProcessMethodQT))
                            {
                                //lấy cấp back
                                var dt1 = (from x in _context.QuytrinhQuatrinhduyet
                                           where x.IdQuytrinhCapquanly.ToString() == quytrinh[0].IdBack.ToString() && x.Loai == Loai && x.IdPhieu == Id_Phieu
                                           select new
                                           {
                                               IdRow = x.IdRow,
                                               Priority = x.Priority,
                                               IdQuytrinhCapquanly = x.IdQuytrinhCapquanly,
                                           }).ToList();
                                if (dt1.Count > 0)
                                {
                                    int.TryParse(dt1[0].Priority.ToString(), out PreviousPriority);
                                    idBack = dt1[0].IdRow;
                                }
                                //update lại toàn bộ bước trước bước hiện tại đến bước back
                                finds = _context.QuytrinhQuatrinhduyet.Where(x => x.Priority >= PreviousPriority && x.Priority < priority && x.Loai == Loai && x.IdPhieu == Id_Phieu).ToList();
                            }
                            else
                            {
                                //Lấy cấp trước đó
                                var dt = _context.QuytrinhQuatrinhduyet.Where(x => x.Priority < priority && x.Loai == Loai && x.IdPhieu == Id_Phieu).OrderByDescending(x => x.Priority).Select(x => new
                                {
                                    IdRow = x.IdRow,
                                    Priority = x.Priority,
                                    IdQuytrinhCapquanly = x.IdQuytrinhCapquanly
                                }).ToList();
                                if (dt.Count > 0)
                                {
                                    int.TryParse(dt[0].Priority.ToString(), out PreviousPriority);
                                    idBack = dt[0].IdRow;
                                }
                                finds = _context.QuytrinhQuatrinhduyet.Where(x => x.Priority == PreviousPriority && x.Loai == Loai && x.IdPhieu == Id_Phieu).ToList();
                            }
                            if (finds == null || finds.Count == 0)
                            {
                                transaction.Rollback();
                                ErrorMessage = "Cập nhật không thành công";
                                return false;
                            }
                            foreach (var find in finds)
                            {
                                //xóa checker, checkers bước ở giữa
                                if (find.Priority != PreviousPriority)
                                {
                                    find.Checker = null;
                                    find.Checkers = null;
                                }
                                find.Valid = null;
                                find.Checknote = null;
                                find.Checkeddate = null;
                                if (find.Ss)//duyệt ss thì xóa
                                {
                                    var kt = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == find.IdRow).ToList();
                                    _context.QuytrinhQuatrinhduyetSs.RemoveRange(kt);
                                }
                            }
                            _context.QuytrinhQuatrinhduyet.UpdateRange(finds);
                            _context.SaveChanges();
                            var back = _context.QuytrinhQuatrinhduyet.Where(x => x.IdRow == idBack).FirstOrDefault();
                            id_quatrinh_return = back.IdRow;
                            if (back.Ss)
                            {
                                //nếu duyệt ss thì clear checker duyệt ss 
                                var kt = _context.QuytrinhQuatrinhduyetSs.Where(x => x.IdQuatrinh == idBack && x.Passed == true).ToList();
                                foreach (var ss1 in kt)
                                {
                                    ss1.Passed = false;
                                    ss1.Checker = null;
                                    ss1.Valid = null;
                                    ss1.Checknote = null;
                                    ss1.Checkeddate = null;
                                }
                                _context.QuytrinhQuatrinhduyetSs.UpdateRange(kt);
                                _context.SaveChanges();
                            }
                            string FCheckID = "";
                            string Nguoiduyettruoc = NotifyToPreviousChecker(Id_Phieu, PreviousPriority, out FCheckID, transaction);
                            ProcessText += "<br />" + DateTime.Now.ToString("dd/MM/yyyy HH:mm") + " - " + Hotennguoiduyet + ": Return to " + Nguoiduyettruoc + " - Note: " + Message;
                        }
                    }
                }
                if (IsUpdateProcessText && !string.IsNullOrEmpty(ProcessTextField))
                {
                    string str = String.Format("update \"{0}\" set \"{1}\"={2} where \"{3}\"={4}", TableName, ProcessTextField, ProcessText, PrimaryKeyField, Id_Phieu);
                    _context.Database.ExecuteSqlCommand(str);
                }
                if (IsFinal)
                {
                    if (!string.IsNullOrEmpty(StatusField))
                    { //Cập nhật lại tình trạng
                        string str1 = string.Format("update \"{0}\" set \"{1}\"=0 where \"{2}\"={3}", TableName, StatusField, PrimaryKeyField, Id_Phieu);
                        _context.Database.ExecuteSqlCommand(str1);
                    }
                    string field = Approved ? ValidField : InValidField;
                    if (Approved)
                    {
                        //Cập nhật phiếu đã được duyệt/k duyệt
                        #region cập nhật lịch sử vào bảng phiếu
                        string strPhieu = "update \"{0}\" set {1} where \"{2}\"={3}";
                        string strPhieu1 = "";
                        if (!string.IsNullOrEmpty(field))
                        {
                            strPhieu1 += string.IsNullOrEmpty(strPhieu1) ? "" : ", ";
                            if (field.Contains("="))
                                strPhieu1 += field;
                            else
                                strPhieu1 += "\"" + field + "\"=" + Approved;
                        }
                        if (!string.IsNullOrEmpty(CheckerField))
                        {
                            strPhieu1 += string.IsNullOrEmpty(strPhieu1) ? "" : ", ";
                            strPhieu1 += "\"" + CheckerField + "\"=" + User;
                        }
                        if (!string.IsNullOrEmpty(CheckedDateField))
                        {
                            strPhieu1 += string.IsNullOrEmpty(strPhieu1) ? "" : ", ";
                            strPhieu1 += "\"" + CheckedDateField + "\"=getdate()";
                        }
                        if (!string.IsNullOrEmpty(CheckNoteField))
                        {
                            strPhieu1 += string.IsNullOrEmpty(strPhieu1) ? "" : ", ";
                            strPhieu1 += "\"" + CheckNoteField + "\"=" + (string.IsNullOrEmpty(Message) ? "''" : "'" + Message + "'");
                        }
                        if (!string.IsNullOrEmpty(strPhieu1))
                        {
                            string str = string.Format(strPhieu, TableName, strPhieu1, PrimaryKeyField, Id_Phieu);
                            _context.Database.ExecuteSqlCommand(str);
                        }
                        #endregion
                    }
                    else
                    {
                        //trả lại
                        string strPhieu = "update \"{0}\" set {3}, SentBy=NULL, SentDate=NULL, Deadline =NULL where \"{1}\"={2}";
                        string str = string.Format(strPhieu, TableName, PrimaryKeyField, Id_Phieu, InValidField);
                        _context.Database.ExecuteSqlCommand(str);
                    }
                    IsSendNotify = false;
                }
                if (!Val.Ss || (Val.Ss && ssfinal))
                {
                    data.Deadline = Val.Deadline;
                    data.Checkers = Val.ListChecker;
                    if (Val.Checker.HasValue && !data.Checkers.Contains(Val.Checker.Value.ToString()))
                        data.Checkers.Add(Val.Checker.Value.ToString());
                    saveHistory(User, Val.IdRow, IsFinal, trangthai, trangthai_new, data, id_quatrinh_return);
                }
                transaction.Commit();
                if (!Val.Ss || (Val.Ss && ssfinal))
                {
                    if (IsSendNotify)
                        NotifyByEmail(Id_Phieu, messageNotifice);
                }
                if (IsFinal)
                {
                    //if (!Approved && priority == first_priority)
                    //    //Gửi mail thông báo
                    //    NotifyReturnToSender(Id_Phieu);
                    SendMailApproved(id_quytrinh, Id_Phieu, Approved, messageNotifice, messageOutput);
                }
                return true;
            }
        }

        private double getSoNgay(QuytrinhCapquanlyduyet cap, IDbContextTransaction transaction = null)
        {
            if (Loai != 2)
                return cap.SoNgayXuLy;
            double re = 0;
            string sql = @"select dk.*, SoNgay from quytrinh_dieukien dk 
join Tbl_NCC ncc on ncc.Id={0} and ncc.Id_LoaiHoSo=dk.value
join quytrinh_dieukien_capquanly dkc on dk.Id=dkc.Id_DieuKien and dkc.Id_QuyTrinh_CapQuanLy={1}
where dk.disabled=0 and Id_QuyTrinh={2}";

            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = string.Format(sql, MInfo.Id, cap.Rowid, cap.IdQuytrinh);
                if (transaction != null)
                    command.Transaction = transaction.GetDbTransaction();
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                {
                    if (dataSet.Tables[0].Rows.Count > 0 && dataSet.Tables[0].Rows[0]["SoNgay"] != DBNull.Value)
                        re = (double)dataSet.Tables[0].Rows[0]["SoNgay"];
                }
            }

            return re;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="Id_Phieu"></param>
        /// <param name="IsTraLai"></param>
        /// <param name="note"></param>
        /// <returns></returns>
        public bool ThuHoi(long Id_Phieu, string note, bool IsTraLai = false)
        {
            var temp = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_Phieu && x.Loai == Loai).OrderBy(x => x.Priority).ToList();
            if (temp == null || temp.Count == 0)
            {
                ErrorMessage = "Không tìm thấy quy trình duyệt";
                return false;
            }
            long id_quatrinh = 0;
            DateTime? deadline = null;
            if (!IsTraLai)
            {
                if (temp[0].Valid.HasValue)
                {
                    ErrorMessage = "Quy trình đang được xử lý, không thể thu hồi";
                    return false;
                }
                id_quatrinh = temp[0].IdRow;
                deadline = temp[0].Deadline;
            }
            else
            {
                if (temp[temp.Count - 1].Valid.HasValue && !temp[temp.Count - 1].Valid.Value)
                {
                    ErrorMessage = "Quy trình chưa kết thúc, không thể trả lại";
                    return false;
                }
                id_quatrinh = temp[temp.Count - 1].IdRow;
                deadline = temp[temp.Count - 1].Deadline;
                foreach (var qt in temp)
                {
                    qt.Valid = null;
                    qt.Checker = null;
                    qt.Checkers = null;
                    qt.Checkeddate = null;
                    qt.Checknote = null;
                    qt.Deadline = null;
                }
                long? IdQuytrinh = _context.QuytrinhCapquanlyduyet.Where(x => x.Rowid == temp[temp.Count - 1].IdQuytrinhCapquanly).Select(x => x.IdQuytrinh).FirstOrDefault();
                if (IdQuytrinh.HasValue)
                    SendMailApproved(IdQuytrinh.ToString(), Id_Phieu, false, RejectMailtitle, "", IsTraLai);
                _context.UpdateRange(temp);
                _context.SaveChanges();
            }
            QuytrinhLichsu ls = new QuytrinhLichsu();
            ls.NgayTao = DateTime.Now;
            ls.NguoiTao = User;
            ls.IdQuatrinh = id_quatrinh;
            ls.Note = note;
            ls.IsFinal = true;
            ls.Status = !IsTraLai ? "0" : "-1";
            ls.Deadline = deadline;
            _context.QuytrinhLichsu.Add(ls);
            _context.SaveChanges();
            return true;
        }

        public void saveHistory(long User, long IdQuatrinh, bool IsFinal, string status, string status_new, DuyetDataModel data, long id_quatrinh_return = 0)
        {
            bool Approved = data.value;
            long Id_Phieu = data.id;
            #region Thêm dòng lịch sử vào bảng ls
            string strLs = "insert into \"{0}\" ({1}) values ({2})";
            string strLs1 = "";
            string strLs2 = "";
            if (!string.IsNullOrEmpty(ls_nguoi))
            {
                strLs1 += string.IsNullOrEmpty(strLs1) ? "" : ", ";
                strLs1 += "\"" + ls_nguoi + "\"";

                strLs2 += string.IsNullOrEmpty(strLs2) ? "" : ", ";
                strLs2 += User;
            }
            if (!string.IsNullOrEmpty(ls_ngay))
            {
                strLs1 += string.IsNullOrEmpty(strLs1) ? "" : ", ";
                strLs1 += "\"" + ls_ngay + "\"";

                strLs2 += string.IsNullOrEmpty(strLs2) ? "" : ", ";
                strLs2 += "getdate()";
            }
            if (!string.IsNullOrEmpty(ls_ghichu))
            {
                strLs1 += string.IsNullOrEmpty(strLs1) ? "" : ", ";
                strLs1 += "\"" + ls_ghichu + "\"";

                strLs2 += string.IsNullOrEmpty(strLs2) ? "" : ", ";
                strLs2 += "'" + Message + "'";
            }
            if (!string.IsNullOrEmpty(ls_tinhtrang))
            {
                strLs1 += string.IsNullOrEmpty(strLs1) ? "" : ", ";
                strLs1 += "\"" + ls_tinhtrang + "\"";

                strLs2 += string.IsNullOrEmpty(strLs2) ? "" : ", ";
                strLs2 += "'" + status_new + "'";
            }
            if (!string.IsNullOrEmpty(ls_khoangoai))
            {
                strLs1 += string.IsNullOrEmpty(strLs1) ? "" : ", ";
                strLs1 += "\"" + ls_khoangoai + "\"";

                strLs2 += string.IsNullOrEmpty(strLs2) ? "" : ", ";
                strLs2 += Id_Phieu;
            }
            if (!string.IsNullOrEmpty(ls_table) && !string.IsNullOrEmpty(strLs1) && !string.IsNullOrEmpty(strLs2))
            {
                string str = string.Format(strLs, ls_table, strLs1, strLs2);
                _context.Database.ExecuteSqlCommand(str);
            }
            #endregion
            QuytrinhLichsu ls = new QuytrinhLichsu();
            ls.NgayTao = DateTime.Now;
            ls.NguoiTao = User;
            ls.IdQuatrinh = IdQuatrinh;
            ls.Note = Message;
            ls.Approved = Approved;
            if (!Approved)
                ls.IdQuatrinhReturn = id_quatrinh_return;
            ls.IsFinal = IsFinal;
            ls.Status = status;
            ls.Deadline = data.Deadline;
            if (data.Checkers != null && data.Checkers.Count > 0)
                ls.Checkers = string.Join(",", data.Checkers);
            if (data.NguoiNhans != null && data.NguoiNhans.Count > 0)
                ls.NguoiNhan = string.Join(",", data.NguoiNhans);
            if (data.FileDinhKem != null && !string.IsNullOrEmpty(data.FileDinhKem.strBase64))
            {
                string x = "";
                if (!UploadHelper.UploadFile(data.FileDinhKem.strBase64, data.FileDinhKem.filename, "/quytrinh_lichsu/", _hostingEnvironment.ContentRootPath, ref x))
                {
                }
                ls.src = x;
                ls.FileDinhKem = data.FileDinhKem.filename;
            }
            _context.QuytrinhLichsu.Add(ls);
            _context.SaveChanges();
            if (Loai == 2 && !Approved)//hướng dẫn hoàn thiện hồ sơ
            {
                string s = "insert into tbl_ncc_huongdan(id_quytrinh_lichsu, id_ncc, noidung, mota) values (@id_quytrinh_lichsu, @idncc, @noidung, @mota)";
                _context.Database.ExecuteSqlCommand(s,
    new SqlParameter("id_quytrinh_lichsu", ls.IdRow),
    new SqlParameter("idncc", data.id),
    new SqlParameter("noidung", string.IsNullOrEmpty(data.HuongDan.NoiDung) ? "" : data.HuongDan.NoiDung),
    new SqlParameter("mota", string.IsNullOrEmpty(data.HuongDan.MoTa) ? "" : data.HuongDan.MoTa));
            }
        }

        public bool IsFinal(long id_phieu, string user)
        {
            var priority = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == id_phieu && x.Loai == Loai).Max(x => x.Priority);
            var find = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == id_phieu && x.Loai == Loai && x.Checker.HasValue && x.Checker.Value.ToString() == user && x.Priority == priority).FirstOrDefault();
            return find != null;
        }

        /// <summary>
        /// Gửi mail thông báo phiếu đã được duyệt hoặc không được duyệt
        /// </summary>
        /// <param name="id_quytrinh"></param>
        /// <param name="Id_Phieu"></param>
        /// <param name="IsAccepted"></param>
        /// <returns></returns>
        public bool SendMailApproved(string id_quytrinh, long Id_Phieu, bool IsAccepted, string messageNotifice = "", string messageOutput = "", bool IsReturn = false)
        {
            try
            {
                ApprovalState e = CheckState(Id_Phieu);
                if (e == ApprovalState.InProcess) return false;
                //Lấy người gửi
                string str = "select dps_user.email,fullname AS hoten, dps_user.UserID as id_nv, cd.Tenchucdanh  from \"" + (useTableName ? TableName : DataViewName) + "\" as bang inner join dps_user on bang.\"" + SenderField + "\"=dps_user.UserID join Tbl_Chucdanh cd on cd.Id_row = dps_user.IdChucVu where \"" + PrimaryKeyField + "\" = " + Id_Phieu;
                DataTable dt = null;
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = str;
                    IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                    dbDataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    dbDataAdapter.Fill(dataSet);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                        dt = dataSet.Tables[0];
                    if (dt == null || dt.Rows.Count == 0)
                    {
                        return false;
                    }
                }
                string mail = dt.Rows[0]["email"].ToString();
                //Lấy danh sách những người nhận thông báo theo quy trình
                var data = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid.ToString() == id_quytrinh).FirstOrDefault();
                string tinhtrang_field = ValidField;
                int index = tinhtrang_field.IndexOf("=");
                if (index >= 0)
                    tinhtrang_field = tinhtrang_field.Remove(index).Replace("\"", "");
                string select = "select top(1) " + tinhtrang_field + " \"valid\" from \"" + TableName + "\" where \"" + PrimaryKeyField + "\"=" + Id_Phieu;
                var quytrinh = new
                {
                    Valid = "",
                    ListCCWhenAccept = "",
                    ListCCWhenReject = ""
                };
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = select;
                    _context.Database.OpenConnection();
                    using (var result = command.ExecuteReader())
                    {
                        while (result.Read())
                        {
                            quytrinh = new
                            {
                                Valid = result[0].ToString(),
                                ListCCWhenAccept = data.Listccwhenaccept.ToString(),
                                ListCCWhenReject = data.Listccwhenreject.ToString()
                            };

                        }
                    }
                }
                Hashtable tmpReplace = GetReplaceMail_Notify(Id_Phieu, 1, IsAccepted);
                var formatLink = tmpReplace["$link$"].ToString().Replace("$id$", Id_Phieu.ToString());
                ThongBaoHelper.sendThongBao(Loai, User, new List<long>() { long.Parse(dt.Rows[0]["id_nv"].ToString()) }, string.IsNullOrEmpty(messageNotifice) ? (IsAccepted ? AcceptMailtitle : RejectMailtitle) : messageNotifice, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);

                if (!string.IsNullOrEmpty(CompleteMailtemplate))
                {
                    string mailtitle = AcceptMailtitle;
                    MailAddressCollection cc = new MailAddressCollection();
                    Hashtable replace = GetReplaceMail_Notify(Id_Phieu, 2, IsAccepted, IsReturn);
                    replace.Add("$your$", "Your");
                    replace.Add("$cuanhanvien_en$", "");
                    replace.Add("$cuanhanvien$", "của bạn");
                    replace.Add("$GhiChu$", Message);
                    string TemplateMail = _hostingEnvironment.ContentRootPath + @"\MailTemplate\" + CompleteMailtemplate;
                    Uri uri = new Uri(TemplateMail);
                    //string TemplateMail = HttpContext.Current.Server.MapPath("~/MailTemplate/" + CompleteMailtemplate);
                    if (!string.IsNullOrEmpty(quytrinh.Valid))
                    {
                        string list = quytrinh.ListCCWhenAccept.ToString();
                        if (bool.FalseString.Equals(quytrinh.Valid))
                        {
                            list = quytrinh.ListCCWhenReject.ToString();
                            mailtitle = RejectMailtitle;
                        }
                        //Conds = new SqlConditions();
                        //Conds.Add("id_nv", list.Split(','), SqlOperator.In);
                        //Conds.Add("thoiviec", 0);
                        //select = "select email from tbl_nhanvien where (where)";
                        //DataTable nhanvien = Conn.CreateDataTable(select, "(where)", Conds);
                        var lst = list.Split(",").ToList();
                        var cnn = new DpsConnection(_config.ConnectionString);
                        bool cc_lienquan = false;
                        var vals = LiteController.GetConfigs(new List<string>() { "CC_LIENQUAN" }, cnn);
                        if (vals != null)
                        {
                            if (vals.ContainsKey("CC_LIENQUAN"))
                                cc_lienquan = vals["CC_LIENQUAN"].ToString() == "1";
                        }
                        if (cc_lienquan)
                        {
                            string sql = "select Checker, checkers from quytrinh_quatrinhduyet where id_phieu=@id_phieu and loai=@loai";
                            DataTable dtC = cnn.CreateDataTable(sql, new SqlConditions() { { "id_phieu", Id_Phieu }, { "loai", Loai } });
                            foreach (DataRow dr in dtC.Rows)
                            {
                                if (dr["Checker"] != DBNull.Value)
                                    lst.Add(dr["Checker"].ToString());
                                if (dr["Checkers"] != DBNull.Value)
                                    lst.AddRange(dr["Checkers"].ToString().Split(","));
                            }
                        }
                        var nhanvien = TblNhanvien.Where(x => lst.Contains(x.UserID.ToString())).ToList();
                        for (int i = 0; i < nhanvien.Count; i++)
                        {
                            if (!string.IsNullOrEmpty(nhanvien[i].Email))
                            {
                                MailAddress item = new MailAddress(nhanvien[i].Email);
                                if (!cc.Contains(item)) cc.Add(item);
                            }
                        }
                    }
                    replace.Add("$nguoinhan$", dt.Rows[0]["hoten"].ToString());
                    replace.Add("$SysName$", _config.SysName);
                    if (IsReturn)
                        mailtitle = RejectMailtitle;
                    SendMail.Send(TemplateMail, replace, mail, mailtitle, cc, null, null, true, out ErrorMessage, MInfo);
                }
                if (IsAccepted && !string.IsNullOrEmpty(OutputMailtemplate))//phiếu duyệt xong cần gửi mail output
                {
                    List<string> lst = GetOutput(long.Parse(id_quytrinh), User.ToString());
                    string mailtitle = OutputMailtitle;
                    string Attachefile = "";
                    Hashtable replace = GetReplaceMail_Notify(Id_Phieu, 2, IsAccepted);
                    replace.Add("$your$", "Your");
                    replace.Add("$cuanhanvien_en$", "");
                    replace.Add("$cuanhanvien$", "của bạn");
                    string TemplateMail = _hostingEnvironment.ContentRootPath + @"\MailTemplate\" + OutputMailtemplate;
                    MailAddressCollection To = new MailAddressCollection();
                    Uri uri = new Uri(TemplateMail);
                    string nguoinhan = "";
                    var nhanvien = TblNhanvien.Where(x => lst.Contains(x.UserID.ToString())).ToList();
                    List<long> lstNV = new List<long>();
                    for (int i = 0; i < nhanvien.Count; i++)
                    {
                        lstNV.Add(nhanvien[i].UserID);
                        if (!string.IsNullOrEmpty(nhanvien[i].Email))
                        {
                            MailAddress item = new MailAddress(nhanvien[i].Email);
                            if (!To.Contains(item))
                            {
                                nguoinhan += (nguoinhan == "" ? "" : ", ") + nhanvien[i].FullName;
                                To.Add(item);
                            }
                        }
                    }

                    if (!string.IsNullOrEmpty(messageOutput))
                    {
                        mailtitle = messageOutput;
                    }
                    ThongBaoHelper.sendThongBao(Loai, User, lstNV, mailtitle, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);
                    replace.Add("$nguoinhan$", nguoinhan);
                    replace.Add("$SysName$", _config.SysName);
                    SendMail.Send(TemplateMail, replace, To, mailtitle, new MailAddressCollection(), null, null, true, out ErrorMessage, MInfo);
                }
                return true;
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
                return false;
            }

        }

        /// <summary>
        /// Lấy dữ liệu duyệt của phiếu loại trừ người đang duyệt ra
        /// </summary>
        /// <param name="Id_phieu">phiếu</param>
        /// <param name="Nguoiduyet">Người sẽ duyệt</param>
        /// <param name="vitriduyet">Danh nghĩa của người duyệt</param>
        /// <param name="tennguoiduyet">Tên người duyệt</param>
        /// <returns>Danh sách các thông tin duyệt ko bao gồm thông tin người duyệt hiện tại</returns>
        public DataTable GetDataApproved(long Id_phieu, string Nguoiduyet, out string vitriduyet, out string tennguoiduyet, out DateTime? Deadline)
        {
            //Lấy id_duyệt của nhân viên và thông tin người duyệt
            var Rw = (from qt in _context.QuytrinhQuatrinhduyet
                      join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                      where qt.IdPhieu == Id_phieu && qt.Loai == Loai && !qt.Valid.HasValue
                      orderby qt.Priority
                      select new
                      {
                          IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                          Checker = qt.Checker,
                          Checkers = qt.Checkers,
                          ListChecker = qt.ListChecker,
                          FullName = "",
                          Title = dm.Title,
                          Deadline = qt.Deadline
                      }).FirstOrDefault();
            if (Rw == null)
            {
                Deadline = null;
                vitriduyet = "";
                tennguoiduyet = "";
                return null;
            }
            if (!(Rw.Checker != null && Rw.Checker.ToString().Equals(Nguoiduyet)) && !Rw.ListChecker.Contains(Nguoiduyet))
            {
                Deadline = null;
                vitriduyet = "";
                tennguoiduyet = "";
                return null;
            }
            var nvs = TblNhanvien.Where(x => Rw.Checker == x.UserID || Rw.ListChecker.Contains(x.UserID.ToString())).Select(x => x.FullName).ToList();
            Deadline = Rw.Deadline;
            vitriduyet = Rw.Title.ToString();
            tennguoiduyet = string.Join(", ", nvs);
            //Lấy danh sách những người duyệt
            DataTable dt = new DataTable();
            dt.Columns.Add("CheckedDate", typeof(DateTime));
            dt.Columns.Add("Valid", typeof(bool));
            dt.Columns.Add("CheckNote", typeof(string));
            dt.Columns.Add("hoten", typeof(string));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("Checker", typeof(long));
            dt.Columns.Add("Checkers", typeof(string));
            dt.Columns.Add("Id_quytrinh_capquanly", typeof(long));
            dt.Columns.Add("Deadline", typeof(DateTime));
            var data = (from qt in _context.QuytrinhQuatrinhduyet
                        join nv in TblNhanvien on qt.Checker equals nv.UserID into item
                        from nv in item.DefaultIfEmpty()
                        join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                        where qt.Loai == Loai && qt.IdPhieu == Id_phieu && qt.IdQuytrinhCapquanly != Rw.IdQuytrinhCapquanly
                        select new
                        {
                            Checkeddate = qt.Checkeddate,
                            Valid = qt.Valid,
                            Checknote = qt.Checknote,
                            FullName = nv.FullName,
                            Title = dm.Title,
                            IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                            Checker = qt.Checker,
                            Checkers = qt.Checkers,
                            Deadline = qt.Deadline
                        }).ToList();
            foreach (var item in data)
            {
                DataRow dr = dt.NewRow();
                if (item.Checkeddate.HasValue)
                    dr["CheckedDate"] = item.Checkeddate;
                if (item.Valid.HasValue)
                    dr["Valid"] = item.Valid;
                dr["CheckNote"] = item.Checknote;
                dr["hoten"] = item.FullName;
                dr["title"] = item.Title;
                if (item.Checker.HasValue)
                    dr["Checker"] = item.Checker;
                dr["Checkers"] = item.Checkers;
                dr["Id_quytrinh_capquanly"] = item.IdQuytrinhCapquanly;
                if (item.Deadline.HasValue)
                    dr["Deadline"] = item.Deadline.Value;
                else
                    dr["Deadline"] = DBNull.Value;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Lấy danh sách tất cả thông tin duyệt của 1 phiếu
        /// </summary>
        /// <param name="Id_phieu">phiếu</param>
        /// <returns></returns>
        public DataTable GetDataApproved(long Id_phieu)
        {

            DataTable dt = new DataTable();
            dt.Columns.Add("Priority", typeof(int));
            dt.Columns.Add("CheckedDate", typeof(DateTime));
            dt.Columns.Add("Valid", typeof(bool));
            dt.Columns.Add("CheckNote", typeof(string));
            dt.Columns.Add("hoten", typeof(string));
            dt.Columns.Add("title", typeof(string));
            dt.Columns.Add("Checker", typeof(long));
            dt.Columns.Add("Id_quytrinh_capquanly", typeof(long));
            dt.Columns.Add("Deadline", typeof(DateTime));
            var data = (from qt in _context.QuytrinhQuatrinhduyet
                        join nv in TblNhanvien on qt.Checker equals nv.UserID into item
                        from nv in item.DefaultIfEmpty()
                        join dm in _context.QuytrinhCapquanlyduyet on qt.IdQuytrinhCapquanly equals dm.Rowid
                        where qt.Loai == Loai && qt.IdPhieu == Id_phieu
                        orderby dm.Priority, qt.Checkeddate descending
                        select new
                        {
                            Priority = dm.Priority,
                            Checkeddate = qt.Checkeddate,
                            Valid = qt.Valid,
                            Checknote = qt.Checknote,
                            FullName = nv.FullName,
                            Title = dm.Title,
                            IdQuytrinhCapquanly = qt.IdQuytrinhCapquanly,
                            Checker = qt.Checker,
                            Deadline = qt.Deadline
                        }).ToList();
            foreach (var item in data)
            {
                DataRow dr = dt.NewRow();
                if (item.Priority.HasValue)
                    dr["Priority"] = item.Priority.Value;
                if (item.Checkeddate.HasValue)
                    dr["CheckedDate"] = item.Checkeddate.Value;
                if (item.Valid.HasValue)
                    dr["Valid"] = item.Valid.Value;
                dr["CheckNote"] = item.Checknote;
                dr["hoten"] = item.FullName;
                dr["title"] = item.Title;
                if (item.Checker.HasValue)
                    dr["Checker"] = item.Checker.Value;
                dr["Id_quytrinh_capquanly"] = item.IdQuytrinhCapquanly;
                if (item.Deadline.HasValue)
                    dr["Deadline"] = item.Deadline.Value;
                else
                    dr["Deadline"] = DBNull.Value;
                dt.Rows.Add(dr);
            }
            return dt;
        }

        /// <summary>
        /// Lấy người duyệt đầu tiên trong 1 quy trình duyệt. Xét đơn duyệt theo 1 nhân viên
        /// </summary>
        /// <param name="id_quytrinh">ID quy trình</param>
        /// <param name="id_nv">Nhân viên cần duyệt</param>
        /// <returns>ID nhân viên duyệt</returns>
        //public string GetIDBeginApproveUser(int id_quytrinh, DpsConnection cnn, string id_chucdanh)
        //{
        //    string result = GetBeginApproverByPosition(id_quytrinh, id_chucdanh, cnn);
        //    return result;
        //}

        /// <summary>
        /// Lấy người duyệt đầu tiền trong 1 quy trình. Xét đơn duyệt theo 1 chức danh
        /// </summary>
        /// <param name="id_quytrinh"></param>
        /// <param name="id_chucdanh"></param>
        /// <param name="cnn"></param>
        /// <returns></returns>
        //public string GetBeginApproverByPosition(int id_quytrinh, string id_chucdanh, DpsConnection cnn)
        //{
        //    try
        //    {
        //        int? range = (from chucdanh in _context.TblChucdanh
        //                      join cap in _context.DmCapquanly on chucdanh.IdCapquanly equals cap.Rowid
        //                      where chucdanh.IdRow.ToString() == id_chucdanh
        //                      select cap.Range).FirstOrDefault();

        //        //        select = @"select DM_Quytrinh_Capquanlyduyet.id_chucdanh, DM_Quytrinh_Capquanlyduyet.id_capquanly, DM_Capquanly.Range from DM_Quytrinh_Capquanlyduyet 
        //        //left join DM_Capquanly on DM_Quytrinh_Capquanlyduyet.Id_Capquanly=DM_Capquanly.RowID and DM_Quytrinh_Capquanlyduyet.Disable=0 
        //        //where (where) and DM_Quytrinh_Capquanlyduyet.disable=0 order by Priority";
        //        //        cond = new SqlConditions();
        //        //        cond.Add("Id_Quytrinh", id_quytrinh);
        //        //        dt = cnn.CreateDataTable(select, "(where)", cond);
        //        var dt = (from dm in _context.DmQuytrinhCapquanlyduyet
        //                  join cap in _context.DmCapquanly on dm.IdCapquanly equals cap.Rowid into item
        //                  from cap in item.DefaultIfEmpty()
        //                  where dm.Disable == false
        //                  select new
        //                  {
        //                      IdChucdanh = dm.IdChucdanh,
        //                      IdCapquanly = dm.IdCapquanly,
        //                      Range = cap.Range
        //                  }).ToList();

        //        if (dt == null || dt.Count == 0)
        //        {
        //            return "";
        //        }
        //        int i = 0;
        //        if (range > 0)
        //        {
        //            int Capquanly_range = 0;
        //            for (i = 0; i < dt.Count; i++)
        //            {
        //                int.TryParse(dt[i].Range.ToString(), out Capquanly_range);
        //                if ((Capquanly_range <= 0) || (Capquanly_range > range)) break;
        //            }
        //        }
        //        Employee bemp = new Employee(_context);
        //        string result = "";
        //        if (i == dt.Count)
        //        {
        //            //Trường hợp các cấp duyệt đều nhỏ hơn cấp của nhân viên gửi thì lấy quản lý trực tiếp
        //            //cond = new SqlConditions();
        //            //cond.Add("id_nv", id_nv);
        //            //DataTable dtchucdanh = cnn.CreateDataTable("select id_chucdanh from tbl_nhanvien where (where)", "(where)", cond);
        //            //if (dtchucdanh.Rows.Count <= 0) return "";
        //            //string id_chucdanh = dtchucdanh.Rows[0][0].ToString();
        //            //result = bemp.GetIDManager(id_chucdanh, cnn);
        //            //return result;
        //            return "";
        //        }
        //        var drow = dt[i];
        //        if (!"".Equals(drow.IdChucdanh.ToString()))
        //        {
        //            //Chức danh cụ thể
        //            result = bemp.GetIDNVByChucdanh(drow.IdChucdanh.ToString(), cnn);
        //        }
        //        else
        //        {
        //            if ("0".Equals(drow.IdCapquanly.ToString()))
        //                result = bemp.GetIDManager(id_chucdanh, cnn);
        //            else
        //            {
        //                //Tim theo cap quan ly
        //                result = bemp.GetIDQuanlyByCap(id_chucdanh, drow.IdCapquanly.ToString(), cnn, false);
        //            }
        //        }
        //        string uyquyen = GetUyquyen(result, DateTime.Today, cnn);
        //        if (!uyquyen.Equals("")) return uyquyen;
        //        return result;
        //    }
        //    catch (Exception ex)
        //    {
        //        ErrorMessage = ex.Message;
        //        return "";
        //    }
        //}

        /// <summary>
        /// Lấy người duyệt đầu tiên trong quy trình duyệt
        /// </summary>
        /// <param name="id_quytrinh"></param>
        /// <param name="id_nv">nhân viên cần duyệt</param>
        /// <returns>Thông tin người duyệt</returns>        
        //public EDTO GetBeginApproveUser(int id_quytrinh, string id_nv, DpsConnection cnn)
        //{
        //    EDTO result = new EDTO();
        //    string select = "select id_chucdanh from tbl_nhanvien where id_nv=@id_nv";
        //    SqlConditions cond = new SqlConditions();
        //    cond.Add("id_nv", id_nv);
        //    DataTable dt = cnn.CreateDataTable(select, cond);
        //    if (dt.Rows.Count > 0)
        //        result = GetBeginApproveUserByPosition(id_quytrinh, dt.Rows[0][0].ToString(), cnn);

        //    //Employee bemp = new Employee(_context);
        //    //EDTO result = bemp.GetById(id_manager, cnn);
        //    return result;
        //}
        //public EDTO GetBeginApproveUserByPosition(int id_quytrinh, string id_chucdanh, DpsConnection cnn)
        //{
        //    EDTO result = new EDTO();
        //    string id_manager = GetIDBeginApproveUser(id_quytrinh, cnn, id_chucdanh);
        //    Employee bemp = new Employee(_context);
        //    result = bemp.GetById(id_manager, cnn);
        //    return result;
        //}

        public DataTable GetCapduyetByQuytrinh(string id_quytrinh, string id_chucdanh)
        {
            //Lấy cấp của người duyệt
            var data = (from dm in _context.QuytrinhCapquanlyduyet
                        join cap in DmCapquanly on dm.IdCapquanly equals cap.Rowid into item
                        from cap in item.DefaultIfEmpty()
                        where dm.IdQuytrinh.ToString() == id_quytrinh
                        orderby dm.Priority
                        select new
                        {
                            Rowid = dm.Rowid,
                            Title = dm.Title,
                            IdChucdanh = dm.IdChucdanh,
                            IdCapquanly = dm.IdCapquanly,
                            Range = cap.Range
                        });
            int? range = (from chucdanh in TblChucdanh
                          join cap in DmCapquanly on chucdanh.IdCapquanly equals cap.Rowid
                          where chucdanh.IdRow.ToString() == id_chucdanh
                          select cap.Range).FirstOrDefault();
            if (range > 0)
            {
                data = data.Where(x => x.IdChucdanh.HasValue || x.IdCapquanly == 0 || x.IdCapquanly == -3 || x.Range > range);
            }
            DataTable dt = new DataTable();
            dt.Columns.Add("rowid", typeof(long));
            dt.Columns.Add("title", typeof(string));
            foreach (var item in data)
            {
                DataRow dr = dt.NewRow();
                dr["rowid"] = item.Rowid.ToString();
                dr["title"] = item.Title.ToString();
                dt.Rows.Add(dr);
            }
            return dt;
        }

        //public string BuilKhungduyet(string id_quytrinh, DpsConnection cnn, string id_chucdanh)
        //{
        //    string Template = @"<td style=""text-align:center; vertical-align: top"">
        //                <table style=""width:100%""  border=""1"" class=""table-bordered table-striped table-condensed cf"">
        //                    <tr>
        //                        <td colspan=""2"" style=""border-bottom: solid 1px darkgray"">{0}</td>
        //                    </tr><tr>
        //                        <td colspan=""2"" style=""height:50px"">&nbsp;</td>
        //                    </tr><tr>
        //                        <td style=""width:30%; text-align:left"">{2}:</td>
        //                        <td class=""sau"" style=""width:70%;"">{1}</td>
        //                    </tr>
        //                </table>
        //            </td>";
        //    DataTable dt = GetCapduyetByQuytrinh(id_quytrinh.ToString(), cnn, id_chucdanh);
        //    StringBuilder data = new StringBuilder();
        //    //using (DpsConnection cnn = new DpsConnection())
        //    //{
        //    for (int i = 0; i < dt.Rows.Count; i++)
        //    {
        //        string nguoiduyet = "";
        //        string idnguoiduyet = GetChecker(dt.Rows[i]["RowID"].ToString(), cnn, id_chucdanh);
        //        if (!"".Equals(idnguoiduyet))
        //        {
        //            string select = "select holot + ' ' + ten as hoten, id_nv from tbl_nhanvien where (where)";
        //            SqlConditions cond = new SqlConditions();
        //            cond.Add("id_nv", idnguoiduyet);
        //            DataTable tmp = cnn.CreateDataTable(select, "(where)", cond);
        //            if (tmp.Rows.Count > 0) nguoiduyet = tmp.Rows[0][0].ToString();
        //        }
        //        data.AppendFormat(Template, new string[] { dt.Rows[i]["title"].ToString(), nguoiduyet, "Họ tên" });
        //    }
        //    string result = "<table cellpadding=\"0\" cellspacing=\"4\" style=\"width:100%\"><tr>" + data.ToString() + "</tr></table>";
        //    return result;
        //    //}
        //}

        public void SendThongbaoChonnguoiduyet(string listnhanthongbao, long id_phieu)
        {
            if (!"".Equals(listnhanthongbao))
            {
                //Lấy nhân viên
                string select = "select fullname as hoten, dps_user.email, dps_user.UserID, cd.Tenchucdanh from \"" + (useTableName ? TableName : DataViewName) + "\" as tb inner join dps_user on tb.\"" + SenderField + "\"=dps_user.UserID join Tbl_Chucdanh cd on cd.Id_row = dps_user.IdChucVu where \"" + PrimaryKeyField + "\"=" + id_phieu;
                DataTable dt = new DataTable();
                using (var command = _context.Database.GetDbConnection().CreateCommand())
                {
                    command.CommandText = select;
                    IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                    dbDataAdapter.SelectCommand = command;
                    DataSet dataSet = new DataSet();
                    dbDataAdapter.Fill(dataSet);
                    if (dataSet != null && dataSet.Tables.Count > 0)
                        dt = dataSet.Tables[0];
                }
                if (dt.Rows.Count > 0)
                {
                    string tennhanvien = dt.Rows[0][0].ToString();
                    string tenPhieu = _context.QuytrinhLoaiphieuduyet.Where(x => x.Rowid == Loai).Select(x => x.Description).FirstOrDefault();
                    string title = (string.IsNullOrEmpty(tenPhieu) ? "Đơn không xác định" : tenPhieu) + " của nhân viên " + tennhanvien + " không tìm thấy người duyệt";
                    List<string> listcc = listnhanthongbao.Split(',').ToList();
                    var dttmp = TblNhanvien.Where(x => listcc.Contains(x.UserID.ToString())).ToList();
                    MailAddressCollection MailTo = new MailAddressCollection();
                    List<long> lst = new List<long>();
                    for (int j = 0; j < dttmp.Count; j++)
                    {
                        lst.Add(dttmp[j].UserID);
                        if (!string.IsNullOrEmpty(dttmp[j].Email)) MailTo.Add(dttmp[j].Email);
                    }
                    Hashtable tmpReplace = GetReplaceMail_Notify(id_phieu, 1, true);
                    var formatLink = tmpReplace["$link$"].ToString().Replace("$id$", id_phieu.ToString());
                    ThongBaoHelper.sendThongBao(Loai, User, lst, title, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);
                    if (MailTo.Count > 0)
                    {
                        string template = _hostingEnvironment.ContentRootPath + @"\MailTemplate\Donkhongconguoiduyet.htm";
                        //string template = HttpContext.Current.Server.MapPath("~/MailTemplate/Donkhongconguoiduyet.htm");
                        Hashtable replace = new Hashtable();
                        replace.Add("$tieude$", title);
                        string link = _config.LinkBackend + "/theo-doi-quy-trinh";
                        replace.Add("$link$", link);
                        replace["$SysName$"] = _config.SysName;
                        SendMail.Send(template, replace, MailTo, title, new MailAddressCollection(), null, null, true, out ErrorMessage, MInfo);
                    }
                }
            }
        }

        private string GetTenPhieu
        {
            get
            {
                switch (Loai)
                {
                    case 1: return "Đơn đăng ký tiếp khách";
                    default: return "Đơn không xác định";
                }
            }
        }

        private string GetUyquyen(string id_nv, DateTime ngay)
        {
            //var dt = (from uy in _context.TblUyquyenduyetphieu
            //          join de in _context.TblUyquyenduyetphieuDetails on uy.IdRow equals de.IdUyquyen
            //          where uy.IdNv.ToString() == id_nv && de.IdLoaiphieu == Loai && uy.Tungay <= ngay && uy.Denngay >= ngay
            //          select uy).FirstOrDefault();
            //if (dt != null)
            //    return dt.IdNvuyquyen.ToString();
            return "";
        }

        //    public static string GetUyquyen(string id_nv, DateTime ngay, DpsConnection cnn, int Loaiphieu)
        //    {
        //        string s = @"select id_nvuyquyen from tbl_uyquyenduyetphieu tbl inner join tbl_uyquyenduyetphieu_details d on tbl.id_row=d.id_uyquyen 
        //where tbl.id_nv=@id_nv and d.id_loaiphieu=@id_loaiphieu and tbl.tungay<=@ngay and tbl.denngay>=@ngay";
        //        SqlConditions cond = new SqlConditions();
        //        cond.Add("id_nv", id_nv);
        //        cond.Add("id_loaiphieu", Loaiphieu);
        //        cond.Add("ngay", ngay);
        //        DataTable dt = cnn.CreateDataTable(s, cond);
        //        if (dt.Rows.Count > 0) return dt.Rows[0]["id_nvuyquyen"].ToString();
        //        return "";
        //    }
        //private void NotifyReturnToSender(long Id_phieu)
        //{
        //    //Lấy người gửi
        //    string select = "select email,fullname AS hoten ,dps_user.UserID, cd.Tenchucdanh  as id_nv from \"" + (useTableName ? TableName : DataViewName) + "\" as bang inner join dps_user on bang.\"" + SenderField + "\"=dps_user.UserID join Tbl_Chucdanh cd on cd.Id_row = dps_user.IdChucVu where \"" + PrimaryKeyField + "\" = " + Id_phieu;
        //    DataTable dt = null;
        //    using (var command = _context.Database.GetDbConnection().CreateCommand())
        //    {
        //        command.CommandText = select;
        //        IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
        //        dbDataAdapter.SelectCommand = command;
        //        DataSet dataSet = new DataSet();
        //        dbDataAdapter.Fill(dataSet);
        //        if (dataSet != null && dataSet.Tables.Count > 0)
        //            dt = dataSet.Tables[0];
        //        if (dt == null || dt.Rows.Count == 0)
        //        {
        //            return;
        //        }
        //    }
        //    string MailTitle = RejectMailtitle.Replace("$tennhanvien$", "bạn");
        //    Hashtable tmpReplace = GetReplaceMail_Notify(Id_phieu, 1, true);
        //    var formatLink = tmpReplace["$link$"].ToString().Replace("$id$", Id_phieu.ToString());
        //    ThongBaoHelper.sendThongBao(Loai, User, new List<long>() { long.Parse(dt.Rows[0]["id_nv"].ToString()) }, MailTitle, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, null);
        //    if (!string.IsNullOrEmpty(CompleteMailtemplate))
        //    {
        //        Hashtable replace = GetReplaceMail_Notify(Id_phieu, 2, false);
        //        string EmailNhanvien = dt.Rows[0]["email"].ToString();
        //        replace.Add("$nguoinhan$", dt.Rows[0]["hoten"].ToString());
        //        replace.Add("$your$", "Your ");
        //        replace.Add("$cuanhanvien_en$", "");
        //        replace.Add("$cuanhanvien$", "của bạn");
        //        replace.Add("$SysName$", _config.SysName);
        //        if (IsValidEmailAddress(EmailNhanvien))
        //        {
        //            MailAddressCollection cc = new MailAddressCollection();
        //            string Attachefile = "";
        //            string TemplateMail = _hostingEnvironment.ContentRootPath + @"\MailTemplate\" + CompleteMailtemplate;
        //            SendMail.Send(TemplateMail, replace, EmailNhanvien, MailTitle, cc, null, null, true, out ErrorMessage, MInfo);
        //        }
        //    }
        //}

        private string NotifyToPreviousChecker(long Id_phieu, int PreviousPriority, out string id_nguoiduyet, IDbContextTransaction transaction = null)
        {
            string Hotennguoiduoctralai = "";
            id_nguoiduyet = "";
            var checker = _context.QuytrinhQuatrinhduyet.Where(x => x.IdPhieu == Id_phieu && x.Loai == Loai && x.Priority == PreviousPriority).Select(x => x.Checker).FirstOrDefault();
            var dt = TblNhanvien.Where(x => checker == x.UserID).FirstOrDefault();
            string MailTitle = "";
            string MailTo = "";
            string Tennhanvien = "";
            if (dt != null && !string.IsNullOrEmpty(dt.Email))
            {
                Tennhanvien = dt.FullName;
                MailTitle = RejectMailtitle.Replace("$tennhanvien$", Tennhanvien);
                Hashtable tmpReplace = GetReplaceMail_Notify(Id_phieu, 1, true, false, transaction);
                var formatLink = tmpReplace["$link$"].ToString().Replace("$id$", Id_phieu.ToString());
                ThongBaoHelper.sendThongBao(Loai, User, new List<long>() { dt.UserID }, MailTitle, formatLink.Replace(_config.LinkBackend, ""), _config, _hostingEnvironment, _hub_context, false);
                MailTo = dt.Email;
            }
            //string select = "select manv, holot, ten, email, id_nv from tbl_nhanvien where id_nv in (select Checker from quytrinh_quatrinhduyet where Id_phieu=@id_phieu and Loai=@Loai and Priority=@priority)";
            //SqlConditions Conds = new SqlConditions();
            //Conds.Add("id_phieu", Id_phieu);
            //Conds.Add("Loai", Loai);
            //Conds.Add("priority", PreviousPriority);
            //DataTable dt = Conn.CreateDataTable(select, Conds);
            //for (int i = 0; i < dt.Count; i++)
            //{
            //    Hotennguoiduoctralai += ", " + dt[i].FullName.ToString();
            //    id_nguoiduyet = dt[i].IdNv.ToString();
            //    if (IsValidEmailAddress(dt[i].Email.ToString()))
            //    {
            //        MailTo.Add(dt[i].Email.ToString());
            //    }
            //}

            //if (!"".Equals(Hotennguoiduoctralai)) Hotennguoiduoctralai = Hotennguoiduoctralai.Substring(2);
            if (!string.IsNullOrEmpty(CompleteMailtemplate))
            {
                MailAddressCollection cc = new MailAddressCollection();
                string Attachefile = "";
                Hashtable replace = GetReplaceMail_Notify(Id_phieu, 2, false, true, transaction);
                replace.Add("$nguoinhan$", Tennhanvien);
                replace.Add("$your$", "");
                replace.Add("$cuanhanvien_en$", "from " + replace["$FullName$"] ?? "");
                replace.Add("$cuanhanvien$", "của " + replace["$FullName$"] ?? "");
                if (replace["$GhiChu$"] == null)
                    replace.Add("$GhiChu$", Message);
                else
                    replace["$GhiChu$"] = Message;
                replace.Add("$SysName$", _config.SysName);

                string TemplateMail = _hostingEnvironment.ContentRootPath + @"\MailTemplate\" + CompleteMailtemplate;
                //string TemplateMail = HttpContext.Current.Server.MapPath("~/MailTemplate/" + CompleteMailtemplate);
                SendMail.Send(TemplateMail, replace, MailTo, MailTitle, cc, null, null, true, out ErrorMessage, MInfo);
            }
            return Hotennguoiduoctralai;
        }

        /// <summary>
        /// Lấy cấp của nhân viên. Bao gồm rank của cấp và id_capquanly trong bảng dm_capquanly
        /// </summary>
        /// <param name="id_nv"></param>
        /// <param name="cnn"></param>
        /// <param name="Id_Capquanly"></param>
        /// <returns></returns>
        private int GetCapNhanvien(string id_nv, out string Id_Capquanly)
        {
            int result = 0;
            string r_id_capquanly = "";
            var data = (from nv in TblNhanvien
                        join chucdanh in TblChucdanh on nv.IdChucVu equals chucdanh.IdRow
                        join dm in DmCapquanly on chucdanh.IdCapquanly equals dm.Rowid into item
                        from dm in item.DefaultIfEmpty()
                        where nv.UserID.ToString() == id_nv
                        select new
                        {
                            Range = dm.Range,
                            Rowid = dm.Rowid,
                            IdChucdanh = nv.IdChucVu
                        }).FirstOrDefault();
            if (data != null)
            {
                int.TryParse(data.Range.ToString(), out result);
                r_id_capquanly = data.Rowid.ToString();
            }
            Id_Capquanly = r_id_capquanly;
            return result;
        }

        /// <summary>
        /// Lấy replace gửi mail theo cách lấy dữ liệu từ table
        /// </summary>
        /// <param name="Id_phieu"></param>
        /// <param name="Loaimail">1: mail notify cho quản lý duyệt, 2: gửi phiếu đã được duyệt</param>
        /// <param name="IsAccept">Phiếu được duyệt hay không, áp dụng đối với trường hợp gửi thông báo kết quả duyệt. Trường hợp k duyệt, kiểm tra thêm processmethod</param>
        /// <returns></returns>
        private Hashtable GetReplaceMail_Notify(long Id_phieu, int Loaimail, bool IsAccept, bool isReturn = false, IDbContextTransaction transaction = null)
        {
            Hashtable result = new Hashtable();
            DataTable data = new DataTable();
            string select = "select * from \"" + DataViewName + "\" where \"" + PrimaryKeyField + "\"=" + Id_phieu;
            if (string.IsNullOrEmpty(DataViewName))
                select = "select dps_user.email,fullname AS hoten, dps_user.UserID, cd.Tenchucdanh from dps_user join Tbl_Chucdanh cd on cd.Id_row = dps_user.IdChucVu where UserID = " + User;

            select += "";
            using (var command = _context.Database.GetDbConnection().CreateCommand())
            {
                command.CommandText = select;
                if (transaction != null)
                    command.Transaction = transaction.GetDbTransaction();
                IDbDataAdapter dbDataAdapter = new SqlDataAdapter();
                dbDataAdapter.SelectCommand = command;
                DataSet dataSet = new DataSet();
                dbDataAdapter.Fill(dataSet);
                if (dataSet != null && dataSet.Tables.Count > 0)
                    data = dataSet.Tables[0];
            }
            if (data.Rows.Count == 1)
            {
                var ReplaceList = _context.QuytrinhLoaiphieuduyetMaildatareplace.Where(x => x.Loaiphieuduyetid == Loai && x.Loaimail == Loaimail).ToList();
                //các case là xử lý các trường hợp đặc biệt không lấy dữ liệu trong view
                foreach (var item in ReplaceList)
                {
                    string replacestring = item.Replacestring.ToString();
                    switch (replacestring)
                    {
                        case "$link$":
                            {
                                string fulllink = _config.LinkBackend + item.Columnname.Replace("$id$", Id_phieu.ToString()).Replace("$idgroup$", Loai.ToString());
                                result.Add("$link$", fulllink);
                                break;
                            }
                        case "$ketquaduyet$":
                            {
                                string ketquatext = "đã được duyệt";
                                if (!IsAccept)
                                {
                                    if (!isReturn)
                                        ketquatext = "không được duyệt";
                                    else
                                        ketquatext = "được trả lại";
                                };
                                result.Add("$ketquaduyet$", ketquatext);
                                break;
                            }
                        case "$ketquaduyet_en$":
                            {
                                string ketquatext = "approved";
                                if (!IsAccept)
                                {
                                    if (!isReturn)
                                        ketquatext = "not approved";
                                    else
                                        ketquatext = "returned";
                                }
                                result.Add("$ketquaduyet_en$", ketquatext);
                                break;
                            }
                        case "$Ngay$":
                            {
                                result.Add("$Ngay$", string.Format(item.Format, DateTime.Now));
                                break;
                            }
                        case "$GhiChu$":
                            {
                                result.Add("$GhiChu$", string.IsNullOrEmpty(Message) ? "" : Message);
                                break;
                            }
                        default:
                            {
                                if (!result.ContainsKey(replacestring))
                                {
                                    if (item.Columnname != null && data.Columns.Contains(item.Columnname))
                                    {
                                        if (!string.IsNullOrEmpty(item.Format))
                                        {
                                            string sformat = item.Format;
                                            string text = string.Format(sformat, data.Rows[0][item.Columnname]);
                                            result.Add(replacestring, text);
                                        }
                                        else
                                            result.Add(replacestring, data.Rows[0][item.Columnname].ToString());
                                    }
                                    else
                                    {
                                        result.Add(replacestring, "");
                                    }
                                }
                                break;
                            }
                    }
                }
            }
            return result;
        }

        public bool IsValidEmailAddress(string s)
        {
            if (string.IsNullOrEmpty(s))
                return false;
            else
            {
                Regex regex = new Regex(@"\w+([-+.']\w+)*@\w+([-.]\w+)*\.\w+([-.]\w+)*");
                return regex.IsMatch(s) && !s.EndsWith(".");
            }
        }

        /// <summary>
        /// Lấy cài đặt quy trình sử dụng cho loại khai báo trong bảng sys_config
        /// </summary>
        /// <param name="id_nv"></param>
        //public int Getquytrinhduyet(string id_nv, int loai = 0)
        //{
        //    if (loai == 0)
        //        loai = Loai;
        //    var conf = _context.SysConfig.Where(x => x.SysConfiggroupId == 2 && x.Pattern == loai.ToString()).FirstOrDefault();
        //    if (conf == null)
        //        return 0;
        //    int id_quytrinhgoc = 0;
        //    int.TryParse(conf.Value, out id_quytrinhgoc);
        //    if (id_quytrinhgoc <= 0) return 0;
        //    //string id_chucdanh = GetIDChucdanh(id_nv);
        //    //DataTable dt = GetCapduyetByQuytrinh(id_quytrinhgoc.ToString(), id_chucdanh);
        //    //int result = id_quytrinhgoc;
        //    //if (dt.Rows.Count <= 0)
        //    //{
        //    //    ErrorMessage = "Cấp quản lý của người duyệt không được tiến hành quy trình";
        //    //    result = GetQuytrinhthaythe(id_quytrinhgoc, id_nv);
        //    //}
        //    return id_quytrinhgoc;
        //}

        private string GetIDChucdanh(string id_nv)
        {
            string id_chucdanh = "";
            var nv = TblNhanvien.Where(x => x.UserID.ToString() == id_nv).FirstOrDefault();
            if (nv != null && nv.IdChucVu > 0)
                id_chucdanh = nv.IdChucVu.ToString();
            return id_chucdanh;
        }

        private int GetQuytrinhthaythe(int id_quytrinh, string id_nv)
        {
            var data = _context.QuytrinhQuytrinhduyet.Where(x => x.Rowid == id_quytrinh).FirstOrDefault();
            int result = 0;
            if ((data == null) || (!int.TryParse(data.IdQuytrinhthaythe.ToString(), out result)))
            {
                return 0;
            }
            string id_chucdanh = GetIDChucdanh(id_nv);
            DataTable dt = GetCapduyetByQuytrinh(result.ToString(), id_chucdanh);
            if (dt.Rows.Count <= 0)
            {
                return GetQuytrinhthaythe(result, id_nv);
            }
            return result;
        }

        public DataTable Bindduyet(Int32 id_phieu, string id_nv, DataTable dt_ApprovingUser)
        {
            bool IsVisibleApprove = true;
            bool isenable_duyet = true;
            DateTime? Deadline = null;
            string vitriduyet = "";
            string tennguoiduyet = "";
            string message = "";
            DataTable dt = new DataTable();
            List<string> lst = GetCurrentApprovingUser(id_phieu).Split(",").ToList();
            if (!lst.Contains(id_nv))
            {
                IsVisibleApprove = false;
            }

            //bool _IsFinal = IsFinal(id_phieu.ToString(), id_nv);

            if (IsVisibleApprove)
            {
                dt = GetDataApproved(id_phieu, id_nv, out vitriduyet, out tennguoiduyet, out Deadline);
                //Kiểm tra phiếu này có cho phép duyệt hay không
                bool AllowApproval = true;
                //chưa xử lý trường hợp không được phép 
                if (!AllowApproval)
                {
                    isenable_duyet = false;
                }
            }
            else dt = GetDataApproved(id_phieu);
            if (!vitriduyet.Equals(string.Empty))
            {
                if (Deadline.HasValue)
                    dt_ApprovingUser.Rows.Add(new object[] { vitriduyet, tennguoiduyet, message, isenable_duyet, Deadline.Value });
                else
                    dt_ApprovingUser.Rows.Add(new object[] { vitriduyet, tennguoiduyet, message, isenable_duyet, DBNull.Value });
            }
            return dt;
        }

        public RequiredDevCheckers RequiredDevCheckers(long Id_Phieu, int Id_Quytrinh)
        {
            RequiredDevCheckers data = new RequiredDevCheckers();
            if (!AllowDevChecker)
                data.Required = false;
            var cql = (from x in _context.QuytrinhQuatrinhduyet
                       join dm in _context.QuytrinhCapquanlyduyet on x.IdQuytrinhCapquanly equals dm.Rowid
                       where x.Loai == Loai && x.IdPhieu == Id_Phieu && dm.Disable != true && x.Valid == null
                       orderby x.Priority
                       select new { dm.IdCapquanly, dm.IdChucdanh }).ToList();
            //đã begin => bước kế bước hiện tại là -4||-5?
            if (cql.Count > 0)
            {
                if (cql.Count > 1)
                {
                    if (cql[1].IdCapquanly == -4)
                        data.Required = true;
                    if (cql[1].IdCapquanly == -5)
                    {
                        data.Required = true;
                        if (cql[1].IdChucdanh.HasValue)
                            data.IdChucdanh = cql[1].IdChucdanh.Value;
                    }
                }
                else//bc hiện tại là bc cuối
                    data.Required = false;
                return data;
            }
            var cap_first = (from x in _context.QuytrinhQuytrinhduyet
                             join cap in _context.QuytrinhCapquanlyduyet on x.Rowid equals cap.IdQuytrinh
                             where x.Rowid == Id_Quytrinh && x.Disable != true && cap.Disable != true
                             orderby cap.Priority
                             select new
                             {
                                 cap.IdCapquanly,
                                 cap.IdChucdanh
                             }).FirstOrDefault();
            if (cap_first != null)
            {
                if (cap_first.IdCapquanly == -4)
                    data.Required = true;
                if (cap_first.IdCapquanly == -5)
                {
                    data.Required = true;
                    if (cap_first.IdChucdanh.HasValue)
                        data.IdChucdanh = cap_first.IdChucdanh.Value;
                }
            }
            return data;
        }

        /// <summary>
        /// Tìm các bước và user thực hiện bước<para/>
        /// Tìm bước thuộc mà user có thể thực hiện
        /// </summary>
        /// <param name="cnn"></param>
        /// <param name="Id"></param>
        /// <param name="IdStep"></param>
        /// <param name="DoiTuong"></param>
        /// <param name="IdUser"></param>
        /// <returns></returns>
        public static BaseModel<object> CheckersByStep(DpsConnection cnn, long Id, int IdStep, int DoiTuong)
        {
            string sql = @"select cap.title, qt.* from quytrinh_quatrinhduyet qt
join quytrinh_capquanlyduyet cap on qt.id_quytrinh_capquanly = cap.rowid
where id_phieu=@id and loai=@loai and qt.id_row=@IdStep
order by priority";
            sql += @" select de.*,u.UserID, u.FullName, px.DonVi as TenPhuongXa,u.Active from quytrinh_quatrinhduyet de
join Dps_User u on (u.UserID=Checker or '%,'+Checkers+',%' like '%,'+cast(u.UserID as varchar)+',%')
join DM_DonVi px on u.IdDonVi=px.Id
where u.Deleted=0 and de.id_row=@IdStep
order by CheckedDate, FullName    ";
            DataSet ds = cnn.CreateDataSet(sql, new SqlConditions() { { "loai", DoiTuong }, { "id", Id }, { "IdStep", IdStep } });
            if (cnn.LastError != null)
            {
                return JsonResultCommon.Exception(cnn.LastError);
            }
            if (ds == null || ds.Tables[0] == null)
            {
                return JsonResultCommon.KhongTonTai("Bước xử lý");
            }
            DataTable dt = ds.Tables[0];
            var data = dt.AsEnumerable().Select(dr => new
            {
                //IdPro = dr["IdPro"],
                IdStep = dr["id_row"],
                //Next = dr["Next"],
                //SS = dr["SS"],
                //BorderColor = dr["BorderColor"],
                //AllowAddUser = dr["IdStep"].ToString() == IdStepCurr && dr["sort"].ToString() == "0" && checkExistStepNext(act, Convert.ToInt32(dr["Next"])),
                //ShowInFo = dr["IdStep"].ToString() == IdStepCurr && dr["CheckDate"] == DBNull.Value ? checkExistStepNext(act, Convert.ToInt32(dr["Next"])) : true,
                Title = dr["Title"],// + " lần " + dr["Index"],
                CheckDate = dr["CheckedDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", dr["CheckedDate"]),
                Checkers = ds.Tables[1].AsEnumerable().Where(r => r["id_row"].ToString() == dr["id_row"].ToString()
                                                                // && r["Next"].ToString() == dr["Next"].ToString()
                                                                && r["CheckedDate"].ToString() == dr["CheckedDate"].ToString())
                                                              .Select(r => new
                                                              {
                                                                  UserID = r["UserID"],
                                                                  FullName = r["FullName"],
                                                                  TenPhuongXa = r["TenPhuongXa"],
                                                                  Checked = r["valid"] != DBNull.Value && (bool)r["valid"] && r["Checker"].ToString() == r["UserID"].ToString(),
                                                                  Active = String.IsNullOrEmpty(r["Active"].ToString()) ? 0 : Convert.ToInt32(r["Active"]),
                                                                  TrangThaiUser = String.IsNullOrEmpty(r["Active"].ToString()) ? "Đã bị khóa" : Convert.ToInt32(r["Active"]) == 1 ? "Đã kích hoạt " : "Đã bị khóa",
                                                                  CheckDate = r["Checker"].ToString() != r["UserID"].ToString() ? "" :
                                                                  (r["CheckedDate"] == DBNull.Value ? "" : string.Format("{0:dd/MM/yyyy HH:mm}", r["CheckedDate"])),
                                                              }).ToList(),
                CheckersXL = ds.Tables[1].AsEnumerable().Where(r => r["id_row"].ToString() == dr["id_row"].ToString()
                                                                    // && r["Next"].ToString() == dr["Next"].ToString()
                                                                    && (String.IsNullOrEmpty(r["Active"].ToString()) ? 0 : Convert.ToInt32(r["Active"])) == 1
                                                                    && r["CheckedDate"].ToString() == dr["CheckedDate"].ToString())
                                                              .Select(r => new
                                                              {
                                                                  UserID = r["UserID"],
                                                                  FullName = r["FullName"],
                                                                  TenPhuongXa = r["TenPhuongXa"],
                                                                  Checked = r["valid"] != DBNull.Value && (bool)r["valid"] && r["Checker"].ToString() == r["UserID"].ToString(),
                                                                  Active = String.IsNullOrEmpty(r["Active"].ToString()) ? 0 : Convert.ToInt32(r["Active"]),
                                                                  TrangThaiUser = String.IsNullOrEmpty(r["Active"].ToString()) ? "Đã bị khóa" : Convert.ToInt32(r["Active"]) == 1 ? "Đã kích hoạt " : "Đã bị khóa",
                                                              }).ToList()
            });

            return JsonResultCommon.ThanhCong(data);
        }
    }
    public class RequiredDevCheckers
    {
        public bool Required { get; set; } = false;
        public long IdChucdanh { get; set; } = 0;
    }
    public class Checker4Detail
    {
        /// <summary>
        ///  đối tượng duyệt ss là phiếu thì có thể null
        /// </summary>
        public long? IdCt { get; set; }
        public List<string> Checkers { get; set; }
        public string strCheckers
        {
            get
            {
                if (Checkers == null)
                    return "";
                return string.Join(",", Checkers);
            }
        }
    }
    public class checkDetail
    {
        public long IdCt { get; set; }
        public string checknote { get; set; }
    }
    public class DuyetNhieuModel
    {
        public List<long> ids { get; set; }
        public bool value { get; set; }
        public string note { get; set; }
        public bool isTongHop { get; set; } = false;
    }
    public class DuyetDataModel
    {
        public long id { get; set; }
        public bool value { get; set; } = true;
        public string note { get; set; } = "";
        public ListImageModel FileDinhKem { get; set; }
        public DateTime? Deadline { get; set; }
        public List<string> Checkers { get; set; }
        public List<string> NguoiNhans { get; set; }
        public HuongDanModel HuongDan { get; set; }
    }
    public class HuongDanModel
    {
        public long id_quytrinh_lichsu { get; set; }
        public long Id_NCC { get; set; }
        public string NoiDung { get; set; }
        public string MoTa { get; set; }
    }
}


