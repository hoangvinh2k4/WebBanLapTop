using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class CartModel
    {
        [Key]
        public int CartItemID { get; set; }
        public int Quantity { get; set; }
        public decimal TotalPrice { get; set; }
        public DateTime ADDed { get; set; }
        public int ProductID { get; set; }
        //UserID INT NOT NULL REFERENCES Users(UserID), 
        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
        public CartModel()
        {

        }
        public CartModel(ProductModel product)
        {
            ProductID = product.ProductID;
            Product = product;   // gán nguyên object Product
            Quantity = 1;
            TotalPrice = product.Price; // giá ban đầu = đơn giá
            ADDed = DateTime.Now;
        }
    }
}
