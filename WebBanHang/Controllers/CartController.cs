using System.Collections.Generic;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models.ViewModel;

namespace WebBanHang.Controllers
{
    public class CartController : Controller
    {
        private readonly DataConnect _datacontext;
        public CartController(DataConnect context)
        {
            _datacontext = context;
        }
        public async Task<IActionResult> CartIndex()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart")
                                            ?? new List<CartModel>();

                foreach (var item in cart)
                {
                    item.Product = await _datacontext.Products
                        .Include(p => p.ProductImage)
                        .FirstOrDefaultAsync(p => p.ProductID == item.ProductID);
                }

                CartViewModel cartVM = new()
                {
                    CartModels = cart,
                    TotalPrice = cart.Sum(x => x.Product.Price * x.Quantity)
                };
                return View(cartVM);
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var cartDb = await _datacontext.CartItems
                    .Where(c => c.UserID == userId)
                    .Include(c => c.Product)
                        .ThenInclude(p => p.ProductImage)
                    .ToListAsync();
                CartViewModel cartVM = new()
                {
                    CartModels = cartDb,
                    TotalPrice = cartDb.Sum(x => x.TotalPrice)
                };
                return View(cartVM);
            }
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            var product = await _datacontext.Products
                .FirstOrDefaultAsync(p => p.ProductID == id);
            if (product == null) return NotFound();

            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart")
                                            ?? new List<CartModel>();

                var cartItem = cart.FirstOrDefault(c => c.ProductID == id);
                if (cartItem == null)
                    cart.Add(new CartModel(product));
                else
                {
                    cartItem.Quantity++;
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                }

                HttpContext.Session.SetJson("Cart", cart);

                return Json(new
                {
                    totalItems = cart.Sum(x => x.Quantity),
                    totalPrice = cart.Sum(x => x.TotalPrice)
                });
            }
            else
            {
                int userId = int.Parse(userIdStr);

                var cartItem = await _datacontext.CartItems
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == id);

                if (cartItem == null)
                {
                    var newItem = new CartModel(product, userId);
                    _datacontext.CartItems.Add(newItem);
                }
                else
                {
                    cartItem.Quantity++;
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                }

                await _datacontext.SaveChangesAsync();

                var cartDb = await _datacontext.CartItems
                    .Where(c => c.UserID == userId)
                    .ToListAsync();

                return Json(new
                {
                    totalItems = cartDb.Sum(x => x.Quantity),
                    totalPrice = cartDb.Sum(x => x.Quantity * x.Price)
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Decrease(int id)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            { 
                var cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();
                var cartItem = cart.FirstOrDefault(c => c.ProductID == id);

                int itemQuantity = 0;

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                        itemQuantity = cartItem.Quantity;
                    }
                    else
                    {
                        cart.Remove(cartItem);
                    }
                }

                if (cart.Count == 0)
                    HttpContext.Session.Remove("Cart");
                else
                    HttpContext.Session.SetJson("Cart", cart);

                return Json(new
                {
                    itemId = id,
                    itemQuantity = itemQuantity,
                    totalItems = cart.Sum(x => x.Quantity),
                    grandTotal = cart.Sum(x => x.Quantity * x.Price)
                });
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var cartItem = await _datacontext.CartItems
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == id);

                int itemQuantity = 0;

                if (cartItem != null)
                {
                    if (cartItem.Quantity > 1)
                    {
                        cartItem.Quantity--;
                        cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                        itemQuantity = cartItem.Quantity;
                    }
                    else
                    {
                        _datacontext.CartItems.Remove(cartItem);
                    }
                    await _datacontext.SaveChangesAsync();
                }

                var cartDb = await _datacontext.CartItems.Where(c => c.UserID == userId).ToListAsync();

                return Json(new
                {
                    itemId = id,
                    itemQuantity = itemQuantity,
                    totalItems = cartDb.Sum(x => x.Quantity),
                    grandTotal = cartDb.Sum(x => x.Quantity * x.Price),
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> Increase(int id)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                var cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();
                var cartItem = cart.FirstOrDefault(c => c.ProductID == id);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                }

                HttpContext.Session.SetJson("Cart", cart);

                return Json(new
                {
                    itemId = id,
                    itemQuantity = cartItem?.Quantity ?? 0,
                    totalItems = cart.Sum(x => x.Quantity),
                    grandTotal = cart.Sum(x => x.Quantity * x.Price)
                });
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var cartItem = await _datacontext.CartItems
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == id);

                if (cartItem != null)
                {
                    cartItem.Quantity++;
                    cartItem.TotalPrice = cartItem.Quantity * cartItem.Price;
                    await _datacontext.SaveChangesAsync();
                }

                var cartDb = await _datacontext.CartItems.Where(c => c.UserID == userId).ToListAsync();

                return Json(new
                {
                    itemId = id,
                    itemQuantity = cartItem?.Quantity ?? 0,
                    totalItems = cartDb.Sum(x => x.Quantity),
                    grandTotal = cartDb.Sum(x => x.Quantity * x.Price)
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> RemoveItem(int productId)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                var cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();
                cart.RemoveAll(c => c.ProductID == productId);

                if (cart.Count == 0)
                    HttpContext.Session.Remove("Cart");
                else
                    HttpContext.Session.SetJson("Cart", cart);

                return Json(new
                {
                    success = true,
                    totalItems = cart.Sum(x => x.Quantity),
                    grandTotal = cart.Sum(x => x.TotalPrice),
                    cart = cart
                });
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var item = await _datacontext.CartItems
                    .FirstOrDefaultAsync(c => c.UserID == userId && c.ProductID == productId);
                if (item != null)
                {
                    _datacontext.CartItems.Remove(item);
                    await _datacontext.SaveChangesAsync();
                }

                var cartDb = await _datacontext.CartItems.Where(c => c.UserID == userId).ToListAsync();

                return Json(new
                {
                    success = true,
                    totalItems = cartDb.Sum(x => x.Quantity),
                    grandTotal = cartDb.Sum(x => x.TotalPrice),
                    cart = cartDb
                });
            }
        }
        [HttpPost]
        public async Task<IActionResult> ClearCart()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                HttpContext.Session.Remove("Cart");
                return Json(new { success = true, totalItems = 0, grandTotal = 0, cart = new List<CartModel>() });
            }
            else
            {
                int userId = int.Parse(userIdStr);
                var cartDb = await _datacontext.CartItems.Where(c => c.UserID == userId).ToListAsync();
                _datacontext.CartItems.RemoveRange(cartDb);
                await _datacontext.SaveChangesAsync();

                return Json(new { success = true, totalItems = 0, grandTotal = 0, cart = new List<CartModel>() });
            }
        }
    }
}


