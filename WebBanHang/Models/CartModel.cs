using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace WebBanHang.Models
{
    [Table("CartItems")]
    public class CartModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int CartItemID { get; set; }
        public int UserID { get; set; }
        public int ProductID { get; set; }
        public int Quantity { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal TotalPrice { get; set; }
        public DateTime ADDed { get; set; }
        //UserID INT NOT NULL REFERENCES Users(UserID), 
        [ForeignKey("ProductID")]
        [JsonIgnore]
        public ProductModel Product { get; set; }
        public CartModel()
        {

        }
        public CartModel(ProductModel product)
        {
            ProductID = product.ProductID;
            Product = product;   // gán nguyên object Product
            Quantity = 1;
            Price = product.Price;
            TotalPrice = product.Price; // giá ban đầu = đơn giá
            ADDed = DateTime.Now;
        }
        public CartModel(ProductModel product, int userId)
        {
            ProductID = product.ProductID;
            Product = product;
            Quantity = 1;
            Price = product.Price;
            TotalPrice = product.Price;
            ADDed = DateTime.Now;
            UserID = userId;
        }

    }
}
