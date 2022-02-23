using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class DanhMucKhacModel
    {
        public int Id { get; set; }
        public string DanhMuc { get; set; }
        public string MaDanhMuc { get; set; }
        public int LoaiDanhMuc { get; set; }
        public int DonVi { get; set; }
        public bool? Locked { get; set; }
        public int? Priority { get; set; }
    }
}
