﻿using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Repositories.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotParkAPI.Repositories
{
    public class ReservationRepository : IReservationRepository
    {
        private readonly SpotParkDbContext _context;

        public ReservationRepository(SpotParkDbContext context)
        {
            _context = context;
        }

        public async Task<List<Reservation>> GetReservationsByParkingLotIdAsync(int parkingLotId)
        {
            return await _context.Reservations
                .Where(r => r.ParkingLotId == parkingLotId)
                .ToListAsync();
        }

        public async Task AddReservationAsync(Reservation reservation)
        {
            _context.Reservations.Add(reservation);
            await _context.SaveChangesAsync();
        }

        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            // Verifică dacă există rezervări care se suprapun
            var overlappingReservations = await _context.Reservations
                .Where(r => r.ParkingLotId == parkingLotId &&
                            ((r.StartTime <= startTime && r.EndTime >= startTime) || // Suprapunere la început
                             (r.StartTime <= endTime && r.EndTime >= endTime) ||     // Suprapunere la sfârșit
                             (r.StartTime >= startTime && r.EndTime <= endTime)))    // Suprapunere completă
                .AnyAsync();

            return !overlappingReservations; // Disponibil dacă nu există rezervări suprapuse
        }
    }
}