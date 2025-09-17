using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace WebBanHang.Models
{
    public class WishListModel
    {
        [Key]
        public int WishListID { get; set; }
        public DateTime Added { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
        public WishListModel() { }  

        public WishListModel(ProductModel product)
        {
            Product = product;
            ProductID = product.ProductID;
        }
    }    
}
