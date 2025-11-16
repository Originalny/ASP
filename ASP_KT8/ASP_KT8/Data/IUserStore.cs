using ASP_KT8.Models;

namespace ASP_KT8.Data
{
    public interface IUserStore
    {
        IEnumerable<User> GetAll();
        User? Get(Guid id);
        User Add(User user);
        bool Update(Guid id, User update);
        bool Delete(Guid id);
        bool ExistsByEmail(string email);
    }
}
