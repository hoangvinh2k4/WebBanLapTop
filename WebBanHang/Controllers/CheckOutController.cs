using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Newtonsoft.Json;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models.ViewModel;

namespace WebBanHang.Controllers
{
    public class CheckOutController : Controller
    {
        private readonly DataConnect _datacontext;
        public CheckOutController(DataConnect context)
        {
            _datacontext = context;
        }
        public IActionResult CheckOutIndex()
        {
            
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {               
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);
            
            var cartItems = _datacontext.CartItems
                .Include(c => c.Product)  
                .Where(c => c.UserID == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, không thể thanh toán!";
                return RedirectToAction("CartIndex", "Cart");
            }

            decimal shippingPrice = 50000; 
            var defaultShipping = _datacontext.Shippings.FirstOrDefault();
            if (defaultShipping != null)
            {
                shippingPrice = defaultShipping.Price;
            }

            ViewBag.ShippingPrice = 0;

            return View(cartItems); 
        }
        [HttpPost]
        public IActionResult PlaceOrder(string paymentMethod, int? discountId, string city)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Vui lòng đăng nhập để đặt hàng." });

            int userId = int.Parse(userIdStr);

            var cartItems = _datacontext.CartItems
                .Where(c => c.UserID == userId)
                .ToList();

            if (!cartItems.Any())
                return Json(new { success = false, message = "Giỏ hàng trống, không thể đặt hàng!" });

            // ✅ Lấy phí ship từ DB dựa theo tỉnh được gửi từ form
            decimal shippingFee = 50000; // Mặc định
            var shipping = _datacontext.Shippings.FirstOrDefault(s => s.City == city);
            if (shipping != null)
                shippingFee = shipping.Price;

            using var transaction = _datacontext.Database.BeginTransaction();
            try
            {
                // 🧾 Tạo đơn hàng
                var order = new OrderModel
                {
                    UserID = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cartItems.Sum(c => c.TotalPrice),
                    ShippingFee = shippingFee,
                    DiscountID = discountId,                  
                };

                _datacontext.Orders.Add(order);
                _datacontext.SaveChanges();

                // 💰 Thêm chi tiết đơn hàng + trừ kho
                foreach (var item in cartItems)
                {
                    var product = _datacontext.Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (product == null)
                        return Json(new { success = false, message = $"Sản phẩm ID {item.ProductID} không tồn tại!" });

                    if (product.Stock < item.Quantity)
                        return Json(new { success = false, message = $"Sản phẩm {product.NameProduct} không đủ hàng!" });

                    product.Stock -= item.Quantity;
                    _datacontext.Products.Update(product);

                    _datacontext.OrderDetails.Add(new OrderDetailModel
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price
                    });
                }

                //  Thanh toán
                _datacontext.Payments.Add(new PaymentModel
                {
                    OrderID = order.OrderID,
                    PaymentMethod = paymentMethod,
                    Amount = order.TotalAmount + order.ShippingFee - order.DiscountAmount,
                    PaidDate = DateTime.Now
                });

                //  Xóa mã giảm giá đã dùng
                if (discountId.HasValue)
                {
                    var userDiscount = _datacontext.UserDiscounts
                        .FirstOrDefault(ud => ud.UserId == userId && ud.DiscountID == discountId.Value);
                    if (userDiscount != null)
                        _datacontext.UserDiscounts.Remove(userDiscount);
                }

                //  Xóa giỏ hàng
                _datacontext.CartItems.RemoveRange(cartItems);
                _datacontext.SaveChanges();

                transaction.Commit();

                return Json(new { success = true, message = "Đặt hàng thành công!" });
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                return Json(new { success = false, message = "Có lỗi khi đặt hàng: " + ex.Message });
            }
        }

        [HttpGet]
        [Route("CheckOut/GetShipping")]
        public async Task<IActionResult> GetShipping(string tinh)
        {
            // Tìm phí ship theo tỉnh
            var existingShipping = await _datacontext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh);

            decimal shippingPrice = existingShipping?.Price ?? 50000; 

            // Gửi về JSON cho JavaScript hiển thị
            return Json(new { success = true, shippingPrice });
        }
   
        [HttpGet]
        public IActionResult CheckDiscount()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            // Lấy danh sách PGG mà user có
            var discounts = (from ud in _datacontext.UserDiscounts
                             join d in _datacontext.Discounts on ud.DiscountID equals d.DiscountID
                             where ud.UserId == userId
                                   && d.IsActive
                                   && d.Quantity > 0
                                   && d.StartDate <= DateTime.Now
                                   && d.EndDate >= DateTime.Now
                             select new
                             {
                                 d.DiscountID,
                                 DisplayText = d.Code + " - Giảm " + d.Percentage + "% (HSD: " + d.EndDate.ToString("dd/MM/yyyy") + ")"
                             }).ToList();

            ViewBag.DiscountList = new SelectList(discounts, "DiscountID", "DisplayText");

            // Load giỏ hàng
            var cartItems = _datacontext.CartItems
                .Include(c => c.Product)
                .Where(c => c.UserID == userId)
                .ToList();

            ViewBag.Total = cartItems.Sum(c => c.Product.Price * c.Quantity);
            return View(cartItems);
        }
        [HttpGet]
        public IActionResult GetDiscounts()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return Json(new { success = false, message = "Chưa đăng nhập" });

            int userId = int.Parse(userIdStr);

            // Load cùng Discount qua navigation property
            var userDiscounts = _datacontext.UserDiscounts
                .Include(ud => ud.Discount)
                .Where(ud => ud.UserId == userId
                          && ud.Discount.IsActive
                          && ud.Discount.Quantity >= 0
                          && ud.Discount.StartDate <= DateTime.Now
                          && ud.Discount.EndDate >= DateTime.Now)
                .Select(ud => new
                {
                    id = ud.Discount.DiscountID,
                    text = $"{ud.Discount.Code} - Giảm {ud.Discount.Percentage}% (HSD: {ud.Discount.EndDate:dd/MM/yyyy})",
                    percent = ud.Discount.Percentage,
                    code = ud.Discount.Code
                })
                .ToList();

            return Json(new { success = true, data = userDiscounts });
        }

        [HttpGet]
        public IActionResult ApplyDiscount(int discountId)
        {
            var discount = _datacontext.Discounts.FirstOrDefault(d => d.DiscountID == discountId);
            if (discount == null)
                return Json(new { success = false });

            return Json(new
            {
                success = true,
                code = discount.Code,
                percent = discount.Percentage
            });
        }
        public IActionResult OrderHistory()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            var orders = _datacontext.Orders
                .Where(o => o.UserID == userId && !o.IsDeleted) // lọc soft delete
                .Include(o => o.OrderDetails)
                    .ThenInclude(od => od.Product)
                .Include(o => o.Discount)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }

        [HttpPost]
        public IActionResult ClearOrderHistory()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
                return RedirectToAction("Login", "Account");

            int userId = int.Parse(userIdStr);

            var orders = _datacontext.Orders
                .Where(o => o.UserID == userId && !o.IsDeleted)
                .Include(o => o.OrderDetails)
                .Include(o => o.Payment)
                .ToList();

            foreach (var order in orders)
            {
                order.IsDeleted = true; // chỉ đánh dấu soft delete
            }

            _datacontext.SaveChanges();

            return RedirectToAction("OrderHistory");
        }

    }
}
