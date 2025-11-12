using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface IMovieRepository
{
    Task<IEnumerable<Movie>> GetAllAsync();
    Task<Movie?> GetByIdAsync(int id);
    Task<Movie> CreateAsync(Movie movie);
    Task<Movie> UpdateAsync(Movie movie);
    Task DeleteAsync(int id);
    Task<IEnumerable<Movie>> GetActiveMoviesAsync(string? searchQuery = null, string? genre = null, int? minDuration = null, int? maxDuration = null);
    Task<(IEnumerable<Movie> Active, IEnumerable<Movie> Others)> GetSearchSuggestionsAsync(string searchQuery, int activeLimit = 5, int otherLimit = 5);
    Task<IReadOnlyList<string>> GetGenresAsync();
}

