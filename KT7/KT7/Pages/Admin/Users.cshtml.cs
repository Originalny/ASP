using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Identity;

namespace KT7.Pages.Admin
{
    public class UsersModel : PageModel
    {
        private readonly UserManager<IdentityUser> _userManager;
        public List<IdentityUser> Users { get; set; }

        public UsersModel(UserManager<IdentityUser> userManager)
        {
            _userManager = userManager;
        }

        public void OnGet()
        {
            Users = _userManager.Users.ToList();
        }
    }
}
