using System.Collections.Generic;


namespace SpotParkAPI.Models.Requests
{
    public class CreateParkingLotRequest
    {
        public string Address { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public string? Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public string AvailabilityType { get; set; } = "always";
        // For "daily" type
        public TimeSpan? DailyOpenTime { get; set; }
        public TimeSpan? DailyCloseTime { get; set; }
        // For "weekly" type
        public List<WeeklySchedule>? WeeklySchedules { get; set; }
    }

    public class WeeklySchedule
    {
        public string DayOfWeek { get; set; } // Monday, Tuesday, etc.
        public TimeSpan OpenTime { get; set; }  // Format "HH:mm"
        public TimeSpan CloseTime { get; set; } // Format "HH:mm"
    }
}
