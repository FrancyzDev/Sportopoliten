using System.ComponentModel.DataAnnotations;

namespace Sportopoliten.ViewModels.AccountViewModels;

public class LoginViewModel
{
    [Required(ErrorMessage = "Введите логин или email")]
    [Display(Name = "Логин или Email")]
    public string LoginOrEmail { get; set; }

    [Required(ErrorMessage = "Введите пароль")]
    [DataType(DataType.Password)]
    [Display(Name = "Пароль")]
    public string Password { get; set; }

    [Display(Name = "Запомнить меня")]
    public bool RememberMe { get; set; }

    public string? ReturnUrl { get; set; }
}
