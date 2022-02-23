using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public class GiayToModel
    {
        public long Id { get; set; }
        public long Id_NCC { get; set; }
        public long Id_LoaiGiayTo { get; set; }
        public string GiayTo { get; set; }
        public string So { get; set; }
        public string NoiCap { get; set; }
        public DateTime NgayCap { get; set; }
        public ListImageModel FileDinhKem { get; set; }
        public string src { get; set; }
    }
}
