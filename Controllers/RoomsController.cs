using Microsoft.AspNetCore.Mvc;
using ReservationsApi.Models;

namespace ReservationsApi.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class RoomsController : ControllerBase
    {
        public static List<Room> rooms = new List<Room>()
        {
            new Room() { Id = 1, Name = "Room 1", BuildingCode = "A", Floor = 1, Capacity = 15, HasProjector = true, IsActive = true},
            new Room() { Id = 2, Name = "Room 2", BuildingCode = "A", Floor = 2, Capacity = 21, HasProjector = false, IsActive = true},
            new Room() { Id = 3, Name = "Room 3", BuildingCode = "B", Floor = 3, Capacity = 47, HasProjector = true, IsActive = true},
            new Room() { Id = 4, Name = "Room 4", BuildingCode = "C", Floor = 5, Capacity = 38, HasProjector = true, IsActive = false},
            new Room() { Id = 5, Name = "Room 5", BuildingCode = "D", Floor = 1, Capacity = 11, HasProjector = true, IsActive = true},
        };

        [HttpGet]
        public IActionResult Get(
            [FromQuery] int? minCapacity,
            [FromQuery] bool? hasProjector,
            [FromQuery] bool? activeOnly)
        {
            var result = rooms.AsQueryable();

            if (minCapacity.HasValue)
            {
                result = result.Where(r => r.Capacity >= minCapacity.Value);
            }

            if (hasProjector.HasValue)
            {
                result = result.Where(r => r.HasProjector == hasProjector.Value);
            }

            if (activeOnly.HasValue && activeOnly.Value)
            {
                result = result.Where(r => r.IsActive);
            }

            return Ok(result.ToList());
        }

        [Route("{id}")]
        [HttpGet]
        public IActionResult GetById([FromRoute] int id)
        {
            var room = rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            return Ok(room);
        }

        [HttpGet("building/{buildingCode}")]
        public IActionResult GetByBuildingCode([FromRoute] string buildingCode)
        {
            var result = rooms
                .Where(r => r.BuildingCode.ToLower() == buildingCode.ToLower())
                .ToList();

            return Ok(result);
        }

        [HttpPost]
        public IActionResult Create([FromBody] Room room)
        {
            room.Id = rooms.Count > 0 ? rooms.Max(r => r.Id) + 1 : 1;

            rooms.Add(room);

            return CreatedAtAction(nameof(GetById), new { id = room.Id }, room);
        }

        [HttpPut("{id}")]
        public IActionResult Update([FromRoute] int id, [FromBody] Room updatedRoom)
        {
            var room = rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            room.Name = updatedRoom.Name;
            room.BuildingCode = updatedRoom.BuildingCode;
            room.Floor = updatedRoom.Floor;
            room.Capacity = updatedRoom.Capacity;
            room.HasProjector = updatedRoom.HasProjector;
            room.IsActive = updatedRoom.IsActive;

            return Ok(room);
        }

        [HttpDelete("{id}")]
        public IActionResult Delete([FromRoute] int id)
        {
            var room = rooms.FirstOrDefault(r => r.Id == id);

            if (room == null)
            {
                return NotFound();
            }

            bool hasReservations = ReservationsController.reservations.Any(r => r.RoomId == id);

            if (hasReservations)
            {
                return Conflict("Nie można usunąć, dla sali istnieją przyszłe rezerwacje");
            }

            rooms.Remove(room);

            return NoContent();
        }
    }
}
