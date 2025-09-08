using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
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
        public IActionResult Index()
        {
            // Lấy toàn bộ danh mục
            var categories = _datacontext.Categories.ToList();
            return View(categories);          
        }
        //public IActionResult Default()
        //{
        //    var categories = _datacontext.Categories.ToList();
        //    return View("~/Views/Shared/Components/Categories/Default.cshtml", categories);
        //}
    }
}
