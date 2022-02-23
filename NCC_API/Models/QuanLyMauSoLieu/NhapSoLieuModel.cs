using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.QuanLyMauSoLieu
{
    public class FromBodyModel
    {
        public List<NhapSoLieuDetail> ListNhapSoLieuDetail { get; set; }

        public List<NhapSoLieuChild> ListNhapSoLieuChild { get; set; }

        public NhapSoLieuModel NhapSoLieuModel { get; set; }
    }


    public class NhapSoLieuModel
    {
        public long Id { get; set; }
        public long Id_MauSoLieu_DonVi { get; set; }
        public long Id_DonVi { get; set; }
        public long Id_MauSoLieu { get; set; }
    }

    public class NhapSoLieuDetail
    {
        public long Id { get; set; }
        public long Id_NhapSoLieu { get; set; }
        public long Id_Detail { get; set; }
        public long? Value { get; set; }
        public string Note { get; set; }
    }

    public class NhapSoLieuChild
    {
        public long Id { get; set; }
        public long Id_Detail_Child { get; set; }
        public long Id_Detail { get; set; }
        public long? Value { get; set; }
        public string Note { get; set; }
    }
}
