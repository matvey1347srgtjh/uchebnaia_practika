using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class HallRepository : IHallRepository
{
    private readonly ApplicationDbContext _context;

    public HallRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Hall>> GetAllAsync()
    {
        return await _context.Halls.ToListAsync();
    }

    public async Task<Hall?> GetByIdAsync(int id)
    {
        return await _context.Halls
            .Include(h => h.Seats)
            .FirstOrDefaultAsync(h => h.Id == id);
    }

    public async Task<Hall> CreateAsync(Hall hall)
    {
        _context.Halls.Add(hall);
        await _context.SaveChangesAsync();

        // Create seats for the hall
        var seats = new List<Seat>();
        for (int row = 1; row <= hall.Rows; row++)
        {
            for (int seat = 1; seat <= hall.SeatsPerRow; seat++)
            {
                seats.Add(new Seat { HallId = hall.Id, Row = row, Number = seat });
            }
        }
        _context.Seats.AddRange(seats);
        await _context.SaveChangesAsync();

        return hall;
    }

    public async Task<Hall> UpdateAsync(Hall hall)
    {
        _context.Halls.Update(hall);
        await _context.SaveChangesAsync();
        return hall;
    }

    public async Task DeleteAsync(int id)
    {
        var hall = await _context.Halls.FindAsync(id);
        if (hall != null)
        {
            _context.Halls.Remove(hall);
            await _context.SaveChangesAsync();
        }
    }
}

