using Sportopoliten.BLL.DTO.Cart;

namespace Sportopoliten.BLL.Interfaces
{
    public interface ICartService
    {
        Task<CartDTO> GetCartAsync(int userId);
        Task<int> GetCartItemCountAsync(int userId);
        Task AddToCartAsync(int userId, int productId, int count);
        Task UpdateQuantityAsync(int productId, int userId, int count);
        Task RemoveItemAsync(int productId, int userId);
        Task ClearCartAsync(int userId);
        Task<decimal> GetTotalSumAsync(int userId);
        Task<int> GetTotalItemsCountAsync(int userId);
    }
}