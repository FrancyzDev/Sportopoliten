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

        public ProductsController(
            IProductService productService,
            ICategoryService categoryService)
        {
            _productService = productService;
            _categoryService = categoryService;
        }

        // GET: AdminProduct/Index
        public async Task<IActionResult> Index()
        {
            var products = await _productService.GetAllProductsAsync();
            return View(products);
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

        // GET: AdminProduct/Create
        [HttpGet]
        public async Task<IActionResult> Create()
        {
            var model = new CreateProductViewModel();

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Title");

            return View(model);
        }

        // POST: AdminProduct/Create
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(CreateProductViewModel model)
        {
            if (!ModelState.IsValid)
            {
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Title");
                return View(model);
            }

            try
            {
                var imageUrls = model.ImageUrls?
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .ToList() ?? new List<string>();

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
                return RedirectToAction(nameof(Index));
            }
            catch (Exception ex)
            {
                ModelState.AddModelError("", $"Произошла ошибка при создании товара: {ex.Message}");
            }

            ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Title");
            return View(model);
        }

        // GET: AdminProduct/Edit/5
        [HttpGet]
        public async Task<IActionResult> Edit(int id)
        {
            var product = await _productService.GetProductForEditAsync(id);
            if (product == null)
                return NotFound();

            var model = new EditProductViewModel
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                CategoryId = product.CategoryId,
                Price = product.Price,
                ImageUrls = product.ImageUrls ?? new List<string>()
            };

            var categories = await _categoryService.GetAllCategoriesAsync();
            ViewBag.Categories = new SelectList(categories, "Id", "Title");

            return View(model);
        }

        // POST: AdminProduct/Edit/5
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, EditProductViewModel model)
        {
            Console.WriteLine("=== START EDIT POST ===");
            Console.WriteLine($"ID: {id}, Model ID: {model.Id}");
            Console.WriteLine($"Title: {model.Title}");
            Console.WriteLine($"Price: {model.Price}");
            Console.WriteLine($"CategoryId: {model.CategoryId}");
            Console.WriteLine($"ModelState.IsValid: {ModelState.IsValid}");

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState errors:");
                foreach (var key in ModelState.Keys)
                {
                    var errors = ModelState[key].Errors;
                    foreach (var error in errors)
                    {
                        Console.WriteLine($"  {key}: {error.ErrorMessage}");
                    }
                }
            }

            if (id != model.Id)
            {
                Console.WriteLine("ID mismatch!");
                return NotFound();
            }

            if (!ModelState.IsValid)
            {
                Console.WriteLine("ModelState is invalid, returning view");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Title");
                return View(model);
            }

            try
            {
                Console.WriteLine("Starting update process...");

                var imagesToKeep = new List<string>();

                if (model.ImageUrls != null && model.ImageUrls.Any())
                {
                    if (model.ImagesToDelete != null && model.ImagesToDelete.Any())
                    {
                        imagesToKeep = model.ImageUrls
                            .Where(img => !model.ImagesToDelete.Contains(img))
                            .ToList();
                    }
                    else
                    {
                        imagesToKeep = model.ImageUrls;
                    }
                }

                var newImageUrls = model.NewImageUrls?
                    .Where(url => !string.IsNullOrWhiteSpace(url))
                    .ToList() ?? new List<string>();

                var allImages = imagesToKeep.Concat(newImageUrls).ToList();

                Console.WriteLine($"Images to keep: {imagesToKeep.Count}");
                Console.WriteLine($"New images: {newImageUrls.Count}");
                Console.WriteLine($"Total images: {allImages.Count}");

                var dto = new UpdateProductDTO
                {
                    Title = model.Title,
                    Description = model.Description,
                    CategoryId = model.CategoryId,
                    Price = model.Price,
                    ProductImages = allImages
                };

                Console.WriteLine($"Calling UpdateProductAsync with price: {dto.Price}");
                await _productService.UpdateProductAsync(id, dto);
                Console.WriteLine("Update completed successfully");

                TempData["Success"] = "Товар успешно обновлен!";
                return RedirectToAction(nameof(Index));
            }
            catch (KeyNotFoundException ex)
            {
                Console.WriteLine($"KeyNotFoundException: {ex.Message}");
                return NotFound();
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Exception: {ex.Message}");
                Console.WriteLine($"Stack trace: {ex.StackTrace}");
                ModelState.AddModelError("", $"Ошибка при обновлении товара: {ex.Message}");
                ViewBag.Categories = new SelectList(await _categoryService.GetAllCategoriesAsync(), "Id", "Title");
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