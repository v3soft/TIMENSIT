using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class DoiTuongNCCModel
    {
        public int Id { get; set; }
        public string DoiTuong { get; set; }
        public string MaDoiTuong { get; set; }
        public string MoTa { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
        /// <summary>
        /// Nhóm loại đối tượng người có công LiteHelper.NhomLoaiDoiTuongNCC
        /// </summary>
        public int? Loai { get; set; }
        public bool IsThanNhan { get; set; }
        public long Id_LoaiQuyetDinh { get; set; }
    }
    public partial class DoiTuongNCCBieuMauModel
    {

        public int Id { get; set; }
        public long? Id_Template { get; set; }
        public long? Id_Template_DiChuyen { get; set; }
        public long? Id_Template_ThanNhan { get; set; }
        public long? Id_Template_CongNhan { get; set; }
    }

    public partial class DoiTuongMucQuaModel
    {
        public long Id_DoiTuongNhanQua { get; set; }
        public long Id_NhomLeTet { get; set; }
        public long Id_NguonKinhPhi { get; set; }
        public double SoTien { get; set; }
    }
    public partial class MucQuaDoiTuongsModel
    {
        public long Id_NhomLeTet { get; set; }
        public long Id_NguonKinhPhi { get; set; }
        public double SoTien { get; set; }
        public List<long> DoiTuongs { get; set; }
    }
}
