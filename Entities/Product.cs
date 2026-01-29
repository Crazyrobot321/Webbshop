using System;
using System.Collections.Generic;
using System.Text;

namespace Webbshop.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public required string Name { get; set; }
        public string? Description { get; set; }
        public decimal? Price { get; set; }
        public bool? IsSelected { get; set; }

        public int SupplierId { get; set; }
        public int CategoryId { get; set; }
        public int? Quantity { get; set; }

        public Supplier? Supplier { get; set; }
        public Category? Category { get; set; }

    }
}
