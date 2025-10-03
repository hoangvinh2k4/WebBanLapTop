using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
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
            // Lấy UserId từ Session
            string? userIdStr = HttpContext.Session.GetString("UserId");

            if (string.IsNullOrEmpty(userIdStr))
            {
                // Nếu chưa đăng nhập → về trang Login
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);

            // Load giỏ hàng theo user
            var cartItems = _datacontext.CartItems
                .Include(c => c.Product)  // ⚡️ Bắt buộc để tránh null
                .Where(c => c.UserID == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, không thể thanh toán!";
                return RedirectToAction("Index", "Cart");
            }

            decimal shippingPrice = 50000; // mặc định
            var defaultShipping = _datacontext.Shippings.FirstOrDefault();
            if (defaultShipping != null)
            {
                shippingPrice = defaultShipping.Price;
            }

            // Gửi qua ViewBag
            ViewBag.ShippingPrice = 0;

            return View(cartItems); // Truyền cartItems sang view
        }


        // Xử lý khi bấm nút Thanh toán
        [HttpPost]
        public IActionResult PlaceOrder(string paymentMethod)
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);
            var cartItems = _datacontext.CartItems
                .Where(c => c.UserID == userId)
                .ToList();

            if (!cartItems.Any())
            {
                TempData["Message"] = "Giỏ hàng trống, không thể đặt hàng!";
                return RedirectToAction("Index", "Cart");
            }
            decimal shippingFee = 0;
            if (Request.Cookies.TryGetValue("ShippingPrice", out var shippingCookie))
            {
                decimal.TryParse(JsonConvert.DeserializeObject<string>(shippingCookie), out shippingFee);
            }
            // ⚡ Transaction đảm bảo tính toàn vẹn dữ liệu
            using var transaction = _datacontext.Database.BeginTransaction();

            try
            {
                // Tạo đơn hàng mới
                var order = new OrderModel
                {
                    UserID = userId,
                    OrderDate = DateTime.Now,
                    TotalAmount = cartItems.Sum(c => c.TotalPrice),
                    ShippingFee = shippingFee,
                    Status = "Đã thanh toán"
                };
                _datacontext.Orders.Add(order);
                _datacontext.SaveChanges();

                // Thêm chi tiết đơn hàng + cập nhật tồn kho
                foreach (var item in cartItems)
                {
                    var product = _datacontext.Products.FirstOrDefault(p => p.ProductID == item.ProductID);
                    if (product == null)
                    {
                        TempData["Message"] = $"Sản phẩm ID {item.ProductID} không tồn tại!";
                        transaction.Rollback();
                        return RedirectToAction("Index", "Cart");
                    }

                    if (product.Stock < item.Quantity)
                    {
                        TempData["Message"] = $"Sản phẩm {product.NameProduct} không đủ hàng trong kho!";
                        transaction.Rollback();
                        return RedirectToAction("Index", "Cart");
                    }

                    // Trừ kho
                    product.Stock -= item.Quantity;
                    _datacontext.Products.Update(product);

                    // Thêm chi tiết đơn hàng
                    var detail = new OrderDetailModel
                    {
                        OrderID = order.OrderID,
                        ProductID = item.ProductID,
                        Quantity = item.Quantity,
                        Price = item.Price
                    };
                    _datacontext.OrderDetails.Add(detail);
                }

                // Thêm Payment
                var payment = new PaymentModel
                {
                    OrderID = order.OrderID,
                    PaymentMethod = paymentMethod,
                    Amount = order.TotalAmount + order.ShippingFee,
                    PaidDate = DateTime.Now
                };
                _datacontext.Payments.Add(payment);

                // Xóa giỏ hàng sau khi đặt hàng
                _datacontext.CartItems.RemoveRange(cartItems);

                _datacontext.SaveChanges();
                transaction.Commit();

                TempData["Message"] = "Đặt hàng thành công!";
            }
            catch (Exception ex)
            {
                transaction.Rollback();
                TempData["Message"] = "Có lỗi xảy ra khi đặt hàng: " + ex.Message;
            }

            return RedirectToAction("HomeIndex", "Home");
        }

        public IActionResult OrderHistory()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);

            // Lấy danh sách đơn hàng của user
            var orders = _datacontext.Orders
                .Include(o => o.OrderDetails)
                .ThenInclude(od => od.Product) 
                .Where(o => o.UserID == userId)
                .OrderByDescending(o => o.OrderDate)
                .ToList();

            return View(orders);
        }
        [HttpPost]
        public IActionResult ClearOrderHistory()
        {
            string? userIdStr = HttpContext.Session.GetString("UserId");
            if (string.IsNullOrEmpty(userIdStr))
            {
                return RedirectToAction("Login", "Account");
            }

            int userId = int.Parse(userIdStr);

            // Lấy tất cả đơn hàng của user
            var orders = _datacontext.Orders
                .Where(o => o.UserID == userId)
                .Include(o => o.OrderDetails)
                .Include(o => o.Payment)
                .ToList();

            if (orders.Any())
            {
                // Xóa chi tiết + payment trước rồi mới xóa order
                foreach (var order in orders)
                {
                    _datacontext.OrderDetails.RemoveRange(order.OrderDetails);
                    _datacontext.Payments.RemoveRange(order.Payment);
                }

                _datacontext.Orders.RemoveRange(orders);
                _datacontext.SaveChanges();
            }

            return RedirectToAction("OrderHistory");
        }
        [HttpPost]
        [Route("CheckOut/GetShipping")]
        public async Task<IActionResult> GetShipping(ShippingModel shippingModel, string tinh)
        {

            var existingShipping = await _datacontext.Shippings
                .FirstOrDefaultAsync(x => x.City == tinh );

            decimal shippingPrice = 0; // Set mặc định giá tiền

            if (existingShipping != null)
            {
                shippingPrice = existingShipping.Price;
            }
            else
            {
                //Set mặc định giá tiền nếu ko tìm thấy
                shippingPrice = 50000;
            }
            ViewBag.ShippingPrice = shippingPrice;
            var shippingPriceJson = JsonConvert.SerializeObject(shippingPrice);
            try
            {
                var cookieOptions = new CookieOptions
                {
                    HttpOnly = true,
                    Expires = DateTimeOffset.UtcNow.AddMinutes(30),
                    Secure = true // using HTTPS
                };

                Response.Cookies.Append("ShippingPrice", shippingPriceJson, cookieOptions);
            }
            catch (Exception ex)
            {
                //
                Console.WriteLine($"Error adding shipping price cookie: {ex.Message}");
            }
            return Json(new { shippingPrice });
        }
    }
}
