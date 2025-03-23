using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public class Account
{
    public int AccountId { get; init; }
    public required string AccountNumber { get; init; }
    public required User Owner { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;

    public List<Transaction> OutgoingTransactions { get; init; } = [];
    public List<Transaction> IncomingTransactions { get; init; } = [];

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("Accounts").HasDiscriminator<string>("AccountType")
            .HasValue<Account>("Base")
            .HasValue<BasicAccount>("Basic")
            .HasValue<SavingsAccount>("Savings")
            .HasValue<LoanAccount>("Loan");

        modelBuilder.Entity<Account>().HasIndex(a => a.AccountNumber).IsUnique();

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountNumber)
            .HasMaxLength(10);

        SavingsAccount.OnModelCreating(modelBuilder);
        LoanAccount.OnModelCreating(modelBuilder);
    }
}