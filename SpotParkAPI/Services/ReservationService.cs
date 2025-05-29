using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Helpers;
using SpotParkAPI.Services.Interfaces;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace SpotParkAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;
        private readonly IWalletService _walletService;

        public ReservationService(
            IWalletService walletService,
            IWalletRepository walletRepository,
            IReservationRepository reservationRepository,
            IMapper mapper,
            ICommonService commonService)
        {
            _walletService = walletService;
            _walletRepository = walletRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _commonService = commonService;
        }

        public async Task<ReservationDto> ReserveParkingLotAsync(CreateReservationRequest request)
        {
            var userId = _commonService.GetCurrentUserId();
            var parkingLot = await _commonService.GetParkingLotByIdAsync(request.ParkingLotId);

            // Convertire date locale în UTC
            var localStartTime = TimeZoneService.ConvertLocalToUtc(request.StartTime);
            var localEndTime = TimeZoneService.ConvertLocalToUtc(request.EndTime);

            var isAvailableInSchedule = await _commonService.IsParkingLotAvailableAsync(request.ParkingLotId, localStartTime, localEndTime);
            if (!isAvailableInSchedule)
                throw new InvalidOperationException("Locul de parcare nu este disponibil în intervalul selectat.");

            var isFreeOfReservations = await _reservationRepository.IsParkingLotAvailableAsync(request.ParkingLotId, localStartTime, localEndTime);
            if (!isFreeOfReservations)
                throw new InvalidOperationException("Locul de parcare este deja rezervat în acest interval de timp.");

            var durationHours = (localEndTime - localStartTime).TotalMinutes / 60.0;
            if (durationHours <= 0)
                throw new InvalidOperationException("Ora de sfârșit trebuie să fie după ora de început.");

            var totalPrice = (decimal)durationHours * parkingLot.PricePerHour;

            var plate = await _commonService.GetUserPlateAsync(userId, request.PlateId);
            if (plate == null)
                throw new InvalidOperationException("Numărul de înmatriculare nu aparține acestui cont.");

            var commission = Math.Round(totalPrice * 0.25m, 2);
            var ownerRevenue = totalPrice - commission;

            var reservation = new Reservation
            {
                DriverId = userId,
                ParkingLotId = request.ParkingLotId,
                StartTime = localStartTime,
                EndTime = localEndTime,
                TotalCost = totalPrice,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                LicensePlate = plate.PlateNumber
            };

            await _reservationRepository.AddReservationAsync(reservation);

            if (request.PaymentMethod == "wallet")
            {
                await _walletService.AddTransactionAsync(userId, totalPrice, WalletTransactionType.ReservationPayment, "out", $"Plată rezervare la {parkingLot.Address}", reservation.ReservationId);
                await _walletService.AddTransactionAsync(parkingLot.OwnerId, ownerRevenue, WalletTransactionType.Earning, "in", $"Venit rezervare la {parkingLot.Address}", reservation.ReservationId);
            }

            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            var localStartTime = TimeZoneService.ConvertLocalToUtc(startTime);
            var localEndTime = TimeZoneService.ConvertLocalToUtc(endTime);

            return await _commonService.IsParkingLotAvailableAsync(parkingLotId, localStartTime, localEndTime);
        }

        public async Task<List<ActiveClientDto>> GetActiveClientsAsync(int ownerId)
        {
            var now = DateTime.UtcNow;
            Console.WriteLine($"[ActiveClients] NOW UTC: {now}");

            var reservations = await _reservationRepository.GetActiveReservationsForOwnerAsync(ownerId, now);
            Console.WriteLine($"[ActiveClients] Found {reservations.Count} active reservations.");

            return reservations.Select(r => new ActiveClientDto
            {
                LicensePlate = string.IsNullOrEmpty(r.LicensePlate) ? "Necunoscut" : r.LicensePlate,
                ParkingSpot = r.ParkingLot?.Address ?? "Necunoscut",
                StartTime = r.StartTime.ToLocalTime().ToString("HH:mm"),
                Duration = Math.Ceiling((r.EndTime - r.StartTime).TotalHours).ToString("0") + "h"
            }).ToList();
        }
    }
}
