namespace CinemaApp.Models;

public class Hall
{
    public int Id { get; set; }
    public string Name { get; set; } = string.Empty;
    public int Rows { get; set; }
    public int SeatsPerRow { get; set; }
    public DateTime CreatedAt { get; set; } = DateTime.Now;
    
    public virtual ICollection<Session> Sessions { get; set; } = new List<Session>();
    public virtual ICollection<Seat> Seats { get; set; } = new List<Seat>();
}

