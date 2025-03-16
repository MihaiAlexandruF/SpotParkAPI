using System;
using System.Collections.Generic;

namespace SpotParkAPI.Models.Entities;

public partial class ParkingLot
{
    public int ParkingLotId { get; set; }

    public int OwnerId { get; set; }

    public string Address { get; set; } = null!;

    public decimal PricePerHour { get; set; }

    public string? Description { get; set; }

    public decimal Latitude { get; set; }

    public decimal Longitude { get; set; }

    public DateTime? CreatedAt { get; set; }

    public DateTime? UpdatedAt { get; set; }

    public virtual ICollection<AvailabilitySchedule> AvailabilitySchedules { get; set; } = new List<AvailabilitySchedule>();

    public virtual User? Owner { get; set; } 

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
}
