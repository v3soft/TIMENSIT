using Timensit_API.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class ProvincesModel : BaseModel<Provinces>
    {

    }
    public class Provinces
    {
        public PageModel page { get; set; }
        public List<ProvincesAddData> Province { get; set; }
    }
    public class ProvincesAddData
    {
        public int Id_row { get; set; } = 0;
        public string ProvinceName { get; set; }
    }
    public class AdministrativeDivisionImport
    {
        /// <summary>
        /// ID node cha
        /// </summary>
        public int ID_Tinh { get; set; }
        public string TenTinh { get; set; }
        public int ID_Huyen { get; set; }
        public string TenHuyen { get; set; }
        public int ID_Xa { get; set; }
        public string TenXa { get; set; }
    }
}
