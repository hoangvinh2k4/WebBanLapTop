using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;
using static System.Collections.Specialized.BitVector32;

namespace WebBanHang.Controllers
{
    public class ProductController : Controller
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
                                     .Include(p => p.Category)
                                     .Include(p => p.OperatingSystem)
                                     .Include(p => p.ProductImage)
                                        .Include(p => p.ProductReviews)
                                     .FirstOrDefault(p => p.ProductID == id);

            if (getbyproductID == null)
            {
                return RedirectToAction("Index");
            }

            // Lấy sản phẩm cùng thương hiệu, trừ sản phẩm hiện tại
            var relatedProducts = _datacontext.Products
                .Include(p => p.Brand)              // ✅ thêm Brand để tránh null
                .Include(p => p.Category)           // (nếu cần show category)
                .Include(p => p.OperatingSystem)    // (nếu cần show hệ điều hành)
                .Include(p => p.ProductImage)

                .Where(p => p.BrandID == getbyproductID.BrandID && p.ProductID != id)
                .Take(4) // lấy 4 sản phẩm thôi
                .ToList();

            ViewBag.RelatedProducts = relatedProducts;


            return View(getbyproductID);
        }
        [HttpPost]
        public IActionResult AddReview(int ProductId, string UserName, string Content, int? Rating)
        {
            var review = new ProductReviews
            {
                ProductId = ProductId,
                UserName = UserName,
                Content = Content,
                Rating = Rating,
                CreatedAt = DateTime.Now
            };
            _datacontext.ProductReviews.Add(review);
            _datacontext.SaveChanges();

            var reviews = _datacontext.ProductReviews
                            .Where(r => r.ProductId == ProductId)
                            .OrderByDescending(r => r.CreatedAt)
                            .ToList();
            return PartialView("_ProductReviews", reviews);
        }
        public IActionResult DeleteReview(int reviewId)
        {
            // tìm review theo Id
            var review = _datacontext.ProductReviews.FirstOrDefault(r => r.Id == reviewId);
            var user = HttpContext.Session.GetString("Username");

            if (review == null)
                return NotFound();

            // chỉ cho xóa nếu là của người đăng
            if (review.UserName == user)
            {
                _datacontext.ProductReviews.Remove(review);
                _datacontext.SaveChanges();
            }

            // lấy lại danh sách review sau khi xóa
            var reviews = _datacontext.ProductReviews
                .Where(r => r.ProductId == review.ProductId)
                .OrderByDescending(r => r.CreatedAt)
                .ToList();

            return PartialView("_ProductReviews", reviews);
        }


    }
}

    



