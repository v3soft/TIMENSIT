using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class NhomNguoiDung
    {
        public string GroupName { get; set; }
        public string GhiChu { get; set; }
        public string IdGroup { get; set; }
    }
    public class NguoiDung
    {
        public string Id { get; set; }
        public long IDNhanVien { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        public int Status { get; set; }
        public string IdGroup { get; set; }
        public string FullName { get; set; }
        public long IDDonVi { get; set; } = 0;
        public long IDBoPhan { get; set; } = 0;
        public string MaNhanVien { get; set; }
    }
    public class NhomNguoiDungDPS
    {
        public string GroupName { get; set; }
        public string GhiChu { get; set; }
        public int IdGroup { get; set; }
        public string Ma { get; set; }
        public int DisplayOrder { get; set; }
        public int DonVi { get; set; }
        public int ChucVu { get; set; }
        public bool IsDefault { get; set; }
        public bool Locked { get; set; }
    }
    public class NguoiDungDPS
    {
        public long UserID { get; set; }
        public string UserName { get; set; }
        public string FullName { get; set; }
        public string Password { get; set; }
        public string RePassword { get; set; }
        public string Email { get; set; }
        public string PhoneNumber { get; set; }
        /// <summary>
        /// 1:đã kích hoạt 0:đã khóa
        /// </summary>
        public int Active { get; set; }
        /// <summary>
        /// 1: online 2:offline
        /// </summary>
        public int Status { get; set; }
        public long IdDonVi { get; set; }
        public int? CapCoCau { get; set; }
        public FileModel avatar { get; set; }
        public FileModel Sign { get; set; }
        public string MaNV { get; set; }
        public int IdChucVu { get; set; }
        public int IdVaiTro { get; set; }
        public string ViettelStudy { get; set; }
        public string SimCA { get; set; }
        public string SerialToken { get; set; }
        public int LoaiChungThu { get; set; }
        public string CMTND { get; set; }
        public int GioiTinh { get; set; }
        public bool NhanLichDonVi { get; set; }
        public DateTime? NgaySinh { get; set; }
        public List<LiteModel> DonViQuanTam { get; set; }
        public List<LiteModel> DonViLayHanXuLy { get; set; }

        //khoa thêm
        public string Cast_ExpDate { get; set; }
        public string Cast_NgaySinh { get; set; }
        public string TenChucVu { get; set; }
        public string TenVaiTro { get; set; }
        public string MaVaiTro { get; set; }
        public string TenDonVi { get; set; }
        public string MaDonVi { get; set; }
        public List<LiteModel> lstDoiTuongNCC { get; set; }
    }
    public class PhanQuyenNhom
    {
        public long IdGroup { get; set; }
        public int PhanMem { get; set; } = -1;
        public List<Quyen> ChiTiet { get; set; }
    }
    public class Quyen
    {
        public int IdRole { get; set; }
        public bool Permitted { get; set; }
    }
    public class PhanQuyenChiNhanh
    {
        public long IdUser { get; set; }
        public List<int> ChiTiet { get; set; }
    }
    public class DoiMatKhau
    {
        public string Id { get; set; } = "";
        public string OldPassword { get; set; } = "";
        public string NewPassword { get; set; } = "";
        public string RePassword { get; set; } = "";
        public bool Logout { get; set; } = true;
    }

    public class NguoidungVaiTro
    {
        public int IdRow { get; set; }
        public int IdUser { get; set; }
        public int IdGroupUser { get; set; }
        public bool NguoiKy { get; set; }
        public bool XuLyViec { get; set; }
        public bool LanhDao { get; set; }
        public bool NhanVanBan { get; set; }
        public bool Locked { get; set; }
        public int Priority { get; set; }

        //khoa thêm
        public int IdGroup { get; set; }
        public string GroupName { get; set; }
        public string Ma { get; set; }
    }
}