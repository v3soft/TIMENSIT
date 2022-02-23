using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.Common
{
    public class TongHopModel
    {
        public long Id { get; set; }
        public List<long> ids { get; set; }
        public int loai { get; set; }
        public string note { get; set; }
    }
}
