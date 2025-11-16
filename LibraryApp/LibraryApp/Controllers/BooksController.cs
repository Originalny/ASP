using LibraryApp.Data;
using LibraryApp.Models;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace LibraryApp.Controllers;

public class BooksController(ApplicationDbContext db) : Controller
{
    // GET: /Books
    public async Task<IActionResult> Index()
    {
        var list = await db.Books.Include(b => b.Author)
            .OrderBy(b => b.Title)
            .ToListAsync();
        return View(list);
    }

    // GET: /Books/Details/5
    public async Task<IActionResult> Details(int id)
    {
        var b = await db.Books.Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return View(b);
    }

    // GET: /Books/Create
    public async Task<IActionResult> Create()
    {
        await FillAuthors();
        return View();
    }

    // POST: /Books/Create
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Create(Book model)
    {
        if (!ModelState.IsValid)
        {
            await FillAuthors();
            return View(model);
        }
        db.Books.Add(model);
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Books/Edit/5
    public async Task<IActionResult> Edit(int id)
    {
        var b = await db.Books.FindAsync(id);
        if (b == null) return NotFound();
        await FillAuthors(b.AuthorId);
        return View(b);
    }

    // POST: /Books/Edit/5
    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> Edit(int id, Book model)
    {
        if (id != model.Id) return BadRequest();
        if (!ModelState.IsValid)
        {
            await FillAuthors(model.AuthorId);
            return View(model);
        }
        db.Entry(model).State = EntityState.Modified;
        await db.SaveChangesAsync();
        return RedirectToAction(nameof(Index));
    }

    // GET: /Books/Delete/5
    public async Task<IActionResult> Delete(int id)
    {
        var b = await db.Books.Include(x => x.Author)
            .FirstOrDefaultAsync(x => x.Id == id);
        if (b == null) return NotFound();
        return View(b);
    }

    // POST: /Books/Delete/5
    [HttpPost, ActionName("Delete")]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteConfirmed(int id)
    {
        var b = await db.Books.FindAsync(id);
        if (b != null)
        {
            db.Books.Remove(b);
            await db.SaveChangesAsync();
        }
        return RedirectToAction(nameof(Index));
    }

    private async Task FillAuthors(int? selectedId = null)
    {
        var authors = await db.Authors.OrderBy(a => a.Name).ToListAsync();
        ViewBag.Authors = new SelectList(authors, nameof(Author.Id), nameof(Author.Name), selectedId);
    }
}
