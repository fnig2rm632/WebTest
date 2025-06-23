using System.Reflection;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore;
using WebTest.Models;

namespace WebTest.Data;

public class ApplicationDbContext : IdentityDbContext<User>
{
    public DbSet<Friendship> Friendship { get; set; }
    public DbSet<Game> Game { get; set; }
    
    public ApplicationDbContext(DbContextOptions dbContextOptions) 
        : base(dbContextOptions)
    {
        
    }
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        base.OnModelCreating(modelBuilder);

        modelBuilder.Entity<Friendship>()
            .HasKey(f => new { f.UserId, f.FriendId });

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.User)
            .WithMany(u => u.Friends)
            .HasForeignKey(f => f.UserId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Friendship>()
            .HasOne(f => f.Friend)
            .WithMany()
            .HasForeignKey(f => f.FriendId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerWhite)
            .WithMany(u => u.GamesAsWhite)
            .HasForeignKey(g => g.PlayerWhiteId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.PlayerBlack)
            .WithMany(u => u.GamesAsBlack)
            .HasForeignKey(g => g.PlayerBlackId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Game>()
            .HasOne(g => g.Winner)
            .WithMany(u => u.GamesWiners)
            .HasForeignKey(g => g.WinnerId)
            .OnDelete(DeleteBehavior.Restrict);

    }
}