namespace SpotParkAPI.Models.Dtos
{
    public class ParkingLotDto
    {
        public int ParkingLotId { get; set; }
        public int OwnerId { get; set; }
        public string Address { get; set; } = null!;
        public decimal PricePerHour { get; set; }
        public string? Description { get; set; }
        public decimal Latitude { get; set; }
        public decimal Longitude { get; set; }
        public DateTime? CreatedAt { get; set; }
        public DateTime? UpdatedAt { get; set; }
        public List<AvailabilityScheduleDto> AvailabilitySchedules { get; set; } = new List<AvailabilityScheduleDto>();
    }
    public class AvailabilityScheduleDto
    {
        public int ScheduleId { get; set; }
        public int ParkingLotId { get; set; }
        public string? AvailabilityType { get; set; }
        public string? DayOfWeek { get; set; }
        public TimeOnly OpenTime { get; set; }
        public TimeOnly CloseTime { get; set; }
    }
}
