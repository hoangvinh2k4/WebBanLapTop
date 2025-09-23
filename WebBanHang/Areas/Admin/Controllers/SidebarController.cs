using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class SidebarController : Controller
    {
        private readonly DataConnect _datacontext;
        private readonly ILogger<SidebarController> _logger;

        public SidebarController(ILogger<SidebarController> logger, DataConnect context)
        {
            _datacontext = context;
            _logger = logger;
        }
        [Route("/Admin/Brands/BrandsIndex/{id}")]
        public IActionResult BrandsIndex(int id) // id chính là BrandID
        {
            var products = _datacontext.Products
                               .Include(p => p.ProductImage) // load kèm bảng ProductImages
                               .Include(p => p.Brand)         // load kèm thương hiệu
                               .Where(p => p.BrandID == id)
                               .ToList();
            return View("Brands/BrandsIndex", products);
        }
        [Route("/Admin/Categories/CategoriesIndex/{id}")]
        public IActionResult CategoriesIndex(int id)
        {
            var categories = _datacontext.Products
                                 .Include(p => p.ProductImage)
                                 .Include(p => p.Category)
                                 .Where(p => p.CategoryID == id)
                                 .ToList();
            return View("Categories/CategoriesIndex", categories);
        }
        [Route("/Admin/OperatingSystem/OperatingSystemIndex/{id}")]
        public IActionResult OperatingSystemIndex(int id)
        {
            // Lấy toàn bộ danh mục
            var os = _datacontext.Products
                               .Include(p => p.ProductImage) // load kèm bảng ProductImages
                               .Include(p => p.OperatingSystem)         // load kèm thương hiệu
                               .Where(p => p.OperatingSystemID == id)
                               .ToList();
            return View("OperatingSystem/OperatingSystemIndex", os);
        }
    }
}
