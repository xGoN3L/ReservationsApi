using ReservationsApi.Models;

namespace ReservationsApi.DTOs
{
    public class ReservationDto
    {
        public int RoomId { get; set; }
        public string OrganizerName { get; set; } = string.Empty;
        public string Topic { get; set; } = string.Empty;
        public DateTime Date { get; set; }
        public TimeSpan StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public ReservationStatus Status { get; set; }
    }
}
