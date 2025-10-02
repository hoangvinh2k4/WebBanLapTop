
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebBanHang.Models.Repository.component;

internal class Program
{
    private static void Main(string[] args)
    {
        var builder = WebApplication.CreateBuilder(args);

        //// Add services to the container.
        //builder.Services.AddControllersWithViews();

        // ✅ Đăng ký DbContext
        builder.Services.AddDbContext<DataConnect>(options =>
        {
            options.UseSqlServer(builder.Configuration.GetConnectionString("DbConnection"));
        });
        // Vòng lặp
        builder.Services.AddControllersWithViews()
       .AddNewtonsoftJson(options =>
       {
           options.SerializerSettings.ReferenceLoopHandling = ReferenceLoopHandling.Ignore;
       });

        // ✅ Đăng ký Session
        builder.Services.AddDistributedMemoryCache();
        builder.Services.AddSession(options =>
        {
            options.IdleTimeout = TimeSpan.FromMinutes(30);
            options.Cookie.HttpOnly = true;
            options.Cookie.IsEssential = true;
        });
        builder.Services.AddHttpContextAccessor();

        var app = builder.Build();

        // ✅ Middleware
        if (!app.Environment.IsDevelopment())
        {
            app.UseExceptionHandler("/Home/Error");
        }

        app.UseHttpsRedirection();
        app.UseStaticFiles();

        app.UseRouting();

        app.UseSession();

        app.UseAuthorization();
        app.MapControllerRoute(
            name: "areas",
            pattern: "{area:exists}/{controller=User}/{action=ListUser}/{id?}");
        app.MapControllerRoute(
            name: "default",
            pattern: "{controller=Home}/{action=HomeIndex}/{id?}");
        app.Run();
    }
}