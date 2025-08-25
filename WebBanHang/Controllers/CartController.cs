using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Controllers
{
    public class CartController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}

