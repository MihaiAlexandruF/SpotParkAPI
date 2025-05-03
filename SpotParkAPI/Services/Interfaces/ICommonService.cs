using SpotParkAPI.Models.Entities;

namespace SpotParkAPI.Services.Interfaces
{
    public interface ICommonService
    {
        int GetCurrentUserId();
        Task<ParkingLot> GetParkingLotByIdAsync(int parkingLotId);
        Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime);
        Task<UserVehicle?> GetUserPlateAsync(int userId, int plateId);

    }
}
