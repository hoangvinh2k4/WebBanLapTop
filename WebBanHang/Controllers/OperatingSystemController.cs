using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Controllers
{
    public class OperatingSystemController : Controller
    {
        private readonly DataConnect _datacontext;
        public OperatingSystemController(DataConnect context)
        {
            _datacontext = context;
        }
        public IActionResult OperatingSystemIndex(int id)
        {
            // Lấy toàn bộ danh mục
            var os = _datacontext.Products
                               .Include(p => p.ProductImage) // load kèm bảng ProductImages
                               .Include(p => p.OperatingSystem)         // load kèm thương hiệu
                               .Where(p => p.OperatingSystemID == id)
                               .ToList();
            return View(os);
        }
    }
}
