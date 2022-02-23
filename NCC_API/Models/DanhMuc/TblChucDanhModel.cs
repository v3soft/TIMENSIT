using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class TblChucDanhModel
    {
        public long IdRow { get; set; }
        /// <summary>
        /// số lượng node con + 1
        /// </summary>
        public int ViTri { get; set; }
        public long ID_PhongBan { get; set; }
        public long ID_ChucDanh { get; set; }
        public string TenChucDanh { get; set; }
        public long? StructureID { get; set; }
        public long IdCapquanly { get; set; }
    }
    public class OrgChartAddData
    {
        /// <summary>
        /// ID node cha
        /// </summary>
        public int ID { get; set; }
        /// <summary>
        /// số lượng node con + 1
        /// </summary>
        public int ViTri { get; set; }
        public int ID_PhongBan { get; set; }
        public int ID_ChucDanh { get; set; }
        public int StructureID { get; set; } = 0;
    }
    public class OrgChartUpdateData
    {
        public int ID_Parent { get; set; } = 0;
        public int ID { get; set; } = 0;
        public string MaCD { get; set; }
        public int SoNhanVien { get; set; }
        public int ViTri { get; set; } = 0;
        public int ID_ChucDanh { get; set; }
        public int ID_ChucVu { get; set; }
        public string TenChucVu { get; set; }
        public string TenTiengAnh { get; set; }
        public int ID_DonVi { get; set; }
        public int ID_PhongBan { get; set; }
        public int ID_Cap { get; set; } = 0;
        public bool? HienThiDonVi { get; set; }
        public bool? DungChuyenCap { get; set; }
        public bool? HienThiCap { get; set; }
        public bool? HienThiPhongBan { get; set; }
        public int StructureID { get; set; } = 0;
        public bool? HienThiID { get; set; }
    }
}
