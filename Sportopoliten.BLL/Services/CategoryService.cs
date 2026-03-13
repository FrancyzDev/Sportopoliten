using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Services
{
    public class CategoryService : ICategoryService
    {
        private readonly ShopDbContext _context;

        public CategoryService(ShopDbContext context)
        {
            _context = context;
        }

        public async Task<IEnumerable<Category>> GetAllCategoriesAsync()
        {
            var categories = await _context.Categories
                .Include(c => c.Products)
                .ToListAsync();

            return categories.Select(c => new Category
            {
                Id = c.Id,
                Name = c.Name
            });
        }

        public async Task<Category?> GetCategoryByIdAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
                return null;

            return new Category
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task<Category> CreateCategoryAsync(Category dto)
        {
            var category = new Category
            {
                Name = dto.Name
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new Category
            {
                Id = category.Id,
                Name = category.Name
            };
        }

        public async Task UpdateCategoryAsync(int id, Category dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Категория с ID {id} не найдена");
            }

            category.Name = dto.Name;

            await _context.SaveChangesAsync();
        }

        public async Task DeleteCategoryAsync(int id)
        {
            var category = await _context.Categories
                .Include(c => c.Products)
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Категория с ID {id} не найдена");
            }

            if (category.Products != null && category.Products.Any())
            {
                throw new InvalidOperationException("Нельзя удалить категорию, в которой есть товары");
            }

            _context.Categories.Remove(category);
            await _context.SaveChangesAsync();
        }
    }
}