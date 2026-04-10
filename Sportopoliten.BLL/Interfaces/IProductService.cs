using Sportopoliten.BLL.DTO.Product;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IProductService
    {
        Task<ProductDTO?> GetProductByIdAsync(int id);
        Task<EditProductDTO?> GetProductForEditAsync(int id);
        Task<IEnumerable<ProductDTO>> GetAllProductsAsync();
        Task<Product> CreateProductAsync(CreateProductDTO dto);
        Task UpdateProductAsync(int productId, UpdateProductDTO dto);
        Task DeleteProductAsync(int productId);
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
            int pageSize);
        Task<decimal> GetMinPriceAsync();
        Task<decimal> GetMaxPriceAsync();
    }
}