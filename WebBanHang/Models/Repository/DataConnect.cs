using Microsoft.EntityFrameworkCore;

namespace WebBanHang.Models.Repository
{
    public class DataConnect : DbContext
    {
        public DataConnect(DbContextOptions<DataConnect> options) : base(options)
        {

        }
        public DbSet<ProductModel> Products { get; set; }
        public DbSet<BrandsModel> Brands { get; set; }
        public DbSet<ProductImagesModel> ProductImages { get; set; }
        public DbSet<CategoriesModel> Categories { get; set; }

    }
}
