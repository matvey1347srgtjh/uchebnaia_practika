namespace CinemaApp.Models;

public class Ticket
{
    public int Id { get; set; }
    public int SessionId { get; set; }
    public string UserId { get; set; } = string.Empty;
    public int Row { get; set; }
    public int SeatNumber { get; set; }
    public decimal Price { get; set; }
    public string TicketCode { get; set; } = string.Empty;
    public DateTime PurchaseDate { get; set; } = DateTime.Now;
    public TicketStatus Status { get; set; } = TicketStatus.Reserved;
    public DateTime? ReservedUntil { get; set; }
    
    public virtual Session Session { get; set; } = null!;
    public virtual ApplicationUser User { get; set; } = null!;
}

public enum TicketStatus
{
    Reserved,    // Забронировано (временно)
    Sold,        // Продано
    Cancelled    // Отменено
}

