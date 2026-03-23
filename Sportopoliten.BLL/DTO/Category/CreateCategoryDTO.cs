namespace Sportopoliten.BLL.DTO.Category
{
    public class CreateCategoryDTO
    {
        public int Id { get; set; }
        public string Title { get; set; } = string.Empty;
        public string? ImageUrl { get; set; }
    }
}
