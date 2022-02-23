using System;
using System.Collections.Generic;
using System.Linq;
using System.Web;
using WebCore_API.Models;

namespace Timensit_API.Models
{

    public class DotTangQuaModel
    {
        public long Id { get; set; }
        public long Id_NhomLeTet { get; set; }
        public string DotTangQua { get; set; }
        public string MoTa { get; set; }
        public int Nam { get; set; }
        //public long Id_NguonKinhPhi { get; set; }
        public DateTime? ThoiHan { get; set; }
        public bool Locked { get; set; } = false;
        public int Priority { get; set; } = 0;
        public string SoQD { get; set; }
        public DateTime? NgayQD { get; set; }
        public string SoCV { get; set; }
        public DateTime? NgayCV { get; set; }
        public string SoTT { get; set; }
        public DateTime? NgayTT { get; set; }
        public List<DotTangQua_DoiTuongModel> DoiTuongs { get; set; }
        public List<ListImageModel> FileDinhKems { get; set; }
    }
    public class DotTangQua_DoiTuongModel
    {
        public long Id { get; set; }
        public long Id_DotTangQua { get; set; }
        public long Id_DoiTuongNCC { get; set; }
        public List<DotTangQua_MucModel> MucQuas { get; set; }
    }
    public class DotTangQua_MucModel
    {
        public long Id_Detail { get; set; }
        public double? SoTien { get; set; }
        public long Id_NguonKinhPhi { get; set; }
    }
    public class DotTangQua_HuyenModel
    {
        public long Id { get; set; }
        public long Id_DotTangQua { get; set; }
        public long Id_Huyen { get; set; }
        public string SoTT { get; set; }
        public DateTime? NgayTT { get; set; }
    }
}
