using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class ContactModel
    {
        public int ID { get; set; } = 0;
        public int InvestorID { get; set; } = 0;
        public string ContractCode { get; set; }
        public double Amount { get; set; }
        public DateTime? StartDate { get; set; }
        public DateTime? EndDate { get; set; }
        public string DepositPeriod { get; set; }
        public string Fund { get; set; }
        public double ProfitShare { get; set; }


    }
}
