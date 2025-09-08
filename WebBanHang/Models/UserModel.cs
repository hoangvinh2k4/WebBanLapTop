using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    [Table("Users")]
    public class UserModel
    {
        [Key]
        public int UserID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Username")]
        [StringLength(50)]
        public string Username { get; set; } = string.Empty;

        [StringLength(20)]
        public string? Phone { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập Email")]
        [EmailAddress(ErrorMessage = "Email không hợp lệ")]
        public string Email { get; set; } = string.Empty;

        [NotMapped]
        [DataType(DataType.Password)]
        [Required(ErrorMessage = "Vui lòng nhập Password")]
        [StringLength(100, MinimumLength = 6, ErrorMessage = "Mật khẩu phải từ 6 ký tự trở lên")]
        public string? Password { get; set; }

        // Lưu hash trong DB
        public string PasswordHash { get; set; } = string.Empty;

        public string Role { get; set; } = "Customer";

        public string? RoleId { get; set; }

        public DateTime Created { get; set; } = DateTime.Now;
    }
}
