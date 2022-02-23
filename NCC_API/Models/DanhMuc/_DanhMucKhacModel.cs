using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
    public class LoaiHoSoModel
    {
        public int Id { get; set; }
        public string MaLoaiHoSo { get; set; }
        public string LoaiHoSo { get; set; }
        public string MoTa { get; set; }
        public bool? Disable { get; set; }
        public int? Id_LoaiGiayTo { get; set; }
        public int? Id_Template { get; set; }
        public int? Id_Template_ThanNhan { get; set; }
        public int? Id_Template_CongNhan { get; set; }
        public int? Id_LoaiGiayTo_CC { get; set; }
        public string MauCongNhan { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
        public List<GiayToR> GiayTos { get; set; }
        public long? Id_DoiTuongNCC { get; set; }
        //public List<long> BieuMaus { get; set; }
        //public List<long> DoiTuongs { get; set; }
        public List<DoiTuongL> DoiTuongs { get; set; }
    }
    public class GiayToR
    {
        public long Id { get; set; }
        public bool IsRequired { get; set; } = false;
    }

    public class BieuMauL
    {
        public long Id { get; set; }
    }
    public class DoiTuongL
    {
        public long Id { get; set; }
        public List<BieuMauL> BieuMaus { get; set; }
    }
}
