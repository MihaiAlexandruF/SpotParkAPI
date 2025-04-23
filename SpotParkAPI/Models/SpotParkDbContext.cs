using System;
using System.Collections.Generic;
using Microsoft.EntityFrameworkCore;
using SpotParkAPI.Models.Entities;

namespace SpotParkAPI.Models;

public partial class SpotParkDbContext : DbContext
{
    public SpotParkDbContext()
    {
    }

    public SpotParkDbContext(DbContextOptions<SpotParkDbContext> options)
        : base(options)
    {
    }

    public virtual DbSet<AvailabilitySchedule> AvailabilitySchedules { get; set; }

    public virtual DbSet<ParkingLot> ParkingLots { get; set; }

    public virtual DbSet<Payment> Payments { get; set; }

    public virtual DbSet<Reservation> Reservations { get; set; }

    public virtual DbSet<User> Users { get; set; }

    public DbSet<ParkingLotImage> ParkingLotImages { get; set; }

    protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    {

    }
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<AvailabilitySchedule>(entity =>
        {
            entity.HasKey(e => e.ScheduleId).HasName("PK__Availabi__C46A8A6FE68A3B18");

            entity.ToTable("Availability_Schedule");

            entity.HasIndex(e => new { e.ParkingLotId, e.DayOfWeek }, "idx_availability_schedule");

            entity.Property(e => e.ScheduleId).HasColumnName("schedule_id");
            entity.Property(e => e.AvailabilityType)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("weekly")
                .HasColumnName("availability_type");
            entity.Property(e => e.CloseTime).HasColumnName("close_time");
            entity.Property(e => e.DayOfWeek)
                .HasMaxLength(9)
                .IsUnicode(false)
                .HasColumnName("day_of_week");
            entity.Property(e => e.OpenTime).HasColumnName("open_time");
            entity.Property(e => e.ParkingLotId).HasColumnName("parking_lot_id");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.AvailabilitySchedules)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Availabil__parki__44FF419A");
        });

        modelBuilder.Entity<ParkingLot>(entity =>
        {
            entity.HasKey(e => e.ParkingLotId).HasName("PK__Parking___7C960F5B597B5EDB");

            entity.ToTable("Parking_Lots");

            entity.HasIndex(e => e.OwnerId, "idx_parking_lots_owner_id");

            entity.Property(e => e.ParkingLotId).HasColumnName("parking_lot_id");
            entity.Property(e => e.Address)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("address");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Description)
                .HasColumnType("text")
                .HasColumnName("description");
            entity.Property(e => e.Latitude)
                .HasColumnType("decimal(10, 8)")
                .HasColumnName("latitude");
            entity.Property(e => e.Longitude)
                .HasColumnType("decimal(11, 8)")
                .HasColumnName("longitude");
            entity.Property(e => e.OwnerId).HasColumnName("owner_id");
            entity.Property(e => e.PricePerHour)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("price_per_hour");
            entity.Property(e => e.UpdatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("updated_at");

            entity.HasOne(d => d.Owner).WithMany(p => p.ParkingLots)
                .HasForeignKey(d => d.OwnerId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Parking_L__owner__3E52440B");
        });

        modelBuilder.Entity<Payment>(entity =>
        {
            entity.HasKey(e => e.PaymentId).HasName("PK__Payments__ED1FC9EAFF800CB2");

            entity.HasIndex(e => e.ReservationId, "idx_payments_reservation_id");

            entity.Property(e => e.PaymentId).HasColumnName("payment_id");
            entity.Property(e => e.Amount)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("amount");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.PaymentMethod)
                .HasMaxLength(20)
                .IsUnicode(false)
                .HasColumnName("payment_method");
            entity.Property(e => e.PaymentStatus)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("pending")
                .HasColumnName("payment_status");
            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");

            entity.HasOne(d => d.Reservation).WithMany(p => p.Payments)
                .HasForeignKey(d => d.ReservationId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Payments__reserv__52593CB8");
        });

        modelBuilder.Entity<Reservation>(entity =>
        {
            entity.HasKey(e => e.ReservationId).HasName("PK__Reservat__31384C295A699A72");

            entity.HasIndex(e => e.DriverId, "idx_reservations_driver_id");

            entity.HasIndex(e => e.ParkingLotId, "idx_reservations_parking_lot_id");

            entity.Property(e => e.ReservationId).HasColumnName("reservation_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.DriverId).HasColumnName("driver_id");
            entity.Property(e => e.EndTime)
                .HasColumnType("datetime")
                .HasColumnName("end_time");
            entity.Property(e => e.ParkingLotId).HasColumnName("parking_lot_id");
            entity.Property(e => e.StartTime)
                .HasColumnType("datetime")
                .HasColumnName("start_time");
            entity.Property(e => e.Status)
                .HasMaxLength(10)
                .IsUnicode(false)
                .HasDefaultValue("active")
                .HasColumnName("status");
            entity.Property(e => e.TotalCost)
                .HasColumnType("decimal(10, 2)")
                .HasColumnName("total_cost");

            entity.HasOne(d => d.Driver).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.DriverId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__drive__4AB81AF0");

            entity.HasOne(d => d.ParkingLot).WithMany(p => p.Reservations)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.ClientSetNull)
                .HasConstraintName("FK__Reservati__parki__4BAC3F29");
        });

        modelBuilder.Entity<User>(entity =>
        {
            entity.HasKey(e => e.UserId).HasName("PK__Users__B9BE370F334D2E48");

            entity.HasIndex(e => e.Email, "UQ__Users__AB6E6164D8C51D1C").IsUnique();

            entity.HasIndex(e => e.Username, "UQ__Users__F3DBC57232E6832F").IsUnique();

            entity.HasIndex(e => e.Email, "idx_users_email").IsUnique();

            entity.HasIndex(e => e.Username, "idx_users_username").IsUnique();

            entity.Property(e => e.UserId).HasColumnName("user_id");
            entity.Property(e => e.CreatedAt)
                .HasDefaultValueSql("(getdate())")
                .HasColumnType("datetime")
                .HasColumnName("created_at");
            entity.Property(e => e.Email)
                .HasMaxLength(100)
                .IsUnicode(false)
                .HasColumnName("email");
            entity.Property(e => e.PasswordHash)
                .HasMaxLength(255)
                .IsUnicode(false)
                .HasColumnName("password_hash");
            entity.Property(e => e.Phone)
                .HasMaxLength(15)
                .IsUnicode(false)
                .HasColumnName("phone");
            entity.Property(e => e.Username)
                .HasMaxLength(50)
                .IsUnicode(false)
                .HasColumnName("username");
        });


        modelBuilder.Entity<ParkingLotImage>(entity =>
        {
            entity.HasKey(e => e.ImageId);

            entity.HasOne(d => d.ParkingLot)
                .WithMany(p => p.Images)
                .HasForeignKey(d => d.ParkingLotId)
                .OnDelete(DeleteBehavior.Cascade);

            entity.HasOne(d => d.User)
                .WithMany(p => p.ParkingLotImages)
                .HasForeignKey(d => d.UserId)
                .OnDelete(DeleteBehavior.Cascade);
        });

        OnModelCreatingPartial(modelBuilder);
    }

    partial void OnModelCreatingPartial(ModelBuilder modelBuilder);
}
