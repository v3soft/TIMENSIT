using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class NCCModel
    {
        public long Id { get; set; }
        public DateTime NgayGui { get; set; }
        public string SoHoSo { get; set; }
        public long Id_DoiTuongNCC { get; set; }
        public long Id_LoaiHoSo { get; set; } = 0;
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int NamSinh { get; set; }
        public int? GioiTinh { get; set; }
        public long Id_Xa { get; set; }
        public long Id_KhomAp { get; set; }
        public long? Id_DanToc { get; set; }
        public long? Id_TonGiao { get; set; }
        public string DiaChi { get; set; }
        public long Id_ThanNhan { get; set; }
        public string BiDanh { get; set; }
        public string NguyenQuan { get; set; }
        public string TruQuan { get; set; }
        public DateTime? NgayNhapNgu { get; set; }
        public DateTime? NgayXuatNgu { get; set; }
        public string NoiCongTac { get; set; }
        public string CapBac { get; set; }
        public string ChucVu { get; set; }
        public DateTime? Ngay_ { get; set; }
        public string TruongHop_ { get; set; }
        public string Noi_ { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public long? GiayBaoTu { get; set; }
        public long? BangTQGC { get; set; }
        /// <summary>
        /// 1: Nghĩa trang, 2: Gia đình quản lý, 3: Không có thông tin
        /// </summary>
        public int? Mo { get; set; }
        public string TiLe { get; set; }
        public string LyDoGTYKhoa { get; set; }
        public int TGThamGiaKC { get; set; } = 0; //số năm
        public int? BangKhenCacCap { get; set; }
        public DateTime? NgayHS { get; set; } //ngày hy sinh
        public string NoiHS { get; set; } //nơi hy sinh
        public DateTime? NgayHop { get; set; }
        public string GioHop { get; set; }
        public string ThanhPhanHop { get; set; }
        public string NoiDungHop { get; set; }
        public string CanCuLietSy { get; set; }
        public string LyDoTangTuat { get; set; }
        public string LyDoDinhChinhTN { get; set; }
        public string LyDoThoCung { get; set; }
        public string ND_HuanChuong { get; set; }
        public string XetToTrinh { get; set; }
        public int? TinhTrangHT { get; set; }
        public string GhiChuTruyTang { get; set; }
        public ListImageModel FileDinhKem { get; set; }
        public List<GiayToModel> GiayTos { get; set; }
        public List<TroCapModel> TroCapModel { get; set; }
        public QTHoatDongModel HoatDongModel { get; set; }
        public DiChuyenModel DiChuyenModel { get; set; }
        public ThanNhanModel ThanNhanModel { get; set; }
        public ThanNhanModel ThanNhanDaMat { get; set; }
        public DinhChinhModel DinhChinhModel { get; set; }
        public List<DinhChinhModel> DinhChinhTN { get; set; }
        public List<ThanNhanModel> ListCanCuLS { get; set; }

    }
    public class NCCImportModel : NCCModel
    {
        public bool IsThanNhan { get; set; } = false;
        public string NguoiThoCungLietSy { get; set; }
        public string NguyenQuan1 { get; set; }
        public string TruQuan1 { get; set; }
        public DateTime? NgaySinh1 { get; set; }
        public int? GioiTinh1 { get; set; }
        public int QuanHeVoiLietSy { get; set; } = 0;
        public string SoTien { get; set; }
        public string DoiTuong { get; set; }
        public string Title { get; set; }//xã
        public string KhomAp { get; set; }
        public string strQHGiaDinh { get; set; }
        public long QHGiaDinh { get; set; }
        public string tmpNgaySinh { get; set; }
        public string messageError { get; set; }
        public string message { get; set; }
        public bool isError { get; set; }
    }

    public class ThanNhanModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public string HoTen { get; set; }
        public int Id_QHGiaDinh { get; set; }
        public DateTime NgaySinh { get; set; }
        public int NamSinh { get; set; }
        public int? GioiTinh { get; set; }
        public string DiaChi { get; set; }
        public string NguyenQuan { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public Boolean IsChet { get; set; } = false;
        public DateTime? NgayChet { get; set; }
        public string SoKhaiTu { get; set; }
        public DateTime? NgayKhaiTu { get; set; }
        public string NoiKhaiTu { get; set; }
        public string SoHoSo { get; set; }
        public bool IsCanCu { get; set; } = false;
        public string SoBangTQCC { get; set; }
        public string SoGCNTB { get; set; }
        public double? TLThuongTat { get; set; }
    }

    public class TroCapModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public long Id_LoaiTroCap { get; set; }
        public double TroCap { get; set; }
        public double? PhuCap { get; set; }
        public double? TienMuaBao { get; set; }
        public double?  TienDinhChiTruyThu { get; set; }
        public double? TroCapNuoiDuong { get; set; }
        public DateTime? TuNgay { get; set; }
        public int? TuNam { get; set; }
        public int? SoThang { get; set; }
        public int? SoThangTruyLinh { get; set; }
        public string STTruyLinhCuThe { get; set; } //vd: tháng 1,2,3,4 (new)
        public int? SLTroCap { get; set; } //số lần trợ cấp khi báo tử (new)
        public string TruyLinh_From { get; set; }
        public DateTime? TruyLinh_To { get; set; }
        //cắt
        public bool IsCat { get; set; } = false;
        public DateTime? NgayCat { get; set; }
        public int? NamCat { get; set; }
        public string? ThangCat { get; set; }
        public bool QDCat { get; set; } = false;
        public string ThangThuHoi { get; set; }
        public QuyetDinhModel QuyetDinh { get; set; }
        public string LyDoKhongGiaiQuyet { get; set; }
        public string LyDoKhongMaiTangPhi { get; set; }
        public float? TiLeTroCap { get; set; }
        //đình chỉ
        public DateTime? NgayDinhChi { get; set; }
        public string LyDoDinhChi { get; set; }
        public string LyDoTamDC { get; set; } //(new)
        public int? ThuHoiDCTu { get; set; } //(new)
        public int? ThuHoiDCDen { get; set; } //(new)
        public string TuThang { get; set; } //(new)
        public string ThangDaNhan { get; set; } //(new)
        public string NDTruyLinh { get; set; } //(new)
    }

    public class QTHoatDongModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public string CapBac { get; set; }
        public string ChucVu { get; set; }
        public string DonVi { get; set; }
        public string DiaBan { get; set; }
        public DateTime TuNgay { get; set; }
        public DateTime? DenNgay { get; set; }
        public bool IsNghiHuu { get; set; }
        public bool IsChet { get; set; }

    }
    public class QuyetDinhModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public long ObjectId { get; set; }
        public int ObjectType { get; set; }//0: công nhận hồ sơ, 1: trợ cấp, 2: di chuyển, 3: cắt trợ cấp
        public string SoQuyetDinh { get; set; }
        public string MoTa { get; set; }
        public DateTime NgayRaQuyetDinh { get; set; }
        public DateTime NgayHieuLuc { get; set; }

    }

    public class DiChuyenModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public int Id_Tinh { get; set; }
        public int Id_Huyen { get; set; }
        public int Id_Xa { get; set; }
        public bool IsDuyet { get; set; }//false
        public int? Id_Xa_Old { get; set; }
        public string DiaChi { get; set; }
        public string DaGiaiQuyet { get; set; }
        public string ChuaGiaiQuyet { get; set; }
        public string GiayTo { get; set; }
        public string ThucHien { get; set; }
        public string GhiChu { get; set; }
        public DateTime NgayChuyen { get; set; }
        public bool IsBanChinh { get; set; }//hồ sơ bản chính
        public int Id_KhomAp { get; set; }
        public string DiaChi_Old { get; set; }
    }
    public class DoiTuongNhanQuaModel
    {
        public long Id { get; set; }
        public string SoHoSo { get; set; }
        public long Id_DoiTuongNCC { get; set; }
        public string HoTen { get; set; }
        public DateTime? NgaySinh { get; set; }
        public int NamSinh { get; set; }
        public int? GioiTinh { get; set; }
        public long Id_Xa { get; set; }
        public long Id_KhomAp { get; set; }
        public string DiaChi { get; set; }
        public string NguoiThoCungLietSy { get; set; }
        public int QuanHeVoiLietSy { get; set; } = 0;
        public bool IsForce { get; set; }
    }
}
