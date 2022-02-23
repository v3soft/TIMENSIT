using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
    public class DMCapQuanLyModel
    {
        public long Rowid { get; set; } = 0;
        public int? Range { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
    }
}
