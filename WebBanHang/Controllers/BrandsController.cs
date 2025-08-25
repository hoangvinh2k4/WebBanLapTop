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
        //public async Task<IActionResult> Index(int id = 0)
        //{
        //    var brands = await _datacontext.Brands.FirstOrDefaultAsync(b=>b.Id == id);
        //    if (brands == null)
        //        return RedirectToAction("Index", "Home");
        
        //    var productsByBrands = await _datacontext.Products
        //        .Include(p => p.Brand)
        //        .Include(p => p.Category)
        //        .Where(p=> p.BrandId==id)
        //        .OrderByDescending(p =>p.Id).ToListAsync();
        //    return View(productsByBrands);
        //}
        //public IActionResult Default()
        //{
        //    var brands = _datacontext.Brands.ToList();
        //    return View("~/Views/Shared/Components/Brands/Default.cshtml",brands);
        //}
    }
}
