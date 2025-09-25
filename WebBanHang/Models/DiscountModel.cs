using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class DiscountModel
    {
        [Key]
        public int DiscountID { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập mã giảm giá")]
        [StringLength(50)]
        public string Code { get; set; }

        [Required(ErrorMessage = "Vui lòng nhập phần trăm giảm")]
        [Range(1, 100, ErrorMessage = "Phần trăm giảm phải từ 1-100")]
        public int Percentage { get; set; }

        [DataType(DataType.Date)]
        public DateTime StartDate { get; set; } = DateTime.Now;

        [DataType(DataType.Date)]
        public DateTime EndDate { get; set; } = DateTime.Now.AddMonths(1);

        public bool IsActive { get; set; } = true;
    }
}

