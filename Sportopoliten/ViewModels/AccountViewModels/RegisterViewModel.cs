using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.AccountViewModels;

public class RegisterViewModel
{
    [Required(ErrorMessage = "Введите полное имя")]
    [Display(Name = "Полное имя")]
    [MaxLength(100)]
    public string FullName { get; set; }

    [Required(ErrorMessage = "Введите email")]
    [EmailAddress(ErrorMessage = "Неверный формат email")]
    [Display(Name = "Email")]
    public string? Email { get; set; }

    [Required(ErrorMessage = "Введите логин")]
    [Display(Name = "Логин")]
    [MinLength(3, ErrorMessage = "Логин должен быть не менее 3 символов")]
    [MaxLength(50)]
    public string Login { get; set; }

    [Phone(ErrorMessage = "Неверный формат телефона")]
    [Display(Name = "Телефон")]
    public string? Phone { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    [MinLength(6, ErrorMessage = "Пароль должен быть не менее 6 символов")]
    public string Password { get; set; }

    [Required(ErrorMessage = "Подтвердите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Подтверждение пароля")]
    [Compare("Password", ErrorMessage = "Пароли не совпадают")]
    public string ConfirmPassword { get; set; }
}