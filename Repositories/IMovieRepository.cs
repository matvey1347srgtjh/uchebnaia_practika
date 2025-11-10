using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task<Movie> CreateAsync(Movie movie);
    Task<Movie> UpdateAsync(Movie movie);
    Task DeleteAsync(int id);
    Task<IEnumerable<Movie>> GetActiveMoviesAsync();
}

