using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Models.Repository.Validation;

namespace WebBanHang.Models
{
    [Table("Products")]
    public class ProductModel
    {
        [Key]
        [Column("ProductID")]
        public int ProductID { get; set; }

        // -------- CATEGORY --------
        [Required(ErrorMessage = "Bạn phải chọn loại sản phẩm")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public CategoriesModel? Category { get; set; } // navigation, không required

        // -------- BRAND --------
        [Required(ErrorMessage = "Bạn phải chọn thương hiệu")]
        public int BrandID { get; set; }

        [ForeignKey("BrandID")]
        public BrandsModel? Brand { get; set; } // navigation, không required

        // -------- PRODUCT INFO --------
        [Required(ErrorMessage = "Nhập tên sản phẩm")]
        public string NameProduct { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Price { get; set; }
        [Required(ErrorMessage = "Bạn phải chọn hệ điều hành")]
        public int OperatingSystemID { get; set; }


        public int Stock { get; set; }

        // -------- IMAGE --------
        public ICollection<ProductImagesModel>? ProductImages { get; set; } // cho phép null

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        [NotMapped]
        [FileExtension]
        public IFormFile? ImageUpload { get; set; }   // ✅ thêm dấu ?
       [ForeignKey("OperatingSystemID")]
public OperatingSystemModel? OperatingSystem { get; set; }
// Cái này là khóa ngoại để liên kết 2 bảng Products vs OperatingSystem
    }
}
        