using System.ComponentModel.DataAnnotations;

namespace ReservationsApi.DTOs
{
    public class RoomDto
    {
        public string Name { get; set; } = string.Empty;
        public string BuildingCode { get; set; } = string.Empty;
        public int Floor { get; set; }
        public int Capacity { get; set; }
        public bool HasProjector { get; set; }
        public bool IsActive { get; set; } 
    }
}
