using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface ISeatRepository
{
    Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId);
}

