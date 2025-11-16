using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Controllers;

public class CoursesController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var data = await db.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Enrollments).ThenInclude(e => e.Student)
            .ToListAsync();


        return View(data);
    }

    public async Task<IActionResult> Create()
    {
        ViewBag.Teachers = new SelectList(await db.Teachers.ToListAsync(), "Id", "Name");
        return View(new Course());
    }

    [HttpPost]
    public async Task<IActionResult> Create(Course m)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teachers = new SelectList(await db.Teachers.ToListAsync(), "Id", "Name", m.TeacherId);
            return View(m);
        }
        db.Add(m);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var m = await db.Courses.FindAsync(id);
        if (m == null) return NotFound();

        ViewBag.Teachers = new SelectList(await db.Teachers.ToListAsync(), "Id", "Name", m.TeacherId);
        return View(m);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Course m)
    {
        if (!ModelState.IsValid)
        {
            ViewBag.Teachers = new SelectList(await db.Teachers.ToListAsync(), "Id", "Name", m.TeacherId);
            return View(m);
        }
        db.Update(m);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var m = await db.Courses
            .Include(c => c.Teacher)
            .Include(c => c.Enrollments).ThenInclude(e => e.Student)
            .FirstOrDefaultAsync(c => c.Id == id);


        if (m == null) return NotFound();

        // кто уже зачислен
        var already = (await db.CourseStudents
            .Where(cs => cs.CourseId == id)
            .Select(cs => cs.StudentId)
            .ToListAsync())
            .ToHashSet(); // <-- обычный метод из LINQ

        // свободные студенты
        ViewBag.FreeStudents = await db.Students
            .Where(s => !already.Contains(s.Id))
            .OrderBy(s => s.Name)
            .ToListAsync();

        return View(m);
    }


    public async Task<IActionResult> Delete(int id)
    {
        var m = await db.Courses
            .Include(c => c.Teacher)
            .FirstOrDefaultAsync(c => c.Id == id);

        return m == null ? NotFound() : View(m);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await db.Courses.FindAsync(id);
        if (m != null)
        {
            db.Courses.Remove(m);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    // -------- управление зачислением --------

    [HttpGet]
    public async Task<IActionResult> AddStudent(int id) // id = courseId
    {
        var course = await db.Courses.FindAsync(id);
        if (course == null) return NotFound();

        var enrolledIds = await db.CourseStudents
            .Where(x => x.CourseId == id)
            .Select(x => x.StudentId)
            .ToListAsync();

        ViewBag.Students = new SelectList(
            await db.Students.Where(s => !enrolledIds.Contains(s.Id)).ToListAsync(),
            "Id", "Name");

        ViewBag.Course = course;

        return View(new CourseStudent
        {
            CourseId = id,
            EnrollmentDate = DateTime.Today
        });
    }

    [HttpPost]
    public async Task<IActionResult> AddStudent(CourseStudent vm)
    {
        if (!await db.Courses.AnyAsync(c => c.Id == vm.CourseId)) return NotFound();
        if (!await db.Students.AnyAsync(s => s.Id == vm.StudentId)) return NotFound();

        var exists = await db.CourseStudents.FindAsync(vm.CourseId, vm.StudentId);
        if (exists == null)
        {
            db.CourseStudents.Add(vm);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = vm.CourseId });
    }

    [HttpPost]
    public async Task<IActionResult> RemoveStudent(int courseId, int studentId)
    {
        var e = await db.CourseStudents.FindAsync(courseId, studentId);
        if (e != null)
        {
            db.CourseStudents.Remove(e);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Details), new { id = courseId });
    }
}
