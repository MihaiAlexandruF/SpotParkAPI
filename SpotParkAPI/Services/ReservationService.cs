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

        public ReservationService(
            IWalletRepository walletRepository,
            IReservationRepository reservationRepository,
            IMapper mapper,
            ICommonService commonService)
        {
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


            // Comision 10%
            var commission = totalPrice * 0.10m;
            var ownerRevenue = totalPrice - commission;

            if (request.PaymentMethod == "wallet")
            {
                var driverWallet = await _walletRepository.GetByUserIdAsync(userId);
                if (driverWallet == null || driverWallet.Balance < totalPrice)
                    throw new InvalidOperationException("Fonduri insuficiente în portofelul electronic.");

                driverWallet.Balance -= totalPrice;

                var ownerWallet = await _walletRepository.GetByUserIdAsync(parkingLot.OwnerId);
                if (ownerWallet == null)
                    throw new InvalidOperationException("Proprietarul nu are portofel electronic disponibil.");

                ownerWallet.Balance += ownerRevenue;
            }

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

            return _mapper.Map<ReservationDto>(reservation);
        }


        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            return await _commonService.IsParkingLotAvailableAsync(parkingLotId, startTime, endTime);
        }


        public async Task<List<ActiveClientDto>> GetActiveClientsAsync(int ownerId)
        {
            var now = DateTime.UtcNow;

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