using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.CategoryViewModels
{
    public class EditCategoryViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов")]
        [Display(Name = "Название категории")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Новое изображение")]
        public IFormFile? Image { get; set; }
        public string? CurrentImageUrl { get; set; }
    }
}