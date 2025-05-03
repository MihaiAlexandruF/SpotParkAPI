namespace SpotParkAPI.Models.Entities
{
    public class UserVehicle
    {
        public int Id { get; set; }
        public int UserId { get; set; }
        public string PlateNumber { get; set; } = string.Empty;
        public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

        public virtual User User { get; set; } = null!;
    }
}
