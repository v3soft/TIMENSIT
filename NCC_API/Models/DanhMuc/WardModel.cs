using Timensit_API.Models.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class WardModel : BaseModel<Ward>
    {

    }
    public class Ward
    {
        public PageModel page { get; set; }
    }
    public class WardAddData
    {
        public int RowID { get; set; } = 0;
        public string WardName { get; set; }
        public int DistrictID { get; set; }
    }
}
