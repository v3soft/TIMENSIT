using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class DungCuChinhHinhModel
    {
        public long Id { get; set; }
        public string DungCu { get; set; }
        public string MaDungCu { get; set; }
        public string MoTa { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
    }

    public partial class TriGiaDungCuModel
    {
        public long Id { get; set; }
        public long Id_DungCu { get; set; }
        public double TriGia { get; set; }
        public DateTime ThoiGian { get; set; }

    }
}
