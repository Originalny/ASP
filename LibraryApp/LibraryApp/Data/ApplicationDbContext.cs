using LibraryApp.Models;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options) { }

    public DbSet<Author> Authors => Set<Author>();
    public DbSet<Book> Books => Set<Book>();

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // AUTHOR 1..* BOOK (каскадное удаление книг при удалении автора)
        modelBuilder.Entity<Book>()
            .HasOne(b => b.Author)
            .WithMany(a => a.Books)
            .HasForeignKey(b => b.AuthorId)
            .OnDelete(DeleteBehavior.Cascade);

        // Индексы/ограничения по желанию
        modelBuilder.Entity<Author>()
            .HasIndex(a => a.Name);
        modelBuilder.Entity<Book>()
            .HasIndex(b => new { b.Title, b.AuthorId }).IsUnique(false);
    }
}
