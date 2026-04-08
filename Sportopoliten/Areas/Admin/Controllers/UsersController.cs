using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.Areas.Admin.ViewModels.Users;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using System.Security.Cryptography;
using System.Text;

namespace Sportopoliten.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly IUserService _userService;
        private readonly IUnitOfWork _unitOfWork;

        public UsersController(IUserService userService, IUnitOfWork unitOfWork)
        {
            _userService = userService;
            _unitOfWork = unitOfWork;
        }

        // GET: /Admin/Users
        public async Task<IActionResult> Index()
        {
            var users = await _userService.GetAllUsersAsync();
            return View(users);
        }

        // GET: /Admin/Users/Create
        public IActionResult Create()
        {
            return View(new CreateUserViewModel());
        }

        // POST: /Admin/Users/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateUserViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                if (!await _userService.IsEmailUniqueAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email уже используется");
                    return View(model);
                }

                if (!await _userService.IsLoginUniqueAsync(model.Login))
                {
                    ModelState.AddModelError("Login", "Логин уже используется");
                    return View(model);
                }

                string salt = GenerateSalt();
                string hashedPassword = HashPassword(model.Password, salt);

                var user = new User
                {
                    FullName = model.FullName,
                    Email = model.Email,
                    Login = model.Login,
                    Phone = model.Phone,
                    PasswordHash = hashedPassword,
                    Salt = salt,
                    IsAdmin = model.IsAdmin,
                    CreatedAt = DateTime.UtcNow,
                    Cart = new Cart()
                };

                await _unitOfWork.Users.AddAsync(user);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Пользователь успешно создан";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании пользователя: {ex.Message}");
                return View(model);
            }
        }

        // GET: /Admin/Users/Edit/5
        public async Task<IActionResult> Edit(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден";
                return RedirectToAction(nameof(Index));
            }

            var model = new EditUserViewModel
            {
                Id = user.Id,
                FullName = user.FullName,
                Email = user.Email,
                Login = user.Login,
                Phone = user.Phone,
                IsAdmin = user.IsAdmin
            };

            return View(model);
        }

        // POST: /Admin/Users/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditUserViewModel model)
        {
            if (id != model.Id)
            {
                return BadRequest();
            }

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    TempData["ErrorMessage"] = "Пользователь не найден";
                    return RedirectToAction(nameof(Index));
                }

                if (user.Email != model.Email && !await _userService.IsEmailUniqueAsync(model.Email))
                {
                    ModelState.AddModelError("Email", "Email уже используется");
                    return View(model);
                }

                if (user.Login != model.Login && !await _userService.IsLoginUniqueAsync(model.Login))
                {
                    ModelState.AddModelError("Login", "Логин уже используется");
                    return View(model);
                }

                user.FullName = model.FullName;
                user.Email = model.Email;
                user.Login = model.Login;
                user.Phone = model.Phone;
                user.IsAdmin = model.IsAdmin;

                if (!string.IsNullOrEmpty(model.NewPassword))
                {
                    if (model.NewPassword.Length < 6)
                    {
                        ModelState.AddModelError("NewPassword", "Пароль должен содержать минимум 6 символов");
                        return View(model);
                    }

                    user.Salt = GenerateSalt();
                    user.PasswordHash = HashPassword(model.NewPassword, user.Salt);
                }

                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                TempData["SuccessMessage"] = "Пользователь успешно обновлен";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении пользователя: {ex.Message}");
                return View(model);
            }
        }

        // POST: /Admin/Users/Delete/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Delete([FromForm] int id)
        {
            try
            {
                await _userService.DeleteUserAsync(id);
                TempData["SuccessMessage"] = "Пользователь успешно удален";
            }
            catch (KeyNotFoundException)
            {
                TempData["ErrorMessage"] = "Пользователь не найден";
            }
            catch (Exception ex)
            {
                TempData["ErrorMessage"] = $"Ошибка при удалении: {ex.Message}";
            }

            return RedirectToAction(nameof(Index));
        }

        // GET: /Admin/Users/Details/5
        public async Task<IActionResult> Details(int id)
        {
            var user = await _userService.GetUserByIdAsync(id);
            if (user == null)
            {
                TempData["ErrorMessage"] = "Пользователь не найден";
                return RedirectToAction(nameof(Index));
            }

            return View(user);
        }

        // POST: /Admin/Users/ToggleAdmin/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> ToggleAdmin(int id)
        {
            try
            {
                var user = await _unitOfWork.Users.GetByIdAsync(id);
                if (user == null)
                {
                    return Json(new { success = false, message = "Пользователь не найден" });
                }

                user.IsAdmin = !user.IsAdmin;
                _unitOfWork.Users.Update(user);
                await _unitOfWork.SaveChangesAsync();

                return Json(new { success = true, isAdmin = user.IsAdmin });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = ex.Message });
            }
        }

        private string GenerateSalt()
        {
            byte[] saltBytes = RandomNumberGenerator.GetBytes(16);
            return Convert.ToHexString(saltBytes);
        }

        private string HashPassword(string password, string salt)
        {
            byte[] bytes = Encoding.Unicode.GetBytes(salt + password);
            byte[] byteHash = MD5.HashData(bytes);
            return Convert.ToHexString(byteHash);
        }
    }
}
