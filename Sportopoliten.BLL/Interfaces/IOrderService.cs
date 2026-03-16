using Sportopoliten.BLL.DTO;
using Sportopoliten.DAL.Entities;
using Sportopoliten.BLL.DTO.Order;

namespace Sportopoliten.BLL.Interfaces
{
    public interface IOrderService
    {
        Task<IEnumerable<Order>> GetAllOrdersAsync();
        Task<Order?> GetOrderByIdAsync(int id);
        Task<Order> CreateOrderAsync(CreateOrderDTO dto);
    }
}
