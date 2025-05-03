using System;
using System.Collections.Generic;

namespace SpotParkAPI.Models.Entities;

public partial class User
{
    public int UserId { get; set; }

    public string Username { get; set; } = null!;

    public string PasswordHash { get; set; } = null!;

    public string Email { get; set; } = null!;

    public string? Phone { get; set; }

    public DateTime? CreatedAt { get; set; }

    public virtual ICollection<ParkingLot> ParkingLots { get; set; } = new List<ParkingLot>();

    public virtual ICollection<Reservation> Reservations { get; set; } = new List<Reservation>();
    public virtual ICollection<ParkingLotImage> ParkingLotImages { get; set; } = new List<ParkingLotImage>();

    public virtual ICollection<UserVehicle> UserVehicles { get; set; } = new List<UserVehicle>();

}
