namespace Sportopoliten.DAL.Entities
{
    public class Product
    {
        public int Id { get; set; }
        public string? Title { get; set; }
        public string? Description { get; set; }
        public ICollection<ProductVariant> Variants { get; set; } = [];
    }
}
