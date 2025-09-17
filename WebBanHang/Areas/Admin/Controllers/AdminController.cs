using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Controllers;
using WebBanHang.Models.Repository.component;
namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        private readonly DataConnect _datacontext;
        private readonly ILogger<AdminController> _logger;

        public AdminController(ILogger<AdminController> logger, DataConnect context)
        {
            _datacontext = context;
            _logger = logger;
        }
        public IActionResult HomeAdmin(string? keyword)
        {
            var products = _datacontext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImage)
                .Include(p => p.OperatingSystem)
                .AsQueryable();

            if (!string.IsNullOrEmpty(keyword))
            {
                products = products.Where(p => p.NameProduct != null &&
                                               p.NameProduct.ToLower().Contains(keyword.ToLower()));
            }
            return View(products.ToList());
        }
    }
}
