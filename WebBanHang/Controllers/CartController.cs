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
        public IActionResult CartIndex()
        {
            List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();
            foreach (var item in cart)
            {
                item.Product = _datacontext.Products
                                .Include(p => p.ProductImage)  
                                .FirstOrDefault(p => p.ProductID == item.ProductID);
            }
            CartViewModel cartVM = new()
            {
                CartModels = cart,
                TotalPrice = cart.Sum(x => x.Product.Price * x.Quantity)
            };
            return View(cartVM);
        }
        [HttpPost]
        public async Task<IActionResult> AddToCart(int id)
        {
            ProductModel product = await _datacontext.Products
                                        .FirstOrDefaultAsync(p => p.ProductID == id);

            List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart")
                                   ?? new List<CartModel>();

            CartModel cartModel = cart.FirstOrDefault(c => c.ProductID == id);

            if (cartModel == null)
                cart.Add(new CartModel(product));
            else
            {
                cartModel.Quantity++;
                cartModel.TotalPrice = cartModel.Quantity * product.Price;
            }

            HttpContext.Session.SetJson("Cart", cart);

            // Trả về dữ liệu JSON cho client
            return Json(new
            {
                totalItems = cart.Sum(x => x.Quantity),
                totalPrice = cart.Sum(x => x.TotalPrice)
            });
        }

        [HttpPost]
        public IActionResult Decrease(int id)
        {
            List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart");
            CartModel cartModel = cart.FirstOrDefault(c => c.ProductID == id);

            if (cartModel != null)
            {
                if (cartModel.Quantity > 1)
                {
                    cartModel.Quantity--;
                    cartModel.TotalPrice = cartModel.Quantity * cartModel.Product.Price;
                }              
            }           
                HttpContext.Session.SetJson("Cart", cart);         

            // trả về JSON cho AJAX
            return Json(new
            {
                itemId = id,
                itemQuantity = cartModel?.Quantity ?? 0,
                totalItems = cart.Sum(x => x.Quantity),
                totalPrice = cart.Sum(x => x.TotalPrice)
            });
        }
        [HttpPost]
        public IActionResult Increase(int id)
        {
            // Lấy giỏ hàng từ Session
            List<CartModel> cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();

            // Tìm sản phẩm cần tăng
            CartModel cartItem = cart.FirstOrDefault(c => c.ProductID == id);

            if (cartItem != null)
            {
                cartItem.Quantity++; // tăng số lượng
                cartItem.TotalPrice = cartItem.Quantity * cartItem.Product.Price;
            }
         
            // Cập nhật lại Session
            HttpContext.Session.SetJson("Cart", cart);

            // Trả về JSON giống Decrease
            return Json(new
            {
                itemId = id,
                itemQuantity = cartItem?.Quantity ?? 0,
                totalItems = cart.Sum(x => x.Quantity),
                totalPrice = cart.Sum(x => x.TotalPrice)
            });
        }
        [HttpPost]
        public IActionResult Remove(int id)
        {
            // Lấy giỏ hàng từ session
            var cart = HttpContext.Session.GetJson<List<CartModel>>("Cart") ?? new List<CartModel>();

            // Tìm và xóa item theo ProductID
            var removed = cart.RemoveAll(c => c.ProductID == id) > 0;

            // Cập nhật session
            if (cart.Count == 0)
            {
                HttpContext.Session.Remove("Cart"); // xóa toàn bộ cart nếu trống
            }
            else
            {
                HttpContext.Session.SetJson("Cart", cart);
            }

            // Tính tổng số lượng và tổng tiền mới
            var totalItems = cart.Sum(x => x.Quantity);
            var totalPrice = cart.Sum(x => x.TotalPrice);

            // Trả về JSON cho AJAX
            return Json(new
            {
                removed = removed,
                totalItems = totalItems,
                totalPrice = totalPrice
            });
        }
        [HttpPost]
        public IActionResult Clear()
        {
            HttpContext.Session.Remove("Cart");
            return Json(new { success = true });
        }
    }
}

