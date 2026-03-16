using Microsoft.EntityFrameworkCore;
using Sportopoliten.BLL.DTO;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.BLL.Services
{
    public class OrderService : IOrderService
    {
        private readonly ShopDbContext _context;

        public OrderService(ShopDbContext context)
        {
            _context = context;
        }

        // Получить все заказы
        public async Task<IEnumerable<Order>> GetAllOrdersAsync()
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // Получить заказ по ID
        public async Task<Order?> GetOrderByIdAsync(int id)
        {
            return await _context.Orders
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                        .ThenInclude(p => p.ProductImages)
                .FirstOrDefaultAsync(o => o.Id == id);
        }

        // Создать новый заказ
        public async Task<Order> CreateOrderAsync(CreateOrderDTO dto)
        {
            // Начинаем транзакцию
            using var transaction = await _context.Database.BeginTransactionAsync();

            try
            {
                // Проверяем существование пользователя
                var user = await _context.Users.FindAsync(dto.UserId);
                if (user == null)
                    throw new KeyNotFoundException($"Пользователь с ID {dto.UserId} не найден");

                // Создаем заказ
                var order = new Order
                {
                    UserId = dto.UserId,
                    OrderDate = DateTime.UtcNow,
                    Status = OrderStatus.Pending,
                    TotalAmount = 0, // Будет пересчитано
                    OrderItems = new List<OrderItem>()
                };

                decimal totalAmount = 0;

                // Добавляем товары в заказ
                foreach (var itemDto in dto.Items)
                {
                    // Проверяем существование товара
                    var product = await _context.Products.FindAsync(itemDto.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Товар с ID {itemDto.ProductId} не найден");

                    // Проверяем наличие на складе

                    // Создаем позицию заказа
                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        Product = product,
                        Count = itemDto.Count,
                        PriceAtPurchase = product.Price, // Цена на момент заказа
                        Subtotal = product.Price * itemDto.Count
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Subtotal;

                    _context.Products.Update(product);
                }

                order.TotalAmount = totalAmount;

                // Сохраняем заказ
                _context.Orders.Add(order);
                await _context.SaveChangesAsync();

                // Подтверждаем транзакцию
                await transaction.CommitAsync();

                return order;
            }
            catch
            {
                // Откатываем транзакцию в случае ошибки
                await transaction.RollbackAsync();
                throw;
            }
        }
        // Получить заказы пользователя
        public async Task<IEnumerable<Order>> GetOrdersByUserIdAsync(int userId)
        {
            return await _context.Orders
                .Where(o => o.UserId == userId)
                .Include(o => o.OrderItems)
                    .ThenInclude(oi => oi.Product)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // Получить заказы по статусу
        public async Task<IEnumerable<Order>> GetOrdersByStatusAsync(OrderStatus status)
        {
            return await _context.Orders
                .Where(o => o.Status == status)
                .Include(o => o.User)
                .Include(o => o.OrderItems)
                .OrderByDescending(o => o.OrderDate)
                .ToListAsync();
        }

        // Обновить статус заказа
        public async Task UpdateOrderStatusAsync(int id, OrderStatus status)
        {
            var order = await _context.Orders.FindAsync(id);
            if (order == null)
                throw new KeyNotFoundException($"Заказ с ID {id} не найден");

            order.Status = status;
            await _context.SaveChangesAsync();
        }
    }
}