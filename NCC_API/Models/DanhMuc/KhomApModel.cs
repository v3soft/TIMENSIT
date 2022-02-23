using Timensit_API.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class KhomApModel
    {
        public int RowID { get; set; } = 0;
        public string Title { get; set; }
        public int WardID { get; set; }
    }

    public class KhomApList
    {
        public List<string> Title { get; set; }
        public int WardID { get; set; }
    }
}
