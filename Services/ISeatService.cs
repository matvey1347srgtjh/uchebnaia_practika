using CinemaApp.Models;

namespace CinemaApp.Services;

public interface ISeatService
{
    Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId);
}

