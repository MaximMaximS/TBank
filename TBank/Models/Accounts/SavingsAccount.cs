using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public class SavingsAccount : Account
{
    public required decimal InterestRate { get; set; }
    public required bool Student { get; set; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavingsAccount>()
            .Property(sa => sa.InterestRate)
            .HasPrecision(5, 2);
    }
}