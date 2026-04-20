using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        IUnitOfWork Database { get; set; }

        public CategoryService(IUnitOfWork uow)
        {
            Database = uow;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            return await Database.Categories.GetWithQueryAsync(query => query
                .Include(c => c.Products)
            );
                
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            return await Database.Categories.GetSingleWithQueryAsync(query => query
                .Include(c => c.Products)
                .Where(c => c.Id == id)
            );
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            var category = new Category
            {
                Title = dto.Title,
                ImageUrl = dto.ImageUrl
            };

            await Database.Categories.AddAsync(category);
            await Database.SaveChangesAsync();

            return category;
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDTO dto)
        {
            var category = await Database.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Категория с ID {id} не найдена");
            }

            category.Title = dto.Title;
            if (dto.ImageUrl != null)
            {
                category.ImageUrl = dto.ImageUrl;
            }

            await Database.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await Database.Categories.GetSingleWithQueryAsync(query => query
                .Include(c => c.Products)
                .Where(c => c.Id == id)
            );

            if (category == null)
            {
                throw new KeyNotFoundException($"Категория с ID {id} не найдена");
            }

            if (category.Products != null && category.Products.Any())
            {
                throw new InvalidOperationException("Нельзя удалить категорию, в которой есть товары");
            }

            Database.Categories.Delete(category);
            await Database.SaveChangesAsync();
        }
        public async Task<IEnumerable<CategoryWithCountDTO>> GetCategoriesWithProductCountAsync()
        {
            var categories = await Database.Categories.GetWithQueryAsync(query => query
                .Include(c => c.Products)
            );

            return categories.Select(c => new CategoryWithCountDTO
            {
                Id = c.Id,
                Title = c.Title,
                ImageUrl = c.ImageUrl,
                ProductCount = c.Products?.Count ?? 0
            });
        }
    }
}