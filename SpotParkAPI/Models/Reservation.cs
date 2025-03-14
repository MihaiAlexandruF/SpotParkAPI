using System;
using System.Collections.Generic;

namespace SpotParkAPI.Models;

public partial class Reservation
{
    public int ReservationId { get; set; }

    public int DriverId { get; set; }

    public int ParkingLotId { get; set; }

    public DateTime StartTime { get; set; }

    public DateTime EndTime { get; set; }

    public decimal TotalCost { get; set; }

    public string? Status { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual User Driver { get; set; } = null!;

    public virtual ParkingLot ParkingLot { get; set; } = null!;

    public virtual ICollection<Payment> Payments { get; set; } = new List<Payment>();
}
