using Sportopoliten.BLL.DTO.Cart;
using Sportopoliten.BLL.Interfaces;
using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Repositories;

namespace Sportopoliten.BLL.Services
{
    public class CartService : ICartService
    {
        private IUnitOfWork Database { get; }

        public CartService(IUnitOfWork uow) => Database = uow;

        public async Task<CartDTO> GetCartAsync(int userId)
        {
            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                return new CartDTO
                {
                    Items = new List<CartItemDTO>(),
                    TotalPrice = 0
                };
            }

            var items = await Database.CartItems.FindAsync(ci => ci.CartId == cart.Id);

            var itemDTOs = new List<CartItemDTO>();
            foreach (var item in items)
            {
                var product = await Database.Products.GetByIdAsync(item.ProductId);

                if (product != null)
                {
                    var productImages = await Database.ProductImages.FindAsync(pi => pi.ProductId == product.Id);
                    var firstImage = productImages.FirstOrDefault();

                    // Формируем название с размером
                    var productNameWithSize = !string.IsNullOrEmpty(item.Size)
                        ? $"{product.Title} ({item.Size})"
                        : product.Title;

                    itemDTOs.Add(new CartItemDTO
                    {
                        Id = item.Id,
                        CartId = item.CartId,
                        ProductId = product.Id,
                        Count = item.Count,
                        Size = item.Size,
                        ProductName = productNameWithSize,
                        Price = product.Price,
                        ImageUrl = firstImage != null ? firstImage.ImageUrl : "/images/no-image.jpg"
                    });
                }
            }

            return new CartDTO
            {
                Items = itemDTOs,
                TotalPrice = itemDTOs.Sum(x => x.Price * x.Count)
            };
        }

        public async Task<int> GetCartItemCountAsync(int userId)
        {
            try
            {
                // Получаем корзину пользователя
                var cart = await GetCartAsync(userId);

                if (cart == null || cart.Items == null)
                    return 0;

                // Суммируем количество всех товаров в корзине
                return cart.Items.Sum(item => item.Count);
            }
            catch (Exception ex)
            {
                // Логирование ошибки
                Console.WriteLine($"Error getting cart item count: {ex.Message}");
                return 0;
            }
        }

        public async Task AddToCartAsync(int userId, int productId, int count, string? size = null)
        {
            var product = await Database.Products.GetByIdAsync(productId);
            if (product == null)
            {
                throw new Exception("Товар не найден");
            }

            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart == null)
            {
                cart = new Cart { UserId = userId };
                await Database.Carts.AddAsync(cart);
                await Database.SaveChangesAsync();
            }

            var cartItems = await Database.CartItems.FindAsync(ci =>
                ci.CartId == cart.Id &&
                ci.ProductId == productId &&
                ci.Size == size);

            var existingItem = cartItems.FirstOrDefault();

            if (existingItem != null)
            {
                existingItem.Count += count;
                Database.CartItems.Update(existingItem);
            }
            else
            {
                var newItem = new CartItem
                {
                    CartId = cart.Id,
                    ProductId = productId,
                    Count = count,
                    Size = size
                };
                await Database.CartItems.AddAsync(newItem);
            }

            await Database.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int productId, int count, string? size = null)
        {
            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart != null)
            {
                var cartItems = await Database.CartItems.FindAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId && ci.Size == size);
                var existingItem = cartItems.FirstOrDefault();

                if (existingItem != null)
                {
                    existingItem.Count = count;
                    Database.CartItems.Update(existingItem);
                    await Database.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveItemAsync(int productId, int userId, string? size = null)
        {
            var cartItems = await Database.CartItems.FindAsync(ci => ci.ProductId == productId && ci.Cart.UserId == userId && ci.Size == size);
            var itemToRemove = cartItems.FirstOrDefault();

            if (itemToRemove != null)
            {
                Database.CartItems.Delete(itemToRemove);
                await Database.SaveChangesAsync();
            }
        }

        public async Task ClearCartAsync(int userId)
        {
            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart != null)
            {
                var cartItems = await Database.CartItems.FindAsync(ci => ci.CartId == cart.Id);
                if (cartItems != null && cartItems.Any())
                {
                    foreach (var item in cartItems.ToList())
                    {
                        Database.CartItems.Delete(item);
                    }
                    await Database.SaveChangesAsync();
                }
            }
        }

        public async Task<decimal> GetTotalSumAsync(int userId)
        {
            var cartItems = await Database.CartItems.FindAsync(ci => ci.Cart.UserId == userId);

            if (cartItems == null || !cartItems.Any())
            {
                return 0m;
            }

            decimal total = cartItems.Sum(item => item.Product.Price * item.Count);

            return total;
        }

        public async Task<int> GetTotalItemsCountAsync(int userId)
        {
            var items = await Database.CartItems.FindAsync(ci => ci.Cart.UserId == userId);

            return items.Sum(item => item.Count);
        }
    }
}