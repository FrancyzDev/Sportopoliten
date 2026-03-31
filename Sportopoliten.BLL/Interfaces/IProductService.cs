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
        Task<IEnumerable<Product>> GetProductsByCategoryAsync(int categoryId);
        Task<IEnumerable<Product>> GetPagedProductsAsync(int page, int pageSize);
        Task<int> GetTotalProductsCountAsync();
        Task<(IEnumerable<Product> Products, int TotalCount)> GetFilteredProductsAsync(
            string? searchTerm, 
            int? categoryId, 
            decimal? minPrice,
            decimal? maxPrice,
            string? sortBy,
            int page,
            int pageSize
        );

        Task<decimal> GetMinPriceAsync();
        Task<decimal> GetMaxPriceAsync();
    }
}