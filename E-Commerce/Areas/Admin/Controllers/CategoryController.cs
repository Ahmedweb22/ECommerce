using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Admin.Controllers
{
    [Area(SD.ADMIN_AREA)]
    public class CategoryController : Controller
    {
        private IRepository<Catgeory> _repository;
        public CategoryController(IRepository<Catgeory> repository) 
        {
        _repository = repository;
        }
        public async Task<IActionResult> Index(string? name, int page = 1)
        {
            var categories = await _repository.GetAsync(tracking: false);
            //Add new filter
            if (name is not null)
                categories = categories.Where(e => e.Name.Contains(name)).ToList();

            // Pagination
            if (page < 1)
                page = 1;
            int pageSize = 5;
            int currentPage = page;
            double totalCount = Math.Ceiling(categories.Count() / (double)pageSize);
            categories = categories.Skip((page - 1) * pageSize).Take(pageSize).ToList();
            return View(new CategoriesVM
            {
                Categories = categories.AsEnumerable(),
                TotalPages = totalCount,
                CurrentPage = currentPage
            });
        }
        [HttpGet]
        public IActionResult Create()
        {
            return View(new Catgeory());
        }
        [HttpPost]
        public async Task<IActionResult> Create(Catgeory catgeory)
        {
            if (!ModelState.IsValid)
                return View(catgeory);
           
            await _repository.CreateAsync(catgeory);
            await _repository.CommitAsync();
            TempData["Notification"] = "Category created successfully";
            return RedirectToAction(nameof(Index));
        }
        [HttpGet]
        public async Task<IActionResult> Edit([FromRoute] int id)
        {
            var category = await _repository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return NotFound();
            return View(category);
        }
        [HttpPost]
        public async Task<IActionResult> Edit(Catgeory catgeory)
        {

            if (!ModelState.IsValid)
                return View(catgeory);
           
            _repository.Update(catgeory);
            await _repository.CommitAsync();
                TempData["Notification"] = "Category updated successfully";
            return RedirectToAction(nameof(Index));
        }
        public async Task<IActionResult> Delete([FromRoute] int id)
        {
                var category = await  _repository.GetOneAsync(e => e.Id == id);
            if (category is null)
                return NotFound();
            _repository.Delete(category);
            await _repository.CommitAsync();
                TempData["Notification"] = "Category deleted successfully";
            return RedirectToAction(nameof(Index));
        }
    }
}
