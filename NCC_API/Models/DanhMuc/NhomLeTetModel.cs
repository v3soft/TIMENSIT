using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class NhomLeTetModel
    {
        public int Id { get; set; }
        public string NhomLeTet { get; set; }
        public string MoTa { get; set; }
        public string NgayKhaiBao { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
        public int MauQD { get; set; }
    }
}
