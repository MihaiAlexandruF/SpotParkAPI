namespace SpotParkAPI.Models.Dtos
{
    public class ParkingLotMapPreviewDto
    {
        public int ParkingLotId { get; set; }
        public decimal PricePerHour { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }
    }
}
