using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class LoaiGiayToModel
    {
        public int Id { get; set; }
        public string LoaiGiayTo { get; set; }
        public string MoTa { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
    }

    public class LoaiTroCapModel
    {
        public int Id { get; set; }
        /// <summary>
        /// Id_LoaiHoSo: Hiểu là loại đối tượng, null là đễ dùng chung
        /// </summary>
        public long? Id_LoaiHoSo { get; set; }
        public string MaTroCap { get; set; }
        public string TroCap { get; set; }
        public float? TienTroCap { get; set; }
        public float? PhuCap { get; set; }
        public float? TienMuaBao { get; set; }
        public float? TroCapNuoiDuong { get; set; }
        public int? Id_Template { get; set; }
        public int? Id_Template_Cat { get; set; }
        public int Id_Parent { get; set; }
        public int SoThang { get; set; }
        public int? SoThangTC { get; set; }
        public int UpdatedBy { get; set; }
        public DateTime UpdatedDate { get; set; }
    }
    public class LoaiTroCapImportModel : LoaiTroCapModel
    {
        public string LoaiHoSo { get; set; }
        public string BieuMau { get; set; }
        public string messageError { get; set; }
        public string message { get; set; }
        public bool isError { get; set; }
    }
}
