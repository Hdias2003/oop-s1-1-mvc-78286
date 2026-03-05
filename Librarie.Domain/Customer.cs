using System;
using System.Collections.Generic;
using System.Text;

namespace Librarie.Domain
{
    public class Customer
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";

        public List<Invoice> Invoices { get; set; } = new();
    }
}
