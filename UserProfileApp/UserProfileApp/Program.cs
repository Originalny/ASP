using Microsoft.EntityFrameworkCore;
using UserProfileApp.Data;

var builder = WebApplication.CreateBuilder(args);

// Подключение EF Core + SQLite
builder.Services.AddDbContext<ApplicationDbContext>(options =>
    options.UseSqlite("Data Source=users.db"));

// Добавляем MVC
builder.Services.AddControllersWithViews();

var app = builder.Build();

// Автоматически создаём БД при первом запуске
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Users}/{action=Index}/{id?}");

app.Run();
