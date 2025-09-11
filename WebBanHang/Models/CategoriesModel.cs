using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class CategoriesModel 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentID { get; set; }
        public ICollection<ProductModel> Products { get; set; } // Cái này để liên kết tới bảng Products
    }
}
