using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public class SavingsAccount : Account
{
    public decimal InterestRate { get; set; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<SavingsAccount>()
            .Property(sa => sa.InterestRate)
            .HasPrecision(5, 2);
    }
}