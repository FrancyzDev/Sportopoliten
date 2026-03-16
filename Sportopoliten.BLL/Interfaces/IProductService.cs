using Sportopoliten.DAL.Entities;
using Sportopoliten.BLL.DTO.Product;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IProductService
    {
        Task<IEnumerable<Product>> GetAllProductsAsync();
        Task<Product?> GetProductByIdAsync(int id);
        Task<Product> CreateProductAsync(CreateProductDTO dto);
        Task UpdateProductAsync(int id, UpdateProductDTO dto);
        Task DeleteProductAsync(int id);
    }
}