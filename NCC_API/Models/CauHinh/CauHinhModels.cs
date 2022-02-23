using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Timensit_API.Models
{
    public class CauHinhEmailModel
    {
        public long Id { get; set; }
        public long DonVi { get; set; }
        public string Server { get; set; }
        public int Port { get; set; }
        public bool EnableSSL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public bool Locked { get; set; }
        public List<long> DonViCon { get; set; }
    }

    public class CauHinhSMSModel
    {
        public long Id { get; set; }
        public long DonVi { get; set; }
        public string URL { get; set; }
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Brandname { get; set; }
        public string ServiceId { get; set; }
        public string DauSo { get; set; }
        public bool Lock { get; set; }

        public List<long> DonViCon { get; set; }
    }
    public class CauHinhLichModel
    {
        public long IdRow { get; set; }
        public long IdDonVi { get; set; }
        public long IdGroup { get; set; }
        public long Id { get; set; }
        /// <summary>
        /// 1: cá nhân nhận lịch đơn vị, 2: lãnh đạo hiển thị lịch, 3: lãnh đạo phê duyệt lịch
        /// </summary>
        public int Loai { get; set; }
        public int Priority { get; set; }
        public bool IsDel { get; set; }
    }
}
