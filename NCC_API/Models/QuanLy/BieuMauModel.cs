using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public class CanCuModel
    {
        public long Id { get; set; }
        public string CanCu { get; set; }
        public string SoCanCu { get; set; }
        public string NguoiKy { get; set; }
        public string TrichYeu { get; set; }
        public string CoQuanBanHanh { get; set; }
        public string PhanLoai { get; set; }
        public int? Priority { get; set; } = 0;
        public DateTime? NgayBanHanh { get; set; }
        public DateTime? HieuLuc_From { get; set; }
        public DateTime? HieuLuc_To { get; set; }
        public ListImageModel FileDinhKem { get; set; }
    }
    public class BieuMauModel
    {
        public long Id { get; set; }
        public long Id_CanCu { get; set; }
        public string So { get; set; }
        public string BieuMau { get; set; }
        /// <summary>
        /// 1: hồ sơ, 2 công nhận, 3 trợ cấp, 4:khác
        /// </summary>
        public int Loai { get; set; }
        public int? Priority { get; set; } = 0;
        public long Id_Version { get; set; }
        public string Version { get; set; }
        //public string content { get; set; }
        public long Id_LoaiQuyetDinh { get; set; }
        public bool IsUp { get; set; } = false;
        public bool isTinh { get; set; } = true;
        public bool isHuyen { get; set; } = true;
        public bool isXa { get; set; } = true;
        public ListImageModel FileDinhKem { get; set; }
    }
    public class BieuMauThanhPhanModel
    {
        public long Id { get; set; }
        public string NoiDung { get; set; }
        public string NoiDungFail { get; set; }
        public bool IsFail { get; set; } = false;
        public ListImageModel FileDinhKem { get; set; }
        public ListImageModel FileDinhKemFail { get; set; }
    }
    public class BieuMauQuaModel
    {
        public long Id { get; set; }
        public string BieuMau { get; set; }
        public string Filename { get; set; }
        public string Version { get; set; }
        public string content { get; set; }
    }
}
