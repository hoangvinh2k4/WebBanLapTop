using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class StatisticalController : Controller
    {
        private readonly DataConnect _datacontext;
        private readonly ILogger<StatisticalController> _logger;

        public StatisticalController(ILogger<StatisticalController> logger, DataConnect context)
        {
            _datacontext = context;
            _logger = logger;
        }

        public IActionResult StatisticalIndex()
        {
            int currentYear = DateTime.Now.Year;

            // Lấy tất cả đơn đã thanh toán trong năm hiện tại
            var orders = _datacontext.Orders
                .Where(o => o.Status == "Đã thanh toán" && o.OrderDate.Year == currentYear)
                .AsNoTracking()
                .ToList();

            var totalRevenue = orders.Sum(o => o.TotalAmount);
            var totalOrders = orders.Count;
            var totalUsers = _datacontext.Users.Count();

            // Doanh thu theo tháng
            var revenueByMonth = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = m,
                    Revenue = orders.Where(o => o.OrderDate.Month == m).Sum(o => o.TotalAmount)
                })
                .ToList();

            ViewBag.Labels = revenueByMonth.Select(r => r.Month.ToString()).ToList();
            ViewBag.Data = revenueByMonth.Select(r => r.Revenue).ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalOrders = totalOrders;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }


    }
}
