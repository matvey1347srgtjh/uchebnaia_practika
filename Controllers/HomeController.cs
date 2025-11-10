using CinemaApp.Repositories;
using CinemaApp.ViewModels;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers;

public class HomeController : Controller
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<HomeController> _logger;

    public HomeController(IMovieRepository movieRepository, ILogger<HomeController> logger)
    {
        _movieRepository = movieRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index()
    {
        var movies = await _movieRepository.GetActiveMoviesAsync();
        return View(movies);
    }

    public IActionResult Privacy()
    {
        return View();
    }
}

