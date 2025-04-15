namespace SpotParkAPI.Models.Requests
{
    public class AvailabilitySchedulesRequest
    {
        public int ParkingLotId { get; set; } // ID-ul parcului de actualizat
        public string AvailabilityType { get; set; } // Tipul de disponibilitate (always, daily, weekly)
        public TimeSpan? DailyOpenTime { get; set; } // Ora de deschidere pentru disponibilitatea zilnică
        public TimeSpan? DailyCloseTime { get; set; } // Ora de închidere pentru disponibilitatea zilnică
        public List<WeeklySchedule>? WeeklySchedules { get; set; } // Programul săptămânal
    }

    
}