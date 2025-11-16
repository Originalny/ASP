using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers;

public class AuthorsController(ApplicationDbContext db) : Controller
{
    // GET: /Authors
    public async Task<IActionResult> Index()
    {
        var authors = await db.Authors
            .Include(a => a.Books)
            .OrderBy(a => a.Name)
            .ToListAsync();
        return View(authors);
    }

    // GET: /Authors/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var a = await db.Authors
            .Include(x => x.Books)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();
        return View(a);
    }

    // GET: /Authors/Create
    public IActionResult Create() => View();

    // POST: /Authors/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Author model)
    {
        if (!ModelState.IsValid) return View(model);
        db.Authors.Add(model);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Authors/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var a = await db.Authors.FindAsync(id);
        if (a == null) return NotFound();
        return View(a);
    }

    // POST: /Authors/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Author model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid) return View(model);

        db.Entry(model).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Authors/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var a = await db.Authors.FirstOrDefaultAsync(x => x.Id == id);
        if (a == null) return NotFound();
        return View(a);
    }

    // POST: /Authors/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var a = await db.Authors.FindAsync(id);
        if (a != null)
        {
            db.Authors.Remove(a);  // каскадом удалятся его книги
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }
}
