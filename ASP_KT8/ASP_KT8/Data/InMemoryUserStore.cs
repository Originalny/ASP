using System.Collections.Concurrent;
using ASP_KT8.Models;

namespace ASP_KT8.Data
{
    public class InMemoryUserStore : IUserStore
    {
        private readonly ConcurrentDictionary<Guid, User> _db = new();

        public IEnumerable<User> GetAll() => _db.Values.OrderBy(x => x.Username);

        public User? Get(Guid id) => _db.TryGetValue(id, out var u) ? u : null;

        public User Add(User user)
        {
            user.Id = Guid.NewGuid();
            _db[user.Id] = user;

            return user;
        }

        public bool Update(Guid id, User update)
        {
            if (!_db.ContainsKey(id)) return false;

            update.Id = id;
            _db[id] = update;

            return true;
        }

        public bool Delete(Guid id) => _db.TryRemove(id, out var _);

        public bool ExistsByEmail(string email) => _db.Values.Any(x => x.Email.Equals(email, StringComparison.OrdinalIgnoreCase));
    }
}
