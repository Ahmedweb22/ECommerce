using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class ProductController : Controller
    {
        private IRepository<Product> _productRepository;
        private IProductSubImgRepository _productSubImgRepository;
        private IRepository<Catgeory> _categoryRepository;
        private IRepository<Brand> _brandRepository;
        public ProductController(IRepository<Product> productRepository, IProductSubImgRepository productSubImgRepository, IRepository<Catgeory> categoryRepository, IRepository<Brand> brandRepository)
        {
            _productRepository = productRepository;
            _productSubImgRepository = productSubImgRepository;
            _categoryRepository = categoryRepository;
            _brandRepository = brandRepository;
        }
        public async Task<IActionResult> Index(ProductFilterVM filterVM, int page = 1)
        {
            var products = await _productRepository.GetAsync(includes:  [ p => p.Catgeory, p => p.Brand ], tracking: false);
           

            //Add new filter
            ProductFilterVM filterVMResponse = new();
  
            var categories = await _categoryRepository.GetAsync(tracking: false);
            var brands = await _brandRepository.GetAsync(tracking: false);
            if (filterVM.Name is not null)
            {
                products = products.Where(e => e.Name.Contains(filterVM.Name)).ToList();
                filterVMResponse.Name = filterVM.Name;
            }
            if (filterVM.MinPrice is not null)
            {
                products = products.Where(e => e.Price >= filterVM.MinPrice).ToList();
                filterVMResponse.MinPrice = filterVM.MinPrice;
            }
            if (filterVM.MaxPrice is not null)
            {
                products = products.Where(e => e.Price <= filterVM.MaxPrice).ToList();
                filterVMResponse.MaxPrice = filterVM.MaxPrice;
            }
            if (filterVM.CategoryId is not null)
            {
                products = products.Where(e => e.CatgeoryId == filterVM.CategoryId).ToList();
                filterVMResponse.CategoryId = filterVM.CategoryId;
            }
            if (filterVM.BrandId is not null)
            {
                products = products.Where(e => e.BrandId == filterVM.BrandId).ToList();
                filterVMResponse.BrandId = filterVM.BrandId;
            }
            if (filterVM.LessQuantity)
            {
                products = products.Where(e => e.Quantity < 50).ToList();
                filterVMResponse.LessQuantity = filterVM.LessQuantity;
            }
            // Pagination
            if (page < 1)
                page = 1;
            int pageSize = 5;
            int currentPage = page;
            double totalCount = Math.Ceiling(products.Count() / (double)pageSize);
            products = products.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(new ProductsVM
            {
                Products = products.AsEnumerable(),
                Categories = categories.AsEnumerable(),
                Brands = brands.AsEnumerable(),
                TotalPages = totalCount,
                CurrentPage = currentPage,
                FilterVM = filterVMResponse
            });
        }
        [HttpGet]
        public async Task<IActionResult> Create()
        {
          
            var categories = await _categoryRepository.GetAsync(tracking: false);
            var brands = await _brandRepository.GetAsync(tracking: false);
            return View(new ProductCreateVM
            {
                Categories = categories,
                Brands = brands
            });
        }
        [HttpPost]
        public async Task<IActionResult> Create(Product product, IFormFile mainImg, List<IFormFile> subImgs)
        {
            if (mainImg is not null && mainImg.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(mainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                product.MainImg = newFileName;
            }

            await _productRepository.CreateAsync(product);
            await _productRepository.CommitAsync();
            if (subImgs.Any())
            {
                foreach (var img in subImgs)
                {
                    if (img is not null && img.Length > 0)
                    {
                        var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images\\sub_images", newFileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            img.CopyTo(stream);
                        }

                        await _productSubImgRepository.CreateAsync(new()
                        {
                            ProductId = product.Id,
                            SubImg = newFileName
                        });
                    }
                }
               await _productSubImgRepository.CommitAsync();
            }
            TempData["Notification"] = "Product created successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productRepository.GetOneAsync(e => e.Id == id);
            if (product is null)
                return NotFound();
            var productSubImgs = await _productSubImgRepository.GetAsync(p => p.ProductId == id);
            var categories = await _categoryRepository.GetAsync(tracking: false);
            var brands = await _brandRepository.GetAsync(tracking: false);
            return View(new ProductUpdateResponseVM
            {
                Product = product,
                SubImg = productSubImgs,
                Categories = categories,
                Brands = brands
            });
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Product product, IFormFile? mainImg, List<IFormFile>? subImgs)
        {
            var PtoductInDB  = await _productRepository.GetOneAsync(p => p.Id == product.Id , tracking: false);
            if (PtoductInDB is null)
                return NotFound();
            // 1.Update main image if exist
            if (mainImg is not null && mainImg.Length > 0)
            {
                //Step1: Create NewImg in wwwroot
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(mainImg.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    mainImg.CopyTo(stream);
                }
                product.MainImg = newFileName;
            //Step2: Delete old image from wwwroot
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images", PtoductInDB.MainImg);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                //step3: Update Img in DB
                product.MainImg = newFileName;
            }
            else
            {
                product.MainImg = PtoductInDB.MainImg;
            }
            //2.Update Product Info
            _productRepository.Update(product);
            await _productRepository.CommitAsync();

            //3. Update Sub Images if exist
            if (subImgs.Any())
            {
                var oldSubImgs = await _productSubImgRepository.GetAsync(p => p.ProductId == product.Id);
                //Step1: Create New Imgs in wwwroot & Save to DB
                foreach (var img in subImgs)
                {
                        var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(img.FileName);
                        var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images\\sub_images", newFileName);
                        using (var stream = System.IO.File.Create(filePath))
                        {
                            img.CopyTo(stream);
                        }
                    //Step2: Save to DB
                    await _productSubImgRepository.CreateAsync(new()
                    {
                        ProductId = product.Id,
                        SubImg = newFileName
                    });
                }
              //step3: Delete old Imgs from wwwroot
                foreach (var oldImg in oldSubImgs)
                {
                    var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images\\sub_images", oldImg.SubImg);
                    if (System.IO.File.Exists(oldFilePath))
                    {
                        System.IO.File.Delete(oldFilePath);
                    }
                }
                //step4: Delete old Imgs from DB
                 _productSubImgRepository.DeleteRange(oldSubImgs);
                await _productSubImgRepository.CommitAsync();
            }
            TempData["Notification"] = "Product updated successfully";

            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> DeleteImg([FromRoute] int id, [FromQuery] int productImgId)
        {
            var subImg = await _productSubImgRepository.GetOneAsync(p => p.Id == productImgId);
            if (subImg is null)
                return NotFound();
            var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images\\sub_images", subImg.SubImg);
            if (System.IO.File.Exists(filePath))
            {
                System.IO.File.Delete(filePath);
            }
            _productSubImgRepository.Delete(subImg);
            await _productSubImgRepository.CommitAsync();
            TempData["Notification"] = "Sub image deleted successfully";

            return RedirectToAction(nameof(Edit), new { id });
        }
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productRepository.GetOneAsync(p => p.Id == id);
            if (product is null)
                return NotFound();
            //Step1: Delete main image from wwwroot
            var mainImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images", product.MainImg);
            if (System.IO.File.Exists(mainImgPath))
            {
                System.IO.File.Delete(mainImgPath);
            }
            //Step2: Delete sub images from wwwroot
            var subImgs = await _productSubImgRepository.GetAsync(p => p.ProductId == id);
            foreach (var subImg in subImgs)
            {
                var subImgPath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\product_images\\sub_images", subImg.SubImg);
                if (System.IO.File.Exists(subImgPath))
                {
                    System.IO.File.Delete(subImgPath);
                }
            }
            //Step3: Delete sub images from DB
            _productSubImgRepository.DeleteRange(subImgs);
            //Step4: Delete product from DB
            _productRepository.Delete(product);
            await _productRepository.CommitAsync();
            TempData["Notification"] = "Product deleted successfully";

            return RedirectToAction(nameof(Index));
        }
    }
}
