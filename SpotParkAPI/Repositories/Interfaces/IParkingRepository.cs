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
       
    }
}