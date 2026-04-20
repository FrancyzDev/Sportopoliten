using Microsoft.EntityFrameworkCore.Storage;
using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Interfaces
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        IRepository<Cart> Carts { get; }
        IRepository<CartItem> CartItems { get; }
        IRepository<Order> Orders { get; }
        IRepository<OrderItem> OrderItems { get; }
        IRepository<Category> Categories { get; }
        IRepository<Product> Products { get; }
        IRepository<ProductImage> ProductImages { get; }
        Task<int> SaveChangesAsync();
        Task<IDbContextTransaction> BeginTransactionAsync();
    }
}