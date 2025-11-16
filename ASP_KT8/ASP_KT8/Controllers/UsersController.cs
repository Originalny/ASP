using ASP_KT8.Data;
using ASP_KT8.Models;
using Microsoft.AspNetCore.Mvc;

namespace ASP_KT8.Controllers
{
    [ApiController]
    [Route("api/[controller]")]
    public class UsersController : ControllerBase
    {
        private readonly IUserStore _store;

        public UsersController(IUserStore store) => _store = store;

        [HttpGet]
        public ActionResult<IEnumerable<User>> GetAll() => Ok(_store.GetAll());

        [HttpGet("{id:guid}")]
        public ActionResult<User> Get(Guid id)
        {
            var u = _store.Get(id);

            return u is null ? NotFound(new { message = "User not found"}) : Ok(u);
        }

        [HttpPost]
        public ActionResult<User> Create(CreateUserDto dto)
        {
            if (_store.ExistsByEmail(dto.Email))
            {
                ModelState.AddModelError(nameof(dto.Email), "Email is already used!");
            }

            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var u = new User { Username = dto.Username, Email = dto.Email, Password = dto.Password };

            _store.Add(u);
            return CreatedAtAction(nameof(Get), new { Id = u.Id }, u);
        }

        [HttpPut("{id:guid}")]
        public IActionResult Update(Guid id, UpdateUserDto dto)
        {
            if (!ModelState.IsValid) return ValidationProblem(ModelState);

            var existing = _store.Get(id);

            if (existing is null) return NotFound(new { message = "User not found" });
            
            if (!existing.Email.Equals(dto.Email, StringComparison.OrdinalIgnoreCase) && _store.ExistsByEmail(dto.Email))
            {
                ModelState.AddModelError(nameof(dto.Email), "Email is already used!");
                return ValidationProblem(ModelState);
            }

            var u = new User { Username = dto.Username, Email = dto.Email, Password = dto.Password };
            _store.Update(id, u);

            return NoContent();
        }

        [HttpDelete("{id:guid}")]
        public IActionResult Delete(Guid id)
        {
            return _store.Delete(id) ? NoContent() : NotFound(new { message = "User not found!" });
        }
    }
}
