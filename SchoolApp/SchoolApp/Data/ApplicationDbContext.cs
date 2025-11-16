using Microsoft.EntityFrameworkCore;
using SchoolApp.Models;

namespace SchoolApp.Data;

public class ApplicationDbContext : DbContext
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options) : base(options) { }

    public DbSet<Teacher> Teachers => Set<Teacher>();
    public DbSet<Student> Students => Set<Student>();
    public DbSet<Course> Courses => Set<Course>();
    public DbSet<CourseStudent> CourseStudents => Set<CourseStudent>();

    protected override void OnModelCreating(ModelBuilder b)
    {
        base.OnModelCreating(b);

        // Teacher (1) -> (many) Course
        b.Entity<Course>()
            .HasOne(c => c.Teacher)
            .WithMany(t => t.Courses)
            .HasForeignKey(c => c.TeacherId)
            .OnDelete(DeleteBehavior.Cascade);

        // Явная таблица связей CourseStudent (PK: CourseId + StudentId)
        b.Entity<CourseStudent>()
            .HasKey(cs => new { cs.CourseId, cs.StudentId });

        b.Entity<CourseStudent>()
            .HasOne(cs => cs.Course)
            .WithMany(c => c.Enrollments)
            .HasForeignKey(cs => cs.CourseId)
            .OnDelete(DeleteBehavior.Cascade);

        b.Entity<CourseStudent>()
            .HasOne(cs => cs.Student)
            .WithMany(s => s.Enrollments)
            .HasForeignKey(cs => cs.StudentId)
            .OnDelete(DeleteBehavior.Cascade);
    }
}
