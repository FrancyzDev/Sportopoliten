namespace Sportopoliten.DAL.Entities
{
    public class ProductVariantImages
    {
        public int Id { get; set; }
        public int ProductVariantId { get; set; }
        public ProductVariant ProductVariant { get; set; } = null!;
        public string? ImageUrl { get; set; }
        public int Priority { get; set; }
    }
}
