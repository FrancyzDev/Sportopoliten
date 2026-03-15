using Sportopoliten.BLL.DTO;

namespace Sportopoliten.BLL.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int count);
        Task RemoveItemAsync(int productId, int userId);
        Task UpdateQuantityAsync(int productId, int userId, int count);
        Task ClearCartAsync(int userId);
        Task<decimal> GetTotalSumAsync(int userId);
        Task<int> GetTotalItemsCountAsync(int userId);
    }
}
