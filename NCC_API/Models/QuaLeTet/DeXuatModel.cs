using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCore_API.Models;

namespace Timensit_API.Models
{

    public class DeXuatModel
    {
        public long Id { get; set; }
        public long Id_DotTangQua { get; set; }
        public long Id_Xa { get; set; }
        public List<DeXuat_NCCModel> NCCs { get; set; }
        public bool review { get; set; } = false;
    }
    public class DeXuat_NCCModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public long Id_NguonKinhPhi { get; set; }
        public string GhiChuTang { get; set; }

    }
    public class DeXuatCloneModel : DeXuatModel
    {
        public List<DoiTuongGiamModel> DoiTuongGiam { get; set; }
    }
    public class DXGiam
    {
        public long Id { get; set; }//id detail
        public long? IdGiam { get; set; }
        public int LyDo { get; set; }

    }
    public class DoiTuongGiamModel
    {
        public long Id { get; set; }//id detail
        public long? IdGiam { get; set; }
        public long Id_NCC { get; set; }
        public long Id_NguonKinhPhi { get; set; }
        public int LyDo { get; set; }
        public string GhiChuGiam { get; set; }
    }
    public class DeXuatImportModel : NCCImportModel
    {
        public long Id_MucQua { get; set; }
    }
    public class PMuc
    {
        public long Id_MucQua { get; set; }
        public string MucQua { get; set; }
        public List<PDoiTuong> DoiTuongs { get; set; }
    }
    public class PDoiTuong
    {
        public long Id_DoituongNCC { get; set; }
        public string DoiTuong { get; set; }
        public List<NCCModel> NCCs { get; set; }
    }
    public class DeXuatHuyenModel
    {
        public long Id { get; set; }
        public long Id_DotTangQua { get; set; }
        public long Id_Huyen { get; set; }
        public string SoQD { get; set; }
        public DateTime? NgayQD { get; set; }
        public string SoCV { get; set; }
        public DateTime? NgayCV { get; set; }
        public string SoTT { get; set; }
        public DateTime? NgayTT { get; set; }
    }
}
