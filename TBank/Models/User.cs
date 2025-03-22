using Microsoft.EntityFrameworkCore;
using TBank.Models.Accounts;

namespace TBank.Models;

public class User(string username, string passwordHash)
{
    public int UserId { get; set; }
    public string Username { get; set; } = username;
    public string PasswordHash { get; set; } = passwordHash;
    
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