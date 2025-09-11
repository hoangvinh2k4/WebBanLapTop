using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;


namespace WebBanHang.Models
{
    [Table("Products")]
    public class ProductModel
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        [Column("ProductID")]
        public int ProductID { get; set; }
        public int CategoryID { get; set; }
        public int OperatingSystemID { get; set; }

        public string NameProduct { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int BrandID { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        //[Required]
        //public string Slug { get; set; }
        public ICollection<WishListModel> WishList { get; set; } // Cái này để liên kết tới bảng WishList
        public ICollection<CartModel> Cart { get; set; } // Cái này để liên kết tới bảng Cart
       
        public ICollection<ProductImagesModel> ProductImage { get; set; } // Cái này để liên kết tới bảng ProductImage
        [ForeignKey("BrandID")]
        public BrandsModel Brand { get; set; } // Cái này là khóa ngoại để liên kết 2 bảng Products vs Brands
        [ForeignKey("CategoryID")]
        public CategoriesModel Category { get; set; } // Cái này là khóa ngoại để liên kết 2 bảng Products vs Categories
        [ForeignKey("OperatingSystemID")]
        public OperatingSystemModel OperatingSystem { get; set; } // Cái này là khóa ngoại để liên kết 2 bảng Products vs OperatingSystem
    }
}
