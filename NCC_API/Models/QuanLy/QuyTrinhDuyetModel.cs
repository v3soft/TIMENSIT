using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.QuanLy
{
    public class ProcessAddData
    {
        public int ID_QuyTrinh { get; set; } = 0;
        public int Loai { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MoTa { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiDuyetDon { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiKhongDuyetDon { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiKhongTimThayNguoiDuyetDon { get; set; }
        public int ProcessMethod { get; set; }
        public long? IdCapquanly { get; set; }
        public long? IdChucdanh { get; set; }
        public string Code { get; set; }
    }
    public class ManagersAddData
    {
        public int ID_QuyTrinh { get; set; } = 0;
        public string TenQuyTrinh { get; set; }
        public int ID_CapQuanLy { get; set; } = 0;
        public string TenCapDuyet { get; set; }
        /// <summary>
        /// 0: QLTT,-1; QLTT của cấp duyệt trước đó; -2: Chức danh cụ thể
        /// </summary>
        public int ID_CapDuyet { get; set; }
        public int ID_ChucVu { get; set; } = 0;
        public string Permission_Code { get; set; }
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 người nhận mail
        /// </summary>
        public string ID_NguoiNhanMail { get; set; }
        public int ID_CapDuyetLonNhat { get; set; }
        public int? ID_Back { get; set; }
        //dành cho loại phiếu có nhiều nút xử lý
        public string Icon { get; set; }
        public bool DuyetSS { get; set; }
        public float SoNgayXuLy { get; set; }
    }

    public class PriorityAddData
    {
        public int ID_CapQuanLy { get; set; }
        /// <summary>
        /// ViTri > 0
        /// </summary>
        public int ViTri { get; set; }
        public int? ID_Back { get; set; }
    }

    public class DmQuytrinhduyetModel
    {
        public long ID_QuyTrinh { get; set; }
        public string TenQuyTrinh { get; set; }
        public string MoTa { get; set; }
        public int Loai { get; set; }
        public string TenLoai { get; set; }
        public int? ProcessMethod { get; set; }
        public int? ProcessMethodLoai { get; set; }
        public bool AllowDevChecker { get; set; }
        public string NhanMailKhiDuyetDon
        {
            get
            {
                if (data_NhanMailKhiDuyetDon == null || data_NhanMailKhiDuyetDon.Count == 0)
                    return "";
                return string.Join(", ", data_NhanMailKhiDuyetDon.Select(x => x.HoTen).ToList());
            }
        }
        public string NhanMailKhiKhongDuyetDon
        {
            get
            {
                if (data_NhanMailKhiKhongDuyetDon == null || data_NhanMailKhiKhongDuyetDon.Count == 0)
                    return "";
                return string.Join(", ", data_NhanMailKhiKhongDuyetDon.Select(x => x.HoTen).ToList());
            }
        }
        public string NhanMailKhiKhongTimThayNguoiDuyetDon
        {
            get
            {
                if (data_NhanMailKhiKhongTimThayNguoiDuyetDon == null || data_NhanMailKhiKhongTimThayNguoiDuyetDon.Count == 0)
                    return "";
                return string.Join(", ", data_NhanMailKhiKhongTimThayNguoiDuyetDon.Select(x => x.HoTen).ToList());
            }
        }
        public bool? IsDefault { get; set; }
        public List<NV> data_NhanMailKhiDuyetDon { get; set; }
        public List<NV> data_NhanMailKhiKhongDuyetDon { get; set; }
        public List<NV> data_NhanMailKhiKhongTimThayNguoiDuyetDon { get; set; }
        public long? IdCapquanly { get; set; }
        public long? IdChucdanh { get; set; }
        public string Code { get; set; }
        public long? StructureID { get; set; }
        public string Permission_CodeGroup { get; set; }
        public long? ID_ChucVu { get; set; }
    }
    public class NV
    {
        public long ID_NV { get; set; }
        public string HoTen { get; set; }
    }
    public class DmQuytrinhCapquanlyduyetModel
    {
        public long ID_CapQuanLy { get; set; }
        public string TenCapQuanLy { get; set; }
        public long ID_CapDuyet { get; set; }
        public string TenCapDuyet { get; set; }
        public long? StructureID { get; set; }
        public long? ID_ChucDanh { get; set; }
        public string TenChucDanh { get; set; }
        public string Permission_CodeGroup { get; set; }
        public string Permission_Code { get; set; }
        public string Permission_Name { get; set; }
        public long? ID_ChucVu { get; set; }
        public long? ID_CapDuyetLonNhat { get; set; }
        public int? ViTri { get; set; }
        public bool DuyetSS { get; set; }
        public double SoNgayXuLy { get; set; }
        public string CapQuanLy
        {
            get
            {
                string kq = "";
                switch (ID_CapQuanLy.ToString())
                {
                    case "-2": kq = TenChucDanh; break;
                    case "-1": kq = "Quản lý trực tiếp của cấp duyệt trước đó"; break;
                    case "0": kq = "Quản lý trực tiếp"; break;
                    default: kq = TenCapQuanLy; break;
                }
                return kq;
            }
        }
        public string Notifyto { get; set; }
        public string NguoiNhanMail
        {
            get
            {
                if (data_NguoiNhanMail == null || data_NguoiNhanMail.Count == 0)
                    return "";
                return string.Join(", ", data_NguoiNhanMail.Select(x => x.HoTen).ToList());
            }
        }
        public string GhiChu { get; set; }
        public List<NV> data_NguoiNhanMail { get; set; }
        public int? Processmethod { get; set; }
        public int? ProcessmethodLoai { get; set; }
        public long ID_QuyTrinh { get; set; }
        public long? ID_Back { get; set; }
        public string Icon { get; set; }
        public List<LiteModel> listCapDuyetQT { get; set; }
    }

    public class NoCheckerModel
    {
        public int id_loai { get; set; }
        public string loai { get; set; }
        public long id_quatrinh { get; set; }
        public long id_phieu { get; set; }
        public string ten_phieu { get; set; }
        public string so_phieu { get; set; }
        public string nguoi_gui { get; set; }
        public string nguoi_tao { get; set; }
        public string ngay_tao { get; set; }
        public string tinh_trang { get; set; }
        public string ten_tinh_trang { get; set; }
        public string Deadline { get; set; }
        public bool IsTre { get; set; } = false;
        public CheckerModel checker { get; set; }
        public NoCheckerModel setNguoiTao(string nguoitao)
        {
            nguoi_tao = nguoitao;
            return this;
        }
        public NoCheckerModel setLoai(string loai)
        {
            this.loai = loai;
            return this;
        }
        public NoCheckerModel setQuaTrinh(long idquatrinh, long idphieu)
        {
            id_quatrinh = idquatrinh;
            id_phieu = idphieu;
            return this;
        }
    }
    public class CheckerModel
    {
        public long IdQuytrinh { get; set; }
        public long? Checker { get; set; }
        public string strChecker { get; set; }
        public string Checkers { get; set; }
        public long IdPhieu { get; set; }
        public long IdQuytrinhCapquanly { get; set; }
        public long IdRow { get; set; }
        public int? Priority { get; set; }
        public DateTime? Deadline { get; set; }
    }
    public class UpdateCheckerModel
    {
        public int id_quatrinh { get; set; }
        public string Checkers { get; set; }
    }
    public class PqPermission
    {
        //public long IdPermit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        //public long? IdGroup { get; set; }
        public int? Position { get; set; }
        public string Code { get; set; }
        public string CodeGroup { get; set; }

        public bool? IsDisable { get; set; }
    }
}
