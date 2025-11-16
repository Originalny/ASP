using KT3.Models;

namespace KT3.Repository
{
    public interface IUnitOfWork : IDisposable
    {
        IRepository<Product> Products { get; }
        int Complete();

    }
}
