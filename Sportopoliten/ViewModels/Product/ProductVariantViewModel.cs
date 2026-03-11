using Microsoft.AspNetCore.Http;
using System.Collections.Generic;

namespace Sportopoliten.Web.ViewModels.Product
{
    public class ProductVariantViewModel
    {
        public string Color { get; set; }

        public string Size { get; set; }

        public decimal Price { get; set; }

        public int Stock { get; set; }

        public List<IFormFile> Images { get; set; } = new();
    }
}