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
        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0)
            .ToList();

        ViewBag.IsLoggedIn = HttpContext.Session.GetString("UserId") != null;

        // Số lượt còn lại hôm nay
        int remainingSpins = 0;
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (!string.IsNullOrEmpty(userIdStr))
        {
            int userId = int.Parse(userIdStr);
            var todaySpins = _datacontext.UserDiscounts
                .Count(ud => ud.UserId == userId && ud.SpinDate.Date == DateTime.Now.Date);
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

        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0) // chỉ lấy voucher còn số lượng
            .ToList();

        if (!discounts.Any())
            return Json(new { error = true, message = "Không còn mã giảm giá nào khả dụng!" });

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
