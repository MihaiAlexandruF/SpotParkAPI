using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using SpotParkAPI.Models;
using System;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;

public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly ILogger<ReservationCleanupService> _logger;

    public ReservationCleanupService(IServiceProvider serviceProvider, ILogger<ReservationCleanupService> logger)
    {
        _serviceProvider = serviceProvider;
        _logger = logger;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            using (var scope = _serviceProvider.CreateScope())
            {
                var dbContext = scope.ServiceProvider.GetRequiredService<SpotParkDbContext>();
                var now = DateTime.UtcNow;

                var expiredReservations = dbContext.Reservations
                    .Where(r => r.Status == "active" && r.EndTime <= now)
                    .ToList();

                if (expiredReservations.Any())
                {
                    foreach (var reservation in expiredReservations)
                    {
                        reservation.Status = "completed";
                    }

                    await dbContext.SaveChangesAsync();
                    _logger.LogInformation($"{expiredReservations.Count} reservations updated to completed at {DateTime.UtcNow}.");
                }
            }

            await Task.Delay(TimeSpan.FromMinutes(2), stoppingToken);
        }
    }
}
