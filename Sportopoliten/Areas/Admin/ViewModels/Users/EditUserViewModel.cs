using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.Areas.Admin.ViewModels.Users
{
    public class EditUserViewModel
    {
        public int Id { get; set; }

        [Required(ErrorMessage = "ФИО обязательно")]
        [StringLength(100, MinimumLength = 2, ErrorMessage = "ФИО должно содержать от 2 до 100 символов")]
        [Display(Name = "ФИО")]
        public string FullName { get; set; }

        [Required(ErrorMessage = "Телефон обязателен")]
        [Phone(ErrorMessage = "Неверный формат телефона")]
        [Display(Name = "Телефон")]
        public string Phone { get; set; }

        [Required(ErrorMessage = "Логин обязателен")]
        [StringLength(50, MinimumLength = 3, ErrorMessage = "Логин должен содержать от 3 до 50 символов")]
        [Display(Name = "Логин")]
        public string Login { get; set; }

        [Required(ErrorMessage = "Email обязателен")]
        [EmailAddress(ErrorMessage = "Неверный формат Email")]
        [Display(Name = "Email")]
        public string Email { get; set; }

        [Display(Name = "Администратор")]
        public bool IsAdmin { get; set; }

        [StringLength(100, MinimumLength = 6, ErrorMessage = "Пароль должен содержать минимум 6 символов")]
        [DataType(DataType.Password)]
        [Display(Name = "Новый пароль (оставьте пустым, если не хотите менять)")]
        public string? NewPassword { get; set; }

        [DataType(DataType.Password)]
        [Compare("NewPassword", ErrorMessage = "Пароли не совпадают")]
        [Display(Name = "Подтверждение нового пароля")]
        public string? ConfirmNewPassword { get; set; }
    }
}