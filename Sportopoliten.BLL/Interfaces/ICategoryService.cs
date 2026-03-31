using Sportopoliten.BLL.DTO.Category;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(CreateCategoryDTO dto);
        Task UpdateCategoryAsync(int id, UpdateCategoryDTO dto);
        Task DeleteCategoryAsync(int id);
        Task<IEnumerable<CategoryWithCountDTO>> GetCategoriesWithProductCountAsync();
    }
}