using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    public class UserController : Controller
    {
        private readonly DataConnect _context;

        public UserController(DataConnect context)
        {
            _context = context;
        }

        // ✅ Khi vào /Admin/User -> chuyển sang ListUser
        public IActionResult Index()
        {
            return RedirectToAction("ListUser");
        }

        // ✅ Hiển thị danh sách User
        [HttpGet]
        public async Task<IActionResult> ListUser()
        {
            var users = await _context.Users
                                      .OrderByDescending(u => u.UserID)
                                      .ToListAsync();
            return View(users);
        }

        // ✅ GET: Tạo User
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Roles = GetRoles();
            return View(new UserModel());
        }

        // ✅ POST: Tạo User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (ModelState.IsValid)
            {
                _context.Users.Add(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListUser");
            }

            ViewBag.Roles = GetRoles();
            return View(user);
        }

        // ✅ GET: Sửa User
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = GetRoles();
            return View(user);
        }

        // ✅ POST: Sửa User
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserModel userUpdate)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            if (ModelState.IsValid)
            {
                user.Username = userUpdate.Username;
                user.Email = userUpdate.Email;
                user.Phone = userUpdate.Phone;
                user.Role = userUpdate.Role;

                if (!string.IsNullOrEmpty(userUpdate.Password))
                {
                    user.Password = userUpdate.Password; // ⚠ chưa hash
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();
                return RedirectToAction("ListUser");
            }

            ViewBag.Roles = GetRoles();
            return View(userUpdate);
        }

        // ✅ XÓA (bằng AJAX)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteUserAjax(int id)


        {
            if (!IsAdmin())
                return Json(new { success = false, message = "Bạn không có quyền xoá." });

            var user = await _context.Users.FindAsync(id);
            if (user == null)
                return Json(new { success = false, message = "Không tìm thấy người dùng." });

            // 🔹 Xoá các bản ghi phụ thuộc
            var userDiscounts = await _context.UserDiscounts
                                              .Where(ud => ud.UserId == id)
                                              .ToListAsync();
            if (userDiscounts.Any())
                _context.UserDiscounts.RemoveRange(userDiscounts);

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return Json(new { success = true, message = $"Đã xoá người dùng \"{user.Username}\" thành công!" });
        }

        // ✅ Kiểm tra Admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // ✅ Danh sách Role
        private List<SelectListItem> GetRoles()
        {
            return new List<SelectListItem>
            {
                new SelectListItem { Value = "Admin", Text = "Admin" },
                new SelectListItem { Value = "Customer", Text = "Customer" }
            };
        }
    }
}
