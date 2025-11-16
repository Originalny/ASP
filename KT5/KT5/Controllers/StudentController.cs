using KT5.Classes;
using Microsoft.AspNetCore.Mvc;

namespace KT5.Controllers
{
    public class StudentController : Controller
    {
        private static List<Student> students = new List<Student>();

        [HttpGet]
        public IEnumerable<Student> Get() => students;

        [HttpPost]
        public ActionResult<Student> Post(Student student)
        {
            student.Id = students.Count > 0 ? students.Count + 1 : 1;
            students.Add(student);
            return CreatedAtAction(nameof(GetById), new { Id = student.Id }, student);
        }

        [HttpGet("{id}")]
        public ActionResult<Student> GetById(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            return student;
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, Student newStudent)
        {
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            student.Name = newStudent.Name;
            student.Age = newStudent.Age;

            return NoContent();
        }

        [HttpDelete]
        public IActionResult Delete(int id)
        {
            var student = students.FirstOrDefault(s => s.Id == id);

            if (student == null)
            {
                return NotFound();
            }

            students.Remove(student);

            return NoContent();
        }
    }
}
