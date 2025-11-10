using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class SeatRepository : ISeatRepository
{
    private readonly ApplicationDbContext _context;

    public SeatRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId)
    {
        return await _context.Seats
            .Where(s => s.HallId == hallId)
            .OrderBy(s => s.Row)
            .ThenBy(s => s.Number)
            .ToListAsync();
    }
}

