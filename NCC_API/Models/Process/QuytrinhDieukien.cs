using System;
using System.Collections.Generic;

namespace Timensit_API.Models.Process
{
    public partial class QuytrinhDieukien
    {
        public long Id { get; set; }
        public string DieuKien { get; set; }
        public long? IdQuyTrinh { get; set; }
        public int IdKey { get; set; } = 13;
        public string Value { get; set; }
        public string Operator { get; set; }
        public bool Disabled { get; set; }
        public DateTime? Createddate { get; set; }
        public long? Createdby { get; set; }
        public DateTime? Lastmodfied { get; set; }
        public long? Modifiedby { get; set; }
        public List<CapQL> CapQL { get; set; }
        public float TGXuLyXa { get; set; }
    }
    public class CapQL
    {
        public long Id { get; set; }
        public long rowid { get; set; }
        public string title { get; set; }
        public int priority { get; set; }
        public float SoNgay { get; set; }
    }
}
