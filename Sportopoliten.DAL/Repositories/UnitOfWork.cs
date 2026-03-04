using Sportopoliten.DAL.Entities;
using Sportopoliten.DAL.Interfaces;
using Sportopoliten.DAL.Interfaces.Repositories;
using Sportopoliten.DAL.Data;
using Sportopoliten.DAL.Repositories;

public class UnitOfWork : IUnitOfWork
{
    private readonly ShopDbContext _context;

    public IRepository<User> Users { get; private set; }

    public UnitOfWork(ShopDbContext context)
    {
        _context = context;
        Users = new Repository<User>(context);
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