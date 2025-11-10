using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class TicketRepository : ITicketRepository
{
    private readonly ApplicationDbContext _context;

    public TicketRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Ticket>> GetAllAsync()
    {
        return await _context.Tickets
            .Include(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(t => t.User)
            .ToListAsync();
    }

    public async Task<Ticket?> GetByIdAsync(int id)
    {
        return await _context.Tickets
            .Include(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(t => t.Session)
            .ThenInclude(s => s.Hall)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.Id == id);
    }

    public async Task<Ticket> CreateAsync(Ticket ticket)
    {
        _context.Tickets.Add(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task<Ticket> UpdateAsync(Ticket ticket)
    {
        _context.Tickets.Update(ticket);
        await _context.SaveChangesAsync();
        return ticket;
    }

    public async Task DeleteAsync(int id)
    {
        var ticket = await _context.Tickets.FindAsync(id);
        if (ticket != null)
        {
            _context.Tickets.Remove(ticket);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Ticket>> GetTicketsByUserIdAsync(string userId)
    {
        return await _context.Tickets
            .Include(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(t => t.Session)
            .ThenInclude(s => s.Hall)
            .Where(t => t.UserId == userId && t.Status == TicketStatus.Sold)
            .OrderByDescending(t => t.PurchaseDate)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketByCodeAsync(string ticketCode)
    {
        return await _context.Tickets
            .Include(t => t.Session)
            .ThenInclude(s => s.Movie)
            .Include(t => t.Session)
            .ThenInclude(s => s.Hall)
            .Include(t => t.User)
            .FirstOrDefaultAsync(t => t.TicketCode == ticketCode);
    }

    public async Task<IEnumerable<Ticket>> GetTicketsBySessionIdAsync(int sessionId)
    {
        return await _context.Tickets
            .Where(t => t.SessionId == sessionId)
            .ToListAsync();
    }

    public async Task<Ticket?> GetTicketBySessionAndSeatAsync(int sessionId, int row, int seatNumber)
    {
        return await _context.Tickets
            .Where(t => t.SessionId == sessionId && 
                       t.Row == row && 
                       t.SeatNumber == seatNumber &&
                       (t.Status == TicketStatus.Reserved || t.Status == TicketStatus.Sold))
            .FirstOrDefaultAsync();
    }
}

