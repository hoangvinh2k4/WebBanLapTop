using Microsoft.EntityFrameworkCore;
namespace WebBanHang.Models.Repository.component
{
    public class DataConnect : DbContext
    {
        public DataConnect(DbContextOptions<DataConnect> options) : base(options)
        {

        }
        public DbSet<UserModel> Users { get; set; }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandsModel> Brands { get; set; }
        public DbSet<ProductImagesModel> ProductImages { get; set; }
        public DbSet<CategoriesModel> Categories { get; set; }
        public DbSet<OperatingSystemModel> OperatingSystem { get; set; }
        public DbSet<WishListModel> WishList { get; set; } // trong này phải đặt tên trùng vs các bảng trong db
    }
}
