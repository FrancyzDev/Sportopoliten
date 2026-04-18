using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO.User;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.BLL.Services;
using Sportopoliten.ViewModels.AccountViewModels;
using System.Security.Claims;

namespace Sportopoliten.Controllers;

public class AccountController : Controller
{
    private readonly IUserService _userService;
    private readonly IOrderService _orderService;

    public AccountController(IUserService userService, IOrderService orderService)
    {
        _userService = userService;
        _orderService = orderService;
    }

    // GET: /Account/Index
    [Authorize]
    [HttpGet]
    public async Task<IActionResult> Index()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        var userDto = await _userService.GetUserByIdAsync(int.Parse(userId));
        if (userDto == null)
            return RedirectToAction("Login");

        var model = new ProfileViewModel
        {
            FullName = userDto.FullName,
            Email = userDto.Email,
            Login = userDto.Login,
            PhoneNumber = userDto.Phone,
            RegisteredAt = userDto.CreatedAt
        };

        return View(model);
    }

    [HttpGet]
    public async Task<IActionResult> Orders()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login", "Account");

        var orders = await _orderService.GetOrdersByUserIdAsync(int.Parse(userId));
        return View(orders);
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

    [Authorize]
    [HttpGet]
    public async Task<IActionResult> EditProfile()
    {
        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        var user = await _userService.GetUserByIdAsync(int.Parse(userId));
        if (user == null)
            return RedirectToAction("Login");

        var model = new EditProfileViewModel
        {
            FullName = user.FullName,
            Email = user.Email,
            Phone = user.Phone
        };

        return View(model);
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditProfile(EditProfileViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        try
        {
            var editProfileDto = new EditProfileDTO
            {
                FullName = model.FullName,
                Email = model.Email,
                Phone = model.Phone,
            };

            await _userService.UpdateUserProfileAsync(int.Parse(userId), editProfileDto);

            await UpdateUserClaimsAsync(int.Parse(userId));

            TempData["SuccessMessage"] = "Профиль успешно обновлен!";
            return RedirectToAction(nameof(Index));
        }
        catch (Exception ex)
        {
            ModelState.AddModelError(string.Empty, ex.Message);
            return View(model);
        }
    }

    [Authorize]
    [HttpGet]
    public IActionResult ChangePassword()
    {
        return View();
    }

    [Authorize]
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> ChangePassword(ChangePasswordViewModel model)
    {
        if (!ModelState.IsValid)
            return View(model);

        var userId = User.FindFirstValue(ClaimTypes.NameIdentifier);
        if (string.IsNullOrEmpty(userId))
            return RedirectToAction("Login");

        try
        {
            await _userService.ChangePasswordAsync(int.Parse(userId), model.CurrentPassword, model.NewPassword);
            TempData["SuccessMessage"] = "Пароль успешно изменен!";
            return RedirectToAction(nameof(Index));
        }
        catch (InvalidOperationException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
            return View(model);
        }
        catch (KeyNotFoundException ex)
        {
            TempData["ErrorMessage"] = ex.Message;
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

    private async Task UpdateUserClaimsAsync(int userId)
    {
        var user = await _userService.GetUserByIdAsync(userId);
        if (user == null) return;

        var identity = (ClaimsIdentity?)User.Identity;
        if (identity == null) return;

        var fullNameClaim = identity.FindFirst("FullName");
        if (fullNameClaim != null)
            identity.RemoveClaim(fullNameClaim);

        identity.AddClaim(new Claim("FullName", user.FullName));

        var emailClaim = identity.FindFirst(ClaimTypes.Email);
        if (emailClaim != null)
            identity.RemoveClaim(emailClaim);

        identity.AddClaim(new Claim(ClaimTypes.Email, user.Email));

        var phoneClaim = identity.FindFirst(ClaimTypes.MobilePhone);
        if (phoneClaim != null)
            identity.RemoveClaim(phoneClaim);

        if (!string.IsNullOrEmpty(user.Phone))
            identity.AddClaim(new Claim(ClaimTypes.MobilePhone, user.Phone));

        var principal = new ClaimsPrincipal(identity);
        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal,
            new AuthenticationProperties
            {
                IsPersistent = true,
                ExpiresUtc = DateTimeOffset.UtcNow.AddDays(30)
            });
    }
}
