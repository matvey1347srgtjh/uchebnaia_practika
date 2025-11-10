using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class SessionRepository : ISessionRepository
{
    private readonly ApplicationDbContext _context;

    public SessionRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<IEnumerable<Session>> GetAllAsync()
    {
        return await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .OrderBy(s => s.DateTime)
            .ToListAsync();
    }

    public async Task<Session?> GetByIdAsync(int id)
    {
        return await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Include(s => s.Tickets)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task<Session> CreateAsync(Session session)
    {
        _context.Sessions.Add(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task<Session> UpdateAsync(Session session)
    {
        _context.Sessions.Update(session);
        await _context.SaveChangesAsync();
        return session;
    }

    public async Task DeleteAsync(int id)
    {
        var session = await _context.Sessions.FindAsync(id);
        if (session != null)
        {
            _context.Sessions.Remove(session);
            await _context.SaveChangesAsync();
        }
    }

    public async Task<IEnumerable<Session>> GetSessionsByMovieIdAsync(int movieId)
    {
        return await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.MovieId == movieId && s.DateTime >= DateTime.Now)
            .OrderBy(s => s.DateTime)
            .ToListAsync();
    }

    public async Task<IEnumerable<Session>> GetUpcomingSessionsAsync()
    {
        return await _context.Sessions
            .Include(s => s.Movie)
            .Include(s => s.Hall)
            .Where(s => s.DateTime >= DateTime.Now)
            .OrderBy(s => s.DateTime)
            .ToListAsync();
    }
}

