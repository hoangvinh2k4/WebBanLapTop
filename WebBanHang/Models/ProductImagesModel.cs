using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;
using static System.Net.Mime.MediaTypeNames;

namespace WebBanHang.Models
{
    public class ProductImagesModel
    {
        [Key]
        [DatabaseGenerated(DatabaseGeneratedOption.Identity)]
        public int ImageID { get; set; }
        public int ProductID { get; set; }
        public string ImageUrl { get; set; }
        public bool IsMain { get; set; }
        [ForeignKey("ProductID")]
        public ProductModel Product { get; set; }
        
    } 
}
