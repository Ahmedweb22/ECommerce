using System.Diagnostics;
using E_Commerce.Models;
using E_Commerce.ViewModels;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace E_Commerce.Areas.Customer.Controllers
{
    [Area(SD.CUSTOMER_AREA)]
    public class HomeController : Controller
    {
        private ApplicationDbContext _context;

        private readonly ILogger<HomeController> _logger;

        public HomeController(ILogger<HomeController> logger, ApplicationDbContext context)
        {
            _logger = logger;
            _context = context;
        }

        public IActionResult Index(int? categoryId)
        {
            const double discount = 50;
            var products = _context.Products
                .Include(e => e.Catgeory)
                .Where(e => e.Discount > discount);
            if (categoryId is not null)
            {
                products = products.Where(e => e.CatgeoryId == categoryId);
            }

            products = products
                   .Skip(0)
                .Take(8);
            var categories = _context.Categories.Include(e => e.Products);

            return View(new ProductWithCategoriesVM
            {
                Products = products.ToList(),
                Categories = categories.ToList()
            });

        }
        public IActionResult Details(int id )
        {
            var product = _context.Products.SingleOrDefault(e => e.Id == id);
             if (product is null)
                return RedirectToAction(nameof(NotFoundPage));
             var sameBrands = _context.Products
                .Where(e => e.CatgeoryId == product.CatgeoryId && e.Id != product.Id)
                .Skip(0)
                .Take(4);
            var miniPrice = product.Price - (product.Price * 0.10);
            var maxPrice = product.Price + (product.Price * 0.10);
            var samePrice = _context.Products
                .Where(e => e.Price >= miniPrice && e.Price <= maxPrice && e.Id != product.Id)
                .Skip(0)
                .Take(4);

            var relatedProducts = _context.Products
                .Where(e => e.Name.Contains(product.Name) && e.Id != product.Id)
                .Skip(0)
                .Take(4);



            return View(new ProductWithRelatedVM
            {
            Product = product,
                SameCategories = sameBrands.ToList(),
                SamePrices = samePrice.ToList(),
                RelatedProducts = relatedProducts.ToList(),        
            });
        }
        public IActionResult NotFoundPage()
        { 
        return View();
        }
        public IActionResult Privacy()
        {
            return View();
        }

        [ResponseCache(Duration = 0, Location = ResponseCacheLocation.None, NoStore = true)]
        public IActionResult Error()
        {
            return View(new ErrorViewModel { RequestId = Activity.Current?.Id ?? HttpContext.TraceIdentifier });
        }
    }
}
