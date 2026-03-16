namespace Sportopoliten.DAL.Entities
{
    public class ProductImage
    {
        public int Id { get; set; }
        public int ProductId { get; set; }
        public Product Product { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Priority { get; set; }
    }
}
