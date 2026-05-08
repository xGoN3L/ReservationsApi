using Microsoft.AspNetCore.Mvc;
using ReservationsApi.Models;

namespace ReservationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ReservationsController : ControllerBase
    {
        public static List<Reservation> reservations = new List<Reservation>()
        {
            new Reservation()
            {
                Id = 1,
                RoomId = 1,
                OrganizerName = "Jan Kowalski",
                Topic = "Podstawy C#",
                Date = new DateTime(2026, 5, 10),
                StartTime = new TimeSpan(9, 0, 0),
                EndTime = new TimeSpan(11, 0, 0),
                Status = ReservationStatus.Planned,
            },
            new Reservation()
            {
                Id = 2,
                RoomId = 2,
                OrganizerName = "Anna Nowak",
                Topic = "REST",
                Date = new DateTime(2026, 5, 10),
                StartTime = new TimeSpan(10, 0, 0),
                EndTime = new TimeSpan(12, 30, 0),
                Status = ReservationStatus.Confirmed
            },
            new Reservation()
            {
                Id = 3,
                RoomId = 3,
                OrganizerName = "Piotr Zieliński",
                Topic = "SQL",
                Date = new DateTime(2026, 5, 11),
                StartTime = new TimeSpan(13, 0, 0),
                EndTime = new TimeSpan(15, 0, 0),
                Status = ReservationStatus.Confirmed
            },
            new Reservation()
            {
                Id = 4,
                RoomId = 1,
                OrganizerName = "Maria Wiśniewska",
                Topic = "ASP.NET Core",
                Date = new DateTime(2026, 5, 12),
                StartTime = new TimeSpan(8, 30, 0),
                EndTime = new TimeSpan(10, 0, 0),
                Status = ReservationStatus.Cancelled
            }
        };

        [HttpGet]
        public IActionResult Get(
            [FromQuery] DateTime? date,
            [FromQuery] string? status,
            [FromQuery] int? roomId)
        {
            var result = reservations.AsQueryable();

            if (date.HasValue)
            {
                result = result.Where(r => r.Date == date.Value);
            }

            if(!string.IsNullOrWhiteSpace(status))
{
                if (!Enum.TryParse<ReservationStatus>(status, true, out var parsedStatus))
                {
                    return BadRequest("Niepoprawny status");
                }

                result = result.Where(r => r.Status == parsedStatus);
            }

            if (roomId.HasValue)
            {
                result = result.Where(r => r.RoomId == roomId.Value);
            }

            return Ok(result.ToList());
        }

        [HttpGet("{id}")]
        public IActionResult GetById([FromRoute] int id)
        {
            var reservation = reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            return Ok(reservation);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Reservation reservation)
        {
            if (reservation.EndTime <= reservation.StartTime)
            {
                return BadRequest("EndTime musi być późniejsze niż StartTime");
            }

            var room = RoomsController.rooms.FirstOrDefault(r => r.Id == reservation.RoomId);

            if (room == null)
            {
                return BadRequest("Sala nie istnieje");
            }

            if (!room.IsActive)
            {
                return BadRequest("Sala nie jest aktywna");
            }

            if (HasConflict(reservation))
            {
                return Conflict("Rezerwacja koliduje z inną rezerwacją dla tej samej sali");
            }

            reservation.Id = reservations.Count > 0 ? reservations.Max(r => r.Id) + 1 : 1;

            reservations.Add(reservation);

            return CreatedAtAction(nameof(GetById), new { id = reservation.Id }, reservation);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Reservation updatedReservation)
        {
            var reservation = reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            if (updatedReservation.EndTime <= updatedReservation.StartTime)
            {
                return BadRequest("EndTime musi być późniejsze niż StartTime");
            }

            var room = RoomsController.rooms.FirstOrDefault(r => r.Id == updatedReservation.RoomId);

            if (room == null)
            {
                return BadRequest("Sala nie istnieje");
            }

            if (!room.IsActive)
            {
                return BadRequest("Sala nie jest aktywna");
            }

            updatedReservation.Id = id;

            if (HasConflict(updatedReservation, id))
            {
                return Conflict("Rezerwacja koliduje z inną rezerwacją dla tej samej sali");
            }

            reservation.RoomId = updatedReservation.RoomId;
            reservation.OrganizerName = updatedReservation.OrganizerName;
            reservation.Topic = updatedReservation.Topic;
            reservation.Date = updatedReservation.Date;
            reservation.StartTime = updatedReservation.StartTime;
            reservation.EndTime = updatedReservation.EndTime;
            reservation.Status = updatedReservation.Status;

            return Ok(reservation);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var reservation = reservations.FirstOrDefault(r => r.Id == id);

            if (reservation == null)
            {
                return NotFound();
            }

            reservations.Remove(reservation);

            return NoContent();
        }
        private bool HasConflict(Reservation reservation, int? ignoredReservationId = null)
        {
            return reservations.Any(r =>
                r.Id != ignoredReservationId &&
                r.RoomId == reservation.RoomId &&
                r.Date == reservation.Date &&
                r.Status != ReservationStatus.Cancelled &&
                reservation.Status != ReservationStatus.Cancelled &&
                reservation.StartTime < r.EndTime &&
                reservation.EndTime > r.StartTime);
        }
    }
}
