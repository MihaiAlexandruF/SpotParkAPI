using Microsoft.AspNetCore.Http;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Interfaces;
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
        private IAvailabilityService _availabilityService;

        public CommonService(
            IHttpContextAccessor httpContextAccessor,
            IParkingRepository parkingRepository,
            IReservationRepository reservationRepository,
            IAvailabilityService availabilityService)
        {
            _httpContextAccessor = httpContextAccessor;
            _parkingRepository = parkingRepository;
            _reservationRepository = reservationRepository;
            _availabilityService = availabilityService;
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
            // Verifică programul de disponibilitate
            var schedules = await _availabilityService.GetAvailabilitySchedulesAsync(parkingLotId);
            if (!IsWithinSchedule(schedules, startTime, endTime))
            {
                return false;
            }

            // Verifică suprapunerea cu rezervările existente
            return await _reservationRepository.IsParkingLotAvailableAsync(parkingLotId, startTime, endTime);
        }

        private bool IsWithinSchedule(List<AvailabilitySchedule> schedules, DateTime start, DateTime end)
        {
            foreach (var day in EachDay(start.Date, end.Date))
            {
                var daySchedule = schedules.FirstOrDefault(s =>
                    s.DayOfWeek.Equals(day.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase));

                if (daySchedule == null) return false;

                var scheduleStart = day.Date.Add(daySchedule.OpenTime.ToTimeSpan());
                var scheduleEnd = day.Date.Add(daySchedule.CloseTime.ToTimeSpan());

                // Ajustare pentru intervale care trec peste miezul nopții
                if (scheduleEnd < scheduleStart) scheduleEnd = scheduleEnd.AddDays(1);

                var slotStart = day == start.Date ? start : day.Date;
                var slotEnd = day == end.Date ? end : day.Date.AddDays(1);

                if (slotStart < scheduleStart || slotEnd > scheduleEnd)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime thru)
        {
            for (var day = from.Date; day.Date <= thru.Date; day = day.AddDays(1))
                yield return day;
        }

    }
}
