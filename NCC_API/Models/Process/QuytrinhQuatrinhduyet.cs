using System;
using System.Collections.Generic;
using System.Linq;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhQuatrinhduyet
    {
        public QuytrinhQuatrinhduyet()
        {
            QuytrinhLichsu = new HashSet<QuytrinhLichsu>();
            QuytrinhQuatrinhduyetSs = new HashSet<QuytrinhQuatrinhduyetSs>();
        }

        public long IdRow { get; set; }
        public long IdPhieu { get; set; }
        public int Loai { get; set; }
        public int? Priority { get; set; }
        public bool? Valid { get; set; }
        public long? Checker { get; set; }
        public DateTime? Checkeddate { get; set; }
        public DateTime? Deadline { get; set; }
        public string Checknote { get; set; }
        public long IdQuytrinhCapquanly { get; set; }
        public string Notifyto { get; set; }
        public string Checkers { get; set; }
        public List<string> ListChecker
        {
            get
            {
                if (Checkers == null)
                    return new List<string>();
                return Checkers.Split(",").ToList<string>();
            }
        }
        public bool Ss { get; set; }

        public virtual QuytrinhCapquanlyduyet IdQuytrinhCapquanlyNavigation { get; set; }
        public virtual QuytrinhLoaiphieuduyet LoaiNavigation { get; set; }
        public virtual ICollection<QuytrinhLichsu> QuytrinhLichsu { get; set; }
        public virtual ICollection<QuytrinhQuatrinhduyetSs> QuytrinhQuatrinhduyetSs { get; set; }
    }
}
