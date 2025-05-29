using SpotParkAPI.Models.Entities;
using SpotParkAPI.Models.Requests;
using SpotParkAPI.Repositories.Interfaces;
using SpotParkAPI.Services.Interfaces;

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
        var existingSchedules = await _availabilityRepository.GetByParkingLotIdAsync(parkingLotId);
        foreach (var schedule in existingSchedules)
        {
            await _availabilityRepository.DeleteAsync(schedule.ScheduleId);
        }

        Console.WriteLine($"[Availability] Type={request.AvailabilityType}, Open={request.DailyOpenTime}, Close={request.DailyCloseTime}");

        switch (request.AvailabilityType.ToLower())
        {
            case "always":
                break;

            case "daily":
                if (!request.DailyOpenTime.HasValue || !request.DailyCloseTime.HasValue)
                    throw new ArgumentException("Daily open and close times are required for daily availability");

                if (request.DailyOpenTime.Value >= request.DailyCloseTime.Value)
                    throw new ArgumentException("Daily open time must be before close time.");

                foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
                {
                    var schedule = new AvailabilitySchedule
                    {
                        ParkingLotId = parkingLotId,
                        AvailabilityType = "daily",
                        DayOfWeek = day,
                        OpenTime = TimeOnly.FromTimeSpan(request.DailyOpenTime.Value),
                        CloseTime = TimeOnly.FromTimeSpan(request.DailyCloseTime.Value),
                    };
                    await _availabilityRepository.AddAsync(schedule);
                }
                break;

            case "weekly":
                if (request.WeeklySchedules == null || request.WeeklySchedules.Count == 0)
                    throw new ArgumentException("Weekly schedules are required for weekly availability");

                foreach (var weeklySchedule in request.WeeklySchedules)
                {
                    if (weeklySchedule.OpenTime >= weeklySchedule.CloseTime)
                        throw new ArgumentException($"On {weeklySchedule.DayOfWeek}, open time must be before close time.");

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
}
