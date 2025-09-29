namespace WebBanHang.Models
{
    public class ProductReviews
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public string UserName { get; set; }
        public string Content { get; set; }
        public int? Rating { get; set; }
        public DateTime CreatedAt { get; set; }

        public virtual ProductModel Product { get; set; }
    }

}
