using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface IHallRepository
{
    Task<IEnumerable<Hall>> GetAllAsync();
    Task<Hall?> GetByIdAsync(int id);
    Task<Hall> CreateAsync(Hall hall);
    Task<Hall> UpdateAsync(Hall hall);
    Task DeleteAsync(int id);
}

