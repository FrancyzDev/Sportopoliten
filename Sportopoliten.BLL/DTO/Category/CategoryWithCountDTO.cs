namespace Sportopoliten.BLL.DTO.Category
{
    public class CategoryWithCountDTO
    {
        public int Id { get; set; }
        public string Title { get; set; }
        public string? ImageUrl { get; set; }
        public int ProductCount { get; set; }
    }
}