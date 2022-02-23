using Timensit_API.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebCore_API.Models
{

    public partial class DM_DonVi_Model
    {



        public decimal Id { get; set; }




        public decimal LoaiDonVi { get; set; }



        public string DonVi { get; set; }



        public string MaDonvi { get; set; }



        public string MaDinhDanh { get; set; }




        public decimal Parent { get; set; }




        public string SDT { get; set; }



        public string Email { get; set; }



        public string DiaChi { get; set; }



        public string Logo { get; set; }




        public bool Locked { get; set; }





        public decimal Priority { get; set; }





        public bool DangKyLichLanhDao { get; set; }





        public bool KhongCoVanThu { get; set; }
        public decimal CreatedBy { get;set; }
        public string CreatedDate { get; set; }



        public string UpdatedBy { get; set; }



        public string UpdatedDate { get; set; }




        public bool Disabled { get; set; }
        public List<ListImageModel> listLinkImage { get; set; }

        // Mở rộng cho trường hợp Import
        public bool isError { get; set; } = false;
        public string messageError { get; set; }
    }

    public class DonViTree
    {
        public string text { get; set; }
        public object data { get; set; }
        public object state { get; set; }
        public List<DonViTree> children { get; set; }
    }
    public class ImageModel
    {
        public string strBase64 { get; set; }
        public string filename { get; set; }
        public string extension { get; set; }
    }
}
