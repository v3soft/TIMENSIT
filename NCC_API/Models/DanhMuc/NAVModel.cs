using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class NAVModel
    {
        public int ID { get; set; } = 0;
        public int ContractID { get; set; } = 0;
        public string ContractCode { get; set; }
        public double Amount { get; set; }
        public DateTime? EffectiveDate { get; set; }
        public string Type { get; set; }
    }
}
