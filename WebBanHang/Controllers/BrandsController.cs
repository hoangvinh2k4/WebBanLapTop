using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Controllers
{
    public class BrandsController : Controller
    {
        private readonly DataConnect _datacontext;
        public BrandsController(DataConnect context)
        {
            _datacontext = context;
        }
        [HttpGet]
        public IActionResult BrandsIndex(int id)
        {
            var products = _datacontext.Products
                .Include(p => p.Brand)
                .Include(p => p.ProductImage)
                .Where(p => p.BrandID == id)
                .ToList();

            return PartialView("~/Views/Shared/ProductListPartial.cshtml", products); // Partial view hiển thị sản phẩm
        }
    }    
}
