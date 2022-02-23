using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
    public partial class NguonKinhPhiModel
    {
        public int Id { get; set; }
    
        public string NguonKinhPhi { get; set; }

        public bool Locked { get; set; } = false;
 
        public int Priority { get; set; } = 0;

    }

}
