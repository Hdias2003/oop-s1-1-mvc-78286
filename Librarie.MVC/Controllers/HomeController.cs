using Microsoft.AspNetCore.Mvc;

namespace Library.MVC.Controllers
{
    // In ASP.NET Core, the "Home" controller is usually the default starting point.
    // The class name must end in "Controller" so the system can find it.
    public class HomeController : Controller
    {
        // GET: / (The Root) or /Home/Index
        // Purpose: This is the "Landing Page" of your library website.
        public IActionResult Index()
        {
            // return View() looks for a file named "Index.cshtml" 
            // inside the /Views/Home/ folder and displays it to the user.
            return View();
        }

        // GET: /Home/Contact
        // Purpose: Displays a simple page with contact information or a support form.
        public IActionResult Contact()
        {
            // Looks for "Contact.cshtml" in the /Views/Home/ folder.
            return View();
        }
    }
}