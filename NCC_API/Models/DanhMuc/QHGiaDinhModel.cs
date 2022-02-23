using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public class QHGiaDinhModel
    {
        public int Id { get; set; }
        public string QHGiaDinh { get; set; }
        public bool Locked { get; set; } = false;
        public bool ByQua { get; set; }
        public bool IsChuYeu { get; set; } = false;
        public int? Priority { get; set; } = 0;
    }
}
