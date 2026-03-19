using Microsoft.AspNetCore.Mvc;

namespace Library.MVC.Controllers
{
    // The class name must end in "Controller"
    public class HomeController : Controller
    {
        // This method handles the URL: /Home/Index or just /
        public IActionResult Index()
        {
            return View();
        }

        // Optional: An "About" or "Contact" page
        public IActionResult Contact()
        {
            return View();
        }
    }
}