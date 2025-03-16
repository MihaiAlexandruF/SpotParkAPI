namespace SpotParkAPI.Models.Requests
{
    public class AvailabilityScheduleRequest
    {
        public string AvailabilityType { get; set; } 
        public string? DayOfWeek { get; set; } 
        public string? OpenTime { get; set; } 
        public string? CloseTime { get; set; } 
    }
}
