using Areas.Admin.Models;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class UsersController : Controller
    {
        private readonly UserManager<IdentityUser> _userManager;
        private readonly RoleManager<IdentityRole> _roleManager;

        public UsersController(UserManager<IdentityUser> userManager, RoleManager<IdentityRole> roleManager)
        {
            _userManager = userManager;
            _roleManager = roleManager;
        }

        public async Task<IActionResult> Index()
        {
            var list = new List<(IdentityUser User, IList<string> Roles)>();
            foreach (var u in _userManager.Users.ToList())
                list.Add((u, await _userManager.GetRolesAsync(u)));
            return View(list);
        }

        // ---------- CREATE ----------
        public async Task<IActionResult> Create()
        {
            var vm = new UserEditVm();
            vm.AllRoles = await _roleManager.Roles
                .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                .ToListAsync();
            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Create(UserEditVm vm)
        {
            if (!ModelState.IsValid)
            {
                vm.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                    .ToListAsync();
                return View(vm);
            }

            var user = new IdentityUser { UserName = vm.Email, Email = vm.Email, EmailConfirmed = true };
            var createRes = await _userManager.CreateAsync(user, vm.NewPassword ?? "");
            if (!createRes.Succeeded)
            {
                foreach (var e in createRes.Errors) ModelState.AddModelError("", e.Description);
                vm.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                    .ToListAsync();
                return View(vm);
            }

            // роли
            if (vm.SelectedRoles?.Count > 0)
            {
                foreach (var role in vm.SelectedRoles.Distinct())
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));

                await _userManager.AddToRolesAsync(user, vm.SelectedRoles);
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- EDIT ----------
        public async Task<IActionResult> Edit(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();

            var roles = await _userManager.GetRolesAsync(u);
            var vm = new UserEditVm
            {
                Id = u.Id,
                Email = u.Email!,
                SelectedRoles = roles.ToList()
            };

            var all = await _roleManager.Roles
                .Select(r => r.Name!)
                .ToListAsync();

            vm.AllRoles = all
                .Select(r => new SelectListItem { Text = r, Value = r, Selected = roles.Contains(r) })
                .ToList();

            return View(vm);
        }

        [HttpPost]
        public async Task<IActionResult> Edit(UserEditVm vm)
        {
            var u = await _userManager.FindByIdAsync(vm.Id!);
            if (u == null) return NotFound();

            if (!ModelState.IsValid)
            {
                vm.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                    .ToListAsync();
                return View(vm);
            }

            u.Email = vm.Email;
            u.UserName = vm.Email;

            var updateRes = await _userManager.UpdateAsync(u);
            if (!updateRes.Succeeded)
            {
                foreach (var e in updateRes.Errors) ModelState.AddModelError("", e.Description);
                vm.AllRoles = await _roleManager.Roles
                    .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                    .ToListAsync();
                return View(vm);
            }

            // роли: синхронизация
            var current = await _userManager.GetRolesAsync(u);
            var toRemove = current.Except(vm.SelectedRoles ?? new()).ToList();
            var toAdd = (vm.SelectedRoles ?? new()).Except(current).ToList();

            if (toRemove.Any()) await _userManager.RemoveFromRolesAsync(u, toRemove);
            if (toAdd.Any())
            {
                foreach (var role in toAdd)
                    if (!await _roleManager.RoleExistsAsync(role))
                        await _roleManager.CreateAsync(new IdentityRole(role));
                await _userManager.AddToRolesAsync(u, toAdd);
            }

            // смена пароля (опционально)
            if (!string.IsNullOrWhiteSpace(vm.NewPassword))
            {
                var token = await _userManager.GeneratePasswordResetTokenAsync(u);
                var pres = await _userManager.ResetPasswordAsync(u, token, vm.NewPassword);
                if (!pres.Succeeded)
                {
                    foreach (var e in pres.Errors) ModelState.AddModelError("", e.Description);
                    vm.AllRoles = await _roleManager.Roles
                        .Select(r => new SelectListItem { Text = r.Name!, Value = r.Name! })
                        .ToListAsync();
                    return View(vm);
                }
            }

            return RedirectToAction(nameof(Index));
        }

        // ---------- DELETE ----------
        public async Task<IActionResult> Delete(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u == null) return NotFound();
            return View(u);
        }

        [HttpPost, ActionName("Delete")]
        public async Task<IActionResult> DeleteConfirmed(string id)
        {
            var u = await _userManager.FindByIdAsync(id);
            if (u != null) await _userManager.DeleteAsync(u);
            return RedirectToAction(nameof(Index));
        }
    }
}
