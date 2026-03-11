using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.DAL.Entities
{
    
        public class Product
        {
            public int Id { get; set; }
            public string? Title { get; set; }
            public string? Description { get; set; }
            public ICollection<ProductVariant> Variants { get; set; } = [];

            public int CategoryId { get; set; }
            public Category Category { get; set; }
    }
}
