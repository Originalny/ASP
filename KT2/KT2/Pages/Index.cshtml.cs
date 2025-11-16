using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT2.Pages
{
    public class IndexModel : PageModel
    {
        [BindProperty]
        public string UserText { get; set; }

        private readonly string _filepath = "usertexts.txt";

        public void OnGet()
        {
            if (System.IO.File.Exists(_filepath))
            {
                UserText = System.IO.File.ReadAllText(_filepath);
            }
        }

        public IActionResult OnPost()
        {
            System.IO.File.WriteAllText(_filepath, UserText);

            return RedirectToPage();
        }
    }
}
