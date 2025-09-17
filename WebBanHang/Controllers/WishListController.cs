using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models.ViewModel;

namespace WebBanHang.Controllers
{
    public class WishListController : Controller
    {
        public readonly DataConnect _datacontext;
        public WishListController(DataConnect context)
        {
            _datacontext = context;
        }
        public IActionResult WishListIndex()
        {
            List<WishListModel> wishlist = HttpContext.Session.GetJson<List<WishListModel>>("WishList") ?? new List<WishListModel>();
            foreach(var item in wishlist)
            {
                item.Product = _datacontext.Products
                                .Include(p => p.ProductImage)
                                .FirstOrDefault(p => p.ProductID == item.ProductID);
            }
            var wishlistVM = new WishListViewModel
            {
                WishLists = wishlist
            };
            return View(wishlistVM);
        }
        public async Task<IActionResult> AddToWishList(int id)
        {
            ProductModel product = await _datacontext.Products
                                        .FirstOrDefaultAsync(p => p.ProductID == id);

            List<WishListModel> wishlist = HttpContext.Session.GetJson<List<WishListModel>>("WishList")
                                   ?? new List<WishListModel>();

            WishListModel wishlistModel = wishlist.FirstOrDefault(c => c.ProductID == id);

            if (wishlistModel == null)
                wishlist.Add(new WishListModel(product));

            HttpContext.Session.SetJson("WishList", wishlist);
           
            return Json(new
            {
                success = true,
                totalItems = wishlist.Count
            });
        }
        [HttpPost]
        public IActionResult Remove(int id)
        {
            // Lấy giỏ hàng từ session
            var wishlist = HttpContext.Session.GetJson<List<WishListModel>>("WishList") ?? new List<WishListModel>();

            // Tìm và xóa item theo ProductID
            var removed = wishlist.RemoveAll(c => c.ProductID == id) > 0;

            // Cập nhật session
            if (wishlist.Count == 0)
            {
                HttpContext.Session.Remove("WishList"); // xóa toàn bộ wishlist nếu trống
            }
            else
            {
                HttpContext.Session.SetJson("WishList", wishlist);
            }
            // Trả về JSON cho AJAX
            return Json(new
            {
                removed = removed,
            });
        }
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("WishList");
            return Json(new { success = true });
        }
    }
}
