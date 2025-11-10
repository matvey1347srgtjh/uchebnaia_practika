using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface ISessionRepository
{
    Task<IEnumerable<Session>> GetAllAsync();
    Task<Session?> GetByIdAsync(int id);
    Task<Session> CreateAsync(Session session);
    Task<Session> UpdateAsync(Session session);
    Task DeleteAsync(int id);
    Task<IEnumerable<Session>> GetSessionsByMovieIdAsync(int movieId);
    Task<IEnumerable<Session>> GetUpcomingSessionsAsync();
}

