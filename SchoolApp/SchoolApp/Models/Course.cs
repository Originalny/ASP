namespace SchoolApp.Models;

public class Course
{
    public int Id { get; set; }
    public string Title { get; set; } = "";

    public int TeacherId { get; set; }
    public Teacher? Teacher { get; set; }

    // Явная навигация многие-ко-многим через таблицу связей
    public ICollection<CourseStudent> Enrollments { get; set; } = new List<CourseStudent>();
}
