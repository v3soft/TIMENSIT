using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class _ChucVuModel
    {
        public int Id { get; set; }
        public string ChucVu { get; set; }
        public string MaChucVu { get; set; }
        public string MoTa { get; set; }
        public int? DonVi { get; set; }
        public bool? Locked { get; set; }
        public int? Priority { get; set; }
        public int? IdParent { get; set; }
    }
}
