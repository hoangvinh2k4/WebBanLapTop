using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository; 
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly DataConnect _datacontext;
        public CategoriesController(DataConnect context)
        {
            _datacontext = context;
        }
        [HttpGet]
        public IActionResult CategoriesIndex(int id)
        {
            var products = _datacontext.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImage)
                .Where(p => p.CategoryID == id)
                .ToList();

            return PartialView("~/Views/Shared/ProductListPartial.cshtml", products); // Partial view hiển thị sản phẩm
        }
        
    }
}