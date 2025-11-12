using CinemaApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Data;

public static class SeedData
{
    public static async Task Initialize(IServiceProvider serviceProvider)
    {
        using var scope = serviceProvider.CreateScope();
        var context = scope.ServiceProvider.GetRequiredService<ApplicationDbContext>();
        var userManager = scope.ServiceProvider.GetRequiredService<UserManager<ApplicationUser>>();
        var roleManager = scope.ServiceProvider.GetRequiredService<RoleManager<IdentityRole>>();

        
        if (!await roleManager.RoleExistsAsync("Admin"))
        {
            await roleManager.CreateAsync(new IdentityRole("Admin"));
        }

        
        if (await userManager.FindByEmailAsync("admin@cinema.com") == null)
        {
            var admin = new ApplicationUser
            {
                UserName = "admin@cinema.com",
                Email = "admin@cinema.com",
                EmailConfirmed = true,
                FullName = "Administrator"
            };
            await userManager.CreateAsync(admin, "admin123");
            await userManager.AddToRoleAsync(admin, "Admin");
        }

        
        if (!await context.Movies.AnyAsync())
        {
            var movies = new[]
            {
                new Movie
                {
                    Title = "Матрица",
                    Description = "Хакер Нео узнает, что реальный мир — это компьютерная симуляция под названием Матрица.",
                    Genre = "Фантастика",
                    Duration = 136,
                    PosterUrl = "https:",
                    IsActive = true
                },
                new Movie
                {
                    Title = "Начало",
                    Description = "Профессиональный вор Доминик Кобб крадет секреты из подсознания людей во время сна.",
                    Genre = "Триллер",
                    Duration = 148,
                    PosterUrl = "https:",
                    IsActive = true
                },
                new Movie
                {
                    Title = "Интерстеллар",
                    Description = "Группа исследователей отправляется в космос через червоточину в поисках нового дома для человечества.",
                    Genre = "Фантастика",
                    Duration = 169,
                    PosterUrl = "https:",
                    IsActive = true
                }
            };
            await context.Movies.AddRangeAsync(movies);
        }

        if (!await context.Halls.AnyAsync())
        {
            var hall1 = new Hall { Name = "Зал 1", Rows = 10, SeatsPerRow = 15 };
            var hall2 = new Hall { Name = "Зал 2", Rows = 8, SeatsPerRow = 12 };
            
            await context.Halls.AddRangeAsync(hall1, hall2);
            await context.SaveChangesAsync();

            var seats1 = new List<Seat>();
            for (int row = 1; row <= hall1.Rows; row++)
            {
                for (int seat = 1; seat <= hall1.SeatsPerRow; seat++)
                {
                    seats1.Add(new Seat { HallId = hall1.Id, Row = row, Number = seat });
                }
            }

            var seats2 = new List<Seat>();
            for (int row = 1; row <= hall2.Rows; row++)
            {
                for (int seat = 1; seat <= hall2.SeatsPerRow; seat++)
                {
                    seats2.Add(new Seat { HallId = hall2.Id, Row = row, Number = seat });
                }
            }

            await context.Seats.AddRangeAsync(seats1);
            await context.Seats.AddRangeAsync(seats2);
        }

        await context.SaveChangesAsync();
    }
}

