using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Services.Helpers;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ServiceResult<ReservationDto>> ReserveParkingLotAsync(CreateReservationRequest request);

        Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime);

        Task<List<ActiveClientDto>> GetActiveClientsAsync(int ownerId);

        Task<List<UserReservationDto>> GetActiveReservationsAsync();
        Task<List<UserReservationDto>> GetPastReservationsAsync();


    }
}