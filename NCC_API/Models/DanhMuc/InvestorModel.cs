using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.DanhMuc
{
    public class InvestorModel
    {
        public long ID { get; set; } = 0;
        public string Name { get; set; }
        public string Email { get; set; }
        public string Phone { get; set; }
        public string Address { get; set; }
        public string CitizenID { get; set; }
    }
}
