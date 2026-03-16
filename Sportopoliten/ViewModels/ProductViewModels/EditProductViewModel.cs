using Microsoft.AspNetCore.Http;
using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.ProductViewModels
{
    public class EditProductViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "Название товара обязательно")]
        [StringLength(200, MinimumLength = 3, ErrorMessage = "Название должно быть от 3 до 200 символов")]
        public string Title { get; set; } = string.Empty;

        [StringLength(1000, ErrorMessage = "Описание не должно превышать 1000 символов")]
        public string? Description { get; set; }

        [Required(ErrorMessage = "Цена обязательна")]
        [Range(0.01, double.MaxValue, ErrorMessage = "Цена должна быть больше 0")]
        public decimal Price { get; set; }

        [Required(ErrorMessage = "Категория обязательна")]
        public int CategoryId { get; set; }
        public List<IFormFile>? NewImages { get; set; }
        public List<string>? ExistingImages { get; set; }
        public List<string>? ImagesToDelete { get; set; }
    }
}