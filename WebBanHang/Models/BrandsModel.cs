using System.ComponentModel.DataAnnotations;
namespace WebBanHang.Models
{
    public class BrandsModel
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        public int BrandID { get; set; }
        public string NameBrand { get; set; }
        //public string Slug { get; set; }
        public ICollection<ProductModel> Products { get; set; } // Cái này để liên kết tới bảng Products
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
        public ICollection<ProductModel> Products { get; set; } // Cái này để liên kết tới bảng Products
    }
}
