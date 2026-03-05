using System;
using System.Collections.Generic;
using System.Text;

namespace Librarie.Domain
{


    public class Product
    {
        public int Id { get; set; }
        public string Name { get; set; } = "";
        public decimal UnitPrice { get; set; }

        public List<InvoiceLine> InvoiceLines { get; set; } = new();
    }
}
