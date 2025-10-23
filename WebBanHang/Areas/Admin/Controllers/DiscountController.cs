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

        // Kiểm tra quyền Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // 📋 Danh sách mã giảm giá
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
                if (model.StartDate >= model.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                    return View(model);
                }

                bool exists = await _context.Discounts.AnyAsync(d => d.Code == model.Code);
                if (exists)
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                    return View(model);
                }

                try
                {
                    model.Quantity = model.Quantity < 0 ? 0 : model.Quantity; // xử lý số lượng
                    _context.Discounts.Add(model);
                    await _context.SaveChangesAsync();
                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu: " + ex.Message);
                }
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
                if (model.StartDate >= model.EndDate)
                {
                    ModelState.AddModelError("EndDate", "Ngày kết thúc phải lớn hơn ngày bắt đầu.");
                    return View(model);
                }

                var discount = await _context.Discounts.FindAsync(id);
                if (discount == null) return NotFound();

                bool exists = await _context.Discounts
                    .AnyAsync(d => d.Code == model.Code && d.DiscountID != id);
                if (exists)
                {
                    ModelState.AddModelError("Code", "Mã giảm giá này đã tồn tại.");
                    return View(model);
                }

                try
                {
                    discount.Code = model.Code;
                    discount.Percentage = model.Percentage;
                    discount.StartDate = model.StartDate;
                    discount.EndDate = model.EndDate;
                    discount.IsActive = model.IsActive;
                    discount.Quantity = model.Quantity < 0 ? 0 : model.Quantity; // cập nhật số lượng

                    _context.Update(discount);
                    await _context.SaveChangesAsync();

                    return RedirectToAction(nameof(Index));
                }
                catch (Exception ex)
                {
                    ModelState.AddModelError("", "Có lỗi xảy ra khi lưu: " + ex.Message);
                }
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

            return View(discount); // mở Delete.cshtml để confirm
        }

        // POST: Xoá
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            // lấy một mã giảm giá trong database theo ID được truyền vào
            var discount = await _context.Discounts.FindAsync(id);
            if (discount == null)
            {
                TempData["Error"] = "⚠️ Không tìm thấy mã giảm giá.";
                return RedirectToAction(nameof(Index));
            }

            if (discount.EndDate.Date >= DateTime.Now.Date)
            {
                // Điều kiện này đúng khi EndDate là ngày hôm nay hoặc tương lai (CHƯA HẾT HẠN)
                TempData["Error"] = "❌ Không thể xoá mã giảm giá **vẫn còn hiệu lực** (chưa qua ngày hết hạn).";
                return RedirectToAction(nameof(Index));
            }

            try
            {
                // ✅ Nếu logic trên sai (EndDate < Today), thì mã đã hết hạn và được phép xóa.
                _context.Discounts.Remove(discount);
                await _context.SaveChangesAsync();

                TempData["Success"] = "✅ Đã xoá mã giảm giá hết hạn thành công.";
                // Quay về trang danh sách
                return RedirectToAction(nameof(Index));
            }
            catch (Exception)
            {
                // Thông báo lỗi nếu không thể xóa
                TempData["Error"] = "❌ Lỗi: Không thể xoá vì mã này có thể đang được sử dụng trong các đơn hàng.";
                return RedirectToAction(nameof(Index));
            }
        }
    }
}
