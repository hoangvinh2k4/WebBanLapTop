using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository;

namespace WebBanHang.Controllers
{
    public class ProductController :Controller
    {
        private readonly DataConnect _datacontext;
        public ProductController(DataConnect context)
        {
            _datacontext = context;
        }
        public IActionResult Index()
        {
            var products = _datacontext.Products.Include(p => p.Brand).Include(p => p.ProductImages).ToList();

            return View(products);
        }
    }
}
//public async Task<IActionResult> DetailProduct(int id )
//{
//    if(id == null) return RedirectToAction("Index");
//    var productsByid = await _datacontext.Products
//        .Include(p => p.Brand)
//        .Include(p => p.Category)
//        .Where(p => p.Id == id)
//        .OrderByDescending(p => p.Id)
//        .FirstOrDefaultAsync(p => p.Id == id);
//    return View(productsByid);
//}

