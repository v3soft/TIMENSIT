using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class HuongDanSDModel
    {
        public long Id { get; set; }
        public string TenHuongDan { get; set; }
        public bool IsUp { get; set; } = false;
        public ListImageModel FileDinhKem { get; set; }
    }
}
