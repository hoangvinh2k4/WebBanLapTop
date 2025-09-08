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
            var products = _datacontext.Products.Include(p => p.Brand).Include(p => p.ProductImage).ToList();

            return View(products);
        }
        public async Task<IActionResult> DetailProduct(int id)
        {
            var getbyproductID = _datacontext.Products
                                     .Include(p => p.Brand)
                                     .Include(p => p.ProductImage)
                                     .FirstOrDefault(p => p.ProductID == id);

            if (getbyproductID == null)
            {
                return RedirectToAction("Index");
            }

            return View(getbyproductID);
        }
    }
}

