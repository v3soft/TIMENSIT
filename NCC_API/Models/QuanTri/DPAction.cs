using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace JeeOfficeAPI.Models
{
    public class DPAction
    {
        public DPStep Step { get; set; }
        public DPStep Next { get; set; }
        public string ButtonText { get; set; }
    }
    public class FlowChartJsonModel
    {
        public long Id { get; set; }
        public int IdLuong { get; set; }
        public int Type { get; set; }
        public string JsonData { get; set; }
    }
}
