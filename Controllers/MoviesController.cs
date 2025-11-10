using CinemaApp.Repositories;
using CinemaApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers;

public class MoviesController : Controller
{
    private readonly IMovieRepository _movieRepository;
    private readonly ISessionRepository _sessionRepository;

    public MoviesController(
        IMovieRepository movieRepository,
        ISessionRepository sessionRepository)
    {
        _movieRepository = movieRepository;
        _sessionRepository = sessionRepository;
    }

    public async Task<IActionResult> Details(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }

        var sessions = await _sessionRepository.GetSessionsByMovieIdAsync(id);
        
        var viewModel = new MovieViewModel
        {
            Id = movie.Id,
            Title = movie.Title,
            Description = movie.Description,
            Genre = movie.Genre,
            Duration = movie.Duration,
            PosterUrl = movie.PosterUrl,
            IsActive = movie.IsActive,
            Sessions = sessions.Select(s => new SessionViewModel
            {
                Id = s.Id,
                DateTime = s.DateTime,
                Price = s.Price,
                HallName = s.Hall.Name,
                HallId = s.HallId
            }).ToList()
        };

        return View(viewModel);
    }
}

