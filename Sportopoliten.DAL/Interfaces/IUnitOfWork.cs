using Sportopoliten.DAL.Entities;

namespace Sportopoliten.DAL.Interfaces.Repositories
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<User> Users { get; }
        Task<int> SaveChangesAsync();
    }
}