using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public class DinhChinhModel
    {
        public long Id { get; set; }
        public List<ListInfoChange> ListColumn { get; set; }
        public long ID_NCC { get; set; }
        public bool IsDuyet { get; set; }
        public string GhiChu { get; set; }
        public List<GiayToModel> GiayTo { get; set; }
    }

    public class ListInfoChange
    {
        public long Id { get; set; }
        public long Id_DinhChinh { get; set; }
        public string ColumnName { get; set; }
        public string GiaTriCu { get; set; }
        public string GiaTriMoi { get; set; }
        public long Type { get; set; }
    }
}
