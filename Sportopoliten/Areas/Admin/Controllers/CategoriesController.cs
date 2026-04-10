using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Sportopoliten.Areas.Admin.ViewModels.Categories;
using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.BLL.Interfaces;

namespace Sportopoliten.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class CategoriesController : Controller
    {
        private readonly ICategoryService _categoryService;

        public CategoriesController(ICategoryService categoryService)
        {
            _categoryService = categoryService;
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
                var dto = new CreateCategoryDTO
                {
                    Title = model.Title,
                    ImageUrl = model.ImageUrl // Просто передаем URL из модели
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
                ImageUrl = category.ImageUrl // Теперь просто строка с URL
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
                // Создаем DTO для обновления
                var dto = new UpdateCategoryDTO
                {
                    Title = model.Title,
                    ImageUrl = model.ImageUrl // Просто передаем URL из модели
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
                // Удаляем категорию (сервис сам должен удалить изображение, если нужно)
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