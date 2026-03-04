using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Cart> Carts { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<OrderHistory> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Product> Products { get; }
        IRepository<ProductVariant> ProductVariants { get; }
        IRepository<ProductVariantImages> ProductVariantImages { get; }
        Task<int> SaveChangesAsync();
    }
}