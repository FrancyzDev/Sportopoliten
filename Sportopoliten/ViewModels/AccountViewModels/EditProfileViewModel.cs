using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.AccountViewModels
{
    public class EditProfileViewModel
    {
        [Required(ErrorMessage = "Введите ваше имя")]
        [Display(Name = "ФИО")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "Имя должно содержать от 2 до 100 символов")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Введите email")]
        [EmailAddress(ErrorMessage = "Неверный формат email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        [RegularExpression(@"^\+?3?8?(0[5-9][0-9]\d{7})$", ErrorMessage = "Неверный формат украинского номера телефона")]
        public string Phone { get; set; }
    }
}