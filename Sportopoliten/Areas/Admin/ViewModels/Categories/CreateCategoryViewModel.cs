using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.Areas.Admin.ViewModels.Categories
{
    public class CreateCategoryViewModel
    {
        [Required(ErrorMessage = "Название категории обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Название должно быть от 2 до 100 символов")]
        [Display(Name = "Название категории")]
        public string Title { get; set; } = string.Empty;

        [Display(Name = "Изображение категории")]
        public IFormFile? Image { get; set; }
    }
}