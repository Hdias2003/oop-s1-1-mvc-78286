using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using Librarie.MVC.Data;
using Library.Domain.Models;

namespace Library.MVC.Controllers
{
    public class BooksController : Controller
    {
        // The _context is our bridge to the database
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        // Purpose: Shows the main list of books with search, filter, and sort options
        public async Task<IActionResult> Index(string searchString, string categoryFilter, string availabilityFilter, string sortOrder)
        {
            // 1. Start with the entire 'Books' table as a query (not loaded into memory yet)
            IQueryable<Book> booksQuery = _context.Books;

            // 2. Search: If the user typed a name or author, narrow down the list
            if (!string.IsNullOrEmpty(searchString))
            {
                booksQuery = booksQuery.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }

            // 3. Category Filter: Match the specific category selected in the dropdown
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                booksQuery = booksQuery.Where(x => x.Category == categoryFilter);
            }

            // 4. Availability: Filter based on whether the book is currently on the shelf or lent out
            if (!string.IsNullOrEmpty(availabilityFilter))
            {
                if (availabilityFilter == "Available")
                {
                    booksQuery = booksQuery.Where(b => b.IsAvailable == true);
                }
                else if (availabilityFilter == "OnLoan")
                {
                    booksQuery = booksQuery.Where(b => b.IsAvailable == false);
                }
            }

            // 5. Sorting: Decide if we show A-Z or Z-A based on the user clicking a column header
            booksQuery = (sortOrder == "title_desc")
                ? booksQuery.OrderByDescending(b => b.Title)
                : booksQuery.OrderBy(b => b.Title);

            // Toggle the sort order for the next time the user clicks the link
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            // 6. UI Preparation: Get a unique list of all categories to fill the dropdown menu
            var categories = await _context.Books
                .Select(b => b.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // Pass current selections back to the page so the search boxes stay filled in
            ViewBag.Categories = new SelectList(categories);
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryFilter;
            ViewBag.CurrentAvailability = availabilityFilter;

            // 7. Execution: NOW we actually talk to the database and get the final filtered list
            return View(await booksQuery.ToListAsync());
        }

        // GET: Books/Details/5
        // Purpose: Shows full information for one specific book based on its ID
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);

            if (book == null) return NotFound();

            return View(book);
        }

        // GET: Books/Create
        // Purpose: Simply displays the blank "New Book" form
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        // Purpose: Receives the data from the "New Book" form and saves it
        [HttpPost]
        [ValidateAntiForgeryToken] // Security feature to prevent cross-site request forgery
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            // If the form meets all validation rules (e.g., Title is required)
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Go back to the list
            }
            return View(book); // If errors, show the form again with error messages
        }

        // GET: Books/Edit/5
        // Purpose: Finds an existing book and puts its data into a form for editing
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FindAsync(id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Edit/5
        // Purpose: Saves the updated information for an existing book
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (id != book.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Handles the rare case where two people edit the same book at the exact same time
                    if (!BookExists(book.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        // Purpose: Shows a confirmation page asking "Are you sure you want to delete this?"
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var book = await _context.Books.FirstOrDefaultAsync(m => m.Id == id);
            if (book == null) return NotFound();

            return View(book);
        }

        // POST: Books/Delete/5
        // Purpose: Actually removes the book from the database after confirmation
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var book = await _context.Books.FindAsync(id);
            if (book != null)
            {
                _context.Books.Remove(book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Internal helper to check if a book exists before trying to edit or delete it
        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}