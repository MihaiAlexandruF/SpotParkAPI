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

        public async Task<ParkingLot> GetParkingLotByIdAsync(int id)
        {
            return await _parkingRepository.GetParkingLotByIdAsync(id);
        }

        public async Task AddParkingLotAsync(ParkingLot parkingLot)
        {
            await _parkingRepository.AddParkingLotAsync(parkingLot);
        }
    }
}
