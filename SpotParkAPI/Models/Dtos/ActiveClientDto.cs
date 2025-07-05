namespace SpotParkAPI.Models.Dtos
{
    public class ActiveClientDto
    {
        public string LicensePlate { get; set; } = string.Empty;
        public string ParkingSpot { get; set; } = string.Empty;

        public DateTime StartTimeUtc { get; set; }
        public DateTime EndTimeUtc { get; set; }

        public int DurationHours => (int)Math.Ceiling((EndTimeUtc - StartTimeUtc).TotalHours);
    }


}
