using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface ITicketRepository
{
    Task<IEnumerable<Ticket>> GetAllAsync();
    Task<Ticket?> GetByIdAsync(int id);
    Task<Ticket> CreateAsync(Ticket ticket);
    Task<Ticket> UpdateAsync(Ticket ticket);
    Task DeleteAsync(int id);
    Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId);
    Task<Ticket?> GetTicketByCodeAsync(string ticketCode);
    Task<IEnumerable<Ticket>> GetTicketsBySessionIdAsync(int sessionId);
    Task<Ticket?> GetTicketBySessionAndSeatAsync(int sessionId, int row, int seatNumber);
}

