using CinemaApp.Models;

namespace CinemaApp.Services;

public interface IBookingService
{
    Task<Ticket?> ReserveSeatAsync(int sessionId, string userId, int row, int seatNumber);
    Task<bool> ConfirmPaymentAsync(int ticketId);
    Task<bool> IsSeatAvailableAsync(int sessionId, int row, int seatNumber);
    Task<IEnumerable<SeatStatus>> GetSeatStatusesAsync(int sessionId);
    Task<string> GenerateTicketCodeAsync();
}

public class SeatStatus
{
    public int Row { get; set; }
    public int SeatNumber { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsReserved { get; set; }
    public bool IsSold { get; set; }
    public DateTime? ReservedUntil { get; set; }
}

