using Microsoft.AspNetCore.Mvc;

namespace E_Commerce.Areas.Customer.Controllers
{
    public class CheckoutController : Controller
    {
        public IActionResult Index()
        {
            return View();
        }
    }
}
