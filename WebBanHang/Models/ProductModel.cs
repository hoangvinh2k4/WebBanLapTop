using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using WebBanHang.Models.Repository.Validation;

namespace WebBanHang.Models
{
    [Table("Products")]
    public class ProductModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        [Column("ProductID")]
        public int ProductID { get; set; }

        // -------- CATEGORY --------
        [Required(ErrorMessage = "Bạn phải chọn loại sản phẩm")]
        public int CategoryID { get; set; }

        [ForeignKey("CategoryID")]
        public CategoriesModel? Category { get; set; }

        // -------- BRAND --------
        [Required(ErrorMessage = "Bạn phải chọn thương hiệu")]
        public int BrandID { get; set; }

        [ForeignKey("BrandID")]
        public BrandsModel? Brand { get; set; }

        // -------- OPERATING SYSTEM --------
        [Required(ErrorMessage = "Bạn phải chọn hệ điều hành")]
        public int OperatingSystemID { get; set; }

        [ForeignKey("OperatingSystemID")]
        public OperatingSystemModel? OperatingSystem { get; set; }

        // -------- PRODUCT INFO --------
        [Required(ErrorMessage = "Nhập tên sản phẩm")]
        public string NameProduct { get; set; }

        public string? Description { get; set; }

        [Column(TypeName = "decimal(18,2)")]
        [Range(0, double.MaxValue, ErrorMessage = "Giá phải >= 0")]
        public decimal Price { get; set; }

        public int Stock { get; set; }

        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }

        // -------- IMAGE --------
        public ICollection<ProductImagesModel>? ProductImage { get; set; }

        [NotMapped]
        [FileExtension]
        public List<IFormFile>? ImageUpload { get; set; } // để upload ảnh mới

        // -------- RELATIONSHIP --------
        public ICollection<WishListModel>? WishList { get; set; }
        public ICollection<CartModel>? Cart { get; set; }
    }
}
