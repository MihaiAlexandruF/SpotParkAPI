using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Helpers;
using SpotParkAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading.Tasks;

namespace SpotParkAPI.Services
{
    public class CommonService : ICommonService
    {
        private readonly IHttpContextAccessor _httpContextAccessor;
        private readonly SpotParkDbContext _context;
        private readonly IParkingRepository _parkingRepository;
        private readonly IReservationRepository _reservationRepository;
        private readonly IAvailabilityService _availabilityService;

        public CommonService(
            SpotParkDbContext context,
            IHttpContextAccessor httpContextAccessor,
            IParkingRepository parkingRepository,
            IReservationRepository reservationRepository,
            IAvailabilityService availabilityService)
        {
            _context = context;
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
            var schedules = await _availabilityService.GetAvailabilitySchedulesAsync(parkingLotId);

            var startUtc = TimeZoneService.ConvertLocalToUtc(startTime);
            var endUtc = TimeZoneService.ConvertLocalToUtc(endTime);

            if (schedules == null || schedules.Count == 0)
            {
                return true;
            }

            return IsWithinSchedule(schedules, startUtc, endUtc);
        }

        private bool IsWithinSchedule(List<AvailabilitySchedule> schedules, DateTime start, DateTime end)
        {
            foreach (var day in EachDay(start.Date, end.Date))
            {
                var daySchedule = schedules.FirstOrDefault(s =>
                    s.DayOfWeek.Equals(day.DayOfWeek.ToString(), StringComparison.OrdinalIgnoreCase));

                if (daySchedule == null) return false;

                var scheduleStartLocal = day.Date.Add(daySchedule.OpenTime.ToTimeSpan());
                var scheduleEndLocal = day.Date.Add(daySchedule.CloseTime.ToTimeSpan());

                // Convertim programul definit local la UTC
                var scheduleStartUtc = TimeZoneService.ConvertLocalToUtc(scheduleStartLocal);
                var scheduleEndUtc = TimeZoneService.ConvertLocalToUtc(scheduleEndLocal);

                if (scheduleEndUtc < scheduleStartUtc)
                {
                    scheduleEndUtc = scheduleEndUtc.AddDays(1);
                }

                var slotStartUtc = day == start.Date ? start : day.Date;
                var slotEndUtc = day == end.Date ? end : day.Date.AddDays(1);

                if (slotStartUtc < scheduleStartUtc || slotEndUtc > scheduleEndUtc)
                {
                    return false;
                }
            }
            return true;
        }

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
            {
                yield return day;
            }
        }

        public async Task<UserVehicle?> GetUserPlateAsync(int userId, int plateId)
        {
            return await _context.UserVehicles
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == plateId);
        }
    }
}
