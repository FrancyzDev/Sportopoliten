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

            // Обновляем основные поля (без Color и Size)
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
                foreach (var imageUrl in dto.ProductImages)
                {
                    product.ProductImages.Add(new ProductImage
                    {
                        ImageUrl = imageUrl,
                        ProductId = product.Id
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

            // Удаляем связанные изображения
            if (product.ProductImages != null && product.ProductImages.Any())
            {
                Database.ProductImages.RemoveRange(product.ProductImages);
            }

            // Удаляем товар
            Database.Products.Delete(product);
            await Database.SaveChangesAsync();
        }

        // Дополнительные методы
        public async Task<Product?> GetProductByIdAsync(int id)
        {
            return await Database.Products.GetSingleWithQueryAsync(query => query
            .Include(p => p.Category)
            .Include(p => p.ProductImages)
            .Where(p => p.Id == id)
    );
        }

        public async Task<IEnumerable<Product>> GetAllProductsAsync()
        {
            return await Database.Products.GetWithQueryAsync(query => query
               .Include(p => p.Category)
               .Include(p => p.ProductImages)
           );
        }

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
    }
}