using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Repositories.Interfaces;

namespace SpotParkAPI.Services
{
    public class ParkingService
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;


        public ParkingService(IParkingRepository parkingRepository,IMapper mapper)
        {
            _parkingRepository = parkingRepository;
            _mapper = mapper;

        }

        public async Task<List<ParkingLot>> GetParkingLotsAsync()
        {
            var parkingLots = await _parkingRepository.GetParkingLotsAsync();
            return _mapper.Map<List<ParkingLot>>(parkingLots);
        }

        public async Task<ParkingLotDto> GetParkingLotByIdAsync(int id)
        {
            var parkingLot = await _parkingRepository.GetParkingLotByIdAsync(id);
            if (parkingLot == null)
            {
                throw new KeyNotFoundException($"Parking lot with ID {id} not found");
            }

            return _mapper.Map<ParkingLotDto>(parkingLot);
        }

        public async Task AddParkingLotAsync(ParkingLot parkingLot)
        {
            await _parkingRepository.AddParkingLotAsync(parkingLot);
        }

        public async Task<ParkingLotDto> CreateParkingLotWithAvailabilityAsync(CreateParkingLotRequest request, int ownerId)
        {
            ValidateParkingLotRequest(request);

            // Create the parking lot
            var parkingLot = new ParkingLot
            {
                OwnerId = ownerId,
                Address = request.Address,
                PricePerHour = request.PricePerHour,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now
            };

            await _parkingRepository.AddParkingLotAsync(parkingLot);

            // Create availability schedules based on the type
            switch (request.AvailabilityType.ToLower())
            {
                case "always":
                    // Always available - no need to create schedules
                    break;
                case "daily":
                    // Same schedule every day
                    if (!request.DailyOpenTime.HasValue || !request.DailyCloseTime.HasValue)
                    {
                        throw new ArgumentException("Daily open and close times are required for daily availability");
                    }

                    foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
                    {
                        var schedule = new AvailabilitySchedule
                        {
                            ParkingLotId = parkingLot.ParkingLotId,
                            AvailabilityType = "daily",
                            DayOfWeek = day,
                            OpenTime = TimeOnly.FromTimeSpan(request.DailyOpenTime.Value),
                            CloseTime = TimeOnly.FromTimeSpan(request.DailyCloseTime.Value)
                        };
                        await _parkingRepository.AddAvailabilityScheduleAsync(schedule);
                    }
                    break;
                case "weekly":
                    // Different schedule for each day of the week
                    if (request.WeeklySchedules == null || request.WeeklySchedules.Count == 0)
                    {
                        throw new ArgumentException("Weekly schedules are required for weekly availability");
                    }

                    foreach (var weeklySchedule in request.WeeklySchedules)
                    {
                        var schedule = new AvailabilitySchedule
                        {
                            ParkingLotId = parkingLot.ParkingLotId,
                            AvailabilityType = "weekly",
                            DayOfWeek = weeklySchedule.DayOfWeek,
                            OpenTime = TimeOnly.FromTimeSpan(weeklySchedule.OpenTime),
                            CloseTime = TimeOnly.FromTimeSpan(weeklySchedule.CloseTime)
                        };
                        await _parkingRepository.AddAvailabilityScheduleAsync(schedule);
                    }
                    break;
                default:
                    throw new ArgumentException($"Unsupported availability type: {request.AvailabilityType}");
            }

            return _mapper.Map<ParkingLotDto>(parkingLot);
        }

        public async Task<List<AvailabilityScheduleDto>> GetAvailabilitySchedulesAsync(int parkingLotId)
        {
            var parkingLot = await _parkingRepository.GetParkingLotByIdAsync(parkingLotId);
            if (parkingLot == null)
            {
                throw new KeyNotFoundException($"Parking lot with ID {parkingLotId} not found");
            }

            var schedules = await _parkingRepository.GetAvailabilitySchedulesByParkingLotIdAsync(parkingLotId);
            return _mapper.Map<List<AvailabilityScheduleDto>>(schedules);
        }


        private void ValidateParkingLotRequest(CreateParkingLotRequest request)
        {
            if (string.IsNullOrWhiteSpace(request.Address))
            {
                throw new ArgumentException("Address is required");
            }

            if (request.PricePerHour <= 0)
            {
                throw new ArgumentException("Price per hour must be greater than zero");
            }

            if (request.Latitude < -90 || request.Latitude > 90)
            {
                throw new ArgumentException("Latitude must be between -90 and 90");
            }

            if (request.Longitude < -180 || request.Longitude > 180)
            {
                throw new ArgumentException("Longitude must be between -180 and 180");
            }

            if (string.IsNullOrWhiteSpace(request.AvailabilityType))
            {
                throw new ArgumentException("Availability type is required");
            }
        }

    }
}