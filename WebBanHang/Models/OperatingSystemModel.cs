using System.ComponentModel.DataAnnotations;

namespace WebBanHang.Models
{
    public class OperatingSystemModel
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        public int OperatingSystemID { get; set; }
        public string OperatingSystemName { get; set; }
        public int? ParentID { get; set; }
        public ICollection<ProductModel> Products { get; set; } // Cái này để liên kết tới bảng Products
    }
}
