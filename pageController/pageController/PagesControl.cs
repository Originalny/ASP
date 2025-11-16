using Microsoft.AspNetCore.Mvc;

namespace pageController
{
    public class PagesControl : Controller
    {
        public ActionResult Welcome() { return View(); }
        
        [HttpGet]
        public ActionResult Greet(string name) {
            if (string.IsNullOrEmpty(name))
            {
                name = "Guest";
            }
            ViewBag.UserName = name;
            return View();
        }
    }
}
