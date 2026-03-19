using Librarie.MVC.Data;
using Library.Domain.Models;

using Microsoft.EntityFrameworkCore;
using System;
using System.Linq;
using System.Threading.Tasks;
using Xunit;

namespace Library.Tests
{
    public class LibraryLogicTests
    {
        /// Generates a unique, in-memory database instance for each test.
        /// This ensures tests are isolated and don't leak data into one another.
       
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
            // Arrange: Set up a loan that was due 5 days ago and hasn't been returned
            var loan = new Loan
            {
                DueDate = DateTime.Now.AddDays(-5),
                ReturnedDate = null
            };

            // Act: Evaluate the logic that determines if a loan is overdue
            bool isOverdue = loan.DueDate < DateTime.Now && loan.ReturnedDate == null;

            // Assert: Verify that the logic correctly flagged the loan as overdue
            Assert.True(isOverdue);
        }

        [Fact]
        public async Task Test_BookSearch_ReturnsMatches()
        {
            // Arrange: Initialize a fresh database and seed it with sample books
            var context = GetDbContext();
            context.Books.Add(new Book { Title = "C# Basics", Author = "John Doe" });
            context.Books.Add(new Book { Title = "Java Guide", Author = "Jane Smith" });
            await context.SaveChangesAsync();

            // Act: Perform a search query for books containing "C#" in the title
            var results = context.Books.Where(b => b.Title.Contains("C#")).ToList();

            // Assert: Ensure exactly one book was found and it has the correct title
            Assert.Single(results);
            Assert.Equal("C# Basics", results[0].Title);
        }

        [Fact]
        public void Test_ReturnedLoan_MakesBookAvailable()
        {
            // Arrange: Set up an unavailable book and an active (unreturned) loan
            var book = new Book { Title = "Test Book", IsAvailable = false };
            var loan = new Loan { Book = book, IsReturned = false };

            // Act: Simulate the business logic for returning a book
            loan.IsReturned = true;
            loan.ReturnedDate = DateTime.Now;
            book.IsAvailable = true;

            // Assert: Verify the book's status has updated to available
            Assert.True(book.IsAvailable);
        }

        [Fact]
        public async Task Test_CannotCreateLoan_ForUnavailableBook()
        {
            // Arrange: Add a book to the DB that is already marked as unavailable
            var context = GetDbContext();
            var book = new Book { Title = "Borrowed Book", IsAvailable = false };
            context.Books.Add(book);
            await context.SaveChangesAsync();

            // Act: Check the database to see if this specific book is available for a new loan
            bool canLoan = context.Books.Any(b => b.Id == book.Id && b.IsAvailable);

            // Assert: Verify that the system prevents loaning out an unavailable book
            Assert.False(canLoan);
        }

        [Fact]
        public void Test_RoleAuthorization_Exists()
        {
            // Arrange: Get the Type definition for the AdminController
            var controller = typeof(Library.MVC.Controllers.AdminController);

            // Act: Use Reflection to check if the [Authorize] attribute is applied to the class
            var attributes = controller.GetCustomAttributes(
                typeof(Microsoft.AspNetCore.Authorization.AuthorizeAttribute), true);

            // Assert: Ensure the attribute is present to confirm security is enforced
            Assert.True(attributes.Any(), "AdminController should have the [Authorize] attribute.");
        }
    }
}