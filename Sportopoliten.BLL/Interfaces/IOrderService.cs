using Sportopoliten.BLL.DTO;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderDTO dto);
        Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId);
        Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status);
        Task UpdateOrderStatusAsync(int id, OrderStatus status);
        Task DeleteOrderAsync(int id);
    }
}
