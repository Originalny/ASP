using KT3.Data;
using KT3.Models;

namespace KT3.Repository
{
    public class UnitOfWork : IUnitOfWork
    {
        private readonly AppDbContext _context;
        public IRepository<Product> Products { get; private set; }
        public UnitOfWork(AppDbContext context)
        {
            _context = context;
            Products = new Repository<Product>(_context);
        }
        public int Complete() => _context.SaveChanges();
        public void Dispose() => _context.Dispose();
    }
}
