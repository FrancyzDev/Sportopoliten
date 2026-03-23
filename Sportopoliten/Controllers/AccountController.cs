using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO.User;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.ViewModels.AccountViewModels;
using System.Security.Claims;

namespace Sportopoliten.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;

    public AccountController(IUserService userService)
    {
        _userService = userService;
    }

    // GET: /Account/Login
    [HttpGet]
    public IActionResult Login(string? returnUrl = null)
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Catalog");

        return View(new LoginViewModel { ReturnUrl = returnUrl });
    }

    // POST: /Account/Login
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Login(LoginViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var loginDto = new LoginUserDTO
        {
            LoginOrEmail = model.LoginOrEmail,
            Password = model.Password,
            RememberMe = model.RememberMe
        };

        var result = await _userService.Login(loginDto);

        if (result == null)
        {
            ModelState.AddModelError(string.Empty, "Неверный логин или пароль");
            return View(model);
        }

        // Найдём пользователя для получения Id и роли
        var userDto = await _userService.GetUserByEmailAsync(model.LoginOrEmail)
                      ?? await _userService.GetUserByLoginAsync(model.LoginOrEmail);

        if (userDto == null)
        {
            ModelState.AddModelError(string.Empty, "Ошибка авторизации");
            return View(model);
        }

        var claims = new List<Claim>
        {
            new (ClaimTypes.NameIdentifier, userDto.Id.ToString()),
            new (ClaimTypes.Name, userDto.Login),
            new (ClaimTypes.Email, userDto.Email),
            new ("FullName", userDto.FullName),
            new ("CartId", userDto.CartId.ToString()),
        };

        if (userDto.IsAdmin)
            claims.Add(new Claim(ClaimTypes.Role, "Admin"));

        var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
        var principal = new ClaimsPrincipal(identity);

        var authProps = new AuthenticationProperties
        {
            IsPersistent = model.RememberMe,
            ExpiresUtc = model.RememberMe
                ? DateTimeOffset.UtcNow.AddDays(30)
                : DateTimeOffset.UtcNow.AddHours(2)
        };

        await HttpContext.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal, authProps);

        if (!string.IsNullOrEmpty(model.ReturnUrl) && Url.IsLocalUrl(model.ReturnUrl))
            return Redirect(model.ReturnUrl);

        return RedirectToAction("Index", "Catalog");
    }

    // GET: /Account/Register
    [HttpGet]
    public IActionResult Register()
    {
        if (User.Identity?.IsAuthenticated == true)
            return RedirectToAction("Index", "Catalog");

        return View();
    }

    // POST: /Account/Register
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Register(RegisterViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        try
        {
            var registerDto = new RegisterUserDTO
            {
                FullName = model.FullName,
                Email = model.Email,
                Login = model.Login,
                Phone = model.Phone ?? string.Empty,
                Password = model.Password
            };

            await _userService.Register(registerDto);

            TempData["SuccessMessage"] = "Регистрация прошла успешно! Войдите в аккаунт.";
            return RedirectToAction(nameof(Login));
        }
        catch (InvalidOperationException ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    // POST: /Account/Logout
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Logout()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
        return RedirectToAction("Index", "Catalog");
    }

    // GET: /Account/AccessDenied
    public IActionResult AccessDenied()
    {
        return View();
    }
}
