using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class DiscountController : Controller
    {
        private readonly DataConnect _context;

        public DiscountController(DataConnect context)
        {
            _context = context;
        }

        // Check Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // Danh sách mã giảm giá
        public async Task<IActionResult> Index()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var discounts = await _context.Discounts
                .OrderByDescending(d => d.DiscountID)
                .ToListAsync();
            return View(discounts);
        }

        // GET: Tạo mới
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            return View(new DiscountModel());
        }

        // POST: Tạo mới
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(DiscountModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (ModelState.IsValid)
            {
                _context.Discounts.Add(model);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Sửa
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();
            return View(discount);
        }

        // POST: Sửa
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, DiscountModel model)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (id != model.DiscountID) return NotFound();

            if (ModelState.IsValid)
            {
                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null) return NotFound();

                discount.Code = model.Code;
                discount.Percentage = model.Percentage;
                discount.StartDate = model.StartDate;
                discount.EndDate = model.EndDate;
                discount.IsActive = model.IsActive;

                _context.Update(discount);
                await _context.SaveChangesAsync();

                return RedirectToAction(nameof(Index));
            }
            return View(model);
        }

        // GET: Xác nhận xoá
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();

            return View(discount); // Mở Delete.cshtml để confirm
        }

        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null) return NotFound();

            _context.Discounts.Remove(discount);
            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

    }
}
