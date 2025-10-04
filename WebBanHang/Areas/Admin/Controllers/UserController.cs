using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.AspNetCore.Http; // để dùng HttpContext.Session
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

        // ✅ Mặc định khi vào /Admin/User thì redirect về ListUser
        public IActionResult Index()
        {
            return RedirectToAction("ListUser");
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
        [HttpGet]
        public IActionResult Create()
        {
            if (!IsAdmin())
                return RedirectToAction("Login", "Account", new { area = "" });

            ViewBag.Roles = GetRoles();
            return View(new UserModel());
        }

        // POST: Tạo user
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

        // GET: Sửa user
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

        // POST: Sửa user
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
                    // Lưu thẳng password (không hash)
                    user.Password = userUpdate.Password;
                }

                _context.Users.Update(user);
                await _context.SaveChangesAsync();

                return RedirectToAction("ListUser");
            }

            ViewBag.Roles = GetRoles();
            return View(userUpdate);
        }

        // Xóa user
        [HttpGet]
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

        // Check admin
        private bool IsAdmin()
        {
            return HttpContext.Session.GetString("UserRole") == "Admin";
        }

        // Danh sách roles
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
