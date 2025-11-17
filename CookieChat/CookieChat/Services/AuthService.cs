using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using CookieChat.Data;

namespace CookieChat.Services;

public class AuthService
{
    private readonly IUserStore _userStore;
    private readonly IHttpContextAccessor _httpContextAccessor;

    public AuthService(IUserStore userStore, IHttpContextAccessor httpContextAccessor)
    {
        _userStore = userStore;
        _httpContextAccessor = httpContextAccessor;
    }

    private HttpContext HttpContext =>
        _httpContextAccessor.HttpContext ?? throw new InvalidOperationException("Нет HttpContext");

    public async Task<(bool Success, string Error)> RegisterAsync(string username, string password)
    {
        if (!_userStore.CreateUser(username, password))
            return (false, "Пользователь с таким именем уже существует или данные некорректны.");

        await SignInAsync(username);
        return (true, string.Empty);
    }

    public async Task<(bool Success, string Error)> LoginAsync(string username, string password)
    {
        if (!_userStore.ValidateUser(username, password))
            return (false, "Неверный логин или пароль.");

        await SignInAsync(username);
        return (true, string.Empty);
    }

    public async Task LogoutAsync()
    {
        await HttpContext.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    }

    private async Task SignInAsync(string username)
    {
        var claims = new List<Claim>
        {
            new(ClaimTypes.Name, username)
        };

        var identity = new ClaimsIdentity(
            claims,
            CookieAuthenticationDefaults.AuthenticationScheme);

        var principal = new ClaimsPrincipal(identity);

        await HttpContext.SignInAsync(
            CookieAuthenticationDefaults.AuthenticationScheme,
            principal);
    }
}
