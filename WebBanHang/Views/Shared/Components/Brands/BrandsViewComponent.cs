using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Views.Shared.Components.Brands
{
    public class BrandsViewComponent : ViewComponent
    {
        private readonly DataConnect _datacontext;

        // Dependency Injection để lấy DataConnect
        public BrandsViewComponent(DataConnect context)
        {
            _datacontext = context;
        }

        // Phương thức chính để lấy dữ liệu và trả về view
        public async Task<IViewComponentResult> InvokeAsync()
        {
            // Lấy danh sách danh mục một cách bất đồng bộ
            var brands = await _datacontext.Brands.ToListAsync();
            // Trả về view với dữ liệu đã lấy được
            return View(brands);
        }
    }
}
