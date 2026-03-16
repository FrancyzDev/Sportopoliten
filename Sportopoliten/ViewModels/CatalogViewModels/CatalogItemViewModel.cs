namespace Sportopoliten.ViewModels.CatalogViewModels
{
    public class CatalogItemViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public string ImageUrl { get; set; } = "/images/no-image.jpg";
        public string? CategoryName { get; set; }
    }
}
