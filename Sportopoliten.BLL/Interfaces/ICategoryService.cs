using Sportopoliten.BLL.DTO;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Interfaces
{
    public interface ICategoryService
    {
        Task<IEnumerable<Category>> GetAllCategoriesAsync();
        Task<Category?> GetCategoryByIdAsync(int id);
        Task<Category> CreateCategoryAsync(Category dto);
        Task UpdateCategoryAsync(int id, Category dto);
        Task DeleteCategoryAsync(int id);
    }
}