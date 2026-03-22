using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.ViewModels.CategoryViewModels;

namespace Sportopoliten.Controllers
{
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;

        public CategoriesController(
            ICategoryService categoryService,
            IWebHostEnvironment env)
        {
            _categoryService = categoryService;
            _env = env;
        }

        // GET: AdminCategory/Index
        public async Task<IActionResult> Index()
        {
            var categories = await _categoryService.GetAllCategoriesAsync();

            var categoryDTOs = categories.Select(c => new CategoryDTO
            {
                Id = c.Id,
                Title = c.Title,
                ImageUrl = c.ImageUrl,
                ProductsCount = c.Products?.Count ?? 0
            });
            return View(categoryDTOs);
        }

        // GET: AdminCategory/Create
        [HttpGet]
        public IActionResult Create()
        {
            return View(new CreateCategoryViewModel());
        }

        // POST: AdminCategory/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateCategoryViewModel model)
        {
            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string? imageUrl = null;  // Здесь будем хранить URL сохраненного изображения

                // Проверяем, загружен ли файл
                if (model.Image != null && model.Image.Length > 0)
                {
                    // Проверяем расширение файла
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                    var fileExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", $"Неподдерживаемый формат. Разрешены: {string.Join(", ", allowedExtensions)}");
                        return View(model);
                    }

                    // Создаем директорию для изображений категорий
                    string uploadPath = Path.Combine(_env.WebRootPath, "images", "categories");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Генерируем уникальное имя файла
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string relativePath = Path.Combine("images", "categories", fileName);
                    string filePath = Path.Combine(_env.WebRootPath, relativePath);

                    // Сохраняем файл
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    imageUrl = "/" + relativePath.Replace("\\", "/");
                }

                var dto = new CreateCategoryDTO
                {
                    Title = model.Title,
                    ImageUrl = imageUrl
                };

                await _categoryService.CreateCategoryAsync(dto);

                TempData["Success"] = "Категория успешно создана!";
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при создании категории: {ex.Message}");
                return View(model);
            }
        }

        // GET: AdminCategory/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            var model = new EditCategoryViewModel
            {
                Id = category.Id,
                Title = category.Title,
                CurrentImageUrl = category.ImageUrl
            };

            return View(model);
        }

        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditCategoryViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                return View(model);
            }

            try
            {
                string? imageUrl = model.CurrentImageUrl; // По умолчанию оставляем текущее

                // Если загружен новый файл
                if (model.Image != null && model.Image.Length > 0)
                {
                    // Проверяем расширение файла
                    var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp", ".svg" };
                    var fileExtension = Path.GetExtension(model.Image.FileName).ToLowerInvariant();

                    if (!allowedExtensions.Contains(fileExtension))
                    {
                        ModelState.AddModelError("Image", $"Неподдерживаемый формат. Разрешены: {string.Join(", ", allowedExtensions)}");
                        return View(model);
                    }

                    // Создаем директорию для изображений категорий
                    string uploadPath = Path.Combine(_env.WebRootPath, "images", "categories");
                    if (!Directory.Exists(uploadPath))
                    {
                        Directory.CreateDirectory(uploadPath);
                    }

                    // Генерируем уникальное имя файла
                    string fileName = $"{Guid.NewGuid()}{fileExtension}";
                    string relativePath = Path.Combine("images", "categories", fileName);
                    string filePath = Path.Combine(_env.WebRootPath, relativePath);

                    // Сохраняем файл
                    using (var stream = new FileStream(filePath, FileMode.Create))
                    {
                        await model.Image.CopyToAsync(stream);
                    }

                    imageUrl = "/" + relativePath.Replace("\\", "/");

                    // Удаляем старое изображение с диска, если оно было
                    if (!string.IsNullOrEmpty(model.CurrentImageUrl) && model.CurrentImageUrl.StartsWith("/images/categories/"))
                    {
                        var oldFilePath = Path.Combine(_env.WebRootPath, model.CurrentImageUrl.TrimStart('/').Replace("/", "\\"));
                        if (System.IO.File.Exists(oldFilePath))
                        {
                            System.IO.File.Delete(oldFilePath);
                        }
                    }
                }

                // Создаем DTO для обновления
                var dto = new UpdateCategoryDTO
                {
                    Title = model.Title,
                    ImageUrl = imageUrl
                };

                // Обновляем категорию в базе данных
                await _categoryService.UpdateCategoryAsync(id, dto);

                TempData["Success"] = "Категория успешно обновлена!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении категории: {ex.Message}");
                return View(model);
            }
        }

        // GET: AdminCategory/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);
            if (category == null)
                return NotFound();

            // Преобразуем Category в CategoryDTO
            var categoryDTO = new CategoryDTO
            {
                Id = category.Id,
                Title = category.Title,
                ImageUrl = category.ImageUrl,
                ProductsCount = category.Products?.Count ?? 0
            };

            return View(categoryDTO);
        }

        // POST: AdminCategory/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                var category = await _categoryService.GetCategoryByIdAsync(id);

                // Удаляем изображение с диска
                if (category != null && !string.IsNullOrEmpty(category.ImageUrl) && category.ImageUrl.StartsWith("/images/categories/"))
                {
                    var filePath = Path.Combine(_env.WebRootPath, category.ImageUrl.TrimStart('/').Replace("/", "\\"));
                    if (System.IO.File.Exists(filePath))
                    {
                        System.IO.File.Delete(filePath);
                    }
                }

                await _categoryService.DeleteCategoryAsync(id);
                TempData["Success"] = "Категория успешно удалена!";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (InvalidOperationException ex)
            {
                TempData["Error"] = ex.Message;
            }

            return RedirectToAction(nameof(Index));
        }
    }
}