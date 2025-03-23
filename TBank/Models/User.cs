using Microsoft.EntityFrameworkCore;
using TBank.Models.Accounts;

namespace TBank.Models;

public class User
{
    public int UserId { get; init; }
    public required string Username { get; init; }
    public required string PasswordHash { get; set; }
    public required int PermissionLevel { get; init; }
    public List<Account> Accounts { get; init; } = [];
    public DateTime Created { get; init; } = DateTime.Now;


    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
        modelBuilder.Entity<User>()
            .Property(u => u.Username)
            .HasMaxLength(24);

        modelBuilder.Entity<User>()
            .Property(u => u.PasswordHash)
            .HasMaxLength(60);
    }
}