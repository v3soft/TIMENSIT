using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
    public class FilterSoLieuModel
    {
        public long id_row { get; set; }
        public string title { get; set; }
        public string color { get; set; }
        public string bang { get; set; }
        public string pheptoan { get; set; }
        public List<FilterDetailModel> details { get; set; }
    }
    public class FilterDetailModel
    {
        public long id_row { get; set; }
        public int id_key { get; set; }
        public string @operator { get; set; }
        public string value { get; set; }

    }
}
