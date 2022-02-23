using System;
using System.Collections.Generic;
using System.Linq;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhQuatrinhduyetSs
    {
        public long IdRow { get; set; }
        public long IdQuatrinh { get; set; }
        public bool? Valid { get; set; }
        public string Checknote { get; set; }
        public long? Checker { get; set; }
        public string Checkers { get; set; }
        public List<string> ListChecker
        {
            get
            {
                return Checkers.Split(",").ToList();
            }
        }
        public long? IdCt { get; set; }
        public DateTime? Checkeddate { get; set; }
        public DateTime? Deadline { get; set; }
        public bool? Passed { get; set; }

        public virtual QuytrinhQuatrinhduyet IdQuatrinhNavigation { get; set; }
    }
}
