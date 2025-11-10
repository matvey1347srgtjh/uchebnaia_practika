namespace CinemaApp.ViewModels;

public class BookingViewModel
{
    public int SessionId { get; set; }
    public string MovieTitle { get; set; } = string.Empty;
    public DateTime SessionDateTime { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int HallRows { get; set; }
    public int HallSeatsPerRow { get; set; }
    public decimal Price { get; set; }
    public List<SeatStatusViewModel> Seats { get; set; } = new();
    public List<int> SelectedSeats { get; set; } = new();
}

public class SeatStatusViewModel
{
    public int Row { get; set; }
    public int SeatNumber { get; set; }
    public bool IsAvailable { get; set; }
    public bool IsReserved { get; set; }
    public bool IsSold { get; set; }
}

