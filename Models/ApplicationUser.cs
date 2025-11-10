using Microsoft.AspNetCore.Identity;

namespace CinemaApp.Models;

public class ApplicationUser : IdentityUser
{
    public string? FullName { get; set; }
    public virtual ICollection<Ticket> Tickets { get; set; } = new List<Ticket>();
}

