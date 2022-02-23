using Timensit_API.Models;
using Timensit_API.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class DistrictModel : BaseModel<District>
    {

    }
    public class District
    {
        public PageModel page { get; set; }
        public List<DistrictAddData> dantoc { get; set; }
    }
    public class DistrictAddData
    {
        public int Id_row { get; set; } = 0;
        public string DistrictName { get; set; }
        public int ProvinceID { get; set; } = 0;
        public string Note { get; set; }
        public string Code { get; set; }
        public DateTime DateCreated { get; set; }
        public DateTime LastModified { get; set; }
    }
}
