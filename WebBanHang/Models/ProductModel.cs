using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.EntityFrameworkCore;

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
        public string NameProduct { get; set; }
        public string Description { get; set; }
        [Column(TypeName = "decimal(18,2)")]
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public int BrandID { get; set; }
        [ForeignKey("BrandID")]
        public BrandsModel Brand { get; set; } // Cái để liên kết 2 bảng Products vs Brands
        public ICollection<ProductImagesModel> ProductImages { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
