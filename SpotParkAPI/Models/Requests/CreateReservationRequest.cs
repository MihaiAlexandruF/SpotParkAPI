namespace SpotParkAPI.Models.Requests
{
    public class CreateReservationRequest
    {
        public int ParkingLotId { get; set; }
        public int PlateId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }
        public string PaymentMethod { get; set; } = string.Empty; // "wallet" sau "card"
    }
}
