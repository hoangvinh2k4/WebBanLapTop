using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;

public class LuckyWheelController : Controller
{
    private readonly DataConnect _datacontext;

    // Giới hạn số lượt quay mỗi người mỗi ngày
    private const int MaxSpinsPerDay = 3;

    public LuckyWheelController(DataConnect context)
    {
        _datacontext = context;
    }

    // 🌀 [GET] Trang hiển thị vòng quay
    public IActionResult Index()
    {
        // Lấy ID người dùng từ Session
        var userIdStr = HttpContext.Session.GetString("UserId");
        int? userId = null;
        if (!string.IsNullOrEmpty(userIdStr))
            userId = int.Parse(userIdStr);

        // Lấy danh sách mã giảm giá còn khả dụng
        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0)
            .ToList();

        // Nếu người dùng đã đăng nhập → loại bỏ các mã đã trúng hôm nay
        if (userId.HasValue)
        {
            var todayWonIds = _datacontext.UserDiscounts
                .Where(ud => ud.UserId == userId.Value && ud.SpinDate.Date == DateTime.Now.Date)
                .Select(ud => ud.DiscountID)
                .ToList();

            discounts = discounts.Where(d => !todayWonIds.Contains(d.DiscountID)).ToList();
        }

        // Truyền trạng thái đăng nhập sang View
        ViewBag.IsLoggedIn = userId.HasValue;

        // Tính số lượt quay còn lại trong ngày
        int remainingSpins = 0;
        if (userId.HasValue)
        {
            var todaySpins = _datacontext.UserDiscounts
                .Count(ud => ud.UserId == userId.Value && ud.SpinDate.Date == DateTime.Now.Date);

            remainingSpins = Math.Max(MaxSpinsPerDay - todaySpins, 0);
        }

        ViewBag.RemainingSpins = remainingSpins;

        return View(discounts); // Truyền danh sách voucher sang view để hiển thị
    }

    // 🎯 [POST] Xử lý quay vòng
    [HttpPost]
    public IActionResult Spin()
    {
        // Phải đăng nhập mới quay được
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
        {
            return Json(new { error = true, message = "Bạn cần đăng nhập để quay!" });
        }

        int userId = int.Parse(userIdStr);

        // Đếm số lượt quay hôm nay của người dùng
        var todaySpins = _datacontext.UserDiscounts
            .Count(ud => ud.UserId == userId && ud.SpinDate.Date == DateTime.Now.Date);

        // Nếu đã quay đủ số lượt trong ngày → báo lỗi
        if (todaySpins >= MaxSpinsPerDay)
        {
            return Json(new { error = true, message = $"Bạn đã hết lượt quay hôm nay ({MaxSpinsPerDay} lượt)!" });
        }

        // Lấy các voucher hợp lệ (đang hoạt động + còn hạn + còn số lượng)
        var discounts = _datacontext.Discounts
            .Where(d => d.IsActive
                     && d.StartDate <= DateTime.Now
                     && d.EndDate >= DateTime.Now
                     && d.Quantity > 0)
            .ToList();

        // Loại bỏ các voucher mà user đã trúng hôm nay
        var todayWonIds = _datacontext.UserDiscounts
            .Where(ud => ud.UserId == userId && ud.SpinDate.Date == DateTime.Now.Date)
            .Select(ud => ud.DiscountID)
            .ToList();

        discounts = discounts.Where(d => !todayWonIds.Contains(d.DiscountID)).ToList();

        // Nếu không còn voucher nào có thể quay
        if (!discounts.Any())
            return Json(new { error = true, message = "Không còn mã giảm giá khả dụng!" });

        // Random chọn 1 voucher trúng
        var random = new Random();
        var selected = discounts[random.Next(discounts.Count)];

        // Lưu lịch sử quay (UserDiscounts)
        var spin = new UserDiscountModel
        {
            UserId = userId,
            DiscountID = selected.DiscountID,
            SpinDate = DateTime.Now
        };
        _datacontext.UserDiscounts.Add(spin);

        // Giảm số lượng voucher sau khi trúng
        selected.Quantity -= 1;
        if (selected.Quantity <= 0)
        {
            selected.IsActive = false; // tự động vô hiệu hóa nếu hết
        }

        _datacontext.SaveChanges();

        // Trả kết quả quay về dạng JSON (dùng JS để hiển thị popup chẳng hạn)
        return Json(new
        {
            error = false,
            code = selected.Code,
            percentage = selected.Percentage,
            message = $"Bạn nhận được mã {selected.Code} - Giảm {selected.Percentage}%",
            remainingSpins = MaxSpinsPerDay - todaySpins - 1
        });
    }

    // 📜 [GET] Xem lịch sử quay của người dùng
    public IActionResult History()
    {
        // Chưa đăng nhập → quay về trang login
        var userIdStr = HttpContext.Session.GetString("UserId");
        if (string.IsNullOrEmpty(userIdStr))
        {
            TempData["Error"] = "Bạn cần đăng nhập để xem lịch sử quay!";
            return RedirectToAction("Login", "Account");
        }

        int userId = int.Parse(userIdStr);

        // Lấy danh sách các lần quay + thông tin mã giảm giá đã trúng
        var history = _datacontext.UserDiscounts
            .Include(ud => ud.Discount)
            .Where(ud => ud.UserId == userId)
            .OrderByDescending(ud => ud.SpinDate)
            .ToList();

        return View(history); // Trả danh sách này về View để hiển thị
    }
}