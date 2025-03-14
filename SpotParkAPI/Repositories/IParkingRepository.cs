using SpotParkAPI.Models;

namespace SpotParkAPI.Repositories
{
    public interface IParkingRepository
    {
        Task<List<ParkingLot>> GetParkingLotsAsync();
    }
}
