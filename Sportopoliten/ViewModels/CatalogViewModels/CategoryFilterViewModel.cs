namespace Sportopoliten.ViewModels.CatalogViewModels
{
    public class CategoryFilterViewModel
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public int ProductCount { get; set; }
        public bool IsSelected { get; set; }
    }
}
