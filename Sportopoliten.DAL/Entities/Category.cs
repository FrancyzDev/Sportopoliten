namespace Sportopoliten.DAL.Entities
{
    public class Category
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public List<Product> Products { get; set; } = new List<Product>();
    }
}
