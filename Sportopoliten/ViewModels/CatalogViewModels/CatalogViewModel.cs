namespace Sportopoliten.ViewModels.CatalogViewModels
{
    public class CatalogViewModel
    {
        public List<CatalogItemViewModel> Items { get; set; } = new();
        public int CurrentPage { get; set; }
        public int TotalPages { get; set; }
        public int TotalItems { get; set; }
        public int PageSize { get; set; }

        public string? SearchTerm { get; set; }
        public int? SelectedCategoryId { get; set; }
        public decimal? MinPrice { get; set; }
        public decimal? MaxPrice { get; set; }
        public string? SortBy { get; set; }
        public List<CategoryFilterViewModel> Categories { get; set; } = new();
        public decimal PriceRangeMin { get; set; }
        public decimal PriceRangeMax { get; set; }
        public bool HasPreviousPage => CurrentPage > 1;
        public bool HasNextPage => CurrentPage < TotalPages;
    }
}
