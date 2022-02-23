using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhQuytrinhduyet
    {
        public QuytrinhQuytrinhduyet()
        {
            QuytrinhCapquanlyduyet = new HashSet<QuytrinhCapquanlyduyet>();
        }

        public long Rowid { get; set; }
        public string Title { get; set; }
        public string Summary { get; set; }
        public long? Custemerid { get; set; }
        public DateTime? Createddate { get; set; }
        public long? Createdby { get; set; }
        public DateTime? Lastmodified { get; set; }
        public long? Modifiedby { get; set; }
        public bool? Disable { get; set; }
        public DateTime? Deleteddate { get; set; }
        public long? Deletedby { get; set; }
        public string Listccwhenaccept { get; set; }
        public string Listccwhenreject { get; set; }
        public bool? Macdinh { get; set; }
        public string Checkernotfoundsendto { get; set; }
        public long? IdQuytrinhthaythe { get; set; }
        public int? Processmethod { get; set; }
        public bool? Isdefault { get; set; }
        public int Loai { get; set; }
        public long? IdCapquanly { get; set; }
        public long? IdChucdanh { get; set; }
        public string Code { get; set; }

        public virtual ICollection<QuytrinhCapquanlyduyet> QuytrinhCapquanlyduyet { get; set; }
    }
}
