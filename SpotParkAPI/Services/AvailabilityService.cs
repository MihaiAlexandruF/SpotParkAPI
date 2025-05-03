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
                // Ștergem toate programele existente
                foreach (var schedule in existingSchedules)
                {
                    await _availabilityRepository.DeleteAsync(schedule.ScheduleId);
                }
                break;

            case "daily":
                if (!request.DailyOpenTime.HasValue || !request.DailyCloseTime.HasValue)
                    throw new ArgumentException("Daily open and close times are required for daily availability");

                var open = TimeOnly.Parse(request.DailyOpenTime.Value.ToString(@"hh\:mm"));
                var close = TimeOnly.Parse(request.DailyCloseTime.Value.ToString(@"hh\:mm"));

                foreach (var day in new[] { "Monday", "Tuesday", "Wednesday", "Thursday", "Friday", "Saturday", "Sunday" })
                {
                    var schedule = new AvailabilitySchedule
                    {
                        ParkingLotId = parkingLotId,
                        AvailabilityType = "daily",
                        DayOfWeek = day,
                        OpenTime = open,
                        CloseTime = close,
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
            throw new ArgumentException("Availability type is required.");
        }

        var type = request.AvailabilityType.ToLower();

        if (type == "daily")
        {
            if (!request.DailyOpenTime.HasValue || !request.DailyCloseTime.HasValue)
            {
                throw new ArgumentException("Daily open and close times are required.");
            }

            if (request.DailyOpenTime.Value >= request.DailyCloseTime.Value)
            {
                throw new ArgumentException("Daily open time must be before close time.");
            }
        }
        else if (type == "weekly")
        {
            if (request.WeeklySchedules == null || request.WeeklySchedules.Count == 0)
            {
                throw new ArgumentException("Weekly schedules are required.");
            }

            foreach (var schedule in request.WeeklySchedules)
            {
                if (schedule.OpenTime >= schedule.CloseTime)
                {
                    throw new ArgumentException($"On {schedule.DayOfWeek}, open time must be before close time.");
                }
            }
        }
    }

}