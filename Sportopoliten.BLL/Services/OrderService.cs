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
            // Начинаем транзакцию
            using var transaction = await Database.BeginTransactionAsync();

            try
            {
                // Проверяем существование пользователя
                var user = await Database.Users.GetByIdAsync(dto.UserId);
                if (user == null)
                    throw new KeyNotFoundException($"Пользователь с ID {dto.UserId} не найден");

                //ОПТИМИЗАЦИЯ: Загружаем все нужные товары ОДНИМ запросом
                var productIds = dto.Items.Select(i => i.ProductId).ToList();
                var products = (await Database.Products.GetWithQueryAsync(q =>
                    q.Where(p => productIds.Contains(p.Id))
                )).ToList();

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
                    var product = products.FirstOrDefault(p => p.Id == itemDto.ProductId);
                    if (product == null)
                        throw new KeyNotFoundException($"Товар с ID {itemDto.ProductId} не найден");

                    //// Проверяем наличие на складе
                    //if (product.StockQuantity < itemDto.Count)
                    //    throw new InvalidOperationException($"Недостаточно товара '{product.Title}' (в наличии: {product.StockQuantity})");
                    //// ОБНОВЛЯЕМ ОСТАТОК
                    //product.StockQuantity -= itemDto.Count;

                    // Создаем позицию заказа
                    var orderItem = new OrderItem
                    {
                        ProductId = itemDto.ProductId,
                        //Product = product,
                        Count = itemDto.Count,
                        PriceAtPurchase = product.Price, // Цена на момент заказа
                        Subtotal = product.Price * itemDto.Count
                    };

                    order.OrderItems.Add(orderItem);
                    totalAmount += orderItem.Subtotal;

                    Database.Products.Update(product);
                }

                order.TotalAmount = totalAmount;

                // Сохраняем заказ
                await Database.Orders.AddAsync(order);
                await Database.SaveChangesAsync();

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
    }
}