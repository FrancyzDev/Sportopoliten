using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.BLL.DTO.Product;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Services
{
    public class ProductService : IProductService
    {
        private readonly ShopDbContext _context;

        public ProductService(ShopDbContext context)
        {
            _context = context;
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

            foreach (var imageUrl in dto.ProductImages)
            {
                product.ProductImages.Add(new ProductImage
                {
                    ImageUrl = imageUrl
                });
            }

            _context.Products.Add(product);
            await _context.SaveChangesAsync();
            return product;
        }

        public async Task UpdateProductAsync(int productId, UpdateProductDTO dto)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
            {
                throw new KeyNotFoundException("Товар не найден");
            }

            // Обновляем основные поля (без Color и Size)
            product.Title = dto.Title;
            product.Description = dto.Description;
            product.CategoryId = dto.CategoryId;
            product.Price = dto.Price;

            // Удаляем старые изображения
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                _context.ProductImages.RemoveRange(product.ProductImages);
            }

            // Добавляем новые изображения
            if (dto.ProductImages != null && dto.ProductImages.Any())
            {
                product.ProductImages = new List<ProductImage>();
                foreach (var imageUrl in dto.ProductImages)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = product.Id
                    });
                }
            }

            await _context.SaveChangesAsync();
        }

        public async Task DeleteProductAsync(int productId)
        {
            var product = await _context.Products
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == productId);

            if (product == null)
                throw new KeyNotFoundException("Товар не найден");

            // Удаляем связанные изображения
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                _context.ProductImages.RemoveRange(product.ProductImages);
            }

            // Удаляем товар
            _context.Products.Remove(product);
            await _context.SaveChangesAsync();
        }

        // Дополнительные методы
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .FirstOrDefaultAsync(p => p.Id == id);
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId)
        {
            return await _context.Products
                .Where(p => p.CategoryId == categoryId)
                .Include(p => p.ProductImages)
                .ToListAsync();
        }

        public async Task<IEnumerable<Product>> GetPagedProductsAsync(int page, int pageSize)
        {
            return await _context.Products
                .Include(p => p.Category)
                .Include(p => p.ProductImages)
                .Skip((page - 1) * pageSize)
                .Take(pageSize)
                .ToListAsync();
        }

        public async Task<int> GetTotalProductsCountAsync()
        {
            return await _context.Products.CountAsync();
        }
    }
}