using Microsoft.AspNetCore.Http;
using Sportopoliten.ViewModels.Product;
using System.Collections.Generic;

namespace Sportopoliten.ViewModels.Product
{
    public class CreateProductViewModel
    {
        public string Title { get; set; }

        public string Description { get; set; }

        public int CategoryId { get; set; }

        public List<ProductVariantViewModel> Variants { get; set; } = new();
    }
}