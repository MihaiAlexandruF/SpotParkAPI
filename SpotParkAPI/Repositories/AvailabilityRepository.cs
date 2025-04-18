﻿using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories
{
    public class AvailabilityRepository : IAvailabilityRepository
    {
        private readonly SpotParkDbContext _context;

        public AvailabilityRepository(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task<List<AvailabilitySchedule>> GetByParkingLotIdAsync(int parkingLotId)
        {
            return await _context.AvailabilitySchedules
                .Where(a => a.ParkingLotId == parkingLotId)
                .ToListAsync();
        }

        public async Task AddAsync(AvailabilitySchedule schedule)
        {
            _context.AvailabilitySchedules.Add(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task UpdateAsync(AvailabilitySchedule schedule)
        {
            _context.AvailabilitySchedules.Update(schedule);
            await _context.SaveChangesAsync();
        }

        public async Task DeleteAsync(int scheduleId)
        {
            var schedule = await _context.AvailabilitySchedules.FindAsync(scheduleId);
            if (schedule != null)
            {
                _context.AvailabilitySchedules.Remove(schedule);
                await _context.SaveChangesAsync();
            }
        }
    }
}