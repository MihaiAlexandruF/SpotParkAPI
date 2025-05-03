using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IParkingService
    {
        Task<List<ParkingLot>> GetParkingLotsAsync();
        Task<ParkingLotDto> GetParkingLotByIdAsync(int id);
        Task AddParkingLotAsync(ParkingLot parkingLot);
        Task<ParkingLotDto> CreateParkingLotWithAvailabilityAsync(CreateParkingLotRequest request, int ownerId);
        Task<List<AvailabilityScheduleDto>> GetAvailabilitySchedulesAsync(int parkingLotId);
        Task<List<ParkingLotDto>> GetParkingLotsByOwnerIdAsync(int ownerId);
        Task<ParkingLotDto> GetParkingLotDetailsByIdAsync(int id);

    }
}
