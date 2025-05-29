namespace SpotParkAPI.Models.Requests
{
    public class CompleteParkingLotRequest
    {
        public string Description { get; set; }
        public string Address { get; set; }
        public decimal PricePerHour { get; set; }
        public double Latitude { get; set; }
        public double Longitude { get; set; }

        public string AvailabilityType { get; set; }
        public TimeSpan? DailyOpenTime { get; set; }
        public TimeSpan? DailyCloseTime { get; set; }
        public List<WeeklySchedule>? WeeklySchedules { get; set; }

        public List<IFormFile> Images { get; set; }
    }

}
