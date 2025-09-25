using System.Threading.Tasks;
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
        public async Task<IActionResult> WishListIndex()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                List<WishListModel> wishlist = HttpContext.Session.GetJson<List<WishListModel>>("Wishlist")
                    ?? new List<WishListModel>();
                foreach (var item in wishlist)
                {
                    item.Product = await _datacontext.Products.Include(p => p.ProductImage)
                        .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);
                }
                WishListViewModel wishlistVM = new()
                {
                    WishLists = wishlist,
                };
                return View(wishlistVM);
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var wishlistDb = await _datacontext.WishList.Where(w => w.UserID == userId)
                    .Include(w => w.Product).ThenInclude(w => w.ProductImage).ToListAsync();
                WishListViewModel wishlishVM = new()
                {
                    WishLists = wishlistDb
                };
                return View(wishlishVM);
            }
        }
        [HttpPost]
        public async Task<IActionResult> AddToWishList(int id)
        {
            var product = await _datacontext.Products.FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();

            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                // Chưa login → lưu Session
                List<WishListModel> wishlist = HttpContext.Session.GetJson<List<WishListModel>>("Wishlist")
                    ?? new List<WishListModel>();

                if (!wishlist.Any(w => w.ProductID == id))
                {
                    wishlist.Add(new WishListModel(product)); // Dùng constructor Session
                }

                // Load Product và ProductImage cho frontend
                foreach (var w in wishlist)
                {
                    w.Product ??= await _datacontext.Products
                        .Include(p => p.ProductImage)
                        .FirstOrDefaultAsync(p => p.ProductID == w.ProductID);
                }

                HttpContext.Session.SetJson("Wishlist", wishlist);
                return Json(new { wl = wishlist });
            }
            else
            {
                // Login → lưu DB
                int userId = int.Parse(userIdStr);

                var existingItem = await _datacontext.WishList
                    .FirstOrDefaultAsync(w => w.UserID == userId && w.ProductID == id);

                if (existingItem == null)
                {
                    // Dùng constructor chỉ set ProductID + UserID
                    var newItem = new WishListModel(product, userId);
                    _datacontext.WishList.Add(newItem);
                    
                }
                await _datacontext.SaveChangesAsync();
                // Load wishlist với Product + ProductImage để frontend render
                var wishlistDb = await _datacontext.WishList
                    .Include(w => w.Product)
                        .ThenInclude(p => p.ProductImage)
                    .Where(w => w.UserID == userId)
                    .ToListAsync();

                return Json(new { wl = wishlistDb });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            int totalItems = 0;

            if (string.IsNullOrEmpty(userIdStr))
            {
                var wishlist = HttpContext.Session.GetJson<List<WishListModel>>("Wishlist") ?? new List<WishListModel>();
                wishlist.RemoveAll(c => c.ProductID == productId);

                totalItems = wishlist.Count;

                if (wishlist.Count == 0)
                    HttpContext.Session.Remove("Wishlist");
                else
                    HttpContext.Session.SetJson("Wishlist", wishlist);
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var item = await _datacontext.WishList.FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);
                if (item != null)
                {
                    _datacontext.WishList.Remove(item);
                    await _datacontext.SaveChangesAsync();
                }

                totalItems = await _datacontext.WishList.CountAsync(c => c.UserID == userId);
            }

            return Json(new
            {
                success = true,
                totalItems = totalItems
            });
        }

        [HttpPost]
        public async Task<IActionResult> ClearWishList()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                HttpContext.Session.Remove("Wishlist");
                return Json(new { 
                    success = true,
                    wl = new List<WishListModel>() 
                });
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var wishlistDb = await _datacontext.WishList.Where(c => c.UserID == userId).ToListAsync();
                _datacontext.WishList.RemoveRange(wishlistDb);
                await _datacontext.SaveChangesAsync();

                return Json(new { 
                    success = true,    
                    wl = new List<WishListModel>() });
            }
        }
    }
}
