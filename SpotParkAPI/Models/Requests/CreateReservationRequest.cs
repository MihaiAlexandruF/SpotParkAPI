namespace SpotParkAPI.Models.Requests
{
    public class CreateReservationRequest
    {
        public int ParkingLotId { get; set; }
        public int PlateId { get; set; }
        public string PaymentMethod { get; set; } = "wallet";
        public int Hours { get; set; } 
    }

}
