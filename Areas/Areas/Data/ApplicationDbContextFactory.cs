using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;
using Microsoft.Extensions.Configuration;
using System.IO;

namespace Areas.Data
{
    public class ApplicationDbContextFactory : IDesignTimeDbContextFactory<ApplicationDbContext>
    {
        public ApplicationDbContext CreateDbContext(string[] args)
        {
            // 1) Пытаемся загрузить appsettings.json из ТЕКУЩЕЙ папки проекта
            // (без дополнительных "Areas" — они уже в текущем каталоге)
            var builder = new ConfigurationBuilder()
                .SetBasePath(Directory.GetCurrentDirectory())
                .AddJsonFile("appsettings.json", optional: true);

            // 2) Доп. попытка: если запускаете из решения/другой папки
            var parent = Directory.GetParent(Directory.GetCurrentDirectory())?.FullName;
            if (parent != null && File.Exists(Path.Combine(parent, "appsettings.json")))
                builder.AddJsonFile(Path.Combine(parent, "appsettings.json"), optional: true);

            var config = builder.Build();

            // 3) Фолбэк, если конфигурацию не нашли
            var conn = config.GetConnectionString("DefaultConnection") ?? "Data Source=areas.db";

            var options = new DbContextOptionsBuilder<ApplicationDbContext>()
                .UseSqlite(conn)
                .Options;

            return new ApplicationDbContext(options);
        }
    }
}
