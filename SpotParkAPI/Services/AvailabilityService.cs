using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories;
using SpotParkAPI.Services;


public class AvailabilityService : IAvailabilityService
{
    private readonly IAvailabilityRepository _availabilityRepository;

    public AvailabilityService(IAvailabilityRepository availabilityRepository)
    {
        _availabilityRepository = availabilityRepository;
    }

    public async Task<List<AvailabilitySchedule>> GetAvailabilitySchedulesAsync(int parkingLotId)
    {
        return await _availabilityRepository.GetByParkingLotIdAsync(parkingLotId);
    }

    public async Task UpdateAvailabilityAsync(int parkingLotId, AvailabilitySchedulesRequest request)
    {
        ValidateAvailabilityRequest(request);

        // Șterge programele de disponibilitate existente
        var existingSchedules = await _availabilityRepository.GetByParkingLotIdAsync(parkingLotId);
        foreach (var schedule in existingSchedules)
        {
            await _availabilityRepository.DeleteAsync(schedule.ScheduleId);
        }

        // Adaugă noile programe de disponibilitate
        switch (request.AvailabilityType.ToLower())
        {
            case "always":
                // Nu este nevoie de programe pentru disponibilitatea permanentă
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
                        ParkingLotId = parkingLotId,
                        AvailabilityType = "daily",
                        DayOfWeek = day,
                        OpenTime = TimeOnly.FromTimeSpan(request.DailyOpenTime.Value),
                        CloseTime = TimeOnly.FromTimeSpan(request.DailyCloseTime.Value)
                    };
                    await _availabilityRepository.AddAsync(schedule);
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
                        ParkingLotId = parkingLotId,
                        AvailabilityType = "weekly",
                        DayOfWeek = weeklySchedule.DayOfWeek,
                        OpenTime = TimeOnly.FromTimeSpan(weeklySchedule.OpenTime),
                        CloseTime = TimeOnly.FromTimeSpan(weeklySchedule.CloseTime)
                    };
                    await _availabilityRepository.AddAsync(schedule);
                }
                break;
            default:
                throw new ArgumentException($"Unsupported availability type: {request.AvailabilityType}");
        }
    }

    private void ValidateAvailabilityRequest(AvailabilitySchedulesRequest request)
    {
        if (string.IsNullOrWhiteSpace(request.AvailabilityType))
        {
            throw new ArgumentException("Availability type is required");
        }
    }
}