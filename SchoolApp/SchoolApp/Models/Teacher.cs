namespace SchoolApp.Models;

public class Teacher
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }

    // 1 -> many Courses
    public ICollection<Course> Courses { get; set; } = new List<Course>();
}
