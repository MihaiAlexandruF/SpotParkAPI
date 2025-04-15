using SpotParkAPI.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories.Interfaces
{
    public interface IAvailabilityRepository
    {
        Task<List<AvailabilitySchedule>> GetByParkingLotIdAsync(int parkingLotId);
        Task AddAsync(AvailabilitySchedule schedule);
        Task UpdateAsync(AvailabilitySchedule schedule);
        Task DeleteAsync(int scheduleId);
    }
}