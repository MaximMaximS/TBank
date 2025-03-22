using Microsoft.EntityFrameworkCore;
using TBank.Models.Accounts;

namespace TBank.Models;

public class User
{
    public int UserId { get; set; }
    public required string Username { get; set; }
    public required string PasswordHash { get; set; }
    public required int PermissionLevel { get; set; }
    public List<Account> Accounts { get; set; } = [];

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