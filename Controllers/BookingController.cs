using CinemaApp.Models;
using CinemaApp.Repositories;
using CinemaApp.Services;
using CinemaApp.ViewModels;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Mvc;

namespace CinemaApp.Controllers;

[Authorize]
public class BookingController : Controller
{
    private readonly ISessionRepository _sessionRepository;
    private readonly IBookingService _bookingService;
    private readonly ITicketRepository _ticketRepository;
    private readonly UserManager<ApplicationUser> _userManager;

    public BookingController(
        ISessionRepository sessionRepository,
        IBookingService bookingService,
        ITicketRepository ticketRepository,
        UserManager<ApplicationUser> userManager)
    {
        _sessionRepository = sessionRepository;
        _bookingService = bookingService;
        _ticketRepository = ticketRepository;
        _userManager = userManager;
    }

    public async Task<IActionResult> SelectSeats(int sessionId)
    {
        var session = await _sessionRepository.GetByIdAsync(sessionId);
        if (session == null)
        {
            return NotFound();
        }

        var seatStatuses = await _bookingService.GetSeatStatusesAsync(sessionId);

        var viewModel = new BookingViewModel
        {
            SessionId = sessionId,
            MovieTitle = session.Movie.Title,
            SessionDateTime = session.DateTime,
            HallName = session.Hall.Name,
            HallRows = session.Hall.Rows,
            HallSeatsPerRow = session.Hall.SeatsPerRow,
            Price = session.Price,
            Seats = seatStatuses.Select(s => new SeatStatusViewModel
            {
                Row = s.Row,
                SeatNumber = s.SeatNumber,
                IsAvailable = s.IsAvailable,
                IsReserved = s.IsReserved,
                IsSold = s.IsSold
            }).ToList()
        };

        return View(viewModel);
    }

    [HttpPost]
    public async Task<IActionResult> ReserveSeats(int sessionId, [FromBody] ReserveSeatsRequest request)
    {
        if (request?.SelectedSeats == null || request.SelectedSeats.Count == 0)
        {
            return Json(new { success = false, message = "Выберите хотя бы одно место" });
        }

        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Json(new { success = false, message = "Пользователь не авторизован" });
        }

        var user = await _userManager.FindByNameAsync(userId);
        if (user == null)
        {
            return Json(new { success = false, message = "Пользователь не найден" });
        }

        var tickets = new List<int>();

        foreach (var seatInfo in request.SelectedSeats)
        {
            // Parse seat info: format "row-seatNumber"
            var parts = seatInfo.Split('-');
            if (parts.Length != 2)
            {
                continue;
            }

            if (!int.TryParse(parts[0], out int row) || !int.TryParse(parts[1], out int seatNumber))
            {
                continue;
            }

            var ticket = await _bookingService.ReserveSeatAsync(sessionId, user.Id, row, seatNumber);
            if (ticket != null)
            {
                tickets.Add(ticket.Id);
            }
        }

        if (tickets.Count == 0)
        {
            return Json(new { success = false, message = "Не удалось забронировать места" });
        }

        return Json(new { success = true, ticketIds = tickets });
    }

    [HttpPost]
    public async Task<IActionResult> ConfirmPayment([FromBody] List<int> ticketIds)
    {
        if (ticketIds == null || ticketIds.Count == 0)
        {
            return Json(new { success = false, message = "Нет билетов для оплаты" });
        }

        var successCount = 0;
        var tickets = new List<Models.Ticket>();

        foreach (var ticketId in ticketIds)
        {
            var success = await _bookingService.ConfirmPaymentAsync(ticketId);
            if (success)
            {
                successCount++;
                var ticket = await _ticketRepository.GetByIdAsync(ticketId);
                if (ticket != null)
                {
                    tickets.Add(ticket);
                }
            }
        }

        if (successCount == 0)
        {
            return Json(new { success = false, message = "Не удалось подтвердить оплату" });
        }

        return Json(new { success = true, tickets = tickets.Select(t => new { 
            id = t.Id, 
            code = t.TicketCode,
            row = t.Row,
            seat = t.SeatNumber
        }) });
    }

    public async Task<IActionResult> Ticket(int id)
    {
        var ticket = await _ticketRepository.GetByIdAsync(id);
        if (ticket == null)
        {
            return NotFound();
        }

        var userId = User.Identity?.Name;
        if (string.IsNullOrEmpty(userId))
        {
            return Forbid();
        }

        var user = await _userManager.FindByNameAsync(userId);
        if (user == null || ticket.UserId != user.Id)
        {
            return Forbid();
        }

        return View(ticket);
    }
}

public class ReserveSeatsRequest
{
    public List<string> SelectedSeats { get; set; } = new();
}
