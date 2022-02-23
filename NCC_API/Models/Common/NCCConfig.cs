using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models.Common
{
    public class NCCConfig
    {
        public string SysName { get; set; }
        public string LinkAPI { get; set; }
        public string ConnectionString { get; set; }
        public string LinkBackend { get; set; }
        public string SecretKey { get; set; }
        public string IdTinh { get; set; }
    }
}
