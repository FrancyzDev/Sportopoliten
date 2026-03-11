using Sportopoliten.BLL.DTO;

public interface IProductService
{
    Task CreateProductAsync(CreateProductDTO dto);
}