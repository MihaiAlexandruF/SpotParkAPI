using SpotParkAPI.Models;
using SpotParkAPI.Repositories;

namespace SpotParkAPI.Services
{
    public class ParkingService
    {
        private readonly IParkingRepository _parkingRepository;
        public ParkingService(IParkingRepository parkingRepository)
        {
            _parkingRepository = parkingRepository;
        }
        public async Task<List<ParkingLot>> GetParkingLotsAsync()
        {
            return await _parkingRepository.GetParkingLotsAsync();
        }
    }
}
