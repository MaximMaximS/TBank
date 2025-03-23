using Microsoft.EntityFrameworkCore;

namespace TBank.Models;

public class Log
{
    public int LogId { get; init; }
    public required string Message { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Log>()
            .Property(l => l.Message)
            .HasMaxLength(255);
    }
}