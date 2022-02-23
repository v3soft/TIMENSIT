using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class ChucVuModel
    {
        public int Id_row { get; set; } = 0;
        public int Id_CV { get; set; } = 0;
        public string Tenchucdanh { get; set; }
        public string Tentienganh { get; set; }
        public bool IsLanhDao { get; set; }
    }
}
