using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models;

public class AppDbContext : DbContext
{
    public AppDbContext(DbContextOptions<AppDbContext> options) : base(options) { }

    public DbSet<User> Users { get; set; }
    public DbSet<ParkingLot> ParkingLots { get; set; }
    public DbSet<AvailabilitySchedule> AvailabilitySchedules { get; set; }
    public DbSet<Reservation> Reservations { get; set; }
    public DbSet<Payment> Payments { get; set; }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        // User entity configuration
        modelBuilder.Entity<User>()
            .HasIndex(u => u.Email)
            .IsUnique();

        modelBuilder.Entity<User>()
            .HasIndex(u => u.Username)
            .IsUnique();

        // ParkingLot entity configuration
        modelBuilder.Entity<ParkingLot>()
            .HasOne(p => p.Owner)
            .WithMany(u => u.ParkingLots)
            .HasForeignKey(p => p.OwnerId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // AvailabilitySchedule entity configuration
        modelBuilder.Entity<AvailabilitySchedule>()
            .HasOne(a => a.ParkingLot)
            .WithMany(p => p.AvailabilitySchedules)
            .HasForeignKey(a => a.ParkingLotId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Reservation entity configuration
        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.Driver)
            .WithMany(u => u.Reservations)
            .HasForeignKey(r => r.DriverId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        modelBuilder.Entity<Reservation>()
            .HasOne(r => r.ParkingLot)
            .WithMany(p => p.Reservations)
            .HasForeignKey(r => r.ParkingLotId)
            .OnDelete(DeleteBehavior.ClientSetNull);

        // Payment entity configuration
        modelBuilder.Entity<Payment>()
            .HasOne(p => p.Reservation)
            .WithMany(r => r.Payments)
            .HasForeignKey(p => p.ReservationId)
            .OnDelete(DeleteBehavior.ClientSetNull);
    }
}