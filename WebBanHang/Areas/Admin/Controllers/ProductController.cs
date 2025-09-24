using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using System.Collections.Immutable;
using WebBanHang.Models;
using WebBanHang.Models.Repository;
using WebBanHang.Models.Repository.component;

namespace WebBanHang.Areas.Admin.Controllers
{
    [Area("Admin")]

    public class ProductController : Controller
    {
        private readonly DataConnect _datacontext;
        private readonly IWebHostEnvironment _webHostEnviroment;
        public ProductController(DataConnect context, IWebHostEnvironment webHostEnvironment)
        {
            _datacontext = context;
            _webHostEnviroment = webHostEnvironment;
        }
        [HttpGet]
        public async Task<IActionResult> Index()
        {
            return View(await _datacontext.Products.OrderByDescending(p => p.ProductID).Include(p => p.Category).Include(p => p.Brand).Include(p => p.ProductImage).Include(p => p.OperatingSystem).ToListAsync());
        }
       
        public IActionResult thongke()
        {
            var products = _datacontext.Products
                .Include(p => p.Brand)
                .Include(p => p.Category)
                .Include(p => p.OperatingSystem)
                .ToList();

            return View(products);
        }

        [HttpGet]
        public IActionResult Create()
        {
            ViewBag.Categories = new SelectList(_datacontext.Categories, "CategoryID", "CategoryName");
            ViewBag.Brands = new SelectList(_datacontext.Brands, "BrandID", "NameBrand");
            ViewBag.OperatingSystem = new SelectList(_datacontext.OperatingSystem, "OperatingSystemID", "OperatingSystemName");

            return View();
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(ProductModel product)
        {
            if (ModelState.IsValid)
            {
                product.Created = product.Updated = DateTime.Now;

                // Bước 1: thêm sản phẩm
                _datacontext.Products.Add(product);
                await _datacontext.SaveChangesAsync(); // lúc này ProductID mới được sinh ra

                // Bước 2: xử lý ảnh
                if (product.ImageUpload != null)
                {
                    string dir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    Directory.CreateDirectory(dir);
                    string name = Guid.NewGuid() + Path.GetExtension(product.ImageUpload.FileName);
                    string path = Path.Combine(dir, name);

                    using (var fs = new FileStream(path, FileMode.Create))
                        await product.ImageUpload.CopyToAsync(fs);

                    _datacontext.ProductImages.Add(new ProductImagesModel
                    {
                        ProductID = product.ProductID, // giờ đã có ID
                        ImageUrl = name,
                        IsMain = true
                    });

                    await _datacontext.SaveChangesAsync();
                }

                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }
            var errors = ModelState.Values.SelectMany(v => v.Errors).Select(e => e.ErrorMessage).ToList();
            TempData["error"] = string.Join(" | ", errors);
            return View(product);
        }

        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            // Load sản phẩm cùng ảnh liên quan theo ProductID
            var product = await _datacontext.Products
                .Include(p => p.ProductImage)
                .FirstOrDefaultAsync(p => p.ProductID == id);

            if (product == null)
                return NotFound();

            // Gán ViewBag cho dropdown Category và Brand
            ViewBag.Categories = new SelectList(_datacontext.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.Brands = new SelectList(_datacontext.Brands, "BrandID", "NameBrand", product.BrandID);
            ViewBag.OperatingSystem = new SelectList(_datacontext.OperatingSystem, "OperatingSystemID", "OperatingSystemName");

            return View(product);
        }
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(ProductModel product)
        {
            var existed_product = _datacontext.Products.Find(product.ProductID); //tìm sp theo id product
            ViewBag.Categories = new SelectList(_datacontext.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.Brands = new SelectList(_datacontext.Brands, "BrandID", "NameBrand", product.BrandID);
            ViewBag.OperatingSystem = new SelectList(_datacontext.OperatingSystem, "OperatingSystemID", "OperatingSystemName");

            if (ModelState.IsValid)
            {

                if (product.ImageUpload != null)
                {
                    string dir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    Directory.CreateDirectory(dir);
                    string name = Guid.NewGuid() + Path.GetExtension(product.ImageUpload.FileName);
                    string path = Path.Combine(dir, name);
                    using (var fs = new FileStream(path, FileMode.Create))
                        await product.ImageUpload.CopyToAsync(fs);

                    // Set tất cả ảnh hiện tại IsMain = false
                    var oldImages = _datacontext.ProductImages
                                        .Where(pi => pi.ProductID == product.ProductID && pi.IsMain);
                    foreach (var img in oldImages)
                        img.IsMain = false;

                    // Thêm ảnh mới và set IsMain = true
                    _datacontext.ProductImages.Add(new ProductImagesModel
                    {
                        ProductID = product.ProductID,
                        ImageUrl = name,
                        IsMain = true
                    });
                }


                // Update other product properties
                existed_product.NameProduct = product.NameProduct;
                existed_product.Description = product.Description;
                existed_product.Price = product.Price;
                existed_product.CategoryID = product.CategoryID;
                existed_product.BrandID = product.BrandID;
                existed_product.Stock = product.Stock;
                existed_product.OperatingSystemID = product.OperatingSystemID;
                // ... other properties
                _datacontext.Update(existed_product);
                await _datacontext.SaveChangesAsync();
                TempData["success"] = "Cập nhật sản phẩm thành công";
                return RedirectToAction("Index");

            }
            else
            {
                TempData["error"] = "Model có một vài thứ đang lỗi";
                List<string> errors = new List<string>();
                foreach (var value in ModelState.Values)
                {
                    foreach (var error in value.Errors)
                    {
                        errors.Add(error.ErrorMessage);
                    }
                }
                string errorMessage = string.Join("\n", errors);
                return BadRequest(errorMessage);
            }
            return View(product);
        }
        [HttpPost]
        public async Task<IActionResult> Delete(long Id)
        {
            // Lấy sản phẩm
            var product = await _datacontext.Products
                                .Include(p => p.ProductImage)
                                .FirstOrDefaultAsync(p => p.ProductID == Id);

            if (product == null)
            {
                TempData["error"] = "Sản phẩm không tồn tại";
                return RedirectToAction("Index");
            }

            // Xóa tất cả ảnh liên quan
            string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
            foreach (var img in product.ProductImage)
            {
                if (!string.IsNullOrEmpty(img.ImageUrl) && img.ImageUrl != "no-image-available.jpg")
                {
                    string filePath = Path.Combine(uploadsDir, img.ImageUrl);
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }
            }

            // Xóa ảnh trong database
            _datacontext.ProductImages.RemoveRange(product.ProductImage);

            // Xóa sản phẩm
            _datacontext.Products.Remove(product);
            await _datacontext.SaveChangesAsync();
            
            TempData["success"] = "Sản phẩm đã được xóa thành công";
            return RedirectToAction("Index");
        }



    }
}
