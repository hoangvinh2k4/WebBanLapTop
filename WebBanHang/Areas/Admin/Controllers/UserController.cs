using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models;
using Microsoft.AspNetCore.Mvc.Rendering;

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

        // Hàm check admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // Danh sách user
        [HttpGet]
        public async Task<IActionResult> ListUser()
        {
            var users = await _context.Users
                                      .OrderByDescending(u => u.UserID)
                                      .ToListAsync();
            return View(users);
        }

        // GET: Tạo user
        [HttpGet("Create")]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Roles = GetRoles();
            return View(new UserModel());
        }

        // POST: Tạo user
        [HttpPost("Create")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserModel user)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            if (ModelState.IsValid)
            {
                // Hash mật khẩu trước khi lưu
                user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);

                _context.Users.Add(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListUser");
            }

            ViewBag.Roles = GetRoles();
            return View(user);
        }

        // GET: Sửa user
        [HttpGet("Edit/{id}")]
        public async Task<IActionResult> Edit(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            ViewBag.Roles = GetRoles();
            return View(user);
        }

        // POST: Sửa user
        [HttpPost("Edit/{id}")]
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

                // Nếu có nhập password mới thì hash lại
                if (!string.IsNullOrEmpty(userUpdate.Password))
                {
                    user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(userUpdate.Password);
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListUser");
            }

            ViewBag.Roles = GetRoles();
            return View(userUpdate);
        }

        // Xóa user
        [HttpGet("Delete/{id}")]
        public async Task<IActionResult> Delete(int id)
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            var user = await _context.Users.FindAsync(id);
            if (user == null) return NotFound();

            _context.Users.Remove(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("ListUser");
        }

        // Hàm tạo danh sách roles
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
