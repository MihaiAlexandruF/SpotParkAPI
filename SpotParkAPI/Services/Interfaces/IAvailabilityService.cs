using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IAvailabilityService
    {
        Task<List<AvailabilitySchedule>> GetAvailabilitySchedulesAsync(int parkingLotId);
        Task UpdateAvailabilityAsync(int parkingLotId, AvailabilitySchedulesRequest request);
    }
}