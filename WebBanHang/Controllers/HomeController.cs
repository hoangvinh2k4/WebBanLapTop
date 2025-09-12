using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {

        private readonly DataConnect _datacontext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataConnect context)
        {
            _datacontext = context;
            _logger = logger;
        }

        public IActionResult HomeIndex(string? keyword)
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


        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}


