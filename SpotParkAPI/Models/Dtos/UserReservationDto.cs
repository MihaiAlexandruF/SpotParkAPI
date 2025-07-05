namespace SpotParkAPI.Models.Dtos
{
    public class UserReservationDto
    {
        public int ReservationId { get; set; }
        public int ParkingLotId { get; set; }
        public string Address { get; set; }
        public string LicensePlate { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public decimal TotalCost { get; set; }
        public string Status { get; set; }
    }

}
