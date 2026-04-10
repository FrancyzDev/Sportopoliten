using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO.Product;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.BLL.Services
{
    public class ProductService : IProductService
    {
        IUnitOfWork Database { get; set; }

        public ProductService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // НОВЫЙ МЕТОД: Возвращает DTO для Edit
        public async Task<EditProductDTO?> GetProductForEditAsync(int id)
        {
            var product = await Database.Products.GetSingleWithQueryAsync(query => query
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == id)
            );

            if (product == null)
                return null;

            return new EditProductDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.Category?.Title,
                ImageUrls = product.ProductImages?.Select(img => img.ImageUrl).ToList() ?? new List<string>()
            };
        }

        // НОВЫЙ МЕТОД: Возвращает DTO для Details/Index
        public async Task<ProductDTO?> GetProductByIdAsync(int id)
        {
            var product = await Database.Products.GetSingleWithQueryAsync(query => query
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Where(p => p.Id == id)
            );

            if (product == null)
                return null;

            return new ProductDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.Category?.Title,
                ProductImages = product.ProductImages?.Select(img => new ProductImage
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    Priority = img.Priority
                }).ToList() ?? new List<ProductImage>()
            };
        }

        // НОВЫЙ МЕТОД: Возвращает коллекцию DTO для Index
        public async Task<IEnumerable<ProductDTO>> GetAllProductsAsync()
        {
            var products = await Database.Products.GetWithQueryAsync(query => query
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
            );

            return products.Select(product => new ProductDTO
            {
                Id = product.Id,
                Title = product.Title,
                Description = product.Description,
                Price = product.Price,
                CategoryId = product.CategoryId ?? 0,
                CategoryName = product.Category?.Title,
                ProductImages = product.ProductImages?.Select(img => new ProductImage
                {
                    Id = img.Id,
                    ImageUrl = img.ImageUrl,
                    Priority = img.Priority
                }).ToList() ?? new List<ProductImage>()
            }).ToList();
        }

        public async Task<Product> CreateProductAsync(CreateProductDTO dto)
        {
            var product = new Product
            {
                Title = dto.Title,
                Description = dto.Description,
                CategoryId = dto.CategoryId,
                Price = dto.Price,
                ProductImages = new List<ProductImage>()
            };

            for (int i = 0; i < dto.ProductImages.Count; i++)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = dto.ProductImages[i],
                    Priority = i
                });
            }

            await Database.Products.AddAsync(product);
            await Database.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(int productId, UpdateProductDTO dto)
        {
            var product = await Database.Products.GetSingleWithQueryAsync(query => query
                .Include(p => p.ProductImages)
                .Where(p => p.Id == productId)
            );

            if (product == null)
            {
                throw new KeyNotFoundException("Товар не найден");
            }

            // Обновляем основные поля
            product.Title = dto.Title;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;

            // Удаляем старые изображения
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                Database.ProductImages.RemoveRange(product.ProductImages);
            }

            // Добавляем новые изображения
            if (dto.ProductImages != null && dto.ProductImages.Any())
            {
                product.ProductImages = new List<ProductImage>();
                for (int i = 0; i < dto.ProductImages.Count; i++)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = dto.ProductImages[i],
                        ProductId = product.Id,
                        Priority = i
                    });
                }
            }

            await Database.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await Database.Products.GetSingleWithQueryAsync(query => query
                .Include(p => p.ProductImages)
                .Where(p => p.Id == productId)
            );

            if (product == null)
                throw new KeyNotFoundException("Товар не найден");

            if (product.ProductImages != null && product.ProductImages.Any())
            {
                Database.ProductImages.RemoveRange(product.ProductImages);
            }

            Database.Products.Delete(product);
            await Database.SaveChangesAsync();
        }

        // Остальные методы без изменений...
        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await Database.Products.GetWithQueryAsync(query => query
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.ProductImages)
            );
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int page, int pageSize)
        {
            return await Database.Products.GetWithQueryAsync(query => query
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .OrderBy(p => p.Id)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
            );
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await Database.Products.CountAsync();
        }

        public async Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            string? searchTerm,
            int? categoryId,
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page,
            int pageSize)
        {
            var allProducts = await Database.Products.GetWithQueryAsync(query => query
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
            );

            var productsQuery = allProducts.AsQueryable();

            if (!string.IsNullOrWhiteSpace(searchTerm))
            {
                productsQuery = productsQuery.Where(p =>
                    p.Title.Contains(searchTerm) ||
                    (p.Description != null && p.Description.Contains(searchTerm)));
            }

            if (categoryId.HasValue && categoryId.Value > 0)
            {
                productsQuery = productsQuery.Where(p => p.CategoryId == categoryId);
            }

            if (minPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price >= minPrice.Value);
            }

            if (maxPrice.HasValue)
            {
                productsQuery = productsQuery.Where(p => p.Price <= maxPrice.Value);
            }

            var totalCount = productsQuery.Count();

            productsQuery = sortBy switch
            {
                "price_asc" => productsQuery.OrderBy(p => p.Price),
                "price_desc" => productsQuery.OrderByDescending(p => p.Price),
                "name_asc" => productsQuery.OrderBy(p => p.Title),
                "name_desc" => productsQuery.OrderByDescending(p => p.Title),
                "newest" => productsQuery.OrderByDescending(p => p.Id),
                _ => productsQuery.OrderByDescending(p => p.Id)
            };

            var products = productsQuery
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToList();

            return (products, totalCount);
        }

        public async Task<decimal> GetMinPriceAsync()
        {
            var products = await Database.Products.GetAllAsync();
            if (!products.Any()) return 0;
            return products.Min(p => p.Price);
        }

        public async Task<decimal> GetMaxPriceAsync()
        {
            var products = await Database.Products.GetAllAsync();
            if (!products.Any()) return 10000;
            return products.Max(p => p.Price);
        }
    }
}