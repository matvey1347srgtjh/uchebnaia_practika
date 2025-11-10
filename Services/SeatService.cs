using CinemaApp.Models;
using CinemaApp.Repositories;

namespace CinemaApp.Services;

public class SeatService : ISeatService
{
    private readonly ISeatRepository _seatRepository;

    public SeatService(ISeatRepository seatRepository)
    {
        _seatRepository = seatRepository;
    }

    public async Task<IEnumerable<Seat>> GetSeatsByHallIdAsync(int hallId)
    {
        return await _seatRepository.GetSeatsByHallIdAsync(hallId);
    }
}

