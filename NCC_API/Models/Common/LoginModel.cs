using Timensit_API.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Web;

namespace Timensit_API.Models
{
    public class LoginModel : BaseModel<LoginData>
    {
    }
    public class LoginData
    {
        public long Id { get; set; }
        /// <summary>
        /// Tên đăng nhập
        /// </summary>
        public string UserName { get; set; }
        /// <summary>
        /// Trạng thái (0 là khoá, 1 là kích hoạt)
        /// </summary>
        public int Active { get; set; } = 0;
        /// <summary>
        /// Mảng quyền của người dùng
        /// </summary>
        public List<long> Rules { get; set; }
        public string FullName { get; set; }
        public string Token { get; set; }

        public int IdDonVi { get; set; }
        public int ID_Goc { get; set; }
        public int ID_Goc_Cha { get; set; }
        /// <summary>
        /// 1: tỉnh,2 huyện, 3 xã
        /// </summary>
        public int Capcocau { get; set; }
        public string DonVi { get; set; }
        public string MaDinhDanh { get; set; }
        public string SDT { get; set; }
        public string Email { get; set; }
        public string ChucVu { get; set; }
        public string Avata { get; set; }
        public int VaiTro { get; set; }
        public string TenVaiTro { get; set; }
        public string ResetToken { get; set; }
        public DateTime LastUpdatePass { get; set; }
        public DateTime? ExpDate { get; set; }
        public long IdTinh { get; set; }
        public long IdHuyen { get; set; }
        public long IdXa { get; set; }
    }
    public class LoginViewModel
    {
        public LoginViewModel() { }
        [Required(ErrorMessage = "Vui nhập tên đăng nhập.")]
        [MaxLength(99, ErrorMessage = "Tài khoản tối đa 99 ký tự.")]
        [DisplayName("Tài khoản")]
        public string UserName { get; set; }
        [Required(ErrorMessage = "Mật khẩu không được để trống.")]
        [DisplayName("Mật khẩu")]
        public string Password { get; set; }
        [DisplayName("Ghi nhớ")]
        public bool isPersistent { get; set; }
        public string ReturnUrl { get; set; }
    }
    public class LoginAPIModel
    {
        public string UserName { get; set; }
        public string Password { get; set; }
        public string Token { get; set; }
    }
}