using Sportopoliten.ViewModels.CatalogViewModels;

namespace Sportopoliten.Areas.Admin.ViewModels.Products
{
    public class DetailProductViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string CategoryName { get; set; } = string.Empty;
        public List<string> Images { get; set; } = new();
        public List<string> AvailableSizes { get; set; } = new() { "XS", "S", "M", "L", "XL", "XXL" };
    }
}