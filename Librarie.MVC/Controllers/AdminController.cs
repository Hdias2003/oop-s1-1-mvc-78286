using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Library.MVC.Controllers
{
    // This attribute ensures that ONLY logged-in users with the "Admin" role 
    // can access any part of this controller. Everyone else is blocked.
    [Authorize(Roles = "Admin")]
    public class AdminController : Controller
    {
        // RoleManager is a built-in ASP.NET Identity service used to manage roles in the database
        private readonly RoleManager<IdentityRole> _roleManager;

        // Dependency Injection: The system provides the RoleManager automatically when the controller starts
        public AdminController(RoleManager<IdentityRole> roleManager)
        {
            _roleManager = roleManager;
        }

        // GET: /Admin/Roles
        // Purpose: Displays a list of all existing roles (e.g., Admin, User, Editor)
        public IActionResult Roles()
        {
            // Fetch all roles from the database and convert them to a list
            var roles = _roleManager.Roles.ToList();

            // Send that list to the View to be displayed in a table or list
            return View(roles);
        }

        // POST: /Admin/CreateRole
        // Purpose: Takes a name from a form and creates a new security role
        [HttpPost]
        public async Task<IActionResult> CreateRole(string roleName)
        {
            // Validation: Ensure the name isn't empty or just spaces
            if (!string.IsNullOrWhiteSpace(roleName))
            {
                // Create the new role and save it to the database
                await _roleManager.CreateAsync(new IdentityRole(roleName.Trim()));
            }

            // Refresh the page by redirecting back to the Roles list
            return RedirectToAction(nameof(Roles));
        }

        // POST: /Admin/DeleteRole
        // Purpose: Removes a specific role from the system using its unique ID
        [HttpPost]
        public async Task<IActionResult> DeleteRole(string id)
        {
            // Look up the role in the database using the provided ID
            var role = await _roleManager.FindByIdAsync(id);

            // If the role exists, delete it
            if (role != null)
            {
                await _roleManager.DeleteAsync(role);
            }

            // Refresh the page by redirecting back to the Roles list
            return RedirectToAction(nameof(Roles));
        }
    }
}