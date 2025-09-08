using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository;

namespace WebBanHang.Controllers
{
    public class BrandsController : Controller
    {
        private readonly DataConnect _datacontext;
        public BrandsController(DataConnect context) 
        {
            _datacontext = context;
        }
        public IActionResult BrandsIndex(int id) // id chính là BrandID
        {
            var products = _datacontext.Products
                               .Include(p => p.ProductImage) // load kèm bảng ProductImages
                               .Include(p => p.Brand)         // load kèm thương hiệu
                               .Where(p => p.BrandID == id)
                               .ToList();
            return View(products);
        }     
    }
}
