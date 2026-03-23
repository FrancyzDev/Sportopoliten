using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Data;
using Microsoft.EntityFrameworkCore.Storage;

namespace Sportopoliten.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _context;

        public IRepository<User> Users { get; private set; }
        public IRepository<Cart> Carts { get; private set; }
        public IRepository<CartItem> CartItems { get; private set; }
        public IRepository<Order> Orders { get; private set; }
        public IRepository<OrderItem> OrderItems { get; private set; }
        public IRepository<Category> Categories { get; private set; }
        public IRepository<Product> Products { get; private set; }
        public IRepository<ProductImage> ProductImages { get; private set; }

        public UnitOfWork(ShopDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            Carts = new Repository<Cart>(context);
            CartItems = new Repository<CartItem>(context);
            Orders = new Repository<Order>(context);
            Categories = new Repository<Category>(context);
            OrderItems = new Repository<OrderItem>(context);
            Products = new Repository<Product>(context);
            ProductImages = new Repository<ProductImage>(context);
        }

        public async Task<IDbContextTransaction> BeginTransactionAsync()
        {
            return await _context.Database.BeginTransactionAsync();
        }

        public async Task<int> SaveChangesAsync()
        {
            return await _context.SaveChangesAsync();
        }

        public void Dispose()
        {
            _context.Dispose();
        }
    }
}