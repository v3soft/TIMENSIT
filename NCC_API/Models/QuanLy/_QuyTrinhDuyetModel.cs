using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class _QuyTrinhDuyetModel
    {
        public int ID_QuyTrinh { get; set; } = 0;
        public string TenQuyTrinh { get; set; }
        public string MoTa { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiDuyetDon { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiKhongDuyetDon { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 nhận email khi duyệt đơn
        /// </summary>
        public string ID_NhanMailKhiKhongTimThayNguoiDuyetDon { get; set; }
        public bool IsKyDuyet;
    }
    public class _ManagersAddData
    {
        public int ID_QuyTrinh { get; set; } = 0;
        public string TenQuyTrinh { get; set; }
        public int ID_CapQuanLy { get; set; } = 0;
        public string TenCapDuyet { get; set; }
        /// <summary>
        /// 0: QLTT,-1; QLTT của cấp duyệt trước đó; -2: Chức danh cụ thể
        /// </summary>
        public int ID_CapDuyet { get; set; }
        public int ID_ChucVu { get; set; } = 0;
        public string TenChucVu { get; set; }
        public string GhiChu { get; set; }
        /// <summary>
        /// Danh sách id_nv1, id_nv2 người nhận mail
        /// </summary>
        public string ID_NguoiNhanMail { get; set; }
        public int ID_CapDuyetLonNhat { get; set; }
        public int Priority { get; set; }
    }
    public class _PriorityAddData
    {
        public int ID_CapQuanLy { get; set; }
        /// <summary>
        /// ViTri > 0
        /// </summary>
        public int ViTri { get; set; }
    }
}
