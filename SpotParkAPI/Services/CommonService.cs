using Microsoft.AspNetCore.Http;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories;
using System;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotParkAPI.Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly IParkingRepository _parkingRepository;
        private readonly IReservationRepository _reservationRepository;

        public CommonService(
            IHttpContextAccessor httpContextAccessor,
            IParkingRepository parkingRepository,
            IReservationRepository reservationRepository)
        {
            _httpContextAccessor = httpContextAccessor;
            _parkingRepository = parkingRepository;
            _reservationRepository = reservationRepository;
        }

        public int GetCurrentUserId()
        {
            var userIdClaim = _httpContextAccessor.HttpContext?.User.FindFirst(ClaimTypes.NameIdentifier);
            if (userIdClaim != null && int.TryParse(userIdClaim.Value, out int userId))
            {
                return userId;
            }
            throw new UnauthorizedAccessException("Invalid user ID in token.");
        }

        public async Task<ParkingLot> GetParkingLotByIdAsync(int parkingLotId)
        {
            var parkingLot = await _parkingRepository.GetParkingLotByIdAsync(parkingLotId);
            if (parkingLot == null)
            {
                throw new KeyNotFoundException("Parking lot not found.");
            }
            return parkingLot;
        }

        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            var overlappingReservations = await _reservationRepository.GetReservationsByParkingLotIdAsync(parkingLotId);
            foreach (var reservation in overlappingReservations)
            {
                if ((startTime >= reservation.StartTime && startTime < reservation.EndTime) ||
                    (endTime > reservation.StartTime && endTime <= reservation.EndTime) ||
                    (startTime <= reservation.StartTime && endTime >= reservation.EndTime))
                {
                    return false;
                }
            }
            return true;
        }
    }
}
