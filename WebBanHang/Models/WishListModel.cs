using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using System.Text.Json.Serialization;


namespace WebBanHang.Models
{
    [Table("WishList")]
    public class WishListModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int WishListID { get; set; }
        public int UserID { get; set; }
        public DateTime Added { get; set; }
        public int ProductID { get; set; }
        [ForeignKey("ProductID")]
        [JsonIgnore]
        public ProductModel Product { get; set; }
        public WishListModel() 
        { }  

        public WishListModel(ProductModel product)
        {
            Product = product;
            ProductID = product.ProductID;
        }
        public WishListModel(ProductModel product , int userId)
        {
            ProductID = product.ProductID;
            Product = product;         
            UserID = userId;
            Added = DateTime.Now;
        }
    }    
}
