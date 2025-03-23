using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public class LoanAccount : Account
{
    public required decimal InterestRate { get; init; }
    public required int InterestFreeDays { get; init; }

    public new static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<LoanAccount>()
            .Property(sa => sa.InterestRate)
            .HasPrecision(5, 2);
    }
}