namespace SpotParkAPI.Models.Requests
{
    public class ReserveRequest
    {
        public int ParkingLotId { get; set; }
        public DateTime StartTime { get; set; }
        public DateTime EndTime { get; set; }


    }
}
