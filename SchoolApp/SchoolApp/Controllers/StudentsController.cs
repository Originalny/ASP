using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Controllers;

public class StudentsController(ApplicationDbContext db) : Controller
{
    // список
    public async Task<IActionResult> Index()
    {
        var data = await db.Students
            .Include(s => s.Enrollments)
            .ToListAsync();
        return View(data);

    }

    // создание
    public IActionResult Create() => View(new Student());

    [HttpPost]
    public async Task<IActionResult> Create(Student m)
    {
        if (!ModelState.IsValid) return View(m);
        db.Add(m);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // редактирование
    public async Task<IActionResult> Edit(int id)
    {
        var m = await db.Students.FindAsync(id);
        return m == null ? NotFound() : View(m);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Student m)
    {
        if (!ModelState.IsValid) return View(m);
        db.Update(m);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // детали
    public async Task<IActionResult> Details(int id)
    {
        var m = await db.Students
            .Include(s => s.Enrollments)
                .ThenInclude(e => e.Course)
            .FirstOrDefaultAsync(s => s.Id == id);

        if (m == null) return NotFound();

        // курсы, где уже учится студент (берём из загруженных Enrollments)
        var already = m.Enrollments.Select(e => e.CourseId).ToHashSet();

        // свободные курсы
        ViewBag.FreeCourses = await db.Courses
            .Where(c => !already.Contains(c.Id))
            .OrderBy(c => c.Title)
            .ToListAsync();

        return View(m);
    }


    // удаление
    public async Task<IActionResult> Delete(int id)
    {
        var m = await db.Students.FindAsync(id);
        return m == null ? NotFound() : View(m);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await db.Students.FindAsync(id);
        if (m != null)
        {
            db.Students.Remove(m);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // ---- управление записями студента на курсы ----

    // GET: подобрать курс и дату
    [HttpGet]
    public async Task<IActionResult> AddCourse(int id) // id = studentId
    {
        var student = await db.Students.FindAsync(id);
        if (student == null) return NotFound();

        var enrolled = await db.CourseStudents
            .Where(x => x.StudentId == id)
            .Select(x => x.CourseId)
            .ToListAsync();

        ViewBag.Student = student;
        ViewBag.Courses = new SelectList(
            await db.Courses.Where(c => !enrolled.Contains(c.Id)).ToListAsync(),
            "Id", "Title");

        return View(new CourseStudent { StudentId = id, EnrollmentDate = DateTime.Today });
    }

    // POST: записать студента
    [HttpPost]
    public async Task<IActionResult> AddCourse(CourseStudent vm)
    {
        if (!await db.Students.AnyAsync(s => s.Id == vm.StudentId)) return NotFound();
        if (!await db.Courses.AnyAsync(c => c.Id == vm.CourseId)) return NotFound();

        var exists = await db.CourseStudents.FindAsync(vm.CourseId, vm.StudentId);
        if (exists == null)
        {
            db.CourseStudents.Add(vm);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = vm.StudentId });
    }

    // POST: отчислить с курса
    [HttpPost]
    public async Task<IActionResult> RemoveCourse(int studentId, int courseId)
    {
        var e = await db.CourseStudents.FindAsync(courseId, studentId);
        if (e != null)
        {
            db.CourseStudents.Remove(e);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = studentId });
    }
}
