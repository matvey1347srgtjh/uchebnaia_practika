using CinemaApp.Models;

namespace CinemaApp.ViewModels;

public class HomeIndexViewModel
{
    public List<Movie> Movies { get; set; } = new();
    public List<HeroSlide> HeroSlides { get; set; } = new();
}


