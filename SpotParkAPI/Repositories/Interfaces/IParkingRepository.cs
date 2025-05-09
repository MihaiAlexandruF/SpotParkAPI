using SpotParkAPI.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories.Interfaces
{
    public interface IParkingRepository
    {
        Task<List<ParkingLot>> GetParkingLotsAsync();
        Task<ParkingLot> GetParkingLotByIdAsync(int id);
        Task AddParkingLotAsync(ParkingLot parkingLot);

        
        Task AddAvailabilityScheduleAsync(AvailabilitySchedule schedule);
        Task<List<AvailabilitySchedule>> GetAvailabilitySchedulesByParkingLotIdAsync(int parkingLotId);

        Task<List<ParkingLot>> GetParkingLotsByOwnerIdAsync(int ownerId);
        Task<List<ParkingLot>> GetFullParkingLotsByOwnerIdAsync(int ownerId);
        Task<decimal> GetTotalEarningsForParkingLotAsync(int parkingLotId);
        Task SetParkingLotActiveStatusAsync(int parkingLotId, bool isActive);

    }
}