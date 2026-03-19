using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Librarie.MVC.Data;
using Library.Domain.Models;
using Bogus; // The library used for generating fake data
using Microsoft.AspNetCore.Identity;

namespace Library.MVC.Controllers
{
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _db;
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        // The constructor pulls in the DB context and Identity managers (for Admin setup)
        public SeedController(
            ApplicationDbContext context,
            UserManager<IdentityUser> userManager,
            RoleManager<IdentityRole> roleManager)
        {
            _db = context;
            _userManager = userManager;
            _roleManager = roleManager;
        }

        // URL: /seed-library
        // Purpose: Wipes the dust off the database and fills it with sample data
        [HttpGet("seed-library")]
        public async Task<IActionResult> SeedLibrary()
        {
            // 1. First, make sure an Admin user exists so you can actually log in
            await SeedAdminAsync();

            // 2. Safety Check: If books already exist, don't double-seed the database
            if (await _db.Books.AnyAsync())
                return Ok("Library data already exists.");

            // 3. Member Factory: Define the "Blueprint" for fake members
            var memberGenerator = new Faker<Member>()
                .RuleFor(m => m.FullName, f => f.Name.FullName())
                .RuleFor(m => m.Email, f => f.Internet.Email())
                .RuleFor(m => m.Phone, f => f.Phone.PhoneNumber())
                .RuleFor(m => m.JoinDate, f => f.Date.Past(2)); // Joined sometime in the last 2 years

            // 4. Book Factory: Define the "Blueprint" for fake books
            var bookGenerator = new Faker<Book>()
                .RuleFor(b => b.Title, f => f.Commerce.ProductName() + " Guide")
                .RuleFor(b => b.Author, f => f.Name.FullName())
                .RuleFor(b => b.Isbn, f => f.Random.Replace("###-###-###"))
                .RuleFor(b => b.Category, f => f.PickRandom(new[] { "Programming", "Fiction", "History", "Science" }))
                .RuleFor(b => b.IsAvailable, true);

            // Generate the lists (10 people, 20 books)
            var members = memberGenerator.Generate(10);
            var books = bookGenerator.Generate(20);

            // Save them to the database
            _db.Members.AddRange(members);
            _db.Books.AddRange(books);
            await _db.SaveChangesAsync();

            // 5. Loan Factory: Link random books to random members
            var loanGenerator = new Faker<Loan>()
                .RuleFor(l => l.BookId, f => f.PickRandom(books).Id)
                .RuleFor(l => l.MemberId, f => f.PickRandom(members).Id);

            var loans = loanGenerator.Generate(15);

            // 6. MANUALLY ASSIGN STATUS: To test your logic, we need specific scenarios
            for (int i = 0; i < loans.Count; i++)
            {
                if (i < 5) // Scenario A: Books that were borrowed and returned on time
                {
                    loans[i].LoanDate = DateTime.Now.AddDays(-20);
                    loans[i].DueDate = loans[i].LoanDate.AddDays(14);
                    loans[i].ReturnedDate = DateTime.Now.AddDays(-2);
                    loans[i].IsReturned = true;
                }
                else if (i < 10) // Scenario B: Books currently out but NOT overdue yet
                {
                    loans[i].LoanDate = DateTime.Now.AddDays(-5);
                    loans[i].DueDate = loans[i].LoanDate.AddDays(14);
                    loans[i].ReturnedDate = null;
                    loans[i].IsReturned = false;
                }
                else // Scenario C: Books that are currently overdue (Due 16 days ago!)
                {
                    loans[i].LoanDate = DateTime.Now.AddDays(-30);
                    loans[i].DueDate = loans[i].LoanDate.AddDays(14);
                    loans[i].ReturnedDate = null;
                    loans[i].IsReturned = false;
                }
            }

            // 7. Syncing: If a loan is active (Scenario B or C), the book must be marked "Unavailable"
            foreach (var loan in loans)
            {
                if (!loan.IsReturned)
                {
                    var loanedBook = books.FirstOrDefault(b => b.Id == loan.BookId);
                    if (loanedBook != null)
                    {
                        loanedBook.IsAvailable = false;
                    }
                }
            }

            // Save the loans and the updated book availability statuses
            _db.Books.UpdateRange(books);
            _db.Loans.AddRange(loans);
            await _db.SaveChangesAsync();

            return Ok("Success! Admin created. Data generated: 5 Returned, 5 Active, 5 Overdue loans.");
        }

        /// <summary>
        /// Creates the "Admin" role and a default admin user if they don't exist.
        /// </summary>
        private async Task SeedAdminAsync()
        {
            // Ensure the 'Admin' role exists in the identity system
            if (!await _roleManager.RoleExistsAsync("Admin"))
            {
                await _roleManager.CreateAsync(new IdentityRole("Admin"));
            }

            var adminEmail = "admin@library.com";
            var adminUser = await _userManager.FindByEmailAsync(adminEmail);

            // If the admin isn't there, create them with a default password
            if (adminUser == null)
            {
                var user = new IdentityUser { UserName = adminEmail, Email = adminEmail, EmailConfirmed = true };
                await _userManager.CreateAsync(user, "Admin123!");
                await _userManager.AddToRoleAsync(user, "Admin");
            }
        }
    }
}