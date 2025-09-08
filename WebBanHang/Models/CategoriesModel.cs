using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Models
{
    public class CategoriesModel 
    {
        [Key]
        //[DatabaseGenerated(DatabaseGeneratedOption.None)] //nếu không muốn id tự tăng
        public int CategoryID { get; set; }
        public string CategoryName { get; set; }
        public int? ParentID { get; set; }

        // Mỗi Category có nhiều Product
        public ICollection<ProductModel> Products { get; set; }

    }
}
