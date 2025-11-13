using System.ComponentModel.DataAnnotations;

namespace CinemaApp.Models;

public class HeroSlide
{
    public int Id { get; set; }

    [StringLength(150)]
    public string? Title { get; set; }

    [StringLength(80)]
    public string? Tagline { get; set; }

    [StringLength(500)]
    public string? Description { get; set; }

    [StringLength(40)]
    public string ButtonText { get; set; } = "Подробнее";

    [StringLength(200)]
    public string? ButtonUrl { get; set; }

    public int? MovieId { get; set; }

    [StringLength(200)]
    public string? VideoPath { get; set; }

    [StringLength(200)]
    public string? PosterUrl { get; set; }

    public bool IsActive { get; set; } = true;

    public int DisplayOrder { get; set; } = 0;

    public DateTime CreatedAt { get; set; } = DateTime.UtcNow;

    public Movie? Movie { get; set; }
}


