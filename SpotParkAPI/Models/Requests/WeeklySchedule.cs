namespace SpotParkAPI.Models.Requests
{
    public class WeeklySchedule
    {
        
        public string DayOfWeek { get; set; } // Ziua săptămânii (ex: "Monday")
        public TimeSpan OpenTime { get; set; } // Ora de deschidere
        public TimeSpan CloseTime { get; set; } // Ora de închidere
        
    }
}
