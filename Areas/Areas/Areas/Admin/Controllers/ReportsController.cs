using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace Areas.Admin.Controllers
{
    [Area("Admin")]
    [Authorize(Roles = "Admin")]
    public class ReportsController : Controller
    {
        // GET: /Admin/Reports
        public IActionResult Index() => View();

        // пример ещё одной страницы: /Admin/Reports/Users
        public IActionResult Users() => View();
    }
}
