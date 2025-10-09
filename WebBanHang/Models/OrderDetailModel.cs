using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class OrderDetailModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int OrderDetailID { get; set; }

        [Required]
        public int OrderID { get; set; }

        [Required]
        public int ProductID { get; set; }

        [Required]
        public int Quantity { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }

        // Quan hệ n-1: OrderDetail -> Order
        [ForeignKey("OrderID")]
        public OrderModel Order { get; set; }

        // Quan hệ n-1: OrderDetail -> Product
        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
    }
}
