using KT5_1.Classes;
using Microsoft.AspNetCore.Mvc;
using System.Linq;

namespace KT5_1.Controllers
{
    public class ReservationController : Controller
    {
        private static List<Reservation> _reservations = new List<Reservation>()
        {
            new Reservation(1, "Hotel1", 1, DateTime.Parse("2025-02-01T19:23:21")),
            new Reservation(2, "Hotel2", 1, DateTime.Parse("2025-05-02T19:33:01")),
            new Reservation(3, "Restoraunt1", 2, DateTime.Parse("2025-01-01T19:23:21")),
            new Reservation(4, "Restoraunt2", 2, DateTime.Parse("2025-04-21T19:23:21")),
        };

        [HttpGet]
        public IEnumerable<Reservation> Get([FromQuery] int? categoryId, [FromQuery] DateTime? date)
        {
            var result = _reservations.AsEnumerable();

            if (categoryId.HasValue)
            {
                result = result.Where(r => r.CategoryId == categoryId.Value);
            }

            if (date.HasValue)
            {
                result = result.Where(d => d.DateTime.Date == date.Value.Date);
            }

            return result;
        }

        [HttpPost]
        public ActionResult<Reservation> Post([FromBody] Reservation reservation)
        {
            bool bReserved = _reservations.Any(r => r.CategoryId == reservation.CategoryId && r.DateTime == reservation.DateTime);

            if (bReserved)
            {
                return Conflict("Place is already reserved!");
            }

            reservation.Id = _reservations.Count > 0 ? _reservations.Count + 1 : 1;
            _reservations.Add(reservation);

            return CreatedAtAction(nameof(GetByID), new { id = reservation.Id }, reservation);
        }

        [HttpGet("{id}")]
        public ActionResult<Reservation> GetByID(int id)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return reservation;
        }

        [HttpDelete("{id}")]
        public IActionResult Delete(int id)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            _reservations.Remove(reservation);
            return NoContent();
        }

        [HttpPut("{id}")]
        public IActionResult Put(int id, [FromBody] Reservation reservationUpdate)
        {
            var reservation = _reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            bool bReserved = _reservations.Any(r => r.CategoryId == reservationUpdate.CategoryId && r.DateTime == reservationUpdate.DateTime && r.Id != id);
            
            if (bReserved)
            {
                return Conflict("Place is already reserved!");
            }

            reservation.CategoryId = reservationUpdate.CategoryId;
            reservation.DateTime = reservationUpdate.DateTime;
            reservation.Name = reservationUpdate.Name;

            return NoContent();
        }
    }
}
