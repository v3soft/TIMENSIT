using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
   
    public class ChucDanhModel
    {
        public long id_cv { get; set; } = 0;
        public string Cap { get; set; }
        public string TenCV { get; set; }
        public string MaCV { get; set; }
        public bool IsManager { get; set; }
        public bool IsTaiXe { get; set; }
    }
}
