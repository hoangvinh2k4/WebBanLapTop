using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository;

namespace WebBanHang.Views.Shared.Components.OperatingSystem
{
    public class OperatingSystemViewComponent : ViewComponent
    {
        private readonly DataConnect _datacontext;

        // Dependency Injection để lấy DataConnect
        public OperatingSystemViewComponent(DataConnect context)
        {
            _datacontext = context;
        }

        // Đây là View Category ở thanh SideBar  muốn dùng CategoriesViewComponent thì chữ đầu viết thường bỏ tiền tố ViewComponent
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách danh mục một cách bất đồng bộ
            var os = await _datacontext.OperatingSystem.ToListAsync();
            // Trả về view với dữ liệu đã lấy được
            return View(os);
        }
    }
}




