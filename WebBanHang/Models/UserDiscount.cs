using System;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    [Table("UserDiscounts")]
    public class UserDiscountModel
    {
        [Key]
        public int Id { get; set; }

        public int UserId { get; set; }
        public int DiscountID { get; set; }
        public DateTime SpinDate { get; set; } = DateTime.Now;

        [ForeignKey("UserId")]
        public UserModel User { get; set; }

        [ForeignKey("DiscountID")]
        public DiscountModel Discount { get; set; }
    }
}
