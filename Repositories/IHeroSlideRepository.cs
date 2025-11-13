using CinemaApp.Models;

namespace CinemaApp.Repositories;

public interface IHeroSlideRepository
{
    Task<List<HeroSlide>> GetAllAsync();
    Task<List<HeroSlide>> GetActiveAsync();
    Task<HeroSlide?> GetByIdAsync(int id);
    Task CreateAsync(HeroSlide slide);
    Task UpdateAsync(HeroSlide slide);
    Task DeleteAsync(int id);
}


