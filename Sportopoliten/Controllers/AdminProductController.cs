using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.DTO;
using Sportopoliten.ViewModels.Product;
using Sportopoliten.BLL.Interfaces;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;

public class AdminProductController : Controller
{
    private readonly IProductService _productService;
    private readonly ICategoryService _categoryService;

    private readonly IWebHostEnvironment _env;

    public AdminProductController(IProductService productService, ICategoryService categoryService, IWebHostEnvironment env)
    {
        _productService = productService;
        _categoryService = categoryService;
        _env = env;
    }

    [HttpGet]
    async public Task<IActionResult> Create()
    {
        var model = new CreateProductViewModel
        {
            Variants = new List<ProductVariantViewModel>
        {
            new ProductVariantViewModel() // Один вариант по умолчанию
        }
        };

        ViewBag.Categories = await _categoryService.GetAllCategoriesAsync(); // Метод для получения категорий

        return View(model);
    }

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
            var dto = new CreateProductDTO
            {
                Title = model.Title,
                Description = model.Description,
                CategoryId = model.CategoryId,
                Variants = new List<ProductVariantDTO>()
            };

            // Создаем директорию для изображений, если её нет
            string uploadPath = Path.Combine(_env.WebRootPath, "images", "products");
            if (!Directory.Exists(uploadPath))
            {
                Directory.CreateDirectory(uploadPath);
            }

            foreach (var variant in model.Variants)
            {
                var imageUrls = new List<string>();

                // Проверяем, есть ли изображения
                if (variant.Images != null && variant.Images.Any())
                {
                    foreach (var image in variant.Images)
                    {
                        // Проверяем, что файл не пустой
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

                dto.Variants.Add(new ProductVariantDTO
                {
                    Color = variant.Color,
                    Size = variant.Size,
                    Price = variant.Price,
                    Stock = variant.Stock,
                    Images = imageUrls
                });
            }

            await _productService.CreateProductAsync(dto);

            TempData["Success"] = "Товар успешно создан!";
            return RedirectToAction("Index");
        }
        catch (UnauthorizedAccessException ex)
        {
            ModelState.AddModelError("", "Нет прав доступа для сохранения файлов. Проверьте права на папку wwwroot/images/products");
        }
        catch (DirectoryNotFoundException ex)
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
}