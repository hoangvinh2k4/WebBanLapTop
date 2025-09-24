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
        public IActionResult CategoriesIndex(int id)
        {
            var categories = _datacontext.Products
                                 .Include(p => p.ProductImage)
                                 .Include(p => p.Category)
                                 .Where(p => p.CategoryID == id)
                                 .ToList();
            return View(categories);
        }
    }
}