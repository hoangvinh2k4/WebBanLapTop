using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Controllers
{
    public class HomeController : Controller
    {
        private readonly DataConnect _datacontext;

        public HomeController(DataConnect context)
        {
            _datacontext = context;
        }

        // Action duy nhất xử lý cả search/filter/sort
        public IActionResult HomeIndex(
            string keyword,
            int? categoryId,
            int? brandId,
            int? osId,
            string sortBy = "default",
            bool isAjax = false)
        {
            var products = _datacontext.Products
                .Include(p => p.Category)
                .Include(p => p.Brand)
                .Include(p => p.ProductImage)
                .Include(p => p.OperatingSystem)
                .AsQueryable();

            // Filter
            if (!string.IsNullOrEmpty(keyword))
                products = products.Where(p => p.NameProduct != null &&
                                               p.NameProduct.ToLower().Contains(keyword.ToLower()));
            if (categoryId.HasValue)
                products = products.Where(p => p.CategoryID == categoryId.Value);
            if (brandId.HasValue)
                products = products.Where(p => p.BrandID == brandId.Value);
            if (osId.HasValue)
                products = products.Where(p => p.OperatingSystemID == osId.Value);

            // Sort
            switch (sortBy)
            {
                case "priceAsc": products = products.OrderBy(p => p.Price); break;
                case "priceDesc": products = products.OrderByDescending(p => p.Price); break;
                case "newest": products = products.OrderByDescending(p => p.Created); break;
                case "bestseller": products = products.OrderByDescending(p => p.Stock); break;
                default: products = products.OrderBy(p => p.ProductID); break;
            }

            var productList = products.ToList();

            if (isAjax)
                return PartialView("~/Views/Shared/ProductListPartial.cshtml", productList);

            return View(productList);
        }
    }
}
