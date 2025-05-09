using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Interfaces;
using System;
using System.Globalization;
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

            var isAvailableInSchedule = await _commonService.IsParkingLotAvailableAsync(request.ParkingLotId, request.StartTime, request.EndTime);
            if (!isAvailableInSchedule)
                throw new InvalidOperationException("Locul de parcare nu este disponibil în intervalul selectat.");

            var isFreeOfReservations = await _reservationRepository.IsParkingLotAvailableAsync(request.ParkingLotId, request.StartTime, request.EndTime);
            if (!isFreeOfReservations)
                throw new InvalidOperationException("Locul de parcare este deja rezervat în acest interval de timp.");

            var durationHours = (request.EndTime - request.StartTime).TotalMinutes / 60.0;
            if (durationHours <= 0)
                throw new InvalidOperationException("Ora de sfârșit trebuie să fie după ora de început.");

            var totalPrice = (decimal)durationHours * parkingLot.PricePerHour;

            // Verificăm că userul deține plăcuța
            var plate = await _commonService.GetUserPlateAsync(userId, request.PlateId);
            if (plate == null)
                throw new InvalidOperationException("Numărul de înmatriculare nu aparține acestui cont.");

            // Comision 25%
            var commission = Math.Round(totalPrice * 0.25m, 2);
            var ownerRevenue = totalPrice - commission;

            // Creează rezervarea
            var reservation = new Reservation
            {
                DriverId = userId,
                ParkingLotId = request.ParkingLotId,
                StartTime = request.StartTime,
                EndTime = request.EndTime,
                TotalCost = totalPrice,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                LicensePlate = plate.PlateNumber
            };

            await _reservationRepository.AddReservationAsync(reservation);

            if (request.PaymentMethod == "wallet")
            {
                // Debitare client
                await _walletService.AddTransactionAsync(
                    userId,
                    totalPrice,
                    WalletTransactionType.ReservationPayment,
                    "out",
                    $"Plată rezervare la {parkingLot.Address}",
                    reservation.ReservationId
                );

                // Creditare proprietar
                await _walletService.AddTransactionAsync(
                    parkingLot.OwnerId,
                    ownerRevenue,
                    WalletTransactionType.Earning,
                    "in",
                    $"Venit rezervare la {parkingLot.Address}",
                    reservation.ReservationId
                );
            }

            return _mapper.Map<ReservationDto>(reservation);
        }



        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            return await _commonService.IsParkingLotAvailableAsync(parkingLotId, startTime, endTime);
        }


        public async Task<List<ActiveClientDto>> GetActiveClientsAsync(int ownerId)
        {
            var now = DateTime.UtcNow;
            Console.WriteLine($"[ActiveClients] NOW UTC: {now}");


            var reservations = await _reservationRepository.GetActiveReservationsForOwnerAsync(ownerId, now);

            return reservations.Select(r => new ActiveClientDto
            {
                LicensePlate = r.LicensePlate,
                ParkingSpot = r.ParkingLot.Address,
                StartTime = r.StartTime.ToString("HH:mm", CultureInfo.InvariantCulture),
                Duration = Math.Ceiling((r.EndTime - r.StartTime).TotalHours).ToString("0") + "h"
            }).ToList();
        }

    }
}