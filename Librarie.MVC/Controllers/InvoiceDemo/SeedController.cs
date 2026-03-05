using Librarie.Domain;
using Librarie.MVC.Data;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace Librarie.MVC.Controllers.InvoiceDemo
{
    public class SeedController : Controller
    {
        private readonly ApplicationDbContext _db;

        public SeedController(ApplicationDbContext context)
        {
            _db = context;
        }
        
        public IActionResult Index()
        {
            return View();
        }

        [HttpGet("seed")]
        public async Task<IActionResult> Demo()
        {
            if (await _db.Customers.AnyAsync())
                return Ok("Already seeded.");

            var customer = new Customer { Name = "Acme Ltd" };

            var p1 = new Product { Name = "Keyboard", UnitPrice = 49.99m };
            var p2 = new Product { Name = "Mouse", UnitPrice = 19.99m };

            var invoice = new Invoice
            {
                Customer = customer,
                InvoiceDate = DateTime.UtcNow,
                Lines =
            {
                new InvoiceLine { Product = p1, Quantity = 2, UnitPrice = p1.UnitPrice },
                new InvoiceLine { Product = p2, Quantity = 1, UnitPrice = p2.UnitPrice }
            }
            };

            _db.Invoices.Add(invoice);
            await _db.SaveChangesAsync();

            return Ok("Seeded 1 customer, 2 products, 1 invoice, 2 lines.");
        }
        public async Task<IActionResult> GetInvoices()
        {
            var data = await _db.Invoices
                .Include(i => i.Customer)
                .Include(i => i.Lines)
                    .ThenInclude(l => l.Product)
                .Select(i => new
                {
                    i.Id,
                    i.InvoiceDate,
                    Customer = i.Customer!.Name,
                    Total = i.Lines.Sum(l => l.Quantity * l.UnitPrice),
                    Lines = i.Lines.Select(l => new
                    {
                        Product = l.Product!.Name,
                        l.Quantity,
                        l.UnitPrice,
                        LineTotal = l.Quantity * l.UnitPrice
                    })
                })
                .ToListAsync();

            return Ok(data);
        }
    }
}
