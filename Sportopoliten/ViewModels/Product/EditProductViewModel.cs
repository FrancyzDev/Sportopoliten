using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.Product
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

        [Required(ErrorMessage = "Количество на складе обязательно")]
        [Range(0, 999999, ErrorMessage = "Количество должно быть от 0 до 999999")]
        [Display(Name = "Количество на складе")]
        public int Stock { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        [Display(Name = "Категория")]
        public int CategoryId { get; set; }

        [Display(Name = "Новые изображения")]
        public List<IFormFile>? NewImages { get; set; }

        [Display(Name = "Существующие изображения")]
        public List<string>? ExistingImages { get; set; }

        [Display(Name = "Изображения для удаления")]
        public List<int>? ImagesToDelete { get; set; }
    }
}