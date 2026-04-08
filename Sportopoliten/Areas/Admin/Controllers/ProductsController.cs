using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Sportopoliten.Areas.Admin.ViewModels.Products;
using Sportopoliten.BLL.DTO.Product;
using Sportopoliten.BLL.Interfaces;

namespace Sportopoliten.Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ProductsController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly IWebHostEnvironment _env;

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService,
            IWebHostEnvironment env)
        {
            _productService = productService;
            _categoryService = categoryService;
            _env = env;
        }

        // GET: AdminProduct/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateProductViewModel();

            // Получаем категории и преобразуем в SelectList
            var categories = await _categoryService.GetAllCategoriesAsync();
            Console.WriteLine(categories);
            ViewBag.Categories = new SelectList(categories, "Id", "Title");

            return View(model);
        }

        // GET: AdminProduct/Details/5
        [HttpGet]
        public async Task<IActionResult> Details(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
            {
                return NotFound();
            }

            return View(product);
        }

        // POST: AdminProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            try
            {
                // Создаем директорию для изображений, если её нет
                string uploadPath = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Обрабатываем изображения
                var imageUrls = new List<string>();

                if (model.Images != null && model.Images.Any())
                {
                    foreach (var image in model.Images)
                    {
                        if (image != null && image.Length > 0)
                        {
                            // Проверяем расширение файла
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                ModelState.AddModelError("", $"Файл {image.FileName} имеет неподдерживаемый формат. Разрешены: {string.Join(", ", allowedExtensions)}");
                                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                                return View(model);
                            }

                            // Генерируем уникальное имя файла
                            var fileName = $"{Guid.NewGuid()}{fileExtension}";
                            var relativePath = Path.Combine("images", "products", fileName);
                            var filePath = Path.Combine(_env.WebRootPath, relativePath);

                            // Сохраняем файл
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Добавляем относительный путь к изображению
                            imageUrls.Add("/" + relativePath.Replace("\\", "/"));
                        }
                    }
                }

                // Создаем DTO
                var dto = new CreateProductDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Price = model.Price,
                    ProductImages = imageUrls
                };

                await _productService.CreateProductAsync(dto);

                TempData["Success"] = "Товар успешно создан!";
                return RedirectToAction("Index");
            }
            catch (UnauthorizedAccessException)
            {
                ModelState.AddModelError("", "Нет прав доступа для сохранения файлов. Проверьте права на папку wwwroot/images/products");
            }
            catch (DirectoryNotFoundException)
            {
                ModelState.AddModelError("", "Директория для сохранения не найдена");
            }
            catch (IOException ex)
            {
                ModelState.AddModelError("", $"Ошибка ввода-вывода при сохранении файла: {ex.Message}");
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Произошла ошибка при создании товара: {ex.Message}");
            }

            ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
            return View(model);
        }

        // GET: AdminProduct/Index
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
        }

        // GET: AdminProduct/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            var model = new EditProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                CategoryId = product.CategoryId ?? 0,
                Price = product.Price,
                ExistingImages = product.ProductImages?.Select(x => x.ImageUrl).ToList() ?? new()
            };

            // Получаем категории и создаем SelectList
            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Title");  // Важно!

            return View(model);
        }

        // POST: AdminProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            if (id != model.Id)
                return NotFound();

            if (!ModelState.IsValid)
            {
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }

            try
            {
                // Создаем директорию для изображений, если её нет
                string uploadPath = Path.Combine(_env.WebRootPath, "images", "products");
                if (!Directory.Exists(uploadPath))
                {
                    Directory.CreateDirectory(uploadPath);
                }

                // Обрабатываем новые изображения
                var newImageUrls = new List<string>();

                if (model.NewImages != null && model.NewImages.Any())
                {
                    foreach (var image in model.NewImages)
                    {
                        if (image != null && image.Length > 0)
                        {
                            // Проверяем расширение файла
                            var allowedExtensions = new[] { ".jpg", ".jpeg", ".png", ".gif", ".webp" };
                            var fileExtension = Path.GetExtension(image.FileName).ToLowerInvariant();

                            if (!allowedExtensions.Contains(fileExtension))
                            {
                                ModelState.AddModelError("", $"Файл {image.FileName} имеет неподдерживаемый формат.");
                                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                                return View(model);
                            }

                            // Генерируем уникальное имя файла
                            var fileName = $"{Guid.NewGuid()}{fileExtension}";
                            var relativePath = Path.Combine("images", "products", fileName);
                            var filePath = Path.Combine(_env.WebRootPath, relativePath);

                            // Сохраняем файл
                            using (var stream = new FileStream(filePath, FileMode.Create))
                            {
                                await image.CopyToAsync(stream);
                            }

                            // Добавляем относительный путь к изображению
                            newImageUrls.Add("/" + relativePath.Replace("\\", "/"));
                        }
                    }
                }

                // Определяем, какие изображения нужно оставить
                var imagesToKeep = new List<string>();
                if (model.ExistingImages != null)
                {
                    if (model.ImagesToDelete != null && model.ImagesToDelete.Any())
                    {
                        // Оставляем только те, которые не отмечены для удаления
                        imagesToKeep = model.ExistingImages
                            .Where(img => !model.ImagesToDelete.Contains(img))
                            .ToList();

                        // Удаляем файлы с диска
                        foreach (var imageToDelete in model.ImagesToDelete)
                        {
                            var filePath = Path.Combine(_env.WebRootPath, imageToDelete.TrimStart('/').Replace("/", "\\"));
                            if (System.IO.File.Exists(filePath))
                            {
                                System.IO.File.Delete(filePath);
                            }
                        }
                    }
                    else
                    {
                        imagesToKeep = model.ExistingImages;
                    }
                }

                // Объединяем старые (неудаленные) и новые изображения
                var allImages = imagesToKeep.Concat(newImageUrls).ToList();

                // Создаем DTO
                var dto = new UpdateProductDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Price = model.Price,
                    ProductImages = allImages
                };

                await _productService.UpdateProductAsync(id, dto);

                TempData["Success"] = "Товар успешно обновлен!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Ошибка при обновлении товара: {ex.Message}");
                ViewBag.Categories = await _categoryService.GetAllCategoriesAsync();
                return View(model);
            }
        }

        // GET: AdminProduct/Delete/5
        [HttpGet]
        public async Task<IActionResult> Delete(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);
            if (product == null)
                return NotFound();

            return View(product);
        }

        // POST: AdminProduct/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            try
            {
                await _productService.DeleteProductAsync(id);
                TempData["Success"] = "Товар успешно удален!";
            }
            catch (KeyNotFoundException)
            {
                return NotFound();
            }

            return RedirectToAction(nameof(Index));
        }
    }
}