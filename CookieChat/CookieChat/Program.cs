using System.Collections.Concurrent;
using System.Security.Claims;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.Cookies;
using Microsoft.AspNetCore.Http;
using CookieChat.Components;
using CookieChat.Hubs;

var builder = WebApplication.CreateBuilder(args);

// Blazor Web App + интерактивный серверный рендеринг
builder.Services.AddRazorComponents()
    .AddInteractiveServerComponents();

// SignalR для чата
builder.Services.AddSignalR();

// Куки-аутентификация
builder.Services
    .AddAuthentication(CookieAuthenticationDefaults.AuthenticationScheme)
    .AddCookie(options =>
    {
        options.LoginPath = "/login";
        options.AccessDeniedPath = "/login";
    });

builder.Services.AddAuthorization();
builder.Services.AddHttpContextAccessor();

var app = builder.Build();

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Error", createScopeForErrors: true);
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseAuthentication();
app.UseAuthorization();

// ВАЖНО: это убирает ошибку про antiforgery для корня (/)
app.UseAntiforgery();

// ======================= API: Регистрация ==========================
var registerEndpoint = app.MapPost("/api/register", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.BadRequest("Имя пользователя и пароль обязательны.");

    if (!InMemoryUserStore.Users.TryAdd(username, password))
        return Results.BadRequest("Пользователь с таким именем уже существует.");

    var claims = new List<Claim> { new(ClaimTypes.Name, username) };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Redirect("/");
});
registerEndpoint.DisableAntiforgery();

// ========================= API: Вход ===============================
var loginEndpoint = app.MapPost("/api/login", async (HttpContext ctx) =>
{
    var form = await ctx.Request.ReadFormAsync();
    var username = form["username"].ToString();
    var password = form["password"].ToString();

    if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        return Results.BadRequest("Имя пользователя и пароль обязательны.");

    if (!InMemoryUserStore.Users.TryGetValue(username, out var storedPassword) ||
        storedPassword != password)
    {
        return Results.BadRequest("Неверный логин или пароль.");
    }

    var claims = new List<Claim> { new(ClaimTypes.Name, username) };
    var identity = new ClaimsIdentity(claims, CookieAuthenticationDefaults.AuthenticationScheme);
    var principal = new ClaimsPrincipal(identity);

    await ctx.SignInAsync(CookieAuthenticationDefaults.AuthenticationScheme, principal);

    return Results.Redirect("/");
});
loginEndpoint.DisableAntiforgery();

// ========================= API: Выход ==============================
var logoutEndpoint = app.MapPost("/api/logout", async (HttpContext ctx) =>
{
    await ctx.SignOutAsync(CookieAuthenticationDefaults.AuthenticationScheme);
    return Results.Redirect("/");
});
logoutEndpoint.DisableAntiforgery();

// =============== SignalR-хаб чата =================================
app.MapHub<ChatHub>("/chathub");

// =============== Blazor Web App (БЕЗ _Host, БЕЗ Razor Pages) ======
app.MapRazorComponents<App>()
    .AddInteractiveServerRenderMode();

app.Run();

// ================= In-memory хранилище пользователей ==============
public static class InMemoryUserStore
{
    public static ConcurrentDictionary<string, string> Users { get; } = new();
}
