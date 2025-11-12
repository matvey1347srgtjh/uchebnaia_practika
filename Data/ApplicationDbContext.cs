using CinemaApp.Models;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;

namespace CinemaApp.Data;

public class ApplicationDbContext : IdentityDbContext<ApplicationUser>
{
    public ApplicationDbContext(DbContextOptions<ApplicationDbContext> options)
        : base(options)
    {
    }

    public DbSet<Movie> Movies { get; set; }
    public DbSet<Hall> Halls { get; set; }
    public DbSet<Session> Sessions { get; set; }
    public DbSet<Ticket> Tickets { get; set; }
    public DbSet<Seat> Seats { get; set; }

    protected override void OnModelCreating(ModelBuilder builder)
    {
        base.OnModelCreating(builder);

        
        builder.Entity<Movie>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Title).IsRequired().HasMaxLength(200);
            entity.Property(e => e.Description).HasMaxLength(2000);
            entity.Property(e => e.Genre).HasMaxLength(100);
            entity.HasIndex(e => e.Title);
            
            
            
            entity.HasMany(m => m.Sessions)
                .WithOne(s => s.Movie)
                .OnDelete(DeleteBehavior.Cascade); 
        });

        
        builder.Entity<Hall>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Name).IsRequired().HasMaxLength(100);
            entity.Property(e => e.Rows).IsRequired();
            entity.Property(e => e.SeatsPerRow).IsRequired();
        });

        
        builder.Entity<Session>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.MovieId).IsRequired();
            entity.Property(e => e.HallId).IsRequired();
            
            
            
            entity.HasOne(e => e.Hall)
                .WithMany(h => h.Sessions)
                .HasForeignKey(e => e.HallId)
                .OnDelete(DeleteBehavior.Restrict); 
            
            entity.HasIndex(e => e.DateTime);
        });

        
        builder.Entity<Ticket>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.Property(e => e.Price).HasColumnType("decimal(18,2)");
            entity.Property(e => e.TicketCode).IsRequired().HasMaxLength(50);
            
            
            entity.HasOne(e => e.Session)
                .WithMany(s => s.Tickets)
                .HasForeignKey(e => e.SessionId)
                .OnDelete(DeleteBehavior.Cascade); 
                
            entity.HasOne(e => e.User)
                .WithMany(u => u.Tickets)
                .HasForeignKey(e => e.UserId)
                .OnDelete(DeleteBehavior.Restrict);
            entity.HasIndex(e => e.TicketCode).IsUnique();
            entity.HasIndex(e => new { e.SessionId, e.Row, e.SeatNumber });
        });

        
        builder.Entity<Seat>(entity =>
        {
            entity.HasKey(e => e.Id);
            entity.HasOne(e => e.Hall)
                .WithMany(h => h.Seats)
                .HasForeignKey(e => e.HallId)
                .OnDelete(DeleteBehavior.Cascade);
            entity.HasIndex(e => new { e.HallId, e.Row, e.Number }).IsUnique();
        });
    }
}