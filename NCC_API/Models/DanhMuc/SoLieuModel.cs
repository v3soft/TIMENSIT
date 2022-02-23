using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class SoLieuModel
    {
        public long Id { get; set; }
        public string SoLieu { get; set; }
        public long Id_LoaiSoLieu { get; set; }
        public long Id_Parent { get; set; }
        public long Id_Filter { get; set; }
        public string MoTa { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
    }
}
