using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.QuanLyMauSoLieu
{
    public class MauSoLieuDetailModel
    {
        public long Id { get; set; }
        public string MauSoLieu { get; set; }
        public string MoTa { get; set; }

        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
    }

}
