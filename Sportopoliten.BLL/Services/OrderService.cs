using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;

namespace Sportopoliten.BLL.Services
{
    public class OrderService : IOrderService
    {
        IUnitOfWork Database { get; set; }

        public OrderService(IUnitOfWork uow)
        {
            Database = uow;
        }

        // Получить все заказы
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await Database.Orders.GetWithQueryAsync(query => query
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                );
        }

        // Получить заказ по ID
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await Database.Orders.GetSingleWithQueryAsync(query => query
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .Where(o => o.Id == id)
            );
        }

        // Создать новый заказ
        public async Task<Order> CreateOrderAsync(CreateOrderDTO dto)
        {
            using var transaction = await Database.BeginTransactionAsync();

            try
            {
                var user = await Database.Users.GetByIdAsync(dto.UserId);
                if (user == null)
                    throw new KeyNotFoundException($"Пользователь с ID {dto.UserId} не найден");

                var productIds = dto.Items.Select(i => i.ProductId).ToList();
                var products = (await Database.Products.GetWithQueryAsync(q =>
                    q.Where(p => productIds.Contains(p.Id))
                )).ToList();

                // Формируем полный адрес доставки
                var fullShippingAddress = $"{dto.City}, {dto.Address}, {dto.PostalCode}, {dto.Country}";

                var order = new Order
                {
                    UserId = dto.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0,
                    ShippingAddress = fullShippingAddress,
                    PaymentMethod = dto.PaymentMethod,
                    FullName = dto.FullName,
                    Phone = dto.Phone,
                    Email = dto.Email,
                    Comment = dto.Comment,
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                foreach (var itemDto in dto.Items)
                {
                    var product = products.FirstOrDefault(p => p.Id == itemDto.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Товар с ID {itemDto.ProductId} не найден");

                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        ProductName = product.Title,
                        Count = itemDto.Count,
                        PriceAtPurchase = product.Price,
                        Subtotal = product.Price * itemDto.Count
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Subtotal;
                }

                order.TotalAmount = totalAmount;

                await Database.Orders.AddAsync(order);
                await Database.SaveChangesAsync();
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                await transaction.RollbackAsync();
                throw;
            }
        }
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await Database.Orders.GetWithQueryAsync(query => query
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                );
        }

        // Получить заказы по статусу
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await Database.Orders.GetWithQueryAsync(query => query
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                );
        }

        // Обновить статус заказа
        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await Database.Orders.GetByIdAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Заказ с ID {id} не найден");

            order.Status = status;
            await Database.SaveChangesAsync();
        }

        public async Task DeleteOrderAsync(int id)
        {
            var order = await Database.Orders.GetSingleWithQueryAsync(query => query
                .Include(o => o.OrderItems)
                .Where(o => o.Id == id)
            );

            if (order == null)
                throw new KeyNotFoundException($"Заказ с ID {id} не найден");

            // Удаляем все позиции заказа
            foreach (var item in order.OrderItems.ToList())
            {
                Database.OrderItems.Delete(item);
            }

            // Удаляем сам заказ
            Database.Orders.Delete(order);
            await Database.SaveChangesAsync();
        }
    }
}