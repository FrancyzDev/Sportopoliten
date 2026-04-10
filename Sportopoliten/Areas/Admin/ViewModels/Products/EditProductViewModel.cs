using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.Areas.Admin.ViewModels.Products
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        [Display(Name = "Название товара")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        [Display(Name = "Описание")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, 999999.99, ErrorMessage = "Цена должна быть от 0.01 до 999999.99")]
        [Display(Name = "Цена")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Существующие URL изображений")]
        public List<string> ImageUrls { get; set; } = new();

        [Display(Name = "Новые URL изображений")]
        public List<string>? NewImageUrls { get; set; }

        [Display(Name = "URL изображений для удаления")]
        public List<string>? ImagesToDelete { get; set; }
    }
}