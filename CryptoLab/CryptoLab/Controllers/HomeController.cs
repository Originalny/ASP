using Microsoft.AspNetCore.Mvc;

namespace CryptoLab.Controllers
{
    public class HomeController : Controller
    {
        public IActionResult Index() => View();
    }
}
