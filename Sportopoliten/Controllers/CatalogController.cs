using Microsoft.AspNetCore.Mvc;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.ViewModels.CatalogViewModels;
using Sportopoliten.ViewModels.ProductViewModels;
using System.Security.Claims;

namespace Sportopoliten.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ICartService _cartService;

        public CatalogController(
            IProductService productService,
            ICategoryService categoryService,
            ICartService cartService)
        {
            _productService = productService;
            _categoryService = categoryService;
            _cartService = cartService;
        }

        // GET: Catalog
        public async Task<IActionResult> Index(
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page = 1,
            int pageSize = 12)
        {
            // Используем новый метод сервиса
            var (products, totalItems) = await _productService.GetFilteredProductsAsync(
                searchTerm, categoryId, minPrice, maxPrice, sortBy, page, pageSize);

            var items = products.Select(p => new CatalogItemViewModel
            {
                Id = p.Id,
                Title = p.Title ?? "Без названия",
                Description = p.Description,
                Price = p.Price,
                ImageUrl = p.ProductImages != null && p.ProductImages.Any()
                    ? p.ProductImages.First().ImageUrl
                    : "/images/no-image.jpg",
                CategoryName = p.Category != null ? p.Category.Title : "Без категории"
            }).ToList();

            // Используем новый метод категорий
            var categoriesWithCount = await _categoryService.GetCategoriesWithProductCountAsync();

            var categories = categoriesWithCount.Select(c => new CategoryFilterViewModel
            {
                Id = c.Id,
                Title = c.Title,
                ProductCount = c.ProductCount
            }).ToList();

            var minPriceValue = await _productService.GetMinPriceAsync();
            var maxPriceValue = await _productService.GetMaxPriceAsync();

            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var viewModel = new CatalogViewModel
            {
                Items = items,
                CurrentPage = page,
                TotalPages = totalPages,
                TotalItems = totalItems,
                PageSize = pageSize,
                SearchTerm = searchTerm,
                SelectedCategoryId = categoryId,
                MinPrice = minPrice,
                MaxPrice = maxPrice,
                SortBy = sortBy,
                Categories = categories,
                PriceRangeMin = minPriceValue,
                PriceRangeMax = maxPriceValue
            };

            return View(viewModel);
        }

        // GET: Catalog/Product/5
        public async Task<IActionResult> Product(int id)
        {
            var product = await _productService.GetProductByIdAsync(id);

            if (product == null)
                return NotFound();

            var relatedProducts = await _productService.GetProductsByCategoryAsync(product.CategoryId ?? 0);

            var relatedProductsViewModels = relatedProducts
                .Where(p => p.Id != id)
                .Take(4)
                .Select(p => new CatalogItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title ?? "Без названия",
                    Price = p.Price,
                    ImageUrl = p.ProductImages != null && p.ProductImages.Any()
                        ? p.ProductImages.First().ImageUrl
                        : "/images/no-image.jpg"
                })
                .ToList();

            var viewModel = new DetailProductViewModel
            {
                Id = product.Id,
                Title = product.Title ?? "Без названия",
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.CategoryName ?? "Без категории",
                Images = product.ProductImages?.Select(i => i.ImageUrl).ToList() ?? new List<string>()
            };

            return View(viewModel);
        }

        // GET: Catalog/Category/5
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var category = await _categoryService.GetCategoryByIdAsync(id);

            if (category == null)
                return NotFound();

            return RedirectToAction(nameof(Index), new { categoryId = id, page });
        }

        // GET: Catalog/Search
        public IActionResult Search(string term)
        {
            return RedirectToAction(nameof(Index), new { searchTerm = term });
        }

        [HttpPost]
        public async Task<IActionResult> AddToCart(int productId)
        {
            try
            {
                if (User.Identity?.IsAuthenticated != true)
                {
                    return Json(new { success = false, redirectToLogin = true, message = "Пожалуйста, войдите в систему" });
                }

                var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
                if (userIdClaim == null)
                {
                    return Json(new { success = false, message = "Пользователь не найден" });
                }
                int userId = int.Parse(userIdClaim.Value);
                await _cartService.AddToCartAsync(userId, productId, 1);
                return Json(new { success = true, message = "Товар успешно добавлен!" });
            }
            catch (Exception ex)
            {
                return Json(new { success = false, message = "Ошибка: " + ex.Message });
            }
        }
    }
}