using KT3.Data;

namespace KT3.Repository
{
    public class Repository<T> : IRepository<T> where T : class
    {
        protected readonly AppDbContext _context;
        public Repository(AppDbContext context)
        {
            _context = context;
        }
        public IEnumerable<T> GetAll() => _context.Set<T>().ToList();
        public T Get(int id) => _context.Set<T>().Find(id);
        public void Add(T entity) => _context.Set<T>().Add(entity);
        public void Update(T entity) => _context.Set<T>().Update(entity);
        public void Delete(int id)
        {
            var entity = Get(id);

            if (entity != null)
            {
                _context.Set<T>().Remove(entity);
            }
        }
    }
}
