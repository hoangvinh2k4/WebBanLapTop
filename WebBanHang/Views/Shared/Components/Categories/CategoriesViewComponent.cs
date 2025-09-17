
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Views.Shared.Components.Categories
{
    public class CategoriesViewComponent : ViewComponent
    {
        private readonly DataConnect _datacontext;

        // Dependency Injection để lấy DataConnect
        public CategoriesViewComponent(DataConnect context)
        {
            _datacontext = context;
        }

        // Đây là View Category ở thanh SideBar  muốn dùng CategoriesViewComponent thì chữ đầu viết thường bỏ tiền tố ViewComponent
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách danh mục một cách bất đồng bộ
            var categories = await _datacontext.Categories.ToListAsync();
            // Trả về view với dữ liệu đã lấy được
            return View(categories);
        }
    }
}
