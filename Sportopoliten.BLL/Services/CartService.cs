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
                    itemDTOs.Add(new CartItemDTO
                    {
                        Id = item.Id,
                        CartId = item.CartId,
                        ProductId = product.Id,
                        Count = item.Count,

                        ProductName = product.Title,
                        Price = product.Price,
                        ImageUrl = firstImage != null ? firstImage.ImageUrl : "https://media.licdn.com/dms/image/v2/C560BAQHvjs3O4Utmdw/company-logo_200_200/company-logo_200_200/0/1631351760522?e=2147483647&v=beta&t=98Nb6ha1qF7VFgRtzDHP0WzmNbTlI_r26j4Q4rm3nMg"
                    });
                }
            }

            return new CartDTO
            {
                Items = itemDTOs,
                TotalPrice = itemDTOs.Sum(x => x.Price * x.Count)
            };
        }

        public async Task AddToCartAsync(int userId, int productId, int count)
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

            var cartItems = await Database.CartItems.FindAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
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
                    Count = count
                };
                await Database.CartItems.AddAsync(newItem);
            }

            await Database.SaveChangesAsync();
        }

        public async Task UpdateQuantityAsync(int userId, int productId, int count)
        {
            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart != null)
            {
                var cartItems = await Database.CartItems.FindAsync(ci => ci.CartId == cart.Id && ci.ProductId == productId);
                var existingItem = cartItems.FirstOrDefault();

                if (existingItem != null)
                {
                    existingItem.Count = count;
                    Database.CartItems.Update(existingItem);
                    await Database.SaveChangesAsync();
                }
            }
        }

        public async Task RemoveItemAsync(int productId, int userId)
        {
            var cartItems = await Database.CartItems.FindAsync(ci => ci.ProductId == productId && ci.Cart.UserId == userId);

            var itemToRemove = cartItems.FirstOrDefault();

            if (itemToRemove != null)
            {
                Database.CartItems.Delete(itemToRemove);

                await Database.SaveChangesAsync();
            }
        }

        //public async Task UpdateQuantityAsync(int productId, int userId, int count)
        //{
        //    var items = await Database.CartItems.FindAsync(ci =>
        //        ci.ProductId == productId && ci.Cart.UserId == userId);
        //    var item = items.FirstOrDefault();

        //    if (item != null)
        //    {
        //        item.Count = count;

        //        if (item.Count <= 0)
        //        {
        //            Database.CartItems.Delete(item);
        //        }
        //        else
        //        {
        //            Database.CartItems.Update(item);
        //        }

        //        await Database.SaveChangesAsync();
        //    }
        //}


        public async Task ClearCartAsync(int userId)
        {
            var carts = await Database.Carts.FindAsync(c => c.UserId == userId);
            var cart = carts.FirstOrDefault();

            if (cart != null)
            {
                Database.Carts.Delete(cart);
                await Database.SaveChangesAsync();
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