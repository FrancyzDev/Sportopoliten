using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Data;

namespace Sportopoliten.DAL.Repositories
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly ShopDbContext _context;

        public IRepository<User> Users { get; private set; }
        public IRepository<Cart> Carts { get; private set; }
        public IRepository<CartItem> CartItems { get; private set; }
        public IRepository<OrderHistory> Orders { get; private set; }
        public IRepository<OrderItem> OrderItems { get; private set; }
        public IRepository<Product> Products { get; private set; }
        public IRepository<ProductVariant> ProductVariants { get; private set; }
        public IRepository<ProductVariantImages> ProductVariantImages { get; private set; }

        public UnitOfWork(ShopDbContext context)
        {
            _context = context;
            Users = new Repository<User>(context);
            Carts = new Repository<Cart>(context);
            CartItems = new Repository<CartItem>(context);
            Orders = new Repository<OrderHistory>(context);
            OrderItems = new Repository<OrderItem>(context);
            Products = new Repository<Product>(context);
            ProductVariants = new Repository<ProductVariant>(context);
            ProductVariantImages = new Repository<ProductVariantImages>(context);
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