namespace SchoolApp.Models;

// таблица-связка + дата зачисления
public class CourseStudent
{
    public int CourseId { get; set; }
    public Course? Course { get; set; }

    public int StudentId { get; set; }
    public Student? Student { get; set; }

    public DateTime EnrollmentDate { get; set; }
}
