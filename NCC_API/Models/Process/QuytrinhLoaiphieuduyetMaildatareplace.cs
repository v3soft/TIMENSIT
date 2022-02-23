using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhLoaiphieuduyetMaildatareplace
    {
        public int Rowid { get; set; }
        public string Replacestring { get; set; }
        public string Columnname { get; set; }
        public long? Custemerid { get; set; }
        public int? Loaiphieuduyetid { get; set; }
        public int? Loaimail { get; set; }
        public string Format { get; set; }
    }
}
