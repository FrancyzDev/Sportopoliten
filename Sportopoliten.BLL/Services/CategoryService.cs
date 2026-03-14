using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO.Category;
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
                Title = c.Title
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
                Title = category.Title
            };
        }

        public async Task<Category> CreateCategoryAsync(CreateCategoryDTO dto)
        {
            var category = new Category
            {
                Title = dto.Title
            };

            _context.Categories.Add(category);
            await _context.SaveChangesAsync();

            return new Category
            {
                Id = category.Id,
                Title = category.Title
            };
        }

        public async Task UpdateCategoryAsync(int id, UpdateCategoryDTO dto)
        {
            var category = await _context.Categories
                .FirstOrDefaultAsync(c => c.Id == id);

            if (category == null)
            {
                throw new KeyNotFoundException($"Категория с ID {id} не найдена");
            }

            category.Title = dto.Title;

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