using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Controllers
{
    public class OperatingSystemController : Controller
    {
        private readonly DataConnect _datacontext;
        public OperatingSystemController(DataConnect context)
        {
            _datacontext = context;
        }
        [HttpGet]
        public IActionResult OperatingSystemsIndex(int id)
        {
            var products = _datacontext.Products
                .Include(p => p.OperatingSystem)
                .Include(p => p.ProductImage)
                .Where(p => p.OperatingSystemID == id)
                .ToList();

            return PartialView("~/Views/Shared/ProductListPartial.cshtml", products); // Partial view hiển thị sản phẩm
        }
    }
}
