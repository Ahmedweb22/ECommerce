using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class BrandController : Controller
    {

        private IRepository<Brand> _brandRepository;
        public BrandController(IRepository<Brand> brandRepository)
        {
            _brandRepository = brandRepository;
        }
        public async Task<IActionResult> Index(string? name, int page = 1)
        {
    
            var brands = await _brandRepository.GetAsync(tracking: false);
            //Add new filter
            if (name is not null)
                brands = brands.Where(e => e.Name.Contains(name)).ToList();

            // Pagination
            if (page < 1)
                page = 1;
            int pageSize = 5;
            int currentPage = page;
            double totalCount = Math.Ceiling(brands.Count() / (double)pageSize);
            brands = brands.Skip((page - 1) * pageSize).Take(pageSize).ToList();

            return View(new BrandsVM
            {
                Brands = brands.AsEnumerable(),
                CurrentPage = currentPage,
                TotalPages = totalCount
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View();
        }
        [HttpPost]
        public async Task<IActionResult> Create(Brand brand , IFormFile logo)
        {
            if (logo is not null && logo.Length > 0)
            { 
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(logo.FileName);
                var filePath =  Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brand_logos" , newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    logo.CopyTo(stream);
                }
                brand.Logo = newFileName;
            }
          
         await  _brandRepository.CreateAsync(brand);
         await _brandRepository.CommitAsync();
            TempData["Notification"] = "Brand created successfully";

            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id);
            if (brand is null)
                return NotFound();
            return View(brand);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Brand brand , IFormFile? logo)
        {
            Brand? existingBrand = await _brandRepository.GetOneAsync(e => e.Id == brand.Id , tracking: false);
            if (existingBrand is null)
                return NotFound();
            if (logo is not null && logo.Length > 0)
            {
                var newFileName = Guid.NewGuid().ToString() + DateTime.UtcNow.ToString("yyyy-MM-dd") + Path.GetExtension(logo.FileName);
                var filePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brand_logos", newFileName);
                using (var stream = System.IO.File.Create(filePath))
                {
                    logo.CopyTo(stream);
                }
                // Optionally delete the old logo file
                var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brand_logos", existingBrand.Logo);
                if (System.IO.File.Exists(oldFilePath))
                {
                    System.IO.File.Delete(oldFilePath);
                }
                brand.Logo = newFileName;
            }
            else
            {
                brand.Logo = existingBrand.Logo;
            }
           _brandRepository.Update(brand);
              await _brandRepository.CommitAsync();
            TempData["Notification"] = "Brand updated successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
            var brand = await _brandRepository.GetOneAsync(e => e.Id == id);
            if (brand is null)
                return NotFound();
            var oldFilePath = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot\\img\\brand_logos", brand.Logo);
            if (System.IO.File.Exists(oldFilePath))
            {
                System.IO.File.Delete(oldFilePath);
            }
           _brandRepository.Delete(brand);
           await _brandRepository.CommitAsync();
            TempData["Notification"] = "Brand deleted successfully";
            return RedirectToAction(nameof(Index));
        }

    }
    }
