using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories;
using System;
using System.Threading.Tasks;

namespace SpotParkAPI.Services
{
    public class ReservationService : IReservationService
    {
        private readonly IReservationRepository _reservationRepository;
        private readonly IMapper _mapper;
        private readonly ICommonService _commonService;

        public ReservationService(
            IReservationRepository reservationRepository,
            IMapper mapper,
            ICommonService commonService)
        {
            _reservationRepository = reservationRepository;
            _mapper = mapper;
            _commonService = commonService;
        }

        public async Task<ReservationDto> ReserveParkingLotAsync(ReserveRequest request)
        {
            // Obține ID-ul utilizatorului curent
            var userId = _commonService.GetCurrentUserId();

            // Verifică disponibilitatea programată
            var parkingLot = await _commonService.GetParkingLotByIdAsync(request.ParkingLotId);

            // Verifică disponibilitatea reală (rezervări existente)
            var isAvailable = await _commonService.IsParkingLotAvailableAsync(request.ParkingLotId, request.StartTime, request.EndTime);
            if (!isAvailable)
            {
                throw new InvalidOperationException("Parking lot is not available for the selected time.");
            }

            // Calculează numărul de ore
            var duration = request.EndTime - request.StartTime;
            var totalHours = (decimal)duration.TotalHours;

            // Calculează prețul total
            var totalPrice = parkingLot.PricePerHour * totalHours;

            // Transformă ReserveRequest în Reservation folosind AutoMapper
            var reservation = _mapper.Map<Reservation>(request);
            reservation.DriverId = userId; // Setează ID-ul utilizatorului
            reservation.TotalCost = totalPrice; // Adaugă prețul total
            reservation.Status = "active";
            reservation.CreatedAt = DateTime.Now;

            await _reservationRepository.AddReservationAsync(reservation);

            // Transformă Reservation în ReservationDto pentru răspuns
            return _mapper.Map<ReservationDto>(reservation);
        }

        public async Task<bool> IsParkingLotAvailableAsync(int parkingLotId, DateTime startTime, DateTime endTime)
        {
            // Folosește CommonService pentru a verifica disponibilitatea
            return await _commonService.IsParkingLotAvailableAsync(parkingLotId, startTime, endTime);
        }
    }
}