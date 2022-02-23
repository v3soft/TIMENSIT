using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhCapquanlyduyet
    {
        public QuytrinhCapquanlyduyet()
        {
            QuytrinhQuatrinhduyet = new HashSet<QuytrinhQuatrinhduyet>();
        }

        public long Rowid { get; set; }
        public long IdQuytrinh { get; set; }
        public long? IdCapquanly { get; set; }
        public int? Priority { get; set; }
        public DateTime? Lastmodfied { get; set; }
        public long? Modifiedby { get; set; }
        public string Note { get; set; }
        public DateTime? Createddate { get; set; }
        public long? Createdby { get; set; }
        public bool? Disable { get; set; }
        public DateTime? Deleteddate { get; set; }
        public long? Deletedby { get; set; }
        public long? IdChucdanh { get; set; }
        public string Title { get; set; }
        public string Notifyto { get; set; }
        public long? Maxlevel { get; set; }
        public string Code { get; set; }
        public long? IdBack { get; set; }
        public string Icon { get; set; }
        public bool DuyetSs { get; set; }
        public double SoNgayXuLy { get; set; }

        public virtual QuytrinhQuytrinhduyet IdQuytrinhNavigation { get; set; }
        public virtual ICollection<QuytrinhQuatrinhduyet> QuytrinhQuatrinhduyet { get; set; }
    }
}
