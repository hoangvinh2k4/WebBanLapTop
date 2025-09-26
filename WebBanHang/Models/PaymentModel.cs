using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class PaymentModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int PaymentID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [MaxLength(50)]
        public string PaymentMethod { get; set; }  // COD, CreditCard, Momo, VNPay

        [Column(TypeName = "decimal(18,2)")]
        public decimal Amount { get; set; }

        public DateTime PaidDate { get; set; } = DateTime.Now;

        // Quan hệ 1-1: Payment -> Order
        [ForeignKey("OrderID")]
        public OrderModel Order { get; set; }
    }
}
