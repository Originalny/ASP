using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.AspNetCore.Identity;
using System.ComponentModel.DataAnnotations;
using System.Threading.Tasks;
using Microsoft.Identity.Client;

namespace KT7.Pages.Account
{
    public class LoginModel : PageModel
    {
        private readonly SignInManager<IdentityUser> _signInManager;
        public LoginModel(SignInManager<IdentityUser> signInManager) => _signInManager = signInManager;

        public InputModel Input { get; set; }
        public string ErrorMessage { get; set; }
        public class InputModel
        {
            [Required]
            [EmailAddress]

            public string Email { get; set; }

            [Required]
            [DataType(DataType.Password)]

            public string Password { get; set; }

            [Display(Name = "Remember me?")]

            public bool RememberMe { get; set; }
        }

        public void OnGet()
        {
        }

        public async Task<IActionResult> OnPostAsync()
        {
            if (ModelState.IsValid)
            {
                var result = await _signInManager.PasswordSignInAsync(Input.Email, Input.Password, Input.RememberMe, false);
            
                if (result.Succeeded)
                {
                    LocalRedirect(Url.Content("~/"));
                }

                ErrorMessage = "Invalid Login Attempt!";
            }

            return Page();
        }
    }
}
