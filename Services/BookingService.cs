using CinemaApp.Models;
using CinemaApp.Repositories;

namespace CinemaApp.Services;

public class BookingService : IBookingService
{
    private readonly ITicketRepository _ticketRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ISeatRepository _seatRepository;
    private const int ReservationTimeoutMinutes = 5;

    public BookingService(
        ITicketRepository ticketRepository,
        ISessionRepository sessionRepository,
        ISeatRepository seatRepository)
    {
        _ticketRepository = ticketRepository;
        _sessionRepository = sessionRepository;
        _seatRepository = seatRepository;
    }

    public async Task<Ticket?> ReserveSeatAsync(int sessionId, string userId, int row, int seatNumber)
    {
        // Check if seat is available
        if (!await IsSeatAvailableAsync(sessionId, row, seatNumber))
        {
            return null;
        }

        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return null;
        }

        var ticket = new Ticket
        {
            SessionId = sessionId,
            UserId = userId,
            Row = row,
            SeatNumber = seatNumber,
            Price = session.Price,
            TicketCode = await GenerateTicketCodeAsync(),
            Status = TicketStatus.Reserved,
            ReservedUntil = DateTime.Now.AddMinutes(ReservationTimeoutMinutes)
        };

        return await _ticketRepository.CreateAsync(ticket);
    }

    public async Task<bool> ConfirmPaymentAsync(int ticketId)
    {
        var ticket = await _ticketRepository.GetByIdAsync(ticketId);
        if (ticket == null || ticket.Status != TicketStatus.Reserved)
        {
            return false;
        }

        // Check if reservation is still valid
        if (ticket.ReservedUntil.HasValue && ticket.ReservedUntil.Value < DateTime.Now)
        {
            return false;
        }

        ticket.Status = TicketStatus.Sold;
        ticket.ReservedUntil = null;
        await _ticketRepository.UpdateAsync(ticket);

        return true;
    }

    public async Task<bool> IsSeatAvailableAsync(int sessionId, int row, int seatNumber)
    {
        var existingTicket = await _ticketRepository.GetTicketBySessionAndSeatAsync(sessionId, row, seatNumber);
        
        if (existingTicket == null)
        {
            return true;
        }

        // If ticket is sold, seat is not available
        if (existingTicket.Status == TicketStatus.Sold)
        {
            return false;
        }

        // If ticket is reserved, check if reservation expired
        if (existingTicket.Status == TicketStatus.Reserved)
        {
            if (existingTicket.ReservedUntil.HasValue && existingTicket.ReservedUntil.Value < DateTime.Now)
            {
                // Reservation expired, seat is available
                return true;
            }
            return false;
        }

        return true;
    }

    public async Task<IEnumerable<SeatStatus>> GetSeatStatusesAsync(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null || session.Hall == null)
        {
            return Enumerable.Empty<SeatStatus>();
        }

        var tickets = await _ticketRepository.GetTicketsBySessionIdAsync(sessionId);
        var seatStatuses = new List<SeatStatus>();

        // Get all seats in the hall
        var seats = await _seatRepository.GetSeatsByHallIdAsync(session.Hall.Id);

        foreach (var seat in seats)
        {
            var ticket = tickets.FirstOrDefault(t => t.Row == seat.Row && t.SeatNumber == seat.Number);
            
            var status = new SeatStatus
            {
                Row = seat.Row,
                SeatNumber = seat.Number,
                IsAvailable = ticket == null || 
                             (ticket.Status == TicketStatus.Reserved && 
                              ticket.ReservedUntil.HasValue && 
                              ticket.ReservedUntil.Value < DateTime.Now),
                IsReserved = ticket != null && 
                            ticket.Status == TicketStatus.Reserved &&
                            (!ticket.ReservedUntil.HasValue || ticket.ReservedUntil.Value >= DateTime.Now),
                IsSold = ticket != null && ticket.Status == TicketStatus.Sold,
                ReservedUntil = ticket?.ReservedUntil
            };

            seatStatuses.Add(status);
        }

        return seatStatuses;
    }

    public async Task<string> GenerateTicketCodeAsync()
    {
        var random = new Random();
        var code = $"TICKET-{DateTime.Now:yyyyMMdd}-{random.Next(100000, 999999)}";
        
        // Ensure uniqueness
        var existingTicket = await _ticketRepository.GetTicketByCodeAsync(code);
        while (existingTicket != null)
        {
            code = $"TICKET-{DateTime.Now:yyyyMMdd}-{random.Next(100000, 999999)}";
            existingTicket = await _ticketRepository.GetTicketByCodeAsync(code);
        }

        return code;
    }
}

