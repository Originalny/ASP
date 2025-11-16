using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.RazorPages;
using Microsoft.Identity.Client;
using KT3.Models;
using System.Text.Json;

namespace KT3.Pages.Products
{
    public class IndexModel : PageModel
    {
        public List<Product> Products { get; set; } = new();
        public async Task OnGetAsync()
        {
            using var http = new HttpClient();
            var response = await http.GetAsync("https://localhost:7270/api/product");

            if (response.IsSuccessStatusCode)
            {
                var json = await response.Content.ReadAsStringAsync();
                Products = JsonSerializer.Deserialize<List<Product>>(json, new JsonSerializerOptions { PropertyNameCaseInsensitive = true });
            }
        }
    }
}
