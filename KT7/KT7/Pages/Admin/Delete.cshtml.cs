using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Authorization;

namespace KT7.Pages.Admin
{
    [Authorize(Roles = "Admin")]
    public class DeleteModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public IdentityUser User { get; set; }

        public DeleteModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public async Task<IActionResult> OnGetAsync(string id)
        {
            User = await _userManager.FindByIdAsync(id);

            if (User == null)
            {
                return RedirectToPage("Users");
            }

            return Page();
        }

        public async Task<IActionResult> OnPostAsync()
        {
            var user = await _userManager.FindByIdAsync(User.Id);

            if (user == null)
            {
                return RedirectToPage("Users");
            }

            await _userManager.DeleteAsync(user);

            return RedirectToPage("Users");
        }
    }
}
