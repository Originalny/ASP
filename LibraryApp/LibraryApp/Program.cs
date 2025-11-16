using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

var builder = WebApplication.CreateBuilder(args);

// MVC
builder.Services.AddControllersWithViews();

// DB
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// ---------- Авто-миграция и минимальное наполнение ----------
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.Migrate(); // применит все миграции к library.db

    // мини-сидинг — чтобы сразу увидеть данные на /Authors
    if (!db.Authors.Any())
    {
        var a = new Author { Name = "Пушкин" };
        db.Authors.Add(a);
        db.Books.Add(new Book { Title = "Капитанская дочка", Author = a });
        db.SaveChanges();
    }
}
// -------------------------------------------------------------

if (!app.Environment.IsDevelopment())
{
    app.UseExceptionHandler("/Home/Error");
    app.UseHsts();
}

app.UseHttpsRedirection();
app.UseStaticFiles();

app.UseRouting();

app.MapControllerRoute(
    name: "default",
    pattern: "{controller=Authors}/{action=Index}/{id?}");

app.Run();
