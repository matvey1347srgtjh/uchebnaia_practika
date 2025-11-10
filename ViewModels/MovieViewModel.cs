namespace CinemaApp.ViewModels;

public class MovieViewModel
{
    public int Id { get; set; }
    public string Title { get; set; } = string.Empty;
    public string Description { get; set; } = string.Empty;
    public string Genre { get; set; } = string.Empty;
    public int Duration { get; set; }
    public string? PosterUrl { get; set; }
    public bool IsActive { get; set; }
    public List<SessionViewModel> Sessions { get; set; } = new();
}

public class SessionViewModel
{
    public int Id { get; set; }
    public DateTime DateTime { get; set; }
    public decimal Price { get; set; }
    public string HallName { get; set; } = string.Empty;
    public int HallId { get; set; }
}

