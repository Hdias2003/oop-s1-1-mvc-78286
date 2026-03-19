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
    public class MembersController : Controller
    {
        // The _context is the database connection for members
        private readonly ApplicationDbContext _context;

        public MembersController(ApplicationDbContext context)
        {
            _context = context;
        }

        // GET: Members
        // Purpose: Displays the full list of library members (the "Phonebook" view)
        public async Task<IActionResult> Index()
        {
            // Simply fetches everyone from the 'Members' table and sends them to the page
            return View(await _context.Members.ToListAsync());
        }

        // GET: Members/Details/5
        // Purpose: Shows detailed contact info for one specific member
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null) return NotFound();

            // Look for a member where the ID matches the one clicked
            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);

            if (member == null) return NotFound();

            return View(member);
        }

        // GET: Members/Create
        // Purpose: Shows the empty form to sign up a new library member
        public IActionResult Create()
        {
            return View();
        }

        // POST: Members/Create
        // Purpose: Saves the new member's info to the database
        [HttpPost]
        [ValidateAntiForgeryToken] // Prevents "form hijacking" from external sites
        public async Task<IActionResult> Create([Bind("Id,FullName,Email,Phone")] Member member)
        {
            // Checks if the user filled in the name, email, etc., correctly
            if (ModelState.IsValid)
            {
                _context.Add(member);
                await _context.SaveChangesAsync();
                return RedirectToAction(nameof(Index)); // Successfully saved, go back to the list
            }
            return View(member); // If something was wrong, stay on the form and show errors
        }

        // GET: Members/Edit/5
        // Purpose: Finds a member and fills a form with their current info for updating
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Members.FindAsync(id);
            if (member == null) return NotFound();

            return View(member);
        }

        // POST: Members/Edit/5
        // Purpose: Saves the updated contact info (like a changed phone number)
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("Id,FullName,Email,Phone")] Member member)
        {
            if (id != member.Id) return NotFound();

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(member);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    // Error handling for if someone else deleted the member while you were editing
                    if (!MemberExists(member.Id)) return NotFound();
                    else throw;
                }
                return RedirectToAction(nameof(Index));
            }
            return View(member);
        }

        // GET: Members/Delete/5
        // Purpose: Shows a "Safety Warning" page before actually removing a member
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null) return NotFound();

            var member = await _context.Members
                .FirstOrDefaultAsync(m => m.Id == id);
            if (member == null) return NotFound();

            return View(member);
        }

        // POST: Members/Delete/5
        // Purpose: The final step that actually removes the person from the database
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var member = await _context.Members.FindAsync(id);
            if (member != null)
            {
                _context.Members.Remove(member);
            }

            await _context.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        // A quick helper to check if a specific Member ID exists
        private bool MemberExists(int id)
        {
            return _context.Members.Any(e => e.Id == id);
        }
    }
}