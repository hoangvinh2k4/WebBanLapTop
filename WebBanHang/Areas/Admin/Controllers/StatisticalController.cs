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

            // Lấy tất cả payment trong năm hiện tại
            var payments = _datacontext.Payments
                .Where(p => p.PaidDate.Year == currentYear)
                .AsNoTracking()
                .ToList();

            var totalRevenue = payments.Sum(p => p.Amount);
            var totalPayments = payments.Count;
            var totalUsers = _datacontext.Users.Count();

            // Doanh thu theo tháng
            var revenueByMonth = Enumerable.Range(1, 12)
                .Select(m => new
                {
                    Month = m,
                    Revenue = payments.Where(p => p.PaidDate.Month == m).Sum(p => p.Amount)
                })
                .ToList();

            ViewBag.Labels = revenueByMonth.Select(r => r.Month.ToString()).ToList();
            ViewBag.Data = revenueByMonth.Select(r => r.Revenue).ToList();

            ViewBag.TotalRevenue = totalRevenue;
            ViewBag.TotalPayments = totalPayments;
            ViewBag.TotalUsers = totalUsers;

            return View();
        }
    }
}
