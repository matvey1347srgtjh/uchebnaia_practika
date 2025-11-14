using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class MovieRepository : IMovieRepository
{
    private readonly ApplicationDbContext _context;

    public MovieRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    private static bool ContainsInsensitive(string? source, string value)
    {
        return !string.IsNullOrEmpty(source)
               && source.IndexOf(value, StringComparison.CurrentCultureIgnoreCase) >= 0;
    }

    public async Task<IEnumerable<Movie>> GetAllAsync()
    {
        return await _context.Movies.ToListAsync();
    }

    public async Task<Movie?> GetByIdAsync(int id)
    {
        return await _context.Movies
            .Include(m => m.Sessions)
            .ThenInclude(s => s.Hall)
            .FirstOrDefaultAsync(m => m.Id == id);
    }

    public async Task<Movie> CreateAsync(Movie movie)
    {
        _context.Movies.Add(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task<Movie> UpdateAsync(Movie movie)
    {
        _context.Movies.Update(movie);
        await _context.SaveChangesAsync();
        return movie;
    }

    public async Task DeleteAsync(int id)
    {
        var movie = await _context.Movies.FindAsync(id);
        if (movie != null)
        {
            _context.Movies.Remove(movie);
            await _context.SaveChangesAsync();
        }
    }

    private static IEnumerable<string> SplitGenres(string? value)
    {
        if (string.IsNullOrWhiteSpace(value))
        {
            return Enumerable.Empty<string>();
        }

        return value
            .Split(new[] { ',', ';', '/', '|' }, StringSplitOptions.RemoveEmptyEntries)
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g));
    }

    public async Task<IEnumerable<Movie>> GetActiveMoviesAsync(string? searchQuery = null, string? genre = null, int? minDuration = null, int? maxDuration = null)
    {
        var movies = await _context.Movies
            .Where(m => m.IsActive)
            .ToListAsync();

        if (!string.IsNullOrWhiteSpace(searchQuery))
        {
            var term = searchQuery.Trim();
            movies = movies
                .Where(m => ContainsInsensitive(m.Title, term)
                            || ContainsInsensitive(m.Genre, term)
                            || ContainsInsensitive(m.Description, term))
                .ToList();
        }

        if (!string.IsNullOrWhiteSpace(genre))
        {
            var normalizedGenre = genre.Trim();
            movies = movies
                .Where(m => SplitGenres(m.Genre)
                    .Any(g => g.Equals(normalizedGenre, StringComparison.CurrentCultureIgnoreCase)))
                .ToList();
        }

        if (minDuration.HasValue)
        {
            movies = movies
                .Where(m => m.Duration >= minDuration.Value)
                .ToList();
        }

        if (maxDuration.HasValue)
        {
            movies = movies
                .Where(m => m.Duration <= maxDuration.Value)
                .ToList();
        }

        return movies
            .OrderBy(m => m.Title)
            .ToList();
    }

    public async Task<(IEnumerable<Movie> Active, IEnumerable<Movie> Others)> GetSearchSuggestionsAsync(string searchQuery, int activeLimit = 5, int otherLimit = 5)
    {
        if (string.IsNullOrWhiteSpace(searchQuery))
        {
            return (Enumerable.Empty<Movie>(), Enumerable.Empty<Movie>());
        }

        var term = searchQuery.Trim();

        var activeMatches = await _context.Movies
            .Where(m => m.IsActive)
            .ToListAsync();

        activeMatches = activeMatches
            .Where(m => ContainsInsensitive(m.Title, term)
                        || ContainsInsensitive(m.Genre, term)
                        || ContainsInsensitive(m.Description, term))
            .OrderBy(m => m.Title)
            .Take(activeLimit)
            .ToList();

        var otherMatches = await _context.Movies
            .Where(m => !m.IsActive)
            .ToListAsync();

        otherMatches = otherMatches
            .Where(m => ContainsInsensitive(m.Title, term)
                        || ContainsInsensitive(m.Genre, term)
                        || ContainsInsensitive(m.Description, term))
            .OrderBy(m => m.Title)
            .Take(otherLimit)
            .ToList();

        return (activeMatches, otherMatches);
    }

    public async Task<IReadOnlyList<string>> GetGenresAsync()
    {
        var rawGenres = await _context.Movies
            .Select(m => m.Genre)
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .ToListAsync();

        return rawGenres
            .SelectMany(SplitGenres)
            .Select(g => g.Trim())
            .Where(g => !string.IsNullOrWhiteSpace(g))
            .Distinct(StringComparer.CurrentCultureIgnoreCase)
            .OrderBy(g => g, StringComparer.CurrentCultureIgnoreCase)
            .ToList();
    }
}

