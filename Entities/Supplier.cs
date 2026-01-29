using System;
using System.Collections.Generic;
using System.Text;

namespace Webbshop.Entities
{
    public class Supplier
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? ContactEmail { get; set; }
        public string? ContactPhone { get; set; }
        public required string Street { get; set; }
        public required string City { get; set; }
        public required string Country { get; set; }
        public ICollection<Product> ProductsSupplied { get; set; } = new List<Product>();
    }
}
