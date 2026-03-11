using System;
using System.Collections.Generic;
using System.Text;

namespace Sportopoliten.BLL.DTO
{
    public class ProductVariantDTO
    {
        public string Color { get; set; }
        public string Size { get; set; }
        public decimal Price { get; set; }
        public int Stock { get; set; }
        public List<string> Images { get; set; }
        public int Id { get; set; }
        public int ProductId { get; set; }
    }
}
