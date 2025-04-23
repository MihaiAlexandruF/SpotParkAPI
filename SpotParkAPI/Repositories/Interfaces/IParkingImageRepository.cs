using SpotParkAPI.Models.Entities;

namespace SpotParkAPI.Repositories.Interfaces
{
    public interface IParkingImageRepository
    {
        Task AddImageAsync(ParkingLotImage image);
        Task<int> GetImageCountForUserAsync(int userId, int parkingLotId);

        Task<List<ParkingLotImage>> GetImagesForParkingLotAsync(int parkingLotId);
    }
}
