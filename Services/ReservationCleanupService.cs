using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Services;

public class ReservationCleanupService : BackgroundService
{
    private readonly IServiceProvider _serviceProvider;
    private readonly TimeSpan _checkInterval = TimeSpan.FromMinutes(1);

    public ReservationCleanupService(IServiceProvider serviceProvider)
    {
        _serviceProvider = serviceProvider;
    }

    protected override async Task ExecuteAsync(CancellationToken stoppingToken)
    {
        while (!stoppingToken.IsCancellationRequested)
        {
            await CleanExpiredReservationsAsync();
            await Task.Delay(_checkInterval, stoppingToken);
        }
    }

    private async Task CleanExpiredReservationsAsync()
    {
        using var scope = _serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();

        var expiredTickets = await context.Tickets
            .Where(t => t.Status == TicketStatus.Reserved &&
                       t.ReservedUntil.HasValue &&
                       t.ReservedUntil.Value < DateTime.Now)
            .ToListAsync();

        if (expiredTickets.Any())
        {
            context.Tickets.RemoveRange(expiredTickets);
            await context.SaveChangesAsync();
        }
    }
}

