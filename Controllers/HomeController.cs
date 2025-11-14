using CinemaApp.Repositories;
using CinemaApp.ViewModels;
using Microsoft.AspNetCore.Mvc;
using System.Collections.Generic;
using System.Linq;

namespace CinemaApp.Controllers;

public class HomeController : Controller
{
    private readonly IMovieRepository _movieRepository;
    private readonly ILogger<HomeController> _logger;
    private readonly IHeroSlideRepository _heroSlideRepository;

    public HomeController(
        IMovieRepository movieRepository,
        IHeroSlideRepository heroSlideRepository,
        ILogger<HomeController> logger)
    {
        _movieRepository = movieRepository;
        _heroSlideRepository = heroSlideRepository;
        _logger = logger;
    }

    public async Task<IActionResult> Index(string? query, string? genre, int? minDuration, int? maxDuration)
    {
        if (minDuration is < 0)
        {
            minDuration = null;
        }

        if (maxDuration is < 0)
        {
            maxDuration = null;
        }

        if (minDuration.HasValue && maxDuration.HasValue && minDuration > maxDuration)
        {
            (minDuration, maxDuration) = (maxDuration, minDuration);
        }

        var movies = (await _movieRepository.GetActiveMoviesAsync(query, genre, minDuration, maxDuration)).ToList();
        var heroSlides = await _heroSlideRepository.GetActiveAsync();

        var model = new HomeIndexViewModel
        {
            Movies = movies,
            HeroSlides = heroSlides
        };

        var chips = new List<string>();
        if (!string.IsNullOrWhiteSpace(query))
        {
            chips.Add($"Поиск: {query.Trim()}");
        }
        if (!string.IsNullOrWhiteSpace(genre))
        {
            chips.Add($"Жанр: {genre.Trim()}");
        }
        if (minDuration.HasValue)
        {
            chips.Add($"От {minDuration.Value} мин");
        }
        if (maxDuration.HasValue)
        {
            chips.Add($"До {maxDuration.Value} мин");
        }

        ViewData["SearchQuery"] = query;
        ViewData["SelectedGenre"] = genre;
        ViewData["MinDuration"] = minDuration;
        ViewData["MaxDuration"] = maxDuration;
        ViewData["Genres"] = await _movieRepository.GetGenresAsync();
        ViewData["ResultsCount"] = movies.Count;
        ViewData["FilterChips"] = chips;
        return View(model);
    }

    public IActionResult Privacy()
    {
        return View();
    }

    public IActionResult About()
    {
        return View();
    }

    [HttpGet]
    public async Task<IActionResult> SearchSuggestions(string term)
    {
        term = term?.Trim();
        if (string.IsNullOrWhiteSpace(term) || term.Length < 2)
        {
            return Ok(new { sections = Array.Empty<object>() });
        }

        var (activeMatches, otherMatches) = await _movieRepository.GetSearchSuggestionsAsync(term, 5, 5);

        var sections = new List<object>();

        if (activeMatches.Any())
        {
            sections.Add(new
            {
                title = "Сегодня в кино",
                actionText = "Билеты",
                actionStyle = "accent",
                items = activeMatches.Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    year = m.CreatedAt.Year,
                    subtitle = $"{m.Genre} • {m.Duration} мин",
                    posterUrl = string.IsNullOrWhiteSpace(m.PosterUrl) ? null : m.PosterUrl,
                    detailsUrl = Url.Action("Details", "Movies", new { id = m.Id })
                })
            });
        }

        if (otherMatches.Any())
        {
            sections.Add(new
            {
                title = "Другие фильмы",
                actionText = "Подробнее",
                actionStyle = "secondary",
                items = otherMatches.Select(m => new
                {
                    id = m.Id,
                    title = m.Title,
                    year = m.CreatedAt.Year,
                    subtitle = $"{m.Genre} • {m.Duration} мин",
                    posterUrl = string.IsNullOrWhiteSpace(m.PosterUrl) ? null : m.PosterUrl,
                    detailsUrl = Url.Action("Details", "Movies", new { id = m.Id })
                })
            });
        }

        return Ok(new { sections });
    }
}

