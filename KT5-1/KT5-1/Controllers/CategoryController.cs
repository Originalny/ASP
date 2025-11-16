using KT5_1.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KT5_1.Controllers
{
    public class CategoryController : Controller
    {
        private static List<Category> _categories = new List<Category>()
        {
            new Category(1, "Hotel"),
            new Category(2, "Plane Tiket"),
            new Category(3, "Resoraunt"),
            new Category(4, "Taxi")
        };

        [HttpGet]
        public IEnumerable<Category> Get() => _categories;
    }
}
