using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.DTO.Product
{
    public class ProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int? CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<ProductImage> ProductImages { get; set; } = new();
        public int ImagesCount => ProductImages?.Count ?? 0;
        public string? MainImageUrl => ProductImages?.FirstOrDefault().ImageUrl;
    }
}