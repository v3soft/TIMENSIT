using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhDieukienCapquanly
    {
        public long Id { get; set; }
        public long? IdDieuKien { get; set; }
        public long? IdQuyTrinhCapQuanLy { get; set; }
        public double? SoNgay { get; set; }
    }
}
