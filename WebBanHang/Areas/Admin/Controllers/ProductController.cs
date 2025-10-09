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
            if (product.ImageUpload == null || product.ImageUpload.Count == 0)
            {
                ModelState.AddModelError("ImageUpload", "Bạn phải chọn ít nhất một ảnh cho sản phẩm");
            }

            if (ModelState.IsValid)
            {
                product.Created = product.Updated = DateTime.Now;

                // Bước 1: thêm sản phẩm
                _datacontext.Products.Add(product);
                await _datacontext.SaveChangesAsync(); // lúc này ProductID mới được sinh ra

                // Bước 2: xử lý ảnh
                if (product.ImageUpload != null && product.ImageUpload.Count > 0)
                {
                    string dir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    Directory.CreateDirectory(dir);
                    bool isFirst = true;
                    foreach (var file in product.ImageUpload)
                    {
                        if (file != null && file.Length > 0)
                        {
                            string name = Guid.NewGuid() + Path.GetExtension(file.FileName);
                            string path = Path.Combine(dir, name);

                            using (var fs = new FileStream(path, FileMode.Create))
                                await file.CopyToAsync(fs);

                            _datacontext.ProductImages.Add(new ProductImagesModel
                            {
                                ProductID = product.ProductID,
                                ImageUrl = name,
                                IsMain = isFirst
                            });
                            isFirst = false;
                        }
                    }

                    await _datacontext.SaveChangesAsync();
                }


                TempData["success"] = "Thêm sản phẩm thành công";
                return RedirectToAction("Index");
            }

            ViewBag.Categories = new SelectList(_datacontext.Categories, "CategoryID", "CategoryName", product.CategoryID);
            ViewBag.Brands = new SelectList(_datacontext.Brands, "BrandID", "NameBrand", product.BrandID);
            ViewBag.OperatingSystem = new SelectList(_datacontext.OperatingSystem, "OperatingSystemID", "OperatingSystemName");
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

                if (product.ImageUpload != null && product.ImageUpload.Count > 0)
                {
                    string dir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
                    Directory.CreateDirectory(dir);

                    foreach (var file in product.ImageUpload)
                    {
                        string name = Guid.NewGuid() + Path.GetExtension(file.FileName);
                        string path = Path.Combine(dir, name);

                        using (var fs = new FileStream(path, FileMode.Create))
                            await file.CopyToAsync(fs);

                        _datacontext.ProductImages.Add(new ProductImagesModel
                        {
                            ProductID = product.ProductID,
                            ImageUrl = name,
                            IsMain = false // ảnh đầu tiên có thể set true nếu muốn
                        });
                    }
                    await _datacontext.SaveChangesAsync();
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

        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> DeleteImage(int imageId, int productId)
        {
            var image = await _datacontext.ProductImages.FindAsync(imageId);
            if (image == null)
                return NotFound();

            // Xóa file vật lý
            string uploadsDir = Path.Combine(_webHostEnviroment.WebRootPath, "media/products");
            string filePath = Path.Combine(uploadsDir, image.ImageUrl);
            if (System.IO.File.Exists(filePath))
                System.IO.File.Delete(filePath);

            // Xóa trong DB
            _datacontext.ProductImages.Remove(image);
            await _datacontext.SaveChangesAsync();

            TempData["success"] = "Xóa ảnh thành công";
            return RedirectToAction("Edit", new { id = productId }); // ✅ về lại form Edit
        }
        [HttpPost]
        [HttpPost]
        public async Task<IActionResult> SetMainImage(int imageId, int productId)
        {
            var product = await _datacontext.Products
                .Include(p => p.ProductImage)
                .FirstOrDefaultAsync(p => p.ProductID == productId);

            if (product == null) return NotFound();

            // reset hết IsMain = false
            foreach (var img in product.ProductImage)
            {
                img.IsMain = false;
            }

            // set ảnh được chọn thành main
            var selectedImg = product.ProductImage.FirstOrDefault(x => x.ImageID == imageId);
            if (selectedImg != null)
            {
                selectedImg.IsMain = true;
            }

            await _datacontext.SaveChangesAsync();

            TempData["success"] = "Đã đổi ảnh đại diện!";
            return RedirectToAction("Edit", new { id = productId });
        }



    }
}
