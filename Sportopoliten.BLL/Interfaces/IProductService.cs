using Sportopoliten.BLL.DTO;

public interface IProductService
{
    Task CreateProductAsync(CreateProductDTO dto);
    Task UpdateProductAsync(int productId, UpdateProductDTO dto);
    Task DeleteProductAsync(int productId);
}