using Microsoft.EntityFrameworkCore;

namespace TBank.Models;

public class User(string username, string passwordHash)
{
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<User>().HasIndex(u => u.Username).IsUnique();
    }
    
    public int UserId { get; set; }
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
}