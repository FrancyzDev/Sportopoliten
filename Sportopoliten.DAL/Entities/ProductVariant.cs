using Sportopoliten.DAL.Entities;
using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.DAL.Entities
{
    public class ProductVariant
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string? Color { get; set; }
        public string? Size { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public ICollection<ProductVariantImages> Images { get; set; } = [];
    }
}
