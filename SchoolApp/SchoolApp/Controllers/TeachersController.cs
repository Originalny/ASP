using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using SchoolApp.Data;
using SchoolApp.Models;

namespace SchoolApp.Controllers;

public class TeachersController(ApplicationDbContext db) : Controller
{
    public async Task<IActionResult> Index()
    {
        var data = await db.Teachers.Include(t => t.Courses).ToListAsync();
        return View(data);
    }

    public IActionResult Create() => View(new Teacher());

    [HttpPost]
    public async Task<IActionResult> Create(Teacher m)
    {
        if (!ModelState.IsValid) return View(m);
        db.Add(m); await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Edit(int id)
    {
        var m = await db.Teachers.FindAsync(id);
        return m == null ? NotFound() : View(m);
    }

    [HttpPost]
    public async Task<IActionResult> Edit(Teacher m)
    {
        if (!ModelState.IsValid) return View(m);
        db.Update(m); await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    public async Task<IActionResult> Details(int id)
    {
        var m = await db.Teachers.Include(t => t.Courses)
                                 .FirstOrDefaultAsync(t => t.Id == id);
        return m == null ? NotFound() : View(m);
    }

    public async Task<IActionResult> Delete(int id)
    {
        var m = await db.Teachers.FindAsync(id);
        return m == null ? NotFound() : View(m);
    }

    [HttpPost, ActionName("Delete")]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var m = await db.Teachers.FindAsync(id);
        if (m != null) { db.Teachers.Remove(m); await db.SaveChangesAsync(); }
        return RedirectToAction(nameof(Index));
    }
}
