using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{

    public partial class ConfigModel
    {
        public int? IdRow { get; set; }
        public string Code { get; set; }
        public string Value { get; set; }
        public int IdGroup { get; set; }
        public int Priority { get; set; } = 1;
    }
}
