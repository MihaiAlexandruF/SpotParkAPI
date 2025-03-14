using System;
using System.Collections.Generic;

namespace SpotParkAPI.Models;

public partial class AvailabilitySchedule
{
    public int ScheduleId { get; set; }

    public int ParkingLotId { get; set; }

    public string? AvailabilityType { get; set; }

    public string DayOfWeek { get; set; } = null!;

    public TimeOnly OpenTime { get; set; }

    public TimeOnly CloseTime { get; set; }

    public virtual ParkingLot ParkingLot { get; set; } = null!;
}
