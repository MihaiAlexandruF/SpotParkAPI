using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Requests;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Services.Interfaces
{
    public interface IReservationService
    {
        Task<ReservationDto> ReserveParkingLotAsync(ReserveRequest request);
        Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime);
    }
}