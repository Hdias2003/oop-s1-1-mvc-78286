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
    public class LoansController : Controller
    {
        private readonly ApplicationDbContext _context;

        public LoansController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Loans
        // Purpose: Shows a history of all loans. 
        // Note: Uses '.Include' to grab the actual Book and Member names, not just their ID numbers.
        public async Task<IActionResult> Index()
        {
            var applicationDbContext = _context.Loans.Include(l => l.Book).Include(l => l.Member);
            return View(await applicationDbContext.ToListAsync());
        }

        // GET: Loans/Details/5
        // Purpose: Shows the full details of a specific loan (who has the book, when it's due, etc.)
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // GET: Loans/Create
        // Purpose: Prepares the "Check Out" form.
        public IActionResult Create()
        {
            // WORKFLOW: We filter the list so the librarian only sees books that are currently ON the shelf.
            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title");
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName");
            return View();
        }

        // POST: Loans/Create
        // Purpose: Actually "checks out" the book to a member.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("Id,BookId,MemberId,LoanDate,DueDate")] Loan loan)
        {
            if (ModelState.IsValid)
            {
                // WORKFLOW: Find the book being borrowed and flip its 'IsAvailable' switch to false.
                var book = await _context.Books.FindAsync(loan.BookId);
                if (book != null)
                {
                    book.IsAvailable = false;
                    _context.Update(book);
                }

                _context.Add(loan);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index));
            }

            // If the form was invalid, reload the dropdowns so the user can try again.
            ViewData["BookId"] = new SelectList(_context.Books.Where(b => b.IsAvailable), "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", loan.MemberId);
            return View(loan);
        }

        // CUSTOM WORKFLOW ACTION: Return Book
        // Purpose: This is triggered when a member drops a book back at the library.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ReturnBook(int id)
        {
            // Find the loan record and the connected book
            var loan = await _context.Loans.Include(l => l.Book).FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            // 1. Update the Loan record: Mark as returned and stamp it with today's date.
            loan.IsReturned = true;
            loan.ReturnedDate = DateTime.Now;

            // 2. Update the Book record: Make the book "Available" for the next person to borrow.
            if (loan.Book != null)
            {
                loan.Book.IsAvailable = true;
                _context.Update(loan.Book);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // GET: Loans/Edit/5
        // Purpose: Allows manual override of a loan (e.g., extending a due date).
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans.FindAsync(id);
            if (loan == null) return NotFound();

            ViewData["BookId"] = new SelectList(_context.Books, "Id", "Title", loan.BookId);
            ViewData["MemberId"] = new SelectList(_context.Members, "Id", "FullName", loan.MemberId);
            return View(loan);
        }

        // POST: Loans/Edit/5
        // Purpose: Saves the manual changes made to a loan record.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,BookId,MemberId,LoanDate,DueDate,IsReturned,ReturnedDate")] Loan loan)
        {
            if (id != loan.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(loan);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!LoanExists(loan.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(loan);
        }

        // GET: Loans/Delete/5
        // Purpose: Confirmation page to remove a loan record from history.
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var loan = await _context.Loans
                .Include(l => l.Book)
                .Include(l => l.Member)
                .FirstOrDefaultAsync(m => m.Id == id);

            if (loan == null) return NotFound();

            return View(loan);
        }

        // POST: Loans/Delete/5
        // Purpose: Finalizes the deletion of a loan record.
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var loan = await _context.Loans.FindAsync(id);
            if (loan != null)
            {
                _context.Loans.Remove(loan);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // Helper to check if a loan exists by ID.
        private bool LoanExists(int id)
        {
            return _context.Loans.Any(e => e.Id == id);
        }
    }
}