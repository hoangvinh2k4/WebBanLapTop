using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;

public class LuckyWheelController : Controller
{
    private readonly DataConnect _datacontext;
    private const int MaxSpinsPerDay = 3; // giới hạn 3 lượt/ngày

    public LuckyWheelController(DataConnect context)
    {
        _datacontext = context;
    }

    // 🌀 Trang vòng quay
    public IActionResult Index()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        int? userId = null;
        if (!string.IsNullOrEmpty(userIdStr))
            userId = int.Parse(userIdStr);

        // Lấy danh sách discount còn khả dụng
        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0)
            .ToList();

        // Nếu user đã login, lọc bỏ các voucher đã trúng hôm nay
        if (userId.HasValue)
        {
            var todayWonIds = _datacontext.UserDiscounts
                .Where(ud => ud.UserId == userId.Value && ud.SpinDate.Date == DateTime.Now.Date)
                .Select(ud => ud.DiscountID)
                .ToList();

            discounts = discounts.Where(d => !todayWonIds.Contains(d.DiscountID)).ToList();
        }

        ViewBag.IsLoggedIn = userId.HasValue;

        // Số lượt còn lại hôm nay
        int remainingSpins = 0;
        if (userId.HasValue)
        {
            var todaySpins = _datacontext.UserDiscounts
                .Count(ud => ud.UserId == userId.Value && ud.SpinDate.Date == DateTime.Now.Date);
            remainingSpins = Math.Max(MaxSpinsPerDay - todaySpins, 0);
        }

        ViewBag.RemainingSpins = remainingSpins;

        return View(discounts);
    }

    // 🎯 Quay vòng
    [HttpPost]
    public IActionResult Spin()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
        {
            return Json(new { error = true, message = "Bạn cần đăng nhập để quay!" });
        }

        int userId = int.Parse(userIdStr);

        // Kiểm tra số lượt hôm nay
        var todaySpins = _datacontext.UserDiscounts
            .Count(ud => ud.UserId == userId && ud.SpinDate.Date == DateTime.Now.Date);

        if (todaySpins >= MaxSpinsPerDay)
        {
            return Json(new { error = true, message = $"Bạn đã hết lượt quay hôm nay ({MaxSpinsPerDay} lượt)!" });
        }

        // Lấy danh sách voucher còn khả dụng
        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0)
            .ToList();

        // Lọc bỏ các voucher user đã trúng hôm nay
        var todayWonIds = _datacontext.UserDiscounts
            .Where(ud => ud.UserId == userId && ud.SpinDate.Date == DateTime.Now.Date)
            .Select(ud => ud.DiscountID)
            .ToList();

        discounts = discounts.Where(d => !todayWonIds.Contains(d.DiscountID)).ToList();

        if (!discounts.Any())
            return Json(new { error = true, message = "Không còn mã giảm giá khả dụng!" });

        // Random chọn mã
        var random = new Random();
        var selected = discounts[random.Next(discounts.Count)];

        // Lưu lịch sử quay
        var spin = new UserDiscountModel
        {
            UserId = userId,
            DiscountID = selected.DiscountID,
            SpinDate = DateTime.Now
        };
        _datacontext.UserDiscounts.Add(spin);

        // ✅ Giảm số lượng voucher
        selected.Quantity -= 1;
        if (selected.Quantity <= 0)
        {
            selected.IsActive = false; // tự động vô hiệu nếu hết
        }

        _datacontext.SaveChanges();

        return Json(new
        {
            error = false,
            code = selected.Code,
            percentage = selected.Percentage,
            message = $"Bạn nhận được mã {selected.Code} - Giảm {selected.Percentage}%",
            remainingSpins = MaxSpinsPerDay - todaySpins - 1
        });
    }

    // 📜 Xem lịch sử voucher
    public IActionResult History()
    {
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
        {
            TempData["Error"] = "Bạn cần đăng nhập để xem lịch sử quay!";
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(userIdStr);

        var history = _datacontext.UserDiscounts
            .Include(ud => ud.Discount)
            .Where(ud => ud.UserId == userId)
            .OrderByDescending(ud => ud.SpinDate)
            .ToList();

        return View(history);
    }
}
