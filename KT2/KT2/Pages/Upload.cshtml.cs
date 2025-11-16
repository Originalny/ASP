using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;

namespace KT2.Pages
{
    [IgnoreAntiforgeryToken]
    public class Upload : PageModel
    {
        public async Task<IActionResult> OnPostAsync(IFormFile file)
        {
            Console.WriteLine("UPLOAD POST COLLECT");

            if (file == null || file.Length == 0)
            {
                return BadRequest("No file uploaded");
            }

            var uploadsFolder = Path.Combine(Directory.GetCurrentDirectory(), "wwwroot", "uploads");

            Directory.CreateDirectory(uploadsFolder);

            var filePath = Path.Combine(uploadsFolder, file.Name);

            using (var stream = new FileStream(filePath, FileMode.Create))
            {
                await file.CopyToAsync(stream);
            }

            var fileUrl = Url.Content($"~/uploads/{file.FileName}");

            return new JsonResult(new { location = fileUrl })
            {
                StatusCode = 200,
                ContentType = "application/json"
            };
        }
    }
}
