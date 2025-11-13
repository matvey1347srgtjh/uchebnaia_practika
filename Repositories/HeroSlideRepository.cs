using CinemaApp.Data;
using CinemaApp.Models;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Repositories;

public class HeroSlideRepository : IHeroSlideRepository
{
    private readonly ApplicationDbContext _context;

    public HeroSlideRepository(ApplicationDbContext context)
    {
        _context = context;
    }

    public async Task<List<HeroSlide>> GetAllAsync()
    {
        return await _context.HeroSlides
            .Include(s => s.Movie)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<List<HeroSlide>> GetActiveAsync()
    {
        return await _context.HeroSlides
            .Include(s => s.Movie)
            .Where(s => s.IsActive)
            .OrderBy(s => s.DisplayOrder)
            .ThenBy(s => s.Id)
            .ToListAsync();
    }

    public async Task<HeroSlide?> GetByIdAsync(int id)
    {
        return await _context.HeroSlides
            .Include(s => s.Movie)
            .FirstOrDefaultAsync(s => s.Id == id);
    }

    public async Task CreateAsync(HeroSlide slide)
    {
        _context.HeroSlides.Add(slide);
        await _context.SaveChangesAsync();
    }

    public async Task UpdateAsync(HeroSlide slide)
    {
        _context.HeroSlides.Update(slide);
        await _context.SaveChangesAsync();
    }

    public async Task DeleteAsync(int id)
    {
        var slide = await _context.HeroSlides.FindAsync(id);
        if (slide != null)
        {
            _context.HeroSlides.Remove(slide);
            await _context.SaveChangesAsync();
        }
    }
}


