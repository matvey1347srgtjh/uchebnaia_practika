using CinemaApp.Models;
using CinemaApp.Repositories;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.Extensions.Logging;
using Microsoft.AspNetCore.Hosting; // 👈 ДОБАВЛЕНО
using System.IO;                   // 👈 ДОБАВЛЕНО
using System.Linq;                 // 👈 ДОБАВЛЕНО

namespace CinemaApp.Controllers;

[Authorize(Roles = "Admin")]
public class AdminController : Controller
{
    private readonly IMovieRepository _movieRepository;
    private readonly IHallRepository _hallRepository;
    private readonly ISessionRepository _sessionRepository;
    private readonly ILogger<AdminController> _logger;
    private readonly IWebHostEnvironment _webHostEnvironment; // 👈 ДОБАВЛЕНО

    public AdminController(
        IMovieRepository movieRepository,
        IHallRepository hallRepository,
        ISessionRepository sessionRepository,
        ILogger<AdminController> logger,
        IWebHostEnvironment webHostEnvironment) // 👈 ИНЖЕКЦИЯ IWebHostEnvironment
    {
        _movieRepository = movieRepository;
        _hallRepository = hallRepository;
        _sessionRepository = sessionRepository;
        _logger = logger;
        _webHostEnvironment = webHostEnvironment; // 👈 ИНИЦИАЛИЗАЦИЯ
    }

    // Хелпер-метод для получения списка трейлеров
    private List<string> GetAvailableTrailers()
    {
        var trailerPath = Path.Combine(_webHostEnvironment.WebRootPath, "videos");
        
        var trailers = Directory.Exists(trailerPath)
            ? Directory.GetFiles(trailerPath)
                       .Select(Path.GetFileName)
                       .Where(f => f.EndsWith(".mp4", StringComparison.OrdinalIgnoreCase) || 
                                   f.EndsWith(".webm", StringComparison.OrdinalIgnoreCase))
                       .ToList()
            : new List<string>();

        // Добавляем пустой элемент для опции "Не выбрано"

        return trailers;
    }


    // Movies CRUD
    public async Task<IActionResult> Movies()
    {
        var movies = await _movieRepository.GetAllAsync();
        return View(movies);
    }

    // 👇 ОБНОВЛЕННЫЙ МЕТОД GET: Заполняем ViewData для выпадающего списка
    public IActionResult CreateMovie()
    {
        ViewData["TrailerFiles"] = GetAvailableTrailers();
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateMovie(Movie movie)
    {
        if (ModelState.IsValid)
        {
            await _movieRepository.CreateAsync(movie);
            return RedirectToAction(nameof(Movies));
        }
        
        // Если валидация не прошла, снова заполняем ViewData
        ViewData["TrailerFiles"] = GetAvailableTrailers();
        return View(movie);
    }

    // 👇 ОБНОВЛЕННЫЙ МЕТОД GET: Заполняем ViewData для выпадающего списка
    public async Task<IActionResult> EditMovie(int id)
    {
        var movie = await _movieRepository.GetByIdAsync(id);
        if (movie == null)
        {
            return NotFound();
        }
        
        // Заполняем ViewData для выпадающего списка
        ViewData["TrailerFiles"] = GetAvailableTrailers(); 
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditMovie(int id, Movie movie)
    {
        if (id != movie.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _movieRepository.UpdateAsync(movie);
            return RedirectToAction(nameof(Movies));
        }

        // Если валидация не прошла, снова заполняем ViewData
        ViewData["TrailerFiles"] = GetAvailableTrailers();
        return View(movie);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteMovie(int id)
    {
        await _movieRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Movies));
    }

    // Halls CRUD
    public async Task<IActionResult> Halls()
    {
        var halls = await _hallRepository.GetAllAsync();
        return View(halls);
    }

    public IActionResult CreateHall()
    {
        return View();
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateHall(Hall hall)
    {
        if (ModelState.IsValid)
        {
            await _hallRepository.CreateAsync(hall);
            return RedirectToAction(nameof(Halls));
        }
        return View(hall);
    }

    public async Task<IActionResult> EditHall(int id)
    {
        var hall = await _hallRepository.GetByIdAsync(id);
        if (hall == null)
        {
            return NotFound();
        }
        return View(hall);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditHall(int id, Hall hall)
    {
        if (id != hall.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            await _hallRepository.UpdateAsync(hall);
            return RedirectToAction(nameof(Halls));
        }
        return View(hall);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteHall(int id)
    {
        await _hallRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Halls));
    }

    // Sessions CRUD
    public async Task<IActionResult> Sessions()
    {
        var sessions = await _sessionRepository.GetAllAsync();
        return View(sessions);
    }

    public async Task<IActionResult> CreateSession()
    {
        var movies = await _movieRepository.GetAllAsync();
        var halls = await _hallRepository.GetAllAsync();

        ViewBag.Movies = new SelectList(movies, "Id", "Title", 0);
        ViewBag.Halls = new SelectList(halls, "Id", "Name", 0);

        // Округляем до минут, так как datetime-local не поддерживает секунды
        var now = DateTime.Now;
        var session = new Session
        {
            DateTime = new DateTime(now.Year, now.Month, now.Day, now.Hour, now.Minute, 0)
        };

        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> CreateSession(Session session)
    {
        // Логируем полученные значения для отладки
        _logger.LogInformation("CreateSession POST: MovieId={MovieId}, HallId={HallId}, DateTime={DateTime}, Price={Price}",
            session.MovieId, session.HallId, session.DateTime, session.Price);

        // Дополнительная проверка на уровне контроллера
        if (session.MovieId == 0)
        {
            ModelState.AddModelError(nameof(session.MovieId), "Выберите фильм");
        }
        if (session.HallId == 0)
        {
            ModelState.AddModelError(nameof(session.HallId), "Выберите зал");
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _sessionRepository.CreateAsync(session);
                return RedirectToAction(nameof(Sessions));
            }
            catch (Exception ex)
            {
                // Логируем ошибку
                _logger.LogError(ex, "Ошибка при создании сеанса");
                
                ModelState.AddModelError("", "Произошла ошибка при создании сеанса. Попробуйте еще раз.");
            }
        }

        // Если валидация не прошла, логируем ошибки
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                foreach (var errorMessage in error.Value.Errors)
                {
                    _logger.LogWarning("Ошибка валидации {Field}: {Error}", error.Key, errorMessage.ErrorMessage);
                }
            }
        }

        var movies = await _movieRepository.GetAllAsync();
        var halls = await _hallRepository.GetAllAsync();
        ViewBag.Movies = new SelectList(movies, "Id", "Title", session.MovieId);
        ViewBag.Halls = new SelectList(halls, "Id", "Name", session.HallId);

        return View(session);
    }

    public async Task<IActionResult> EditSession(int id)
    {
        var session = await _sessionRepository.GetByIdAsync(id);
        if (session == null)
        {
            return NotFound();
        }

        var movies = await _movieRepository.GetAllAsync();
        var halls = await _hallRepository.GetAllAsync();
        ViewBag.Movies = new SelectList(movies, "Id", "Title", session.MovieId > 0 ? session.MovieId : 0);
        ViewBag.Halls = new SelectList(halls, "Id", "Name", session.HallId > 0 ? session.HallId : 0);

        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> EditSession(int id, Session session)
    {
        if (id != session.Id)
        {
            return NotFound();
        }

        if (ModelState.IsValid)
        {
            try
            {
                await _sessionRepository.UpdateAsync(session);
                return RedirectToAction(nameof(Sessions));
            }
            catch (Exception ex)
            {
                _logger.LogError(ex, "Ошибка при обновлении сеанса");
                ModelState.AddModelError("", "Произошла ошибка при обновлении сеанса. Попробуйте еще раз.");
            }
        }

        // Если валидация не прошла, логируем ошибки
        if (!ModelState.IsValid)
        {
            foreach (var error in ModelState)
            {
                foreach (var errorMessage in error.Value.Errors)
                {
                    _logger.LogWarning("Ошибка валидации {Field}: {Error}", error.Key, errorMessage.ErrorMessage);
                }
            }
        }

        var movies = await _movieRepository.GetAllAsync();
        var halls = await _hallRepository.GetAllAsync();
        ViewBag.Movies = new SelectList(movies, "Id", "Title", session.MovieId > 0 ? session.MovieId : 0);
        ViewBag.Halls = new SelectList(halls, "Id", "Name", session.HallId > 0 ? session.HallId : 0);

        return View(session);
    }

    [HttpPost]
    [ValidateAntiForgeryToken]
    public async Task<IActionResult> DeleteSession(int id)
    {
        await _sessionRepository.DeleteAsync(id);
        return RedirectToAction(nameof(Sessions));
    }
}