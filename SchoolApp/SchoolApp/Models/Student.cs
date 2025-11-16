namespace SchoolApp.Models;

public class Student
{
    public int Id { get; set; }
    public string Name { get; set; } = "";
    public string? Email { get; set; }

    // Явная навигация многие-ко-многим через таблицу связей
    public ICollection<CourseStudent> Enrollments { get; set; } = new List<CourseStudent>();
}
