using KT3.Models;
using Microsoft.EntityFrameworkCore;

namespace KT3.Data
{
    public class AppDbContext : DbContext
    {
        public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

        public DbSet<Product> Products { get; set; }

        public static void Seed(AppDbContext context)
        {
            if (!context.Products.Any())
            {
                context.Products.AddRange(
                    new Product { Name = "Melon", Price = 120 },
                    new Product { Name = "Kiwi", Price = 40 },
                    new Product { Name = "Apple", Price = 80 }
                    );

                context.SaveChanges();
            }
        }
    }
}
