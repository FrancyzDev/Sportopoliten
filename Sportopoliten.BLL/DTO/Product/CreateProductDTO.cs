namespace Sportopoliten.BLL.DTO.Product
{
    public class CreateProductDTO
    {
        public string Title { get; set; } = string.Empty;
        public string? Description { get; set; }
        public decimal Price { get; set; }
        public int CategoryId { get; set; }
        public List<string> ProductImages { get; set; } = new();
    }
}