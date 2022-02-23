using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCore_API.Models;

namespace Timensit_API.Models
{

    public class PhatQuaModel
    {
        public long Id_DeXuat { get; set; }
        public long Id { get; set; }
        public long Id_DeXuatTangQua_Detail { get; set; }
        public string SoPhieu { get; set; }
        public string NguoiNhan { get; set; }
        public DateTime ThoiGianNhan{ get; set; }

    }
}
