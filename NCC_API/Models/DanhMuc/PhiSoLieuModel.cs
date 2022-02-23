using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class PhiSoLieuModel
    {
        public int Id { get; set; }
        public string PhiSoLieu { get; set; }
        public string MoTa { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
        public long Id_Filter { get; set; }

    }
}
