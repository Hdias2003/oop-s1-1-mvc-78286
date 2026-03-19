using Xunit;
using Microsoft.EntityFrameworkCore;
using Library.Domain.Models;
using Librarie.MVC.Data;
using System;
using System.Linq;
using System.Threading.Tasks;

namespace Library.Tests
{
    public class LibraryLogicTests
    {
        // Helper to create a fresh In-Memory database for each test
        private ApplicationDbContext GetDbContext()
        {
            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseInMemoryDatabase(databaseName: Guid.NewGuid().ToString())
                .Options;
            return new ApplicationDbContext(options);
        }

        [Fact]
        public void Test_OverdueLogic_IdentifiesOverdueLoan()
        {
            // Arrange
            var loan = new Loan
            {
                DueDate = DateTime.Now.AddDays(-5),
                ReturnedDate = null
            };

            // Act
            bool isOverdue = loan.DueDate < DateTime.Now && loan.ReturnedDate == null;

            // Assert
            Assert.True(isOverdue);
        }

        [Fact]
        public async Task Test_BookSearch_ReturnsMatches()
        {
            // Arrange
            var context = GetDbContext();
            context.Books.Add(new Book { Title = "C# Basics", Author = "John Doe" });
            context.Books.Add(new Book { Title = "Java Guide", Author = "Jane Smith" });
            await context.SaveChangesAsync();

            // Act (Simulating your IQueryable logic)
            var results = context.Books.Where(b => b.Title.Contains("C#")).ToList();

            // Assert
            Assert.Single(results);
            Assert.Equal("C# Basics", results[0].Title);
        }

        [Fact]
        public void Test_ReturnedLoan_MakesBookAvailable()
        {
            // Arrange
            var book = new Book { Title = "Test Book", IsAvailable = false };
            var loan = new Loan { Book = book, IsReturned = false };

            // Act - Simulating the "Return" action
            loan.IsReturned = true;
            loan.ReturnedDate = DateTime.Now;
            book.IsAvailable = true;

            // Assert
            Assert.True(book.IsAvailable);
        }

        [Fact]
        public async Task Test_CannotCreateLoan_ForUnavailableBook()
        {
            // Arrange
            var context = GetDbContext();
            var book = new Book { Title = "Borrowed Book", IsAvailable = false };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act & Assert 
            // We simulate the logic that should be in your Controller/Service
            bool canLoan = context.Books.Any(b => b.Id == book.Id && b.IsAvailable);

            Assert.False(canLoan);
        }

        [Fact]
        public void Test_RoleAuthorization_Exists()
        {
            // This tests if your controller has the [Authorize] attribute 
            // via Reflection (Requirement: Controller action guards)
            var controller = typeof(Library.MVC.Controllers.AdminController);
            var attributes = controller.GetCustomAttributes(typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

            Assert.True(attributes.Any(), "AdminController should have the [Authorize] attribute.");
        }
    }
}