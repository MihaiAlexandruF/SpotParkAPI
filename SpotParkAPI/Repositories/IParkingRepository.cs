using SpotParkAPI.Models;

namespace SpotParkAPI.Repositories
{
    public interface IParkingRepository
    {
        Task<List<ParkingLot>> GetParkingLotsAsync();
        Task<ParkingLot> GetParkingLotByIdAsync(int id);

        Task AddParkingLotAsync(ParkingLot parkingLot);
    }
}
