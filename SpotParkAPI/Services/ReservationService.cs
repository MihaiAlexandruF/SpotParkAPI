using AutoMapper;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;
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
        private readonly SpotParkDbContext _context;
        private readonly IReservationRepository _reservationRepository;
        private readonly IWalletRepository _walletRepository;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;
        private readonly IWalletService _walletService;

        public ReservationService(
            SpotParkDbContext context,
            IWalletService walletService,
            IWalletRepository walletRepository,
            IReservationRepository reservationRepository,
            IMapper mapper,
            ICommonService commonService)
        {
            _context = context;
            _walletService = walletService;
            _walletRepository = walletRepository;
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _commonService = commonService;
        }

        public async Task<ServiceResult<ReservationDto>> ReserveParkingLotAsync(CreateReservationRequest request)
        {
            var userId = _commonService.GetCurrentUserId();

            if (request.Hours < 1 || request.Hours > 24)
                return ServiceResult<ReservationDto>.Fail("Numărul de ore trebuie să fie între 1 și 24.");

            var parkingLot = await _commonService.GetParkingLotByIdAsync(request.ParkingLotId);
            if (parkingLot == null)
                return ServiceResult<ReservationDto>.Fail("Locul de parcare nu a fost găsit.");

            var startTimeUtc = DateTime.UtcNow;
            var endTimeUtc = startTimeUtc.AddHours(request.Hours);

            var isAvailableInSchedule = await _commonService.IsParkingLotAvailableAsync(request.ParkingLotId, startTimeUtc, endTimeUtc);
            if (!isAvailableInSchedule)
                return ServiceResult<ReservationDto>.Fail("Locul de parcare nu este disponibil în acest interval.");

            var isFreeOfReservations = await _reservationRepository.IsParkingLotAvailableAsync(request.ParkingLotId, startTimeUtc, endTimeUtc);
            if (!isFreeOfReservations)
                return ServiceResult<ReservationDto>.Fail("Locul de parcare este deja rezervat în acest interval.");

            var totalPrice = Math.Round(parkingLot.PricePerHour * request.Hours, 2);
            var commission = Math.Round(totalPrice * 0.25m, 2);
            var ownerRevenue = totalPrice - commission;

            var plate = await _commonService.GetUserPlateAsync(userId, request.PlateId);
            if (plate == null)
                return ServiceResult<ReservationDto>.Fail("Numărul de înmatriculare nu aparține acestui cont.");

            if (request.PaymentMethod == "wallet")
            {
                var balance = await _walletService.GetBalanceAsync(userId);
                if (balance < totalPrice)
                    return ServiceResult<ReservationDto>.Fail("Fonduri insuficiente în portofel.");
            }

            var reservation = new Reservation
            {
                DriverId = userId,
                ParkingLotId = request.ParkingLotId,
                StartTime = startTimeUtc,
                EndTime = endTimeUtc,
                TotalCost = totalPrice,
                Status = "active",
                CreatedAt = DateTime.UtcNow,
                LicensePlate = plate.PlateNumber
            };

            var strategy = _context.Database.CreateExecutionStrategy();
            ServiceResult<ReservationDto> result = null;

            await strategy.ExecuteAsync(async () =>
            {
                await using var transaction = await _context.Database.BeginTransactionAsync();

                _context.Reservations.Add(reservation);
                await _context.SaveChangesAsync();

                if (request.PaymentMethod is "wallet" or "balance")
                {
                    var debit = await _walletService.AddTransactionAsync(
                        userId,
                        totalPrice,
                        WalletTransactionType.ReservationPayment,
                        "out",
                        $"Plată rezervare la {parkingLot.Address}",
                        reservation.ReservationId);

                    if (!debit.Success)
                    {
                        await transaction.RollbackAsync();
                        result = ServiceResult<ReservationDto>.Fail(debit.ErrorMessage);
                        return;
                    }

                    var credit = await _walletService.AddTransactionAsync(
                        parkingLot.OwnerId,
                        ownerRevenue,
                        WalletTransactionType.Earning,
                        "in",
                        $"Venit rezervare la {parkingLot.Address}",
                        reservation.ReservationId);

                    if (!credit.Success)
                    {
                        await transaction.RollbackAsync();
                        result = ServiceResult<ReservationDto>.Fail(credit.ErrorMessage);
                        return;
                    }

                    var platformUser = await _context.Users.FirstOrDefaultAsync(u => u.Role == "Platform");
                    if (platformUser == null)
                    {
                        await transaction.RollbackAsync();
                        result = ServiceResult<ReservationDto>.Fail("Utilizatorul platformei nu a fost găsit.");
                        return;
                    }

                    var commissionTx = await _walletService.AddTransactionAsync(
                        platformUser.UserId,
                        commission,
                        WalletTransactionType.Commission,
                        "in",
                        $"Comision aplicație pentru rezervare la {parkingLot.Address}",
                        reservation.ReservationId);

                    if (!commissionTx.Success)
                    {
                        await transaction.RollbackAsync();
                        result = ServiceResult<ReservationDto>.Fail(commissionTx.ErrorMessage);
                        return;
                    }
                }

                await transaction.CommitAsync();
            });

            if (result != null)
                return result;

            return ServiceResult<ReservationDto>.Ok(_mapper.Map<ReservationDto>(reservation));
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
            var reservations = await _reservationRepository.GetActiveReservationsForOwnerAsync(ownerId, now);

            return reservations.Select(r => new ActiveClientDto
            {
                LicensePlate = string.IsNullOrEmpty(r.LicensePlate) ? "Necunoscut" : r.LicensePlate,
                ParkingSpot = r.ParkingLot?.Address ?? "Necunoscut",
                StartTimeUtc = r.StartTime,
                EndTimeUtc = r.EndTime
            }).ToList();
        }


        public async Task<List<UserReservationDto>> GetActiveReservationsAsync()
        {
            var userId = _commonService.GetCurrentUserId();
            var reservations = await _reservationRepository.GetUserReservationsAsync(userId, true);

            return reservations.Select(r => new UserReservationDto
            {
                ReservationId = r.ReservationId,
                ParkingLotId = r.ParkingLotId,
                Address = r.ParkingLot?.Address ?? "",
                LicensePlate = r.LicensePlate ?? "",
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                TotalCost = r.TotalCost,
                Status = r.Status ?? ""
            }).ToList();
        }

        public async Task<List<UserReservationDto>> GetPastReservationsAsync()
        {
            var userId = _commonService.GetCurrentUserId();
            var reservations = await _reservationRepository.GetUserReservationsAsync(userId, false);

            return reservations.Select(r => new UserReservationDto
            {
                ReservationId = r.ReservationId,
                ParkingLotId = r.ParkingLotId,
                Address = r.ParkingLot?.Address ?? "",
                LicensePlate = r.LicensePlate ?? "",
                StartTime = r.StartTime,
                EndTime = r.EndTime,
                TotalCost = r.TotalCost,
                Status = r.Status ?? ""
            }).ToList();
        }



    }
}
