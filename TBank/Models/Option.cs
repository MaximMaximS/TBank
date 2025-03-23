using Microsoft.EntityFrameworkCore;

namespace TBank.Models;

public class Option
{
    public int OptionId { get; init; }
    public required string Key { get; init; }
    public required string Value { get; init; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Option>().HasIndex(o => o.Key).IsUnique();
        modelBuilder.Entity<Option>()
            .Property(o => o.Key)
            .HasMaxLength(24);
        modelBuilder.Entity<Option>()
            .Property(o => o.Value)
            .HasMaxLength(255);
    }
}