using KT5.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KT5.Controllers
{
    public class TaskController : Controller
    {
        private static List<TaskItem> _tasks = new List<TaskItem>();

        [HttpGet]
        public IEnumerable<TaskItem> Get([FromQuery] string status)
        {
            if (!string.IsNullOrEmpty(status))
            {
                return _tasks.Where(t => t.Status == status);
            }

            return _tasks;
        }

        [HttpPost]
        public ActionResult<TaskItem> Post(TaskItem task)
        {
            task.Id = _tasks.Count > 0 ? _tasks.Count + 1 : 1;
            _tasks.Add(task);

            return CreatedAtAction(nameof (GetById), new { id = task.Id }, task);
        }

        [HttpGet("{id}")]
        public ActionResult<TaskItem> GetById(int id)
        {
            var task = _tasks.FirstOrDefault(t => t.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            return task;
        }

        [HttpPatch("{id}")]
        public IActionResult Path(int id, [FromBody] string status)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            task.Status = status;

            return NoContent();
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var task = _tasks.FirstOrDefault(task => task.Id == id);

            if (task == null)
            {
                return NotFound();
            }

            _tasks.Remove(task);
            return NoContent();
        }
    }
}
