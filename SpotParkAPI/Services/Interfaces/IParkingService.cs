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
        Task<List<ParkingLotForOwnerDto>> GetParkingLotsForOwnerDashboardAsync(int ownerId);
        Task<bool> ToggleParkingLotActiveStatusAsync(int userId, int parkingLotId);
        Task<List<ParkingLotMapPreviewDto>> GetParkingLotMapPreviewsAsync();

        Task<List<ParkingLotMapPreviewDto>> GetAvailableMapPreviewsAsync(DateTime startTime, DateTime endTime);

    }
}
