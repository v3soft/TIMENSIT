using Timensit_API.Controllers.QuanLy;
using Timensit_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Timensit_API.Classes
{
    public class LiteHelper
    {
        #region giá trị mặc định
        /// <summary>
        /// Cấp duyệt mặc định
        /// </summary>
        public static List<LiteModel> CapDuyet = new List<LiteModel>()
        {
            //new LiteModel()
            //{
            //    id = -4,
            //    title = "Phiếu xác định người duyệt",
            //},
            //new LiteModel()
            //{
            //    id = -5,
            //    title = "Phiếu xác định phòng ban kết hợp với chức danh",
            //},
            new LiteModel()
            {
                id = -3,
                title = "Quyền cụ thể",
            },
            new LiteModel()
            {
                id = -2,
                title = "Chức vụ cụ thể",
            },
            new LiteModel() {
                id = -1,
                title = "Quản lý trực tiếp của cấp duyệt trước đó"
            },
            new LiteModel() {
                id = 0,
                title = "Quản lý trực tiếp"
            }
        };

        /// <summary>
        /// Lý do giảm trong đề xuất tặng quà
        /// </summary>
        public static List<LiteModel> LyDoGiam = new List<LiteModel>()
        {
            new LiteModel(){
                id = 0,
                title = "ĐT bị giảm"
            },
            new LiteModel(){
                id = 1,
                title = "Chết"
            },
            new LiteModel(){
                id = 2,
                title = "Chuyển đi"
            },
            new LiteModel(){
                id = 3,
                title = "Khác"
            }
        };
        /// <summary>
        /// Loại đối tượng
        /// </summary>
        public static List<LiteModel> NhomLoaiDoiTuongNCC = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Đối tượng theo pháp lệnh người có công"
            },
            new LiteModel(){
            id = 2,
                title = "Đối tượng ngoài pháp lệnh người có công"
            },
            new LiteModel(){
            id = 3,
                title = "Đối tượng khai báo thêm"
            }
        };

        /// <summary>
        /// Cấp cơ cấu
        /// </summary>
        public static List<LiteModel> CapCoCau = new List<LiteModel>()
        {
            new LiteModel(){
                id = 1,
                title = "Cấp tỉnh"
            },
            new LiteModel(){
                id = 2,
                title = "Cấp huyện"
            },
            new LiteModel(){
                id = 3,
                title = "Cấp xã"
            }
        };

        /// <summary>
        /// Loại chứng thư
        /// </summary>
        public static List<LiteModel> LoaiChungThu = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Viettel"
            },
            new LiteModel(){
            id = 2,
                title = "Ban cơ yếu"
            }
        };

        /// <summary>
        /// Giới tính
        /// </summary>
        public static List<LiteModel> GioiTinh = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Nam"
            },
            new LiteModel(){
            id = 2,
                title = "Nữ"
            }
        };
        /// <summary>
        /// Bằng khen đơn vị các cấp
        /// </summary>
        public static List<LiteModel> DonViCap = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Thủ tướng chính phủ"
            },
            new LiteModel(){
            id = 2,
                title = "Chủ tịch UBND tỉnh"
            },
            new LiteModel(){
            id = 3,
                title = "Hội đồng Bộ trưởng"
            }
        };
        /// <summary>
        /// Mộ an táng
        /// </summary>
        public static List<LiteModel> Mo = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Nghĩa trang"
            },
            new LiteModel(){
            id = 2,
                title = "Gia đình quản lý"
            },
            new LiteModel(){
            id = 3,
                title = "Không có thông tin"
            }
        };
        /// <summary>
        /// Tình trạng hiện tại BMVN
        /// </summary>
        public static List<LiteModel> TinhTrangBMVN = new List<LiteModel>()
        {
            new LiteModel(){
            id = 1,
                title = "Còn sống"
            },
            new LiteModel(){
            id = 2,
                title = "Từ trần"
            },
            new LiteModel(){
            id = 2,
                title = "liệt sỹ"
            }
        };


        #endregion

        #region đợt tặng quà
        /// <summary>
        /// Trạng thái đợt tặng quà Tbl_DotTangQua.Status
        /// </summary>
        public static List<LiteModel> DotTangQua_Status = new List<LiteModel>()
        {
            new LiteModel(){
                id = -1,
                title = "Xét duyệt lại",
                data = new
                {
                    color="warn"
                }
            },
            new LiteModel(){
                id = 0,
                title = "Nháp",
                data = new
                {
                    color=""
                }
            },
            new LiteModel(){
                id = 1,
                title = "Chờ phê duyệt",
                data = new
                {
                    color="metal"
                }
            },
            new LiteModel(){
                id = 2,
                title = "Đã phê duyệt",
                data = new
                {
                    color="primary"
                }
            }
        };
        /// <summary>
        /// Trạng thái hồ sơ NCC Tbl_NCC.Status
        /// </summary>861
        public static List<LiteModel> MauQD = new List<LiteModel>()
        {
            new LiteModel(){
                id = 1,
                title = "QĐ phê duyệt Tết Nguyên Đán",
                data = new
                {
                    tw = "quyet-dinh-tet.htm",
                    dp = "quyet-dinh-tet.htm", //loại này chỉ có 1 mẫu
                }
            },
            new LiteModel(){
                id = 2,
                title = "QĐ phê duyệt Lễ 27/7",
                data = new
                {
                    tw = "quyet-dinh-277-tw.htm", 
                    dp = "quyet-dinh-277-dp.htm",
                }
            }
        };
        #endregion

        #region hồ sơ NCC
        /// <summary>
        /// Trạng thái hồ sơ NCC Tbl_NCC.Status
        /// </summary>
        public static List<LiteModel> NCC_Status = new List<LiteModel>()
        {
            new LiteModel(){
                id = -1,
                title = "Xét duyệt lại",
                data = new
                {
                    color="warn"
                }
            },
            new LiteModel(){
                id = 0,
                title = "Nháp",
                data = new
                {
                    color=""
                }
            },
            new LiteModel(){
                id = 1,
                title = "Chờ phê duyệt",
                data = new
                {
                    color="metal"
                }
            },
            new LiteModel(){
                id = 2,
                title = "Đã phê duyệt",
                data = new
                {
                    color="primary"
                }
            }
        };
        #endregion

        #region Nhập số liệu
        /// <summary>
        /// Trạng thái Nhập số liệu Tbl_NhapSoLieu
        /// </summary>
        public static List<LiteModel> NhapSoLieu_Status = new List<LiteModel>()
        {
            new LiteModel(){
                id = -1,
                title = "Xét duyệt lại",
                data = new
                {
                    color="warn"
                }
            },
            new LiteModel(){
                id = 0,
                title = "Nháp",
                data = new
                {
                    color=""
                }
            },
            new LiteModel(){
                id = 1,
                title = "Chờ phê duyệt",
                data = new
                {
                    color="metal"
                }
            },
            new LiteModel(){
                id = 2,
                title = "Đã phê duyệt",
                data = new
                {
                    color="primary"
                }
            }
        };
        #endregion

        public static List<LiteModel> LoaiBieuMau = new List<LiteModel>()
        {
            new LiteModel(){ id= 1, title= "Hồ sơ" },
            new LiteModel(){ id= 2, title= "Quyết định công nhận" },
            new LiteModel(){ id= 3, title= "Quyết định trợ cấp" },
            new LiteModel(){ id= 5, title= "Quyết định thôi trả trợ cấp" },
            new LiteModel(){ id= 4, title= "Quyết định di chuyển" },
            new LiteModel(){ id= 6, title= "Phiếu báo giảm" },
            //new LiteModel(){ id= 7, title= "Đính chính thông tin" }
        };
        public static List<LiteModelT<string>> DieuKienBieuMau = new List<LiteModelT<string>>()
        {
            new LiteModelT<string>(){ id= "IsChuYeu=1", title= "Có thân nhân chủ yếu" },
            new LiteModelT<string>(){ id= "IsTruyLinh24=1", title= "Có truy lĩnh tiền tuất" },
            new LiteModelT<string>(){ id= "IsKoLDKoMaiTangPhi=1", title= "Không có lý do không mai táng phí" },
        };

        public static List<LiteModelT<string>> HDSDs = new List<LiteModelT<string>>()
        {
            new LiteModelT<string>(){
                id ="1. HDSD_NCC_DanhMuc.docx",
                title = "Danh mục"
            },
            new LiteModelT<string>(){
                id ="2. HDSD_NCC_QuanTri.doc",
                title = "Quản trị"
            },
            new LiteModelT<string>(){
                id ="3. HDSD_NCC_CauHinh.docx",
                title = "Cấu hình"
            },
            new LiteModelT<string>(){
                id ="4. HDSD_NCC_HoSoNCC.docx",
                title = "Hồ sơ người có công"
            },
            new LiteModelT<string>(){
                id ="5. HDSD_NCC_DuLieuQuaTet.docx",
                title = "Dữ liệu quà lễ tết"
            },
            new LiteModelT<string>(){
                id ="6. HDSD_NCC_SoLieuHangNam.docx",
                title = "Số liệu hàng năm"
            }
        };

        /// <summary>
        /// hàm lấy giá trị của danh sách lite dựa vào id
        /// </summary>
        /// <param name="id"></param>
        /// <param name="lites"></param>
        /// <returns></returns>
        public static string GetLiteById(int id, List<LiteModel> lites)
        {
            foreach (var item in lites)
            {
                if (item.id == id)
                    return item.title;
            }
            return "";
        }
        public static string GetLiteById(object obj, List<LiteModel> lites)
        {
            if (obj == DBNull.Value)
                return "";
            int id = int.Parse(obj.ToString());
            foreach (var item in lites)
            {
                if (item.id == id)
                    return item.title;
            }
            return "";
        }
        public static string GetLiteById(object obj, List<LiteModelT<string>> lites)
        {
            if (obj == DBNull.Value)
                return "";
            string id = obj.ToString();
            foreach (var item in lites)
            {
                if (item.id == id)
                    return item.title;
            }
            return "";
        }

        public static string convertToPDFString(string input)
        {
            //xóa property và style width
            string str = Regex.Replace(input, @"width:(.*?)", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"width:(.*?);", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"width=""(.*?)""", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            str = Regex.Replace(str, @"width=""(.*?);""", "", RegexOptions.Multiline | RegexOptions.IgnoreCase);
            string output = "<style>.pdf-container,.pdf-container table{font-size:26px;}.no-margin p{margin:0;}</style><div class='pdf-container no-margin'>";
            output += str;
            output += "</div>";
            return output;
        }

        public static string UpperCaseFirstChar(string kq)
        {
            if (string.IsNullOrEmpty(kq))
                return "";
            var aStringBuilder = new StringBuilder(kq);
            aStringBuilder.Remove(0, 1);
            aStringBuilder.Insert(0, kq[0].ToString().ToUpper());
            return aStringBuilder.ToString(); ;
        }
        public static string LowerCaseFirstChar(string kq)
        {
            if (string.IsNullOrEmpty(kq))
                return "";
            var aStringBuilder = new StringBuilder(kq);
            aStringBuilder.Remove(0, 1);
            aStringBuilder.Insert(0, kq[0].ToString().ToLower());
            return aStringBuilder.ToString();
        }

        internal static object GetValueByName(object x, string v)
        {
            var re = x.GetType().GetProperty(v).GetValue(x);
            return re;
        }
    }
}
