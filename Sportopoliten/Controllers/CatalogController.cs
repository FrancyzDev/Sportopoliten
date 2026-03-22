using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.ViewModels.CatalogViewModels;

namespace Sportopoliten.Controllers
{
    public class CatalogController : Controller
    {
        private readonly IProductService _productService;
        private readonly ICategoryService _categoryService;
        private readonly ShopDbContext _context;

        public CatalogController(
            IProductService productService,
            ICategoryService categoryService,
            ShopDbContext context)
        {
            _productService = productService;
            _categoryService = categoryService;
            _context = context;
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
            var query = _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .AsQueryable();

            // Фильтрация по поисковому запросу
            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                query = query.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            // Фильтрация по категории
            if (categoryId.HasValue && categoryId.Value > 0)
            {
                query = query.Where(p => p.CategoryId == categoryId);
            }

            // Фильтрация по цене
            if (minPrice.HasValue)
            {
                query = query.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                query = query.Where(p => p.Price <= maxPrice.Value);
            }

            // Сортировка
            query = sortBy switch
            {
                "price_asc" => query.OrderBy(p => p.Price),
                "price_desc" => query.OrderByDescending(p => p.Price),
                "name_asc" => query.OrderBy(p => p.Title),
                "name_desc" => query.OrderByDescending(p => p.Title),
                "newest" => query.OrderByDescending(p => p.Id),
                _ => query.OrderByDescending(p => p.Id)
            };

            // Пагинация
            var totalItems = await query.CountAsync();
            var totalPages = (int)Math.Ceiling(totalItems / (double)pageSize);

            var items = await query
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .Select(p => new CatalogItemViewModel
                {
                    Id = p.Id,
                    Title = p.Title ?? "Без названия",
                    Description = p.Description,
                    Price = p.Price,
                    ImageUrl = p.ProductImages != null && p.ProductImages.Any()
                        ? p.ProductImages.First().ImageUrl
                        : "/images/no-image.jpg",
                    CategoryName = p.Category != null ? p.Category.Title : "Без категории"
                })
                .ToListAsync();

            // Получаем все категории для фильтра
            var categories = await _context.Categories
                .Select(c => new CategoryFilterViewModel
                {
                    Id = c.Id,
                    Title = c.Title,
                    ProductCount = _context.Products.Count(p => p.CategoryId == c.Id)
                })
                .ToListAsync();

            // Получаем диапазон цен для фильтра
            var priceRange = await _context.Products
                .GroupBy(p => 1)
                .Select(g => new
                {
                    MinPrice = g.Min(p => p.Price),
                    MaxPrice = g.Max(p => p.Price)
                })
                .FirstOrDefaultAsync();

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
                PriceRangeMin = priceRange?.MinPrice ?? 0,
                PriceRangeMax = priceRange?.MaxPrice ?? 10000
            };

            return View(viewModel);
        }

        // GET: Catalog/Category/5
        public async Task<IActionResult> Category(int id, int page = 1)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return NotFound();

            return RedirectToAction(nameof(Index), new { categoryId = id, page });
        }

        // GET: Catalog/Search
        public IActionResult Search(string term)
        {
            return RedirectToAction(nameof(Index), new { searchTerm = term });
        }

        // GET: Catalog/Product/5
        public async Task<IActionResult> Product(int id)
        {
            var product = await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);

            if (product == null)
                return NotFound();

            // Похожие товары из той же категории
            var relatedProducts = await _context.Products
                .Where(p => p.CategoryId == product.CategoryId && p.Id != id)
                .Include(p => p.ProductImages)
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
                .ToListAsync();

            var viewModel = new ProductDetailViewModel
            {
                Id = product.Id,
                Title = product.Title ?? "Без названия",
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.Category?.Title ?? "Без категории",
                Images = product.ProductImages?.Select(i => i.ImageUrl).ToList() ?? new List<string>(),
                RelatedProducts = relatedProducts
            };

            return View(viewModel);
        }
    }
}