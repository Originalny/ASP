using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using UserProfileApp.Data;
using UserProfileApp.Models;

namespace UserProfileApp.Controllers
{
    public class ProfilesController : Controller
    {
        private readonly ApplicationDbContext _db;
        public ProfilesController(ApplicationDbContext db) => _db = db;

        public async Task<IActionResult> Index() =>
            View(await _db.Profiles.Include(p => p.User).ToListAsync());

        public IActionResult Create() =>
            View(new UserProfile());

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Create(UserProfile profile)
        {
            if (!ModelState.IsValid) return View(profile);
            _db.Add(profile);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Edit(int id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost, ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, UserProfile profile)
        {
            if (id != profile.Id) return NotFound();
            if (!ModelState.IsValid) return View(profile);
            _db.Update(profile);
            await _db.SaveChangesAsync();
            return RedirectToAction(nameof(Index));
        }

        public async Task<IActionResult> Details(int id)
        {
            var profile = await _db.Profiles.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (profile == null) return NotFound();
            return View(profile);
        }

        public async Task<IActionResult> Delete(int id)
        {
            var profile = await _db.Profiles.Include(p => p.User)
                .FirstOrDefaultAsync(p => p.Id == id);
            if (profile == null) return NotFound();
            return View(profile);
        }

        [HttpPost, ActionName("Delete"), ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var profile = await _db.Profiles.FindAsync(id);
            if (profile != null)
            {
                _db.Profiles.Remove(profile);
                await _db.SaveChangesAsync();
            }
            return RedirectToAction(nameof(Index));
        }
    }
}
