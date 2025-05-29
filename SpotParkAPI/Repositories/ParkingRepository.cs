using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories
{
    public class ParkingRepository : IParkingRepository
    {
        private readonly SpotParkDbContext _context;

        public ParkingRepository(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task<List<ParkingLot>> GetParkingLotsAsync()
        {
            return await _context.ParkingLots.ToListAsync();
        }

        public async Task<ParkingLot> GetParkingLotByIdAsync(int id)
        {
            return await _context.ParkingLots.FindAsync(id);
        }

        public async Task AddParkingLotAsync(ParkingLot parkingLot)
        {
            _context.ParkingLots.Add(parkingLot);
            await _context.SaveChangesAsync();
        }

        public async Task AddAvailabilityScheduleAsync(AvailabilitySchedule schedule)
        {
            _context.AvailabilitySchedules.Add(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task<List<AvailabilitySchedule>> GetAvailabilitySchedulesByParkingLotIdAsync(int parkingLotId)
        {
            return await _context.AvailabilitySchedules
                .Where(a => a.ParkingLotId == parkingLotId)
                .ToListAsync();
        }
        public async Task<List<ParkingLot>> GetParkingLotsByOwnerIdAsync(int ownerId)
        {
            return await _context.ParkingLots
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }
        public async Task<List<ParkingLot>> GetFullParkingLotsByOwnerIdAsync(int ownerId)
        {
            return await _context.ParkingLots
                .Include(p => p.AvailabilitySchedules)
                .Include(p => p.Images)
                .Where(p => p.OwnerId == ownerId)
                .ToListAsync();
        }

        public async Task<decimal> GetTotalEarningsForParkingLotAsync(int parkingLotId)
        {
            return await _context.Reservations
                .Where(r => r.ParkingLotId == parkingLotId && r.Status == "completed")
                .SumAsync(r => (decimal?)r.TotalCost) ?? 0;
        }
        public async Task SetParkingLotActiveStatusAsync(int parkingLotId, bool isActive)
        {
            var parkingLot = await _context.ParkingLots.FindAsync(parkingLotId);
            if (parkingLot == null) return;

            parkingLot.IsActive = isActive;
            _context.ParkingLots.Update(parkingLot);
            await _context.SaveChangesAsync();
        }

        public async Task<List<ParkingLot>> GetActiveAvailableAndUnreservedParkingLotsAsync(DateTime startTime, DateTime endTime)
        {
            return await _context.ParkingLots
                .Include(p => p.AvailabilitySchedules)
                .Where(p => p.IsActive)
                .Where(p => p.AvailabilitySchedules.Any(s =>
                    s.DayOfWeek == startTime.DayOfWeek.ToString() &&
                    s.OpenTime <= TimeOnly.FromDateTime(startTime) &&
                    s.CloseTime >= TimeOnly.FromDateTime(endTime)
                ))
                .Where(p => !_context.Reservations.Any(r =>
                    r.ParkingLotId == p.ParkingLotId &&
                    r.Status == "active" &&
                    (
                        (r.StartTime <= endTime && r.EndTime >= startTime) ||
                        (r.StartTime >= startTime && r.StartTime <= endTime)
                    )
                ))
                .ToListAsync();
        }

    }
}