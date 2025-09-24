using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models.ViewModel
{
    public class LoginViewModel
    {
        [Required(ErrorMessage = "Vui lòng nhập Username")]
        public string? Username { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Password")]
        [DataType(DataType.Password)]
        public string? Password { get; set; }

        public string? ReturnUrl { get; set; }
    }
}
