using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhLichsu
    {
        public long IdRow { get; set; }
        public long NguoiTao { get; set; }
        public DateTime? NgayTao { get; set; }
        public string Note { get; set; }
        public bool? Approved { get; set; }
        public bool IsFinal { get; set; }
        public string Status { get; set; }
        public long IdQuatrinh { get; set; }
        public long IdQuatrinhReturn { get; set; }
        public string FileDinhKem { get; set; }
        public string src { get; set; }
        public DateTime? Deadline { get; set; }
        public string Checkers { get; set; }
        public string NguoiNhan { get; set; }

        public virtual QuytrinhQuatrinhduyet IdQuatrinhNavigation { get; set; }
    }
}
