namespace SpotParkAPI.Models.Dtos
{
    public class ReservationDto
    {
        public int ReservationId { get; set; }
        public int ParkingLotId { get; set; }
        public int DriverId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
        public DateTime CreatedAt { get; set; }
    }
}