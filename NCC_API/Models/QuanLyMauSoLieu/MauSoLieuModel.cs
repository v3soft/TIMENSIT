using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.QuanLyMauSoLieu
{
    public class FromBodyData
    {
        public List<FormMauSoLieuDetailModel> ListFormMauSoLieuDetailModel { get; set; }

        public MauSoLieuModel MauSoLieu { get; set; }
    }
    public class UpMauSoLieuModel
    {
        public MauSoLieuModel data_old { get; set; }
        public MauSoLieuModel data { get; set; }
        public bool Force { get; set; } = false;
    }
    public class MauSoLieuModel
    {

        public long Id { get; set; }
        public string MauSoLieu { get; set; }
        public string MoTa { get; set; }
        public List<DonVi> ListDonVi { get; set; }
        public List<FormMauSoLieuDetailModel> Details { get; set; }
        public bool Locked { get; set; } = false;
        public bool IsMauTheoPhong { get; set; } = false;
        public int Priority { get; set; } = 0;
        public long? IdParent { get; set; }
        public int? Nam { get; set; }
    }
    public class FormMauSoLieuDetailModel
    {
        public long IdSoLieu { get; set; }
        public string SoLieu { get; set; }
        public long Id_Detail { get; set; }
        public string LoaiSoLieu { get; set; }
        public long Priority { get; set; }
        public string MoTa { get; set; }
        public double? @default { get; set; }
        public List<FormDetail> Detail { get; set; }
        public List<FormSoLieuConModel> SoLieuCon { get; set; }
        public bool Force { get; set; } = false;
    }
    public class FormSoLieuConModel
    {
        public long IdSoLieu { get; set; }
        public long Id_Detail { get; set; }
        public string LoaiSoLieu { get; set; }
        public string MoTa { get; set; }
        public long Priority { get; set; }
        public string SoLieu { get; set; }
        public double? @default { get; set; }
        public List<FormDetail> Detail { get; set; }
    }

    public class FormDetail
    {
        public long Id_Detail { get; set; }
        public long Id_Detail_child { get; set; }
        public long CachNhap { get; set; }
        public double? @default { get; set; }
        public long IdPhiSoLieu { get; set; }
        public string PhiSoLieu { get; set; }
        public string MoTa { get; set; }
        public bool Force { get; set; } = false;
    }

    public class FormDonVi
    {
        public long Id { get; set; }
        public long Id_MauSoLieu { get; set; }
        public DateTime ThoiGian { get; set; }
        public List<DonVi> ListDonVi { get; set; }
    }

    public class DonVi
    {
        public long id { get; set; }
        public long Id_DonVi { get; set; }
        public string title { get; set; }
    }
}
