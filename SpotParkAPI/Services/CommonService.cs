using Microsoft.AspNetCore.Http;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
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
        private readonly SpotParkDbContext _context;
        private readonly IParkingRepository _parkingRepository;
        private readonly IReservationRepository _reservationRepository;
        private IAvailabilityService _availabilityService;

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

            // Dacă parcarea este de tip always => întotdeauna disponibilă
            if (schedules == null || schedules.Count == 0)
            {
                return true;
            }

            // Dacă există program setat, verificăm fiecare zi
            foreach (var day in EachDay(startTime.Date, endTime.Date))
            {
                var dayOfWeek = day.DayOfWeek.ToString(); // ex: Monday, Tuesday
                var scheduleForDay = schedules.FirstOrDefault(s =>
                    s.DayOfWeek.Equals(dayOfWeek, StringComparison.OrdinalIgnoreCase));

                if (scheduleForDay == null)
                {
                    return false;
                }

                // Comparăm orele
                var scheduleStart = scheduleForDay.OpenTime;
                var scheduleEnd = scheduleForDay.CloseTime;

                var slotStart = TimeOnly.FromDateTime(day == startTime.Date ? startTime : day);
                var slotEnd = TimeOnly.FromDateTime(day == endTime.Date ? endTime : day.AddDays(1));

                if (slotStart < scheduleStart || slotEnd > scheduleEnd)
                {
                    return false;
                }
            }

            return true;
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

        private IEnumerable<DateTime> EachDay(DateTime from, DateTime to)
        {
            for (var day = from.Date; day.Date <= to.Date; day = day.AddDays(1))
                yield return day;
        }

        public async Task<UserVehicle?> GetUserPlateAsync(int userId, int plateId)
        {
            return await _context.UserVehicles
                .FirstOrDefaultAsync(p => p.UserId == userId && p.Id == plateId);
        }

    }
}
