using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models.ViewModel;
using System.Text.RegularExpressions;

namespace WebBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataConnect _context;

        public AccountController(DataConnect context)
        {
            _context = context;
        }

        // ====================== LOGIN ======================
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            if (TempData["SuccessMessage"] != null)
                ViewBag.SuccessMessage = TempData["SuccessMessage"];

            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _context.Users.FirstOrDefaultAsync(u => u.Username == loginVM.Username);

            if (user != null && user.Password == loginVM.Password)
            {
                HttpContext.Session.SetString("UserId", user.UserID.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                if (user.Role == "Admin")
<<<<<<< HEAD
                {
                    return RedirectToAction("ListUser", "User", new { area = "Admin" });
                }
                else if (user.Role == "Customer")
                {
                    return RedirectToAction("HomeIndex", "Home", new { area = (string?)null });

                }
=======
                    return RedirectToAction("HomeAdmin", "Admin", new { area = "Admin" });
>>>>>>> EnViDi
                else
                    return RedirectToAction("HomeIndex", "Home", new { area = (string?)null });
            }

            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return View(loginVM);
        }

        // ====================== REGISTER ======================
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserModel());
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel user)
        {
            if (!ModelState.IsValid)
                return View(user);

            if (await _context.Users.AnyAsync(u => u.Username == user.Username))
            {
                ModelState.AddModelError("Username", "Tên đăng nhập đã tồn tại");
                return View(user);
            }

            if (await _context.Users.AnyAsync(u => u.Email == user.Email))
            {
                ModelState.AddModelError("Email", "Email đã tồn tại");
                return View(user);
            }

            if (!string.IsNullOrWhiteSpace(user.Phone))
            {
                if (!Regex.IsMatch(user.Phone, @"^0\d{9}$"))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại phải bắt đầu bằng 0 và có đúng 10 số");
                    return View(user);
                }

                if (await _context.Users.AnyAsync(u => u.Phone == user.Phone))
                {
                    ModelState.AddModelError("Phone", "Số điện thoại này đã được sử dụng");
                    return View(user);
                }
            }

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu");
                return View(user);
            }

            user.Role = "Customer";
            user.Created = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            TempData["SuccessMessage"] = "Đăng ký tài khoản thành công! Vui lòng đăng nhập.";
            return RedirectToAction("Login");
        }

        // ====================== LOGOUT ======================
        [HttpGet]
        [Route("/logout")]
        public IActionResult Logout()
        {
            HttpContext.Session.Clear();
            return RedirectToAction("HomeIndex", "Home", new { area = (string?)null });
        }
    }
}
