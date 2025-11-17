using System.Collections.Concurrent;

namespace CookieChat.Data;

public interface IUserStore
{
    bool CreateUser(string username, string password);
    bool ValidateUser(string username, string password);
}

public class InMemoryUserStore : IUserStore
{
    // username -> password
    private readonly ConcurrentDictionary<string, string> _users = new();

    public bool CreateUser(string username, string password)
    {
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
            return false;

        return _users.TryAdd(username, password);
    }

    public bool ValidateUser(string username, string password)
    {
        return _users.TryGetValue(username, out var stored) && stored == password;
    }
}
