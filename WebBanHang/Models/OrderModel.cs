using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class OrderModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderID { get; set; }

        [Required]
        public int UserID { get; set; }

        public DateTime OrderDate { get; set; } = DateTime.Now;

        [Column(TypeName = "decimal(18,2)")]
        public decimal ShippingFee { get; set; } = 0;
        public int? DiscountID { get; set; }  // Nullable vì đơn hàng có thể không áp dụng mã
        [ForeignKey("DiscountID")]
        public DiscountModel Discount { get; set; }


        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalAmount { get; set; }
 
        [MaxLength(20)]
        public string? Status { get; set; }

        // Quan hệ 1-n: Order -> OrderDetails
        public ICollection<OrderDetailModel> OrderDetails { get; set; }

        // Quan hệ 1-1: Order -> Payment
        public PaymentModel Payment { get; set; }
        public bool IsDeleted { get; set; } = false;

        [NotMapped]
        public string DiscountCode => Discount?.Code;

        [NotMapped]
        public decimal DiscountAmount => Discount != null ? TotalAmount * Discount.Percentage / 100 : 0;
    }
}
