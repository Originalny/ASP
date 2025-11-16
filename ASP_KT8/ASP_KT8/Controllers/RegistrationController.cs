using ASP_KT8.Data;
using ASP_KT8.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP_KT8.Controllers
{
    public class RegistrationController : Controller
    {
        private readonly IUserStore _store;
        public RegistrationController(IUserStore store) => _store = store;

        [HttpGet]
        public IActionResult Index() => View(new RegisterVM());

        [HttpPost]
        public IActionResult Index(RegisterVM vm)
        {
            if (_store.ExistsByEmail(vm.Email))
            {
                ModelState.AddModelError(nameof(vm.Email), "User with this email is already exists!");
            }

            if (!ModelState.IsValid) return View(vm);

            var user = new User() { Username = vm.Username, Email = vm.Email, Password = vm.Password };

            _store.Add(user);

            return RedirectToAction(nameof(Success));
        }

        public IActionResult Success => View();
    }
}
