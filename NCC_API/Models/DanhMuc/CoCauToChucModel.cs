using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class CoCauToChucModel
    {
        /// <summary>
        /// ID node cha
        /// </summary>
        public int ParentID { get; set; }
        public string TitleParent { get; set; }
        /// <summary>
        /// Cấp cơ cấp ID LiteHelper.CapCoCau
        /// </summary>
        public int Level { get; set; }
        /// <summary>
        /// Map với Id của đơn vị hành chánh
        /// </summary>
        public long ID_Goc { get; set; }
        /// <summary>
        /// Vị trí
        /// </summary>
        public int Position { get; set; }
        public string Title { get; set; }
        public string Code { get; set; }
        public int RowID { get; set; }
        public int WorkingModeID { get; set; }
        public int id_lh { get; set; }
        public int CapCoCau { get; set; }
    }
    public class CoCauMapModel
    {
        public long ID_Goc { get; set; }
        public long DVHC_Cha { get; set; }
        public long? RowID { get; set; }
        public long? ParentID { get; set; }
        public string Title { get; set; }
        public string Type { get; set; }
    }
}
