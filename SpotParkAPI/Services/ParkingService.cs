using System;
using Microsoft.EntityFrameworkCore;
using System.Threading.Tasks;
using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using AutoMapper;
using SpotParkAPI.Models.Dtos;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Interfaces;
using SpotParkAPI.Repositories;
using SpotParkAPI.Services.Helpers;

namespace SpotParkAPI.Services
{
    public class ParkingService : IParkingService
    {
        private readonly IParkingRepository _parkingRepository;
        private readonly IMapper _mapper;
        private readonly IParkingImageRepository _parkingImageRepository;
        private readonly ParkingImageService _parkingImageService;


        public ParkingService(IParkingRepository parkingRepository,IMapper mapper,IParkingImageRepository parkingImageRepository,ParkingImageService parkingImageService )
        {
            _parkingRepository = parkingRepository;
            _mapper = mapper;
            _parkingImageRepository = parkingImageRepository;
            _parkingImageService = parkingImageService;
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
            var parkingLot = new ParkingLot
            {
                OwnerId = ownerId,
                Address = request.Address,
                PricePerHour = request.PricePerHour,
                Description = request.Description,
                Latitude = request.Latitude,
                Longitude = request.Longitude,
                CreatedAt = DateTime.Now,
                UpdatedAt = DateTime.Now,
                IsActive = true 
            };
            await _parkingRepository.AddParkingLotAsync(parkingLot);
            switch (request.AvailabilityType.ToLower())
            {
                case "always":
                    
                    break;
                case "daily":
                    
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

        public async Task<List<ParkingLotDto>> GetParkingLotsByOwnerIdAsync(int ownerId)
        {
            var parkingLots = await _parkingRepository.GetParkingLotsByOwnerIdAsync(ownerId);
            return _mapper.Map<List<ParkingLotDto>>(parkingLots);
        }

        public async Task<ServiceResult<ParkingLotDto>> GetParkingLotDetailsByIdAsync(int id)
        {
            var parkingLot = await _parkingRepository.GetParkingLotByIdAsync(id);
            if (parkingLot == null)
                return ServiceResult<ParkingLotDto>.Fail("Parcarea nu a fost găsită.");

            var dto = _mapper.Map<ParkingLotDto>(parkingLot);

            var schedules = await _parkingRepository.GetAvailabilitySchedulesByParkingLotIdAsync(id);
            dto.AvailabilitySchedules = _mapper.Map<List<AvailabilityScheduleDto>>(schedules);

            var images = await _parkingImageRepository.GetImagesForParkingLotAsync(id);
            dto.Images = images.Select(img => new ParkingLotImageDto
            {
                ImageId = img.ImageId,
                UploadedAt = img.UploadedAt,
                ImageUrl = $"{GetBaseUrl()}/{img.ImagePath.Replace("\\", "/")}"
            }).ToList();

            return ServiceResult<ParkingLotDto>.Ok(dto);
        }


        private string GetBaseUrl()
        {
            return "http://192.168.0.126:5000"; 
        }

        public async Task<List<ParkingLotForOwnerDto>> GetParkingLotsForOwnerDashboardAsync(int ownerId)
        {
            var parkingLots = await _parkingRepository.GetFullParkingLotsByOwnerIdAsync(ownerId);
            var result = new List<ParkingLotForOwnerDto>();

            foreach (var lot in parkingLots)
            {
                var earnings = await _parkingRepository.GetTotalEarningsForParkingLotAsync(lot.ParkingLotId);

                var dto = new ParkingLotForOwnerDto
                {
                    ParkingLotId = lot.ParkingLotId,
                    Address = lot.Address,
                    Description = lot.Description,
                    PricePerHour = lot.PricePerHour,
                    Latitude = (double)lot.Latitude,
                    Longitude = (double)lot.Longitude,
                    IsActive = lot.IsActive,
                    AvailabilitySchedules = _mapper.Map<List<AvailabilityScheduleDto>>(lot.AvailabilitySchedules),

                    ImageUrls = lot.Images?.Select(img =>
                        $"{GetBaseUrl()}/{img.ImagePath.Replace("\\", "/")}"
                    ).ToList() ?? new List<string>(),

                    Earnings = (double)earnings
                };

                result.Add(dto);
            }

            return result;
        }

        public async Task<bool> ToggleParkingLotActiveStatusAsync(int userId, int parkingLotId)
        {
            var parkingLot = await _parkingRepository.GetParkingLotByIdAsync(parkingLotId);
            if (parkingLot == null)
                throw new KeyNotFoundException("Locul de parcare nu există.");

            if (parkingLot.OwnerId != userId)
                throw new UnauthorizedAccessException("Nu ești proprietarul acestui loc de parcare.");

            var newStatus = !parkingLot.IsActive;

            await _parkingRepository.SetParkingLotActiveStatusAsync(parkingLotId, newStatus);

            return newStatus;
        }

        public async Task<List<ParkingLotMapPreviewDto>> GetParkingLotMapPreviewsAsync()
        {
            var parkingLots = await _parkingRepository.GetParkingLotsAsync();
            return parkingLots.Select(lot => new ParkingLotMapPreviewDto
            {
                ParkingLotId = lot.ParkingLotId,
                PricePerHour = lot.PricePerHour,
                Latitude = (double)lot.Latitude,
                Longitude = (double)lot.Longitude
            }).ToList();
        }

        public async Task<List<ParkingLotMapPreviewDto>> GetAvailableMapPreviewsAsync(DateTime localNow, DateTime utcNow)
        {
            var availableLots = await _parkingRepository.GetActiveAvailableAndUnreservedParkingLotsAsync(localNow, utcNow);

            return availableLots.Select(lot => new ParkingLotMapPreviewDto
            {
                ParkingLotId = lot.ParkingLotId,
                PricePerHour = lot.PricePerHour,
                Latitude = (double)lot.Latitude,
                Longitude = (double)lot.Longitude
            }).ToList();
        }




        public async Task<ParkingLotDto> CreateCompleteParkingLotAsync(CompleteParkingLotRequest request, int userId)
        {
            var coreDto = new CreateParkingLotRequest
            {
                Address = request.Address,
                Description = request.Description,
                Latitude = (decimal)request.Latitude,
                Longitude =(decimal) request.Longitude,
                PricePerHour =(decimal)request.PricePerHour,
                AvailabilityType = request.AvailabilityType,
                DailyOpenTime = request.DailyOpenTime,
                DailyCloseTime = request.DailyCloseTime,
                WeeklySchedules = request.WeeklySchedules
            };

            var parkingLot = await CreateParkingLotWithAvailabilityAsync(coreDto, userId);

            if (request.Images != null)
            {
                foreach (var file in request.Images)
                {
                    await _parkingImageService.UploadImageAsync(parkingLot.ParkingLotId, file);
                }
            }


            return parkingLot;
        }

        Task<ParkingLotDto> IParkingService.GetParkingLotDetailsByIdAsync(int id)
        {
            throw new NotImplementedException();
        }
    }
}