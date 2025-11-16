using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

var builder = WebApplication.CreateBuilder(args);

builder.Services.AddControllersWithViews();
builder.Services.AddDbContext<ApplicationDbContext>(opt =>
    opt.UseSqlite(builder.Configuration.GetConnectionString("DefaultConnection")));

var app = builder.Build();

// Авто-создание БД и начальное наполнение (без миграций)
using (var scope = app.Services.CreateScope())
{
    var db = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
    db.Database.EnsureCreated();

    if (!db.Teachers.Any())
    {
        var t1 = new Teacher { Name = "Ирина Петрова", Email = "petrova@uni.edu" };
        var t2 = new Teacher { Name = "Алексей Смирнов", Email = "smirnov@uni.edu" };
        var s1 = new Student { Name = "Мария Иванова", Email = "m.ivanova@mail.com" };
        var s2 = new Student { Name = "Дмитрий Орлов", Email = "d.orlov@mail.com" };
        var s3 = new Student { Name = "Софья Ким", Email = "sofia.kim@mail.com" };

        var c1 = new Course { Title = "Базы данных", Teacher = t1 };
        var c2 = new Course { Title = "Алгоритмы", Teacher = t2 };

        db.AddRange(t1, t2, s1, s2, s3, c1, c2);
        db.SaveChanges();

        db.CourseStudents.AddRange(
            new CourseStudent { CourseId = c1.Id, StudentId = s1.Id, EnrollmentDate = DateTime.Today },
            new CourseStudent { CourseId = c1.Id, StudentId = s2.Id, EnrollmentDate = DateTime.Today },
            new CourseStudent { CourseId = c2.Id, StudentId = s2.Id, EnrollmentDate = DateTime.Today },
            new CourseStudent { CourseId = c2.Id, StudentId = s3.Id, EnrollmentDate = DateTime.Today }
        );
        db.SaveChanges();
    }
}

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
    pattern: "{controller=Home}/{action=Index}/{id?}");

app.Run();
