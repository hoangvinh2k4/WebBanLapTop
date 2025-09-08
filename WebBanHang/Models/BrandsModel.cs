using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Http.HttpResults;
using static Microsoft.EntityFrameworkCore.DbLoggerCategory;

namespace WebBanHang.Models
{
    public class BrandsModel
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        //dotnet ef migrations remove
        public int BrandID { get; set; }
        public string NameBrand { get; set; }
        public ICollection<ProductModel> Products { get; set; } // Cái này để liên kết tới bảng Products
        public string Description { get; set; }
        public DateTime Created { get; set; }
        public DateTime Updated { get; set; }
    }
}
