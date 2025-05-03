namespace SpotParkAPI.Models.Dtos
{
    public class ParkingLotImageDto
    {
        public int ImageId { get; set; }
        public string ImageUrl { get; set; } 
        public DateTime UploadedAt { get; set; }
    }
}
