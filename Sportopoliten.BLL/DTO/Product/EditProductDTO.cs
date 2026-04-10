using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.BLL.DTO.Product
{
    public class EditProductDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public string? CategoryName { get; set; }
        public List<string> ImageUrls { get; set; } = new();
    }
}