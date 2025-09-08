using System.Diagnostics;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Query.Internal;
using WebBanHang.Models;
using WebBanHang.Models.Repository;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {

        private readonly DataConnect _dataContext;
        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, DataConnect context)
        {
            _dataContext = context;
            _logger = logger;
        }

        public IActionResult HomeIndex()
        {
            var products = _dataContext.Products.Include(p => p.Brand)
                                                .Include(p => p.ProductImage)
                                                .Include(p => p.Category)
                                                .Include(p => p.OperatingSystem).ToList();
            return View(products);
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
