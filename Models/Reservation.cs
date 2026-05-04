using System.ComponentModel.DataAnnotations;

namespace ReservationsApi.Models
{
    public class Reservation
    {
        public int Id { get; set; }
        public int RoomId { get; set; }
        [Required]
        public string OrganizerName { get; set; } = string.Empty;
        [Required]
        public string Topic { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public TimeSpan EndTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
