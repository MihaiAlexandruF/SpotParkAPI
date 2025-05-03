using SpotParkAPI.Models.Entities;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories.Interfaces
{
    public interface IReservationRepository
    {
        Task<List<Reservation>> GetReservationsByParkingLotIdAsync(int parkingLotId);
        Task AddReservationAsync(Reservation reservation);
        Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime);
        Task<List<Reservation>> GetActiveReservationsForOwnerAsync(int ownerId, DateTime now);

    }
}