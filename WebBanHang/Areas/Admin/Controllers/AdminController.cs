using Microsoft.AspNetCore.Mvc;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Route("Admin")]
    public class AdminController : Controller
    {
        [HttpGet("")]
        public IActionResult Index()
        {
            return View();
        }
    }
}
