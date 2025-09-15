using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models;
using WebBanHang.Models.Repository.component;
using WebBanHang.Models.ViewModel;
using BCrypt.Net; // NuGet: BCrypt.Net-Next

namespace WebBanHang.Controllers
{
    public class AccountController : Controller
    {
        private readonly DataConnect _context;

        public AccountController(DataConnect context)
        {
            _context = context;
        }

        // GET: /Account/Login
        [HttpGet]
        public IActionResult Login(string returnUrl = null)
        {
            return View(new LoginViewModel { ReturnUrl = returnUrl });
        }

        // POST: /Account/Login
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Login(LoginViewModel loginVM)
        {
            if (!ModelState.IsValid)
                return View(loginVM);

            var user = await _context.Users
                                     .FirstOrDefaultAsync(u => u.Username == loginVM.Username);

            if (user != null && BCrypt.Net.BCrypt.Verify(loginVM.Password, user.PasswordHash))
            {
                // Lưu session
                HttpContext.Session.SetString("UserId", user.UserID.ToString());
                HttpContext.Session.SetString("Username", user.Username);
                HttpContext.Session.SetString("UserRole", user.Role);

                // Điều hướng theo Role
                if (user.Role == "Admin")
                {
                    return RedirectToAction("Index", "Home", new { area = "Admin" });
                }
                return RedirectToAction("Index", "Home", new { area = (string?)null });
            }

            ModelState.AddModelError("", "Sai tên đăng nhập hoặc mật khẩu");
            return View(loginVM);
        }

        // GET: /Account/Register
        [HttpGet]
        public IActionResult Register()
        {
            return View(new UserModel());
        }

        // POST: /Account/Register
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Register(UserModel user)
        {
            if (!ModelState.IsValid)
                return View(user);

            // Kiểm tra trùng Username hoặc Email
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

            if (string.IsNullOrWhiteSpace(user.Password))
            {
                ModelState.AddModelError("Password", "Vui lòng nhập mật khẩu");
                return View(user);
            }

            // Hash mật khẩu
            user.PasswordHash = BCrypt.Net.BCrypt.HashPassword(user.Password);
            user.Role = "Customer"; // mặc định khi đăng ký là Customer
            user.Created = DateTime.Now;

            _context.Users.Add(user);
            await _context.SaveChangesAsync();

            return RedirectToAction("RegisterSuccess");
        }

        // GET: /Account/RegisterSuccess
        [HttpGet]
        public IActionResult RegisterSuccess()
        {
            return View();
        }

        // GET: /logout
        [HttpGet]
        [Route("/logout")] // Bắt thẳng route, không phụ thuộc area
        public IActionResult Logout()
        {
            // Xoá toàn bộ session
            HttpContext.Session.Clear();

            // Chuyển về Login ngoài area Admin
            return RedirectToAction("Login", "Account", new { area = (string?)null });
        }
    }
}
