namespace SpotParkAPI.Models.Entities
{
    public class ParkingLotImage
    {
        public int ImageId { get; set; }
        public int ParkingLotId { get; set; }
        public int UserId { get; set; }
        public string ImagePath { get; set; } 
        public DateTime UploadedAt { get; set; }

        public virtual ParkingLot ParkingLot { get; set; }
        public virtual User User { get; set; }
    }
}
