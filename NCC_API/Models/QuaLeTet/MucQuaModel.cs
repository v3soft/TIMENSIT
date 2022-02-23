using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public class MucQuaModel
    {
        public int Id { get; set; }
        public string MucQua { get; set; }
        public string MoTa { get; set; }
        public double SoTien { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
    }
}
