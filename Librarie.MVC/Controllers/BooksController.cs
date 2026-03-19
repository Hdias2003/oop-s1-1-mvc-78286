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
        private readonly ApplicationDbContext _context;

        public BooksController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Books
        public async Task<IActionResult> Index(string searchString, string categoryFilter, string availabilityFilter, string sortOrder)
        {
            // 1. Initial IQueryable (Query Composition starts here)
            IQueryable<Book> booksQuery = _context.Books;

            // 2. Search by Title or Author (Contains)
            if (!string.IsNullOrEmpty(searchString))
            {
                booksQuery = booksQuery.Where(s => s.Title.Contains(searchString) || s.Author.Contains(searchString));
            }

            // 3. Filter by Category (Dropdown)
            if (!string.IsNullOrEmpty(categoryFilter))
            {
                booksQuery = booksQuery.Where(x => x.Category == categoryFilter);
            }

            // 4. Filter by Availability (All / Available / On Loan)
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

            // 5. Sorting (Title A-Z / Z-A)
            // If sortOrder is "title_desc", sort Z-A. Otherwise, default to A-Z.
            booksQuery = (sortOrder == "title_desc")
                ? booksQuery.OrderByDescending(b => b.Title)
                : booksQuery.OrderBy(b => b.Title);

            // Store the "opposite" sort order in ViewBag for the clickable link in the View
            ViewBag.TitleSortParm = String.IsNullOrEmpty(sortOrder) ? "title_desc" : "";

            // 6. Prepare data for the View Dropdowns
            var categories = await _context.Books
                .Select(b => b.Category)
                .Distinct()
                .OrderBy(c => c)
                .ToListAsync();

            // Store current values in ViewBag so the UI keeps the user's selection
            ViewBag.Categories = new SelectList(categories);
            ViewBag.CurrentSearch = searchString;
            ViewBag.CurrentCategory = categoryFilter;
            ViewBag.CurrentAvailability = availabilityFilter;

            // 7. Execute query (Only one hit to the database)
            return View(await booksQuery.ToListAsync());
        }

        // GET: Books/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // GET: Books/Create
        public IActionResult Create()
        {
            return View();
        }

        // POST: Books/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (ModelState.IsValid)
            {
                _context.Add(book);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books.FindAsync(id);
            if (book == null)
            {
                return NotFound();
            }
            return View(book);
        }

        // POST: Books/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,Title,Author,Isbn,Category,IsAvailable")] Book book)
        {
            if (id != book.Id)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(book);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!BookExists(book.Id))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction(nameof(Index));
            }
            return View(book);
        }

        // GET: Books/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var book = await _context.Books
                .FirstOrDefaultAsync(m => m.Id == id);
            if (book == null)
            {
                return NotFound();
            }

            return View(book);
        }

        // POST: Books/Delete/5
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

        private bool BookExists(int id)
        {
            return _context.Books.Any(e => e.Id == id);
        }
    }
}