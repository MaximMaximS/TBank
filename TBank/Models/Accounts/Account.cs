using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public class Account
{
    public int AccountId { get; set; }
    public required string AccountNumber { get; set; }
    public required User Owner { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    public List<Transaction> OutgoingTransactions { get; set; } = [];
    public List<Transaction> IncomingTransactions { get; set; } = [];

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("Accounts").HasDiscriminator<string>("AccountType")
            .HasValue<Account>("Base")
            .HasValue<BasicAccount>("Basic")
            .HasValue<SavingsAccount>("Savings");

        modelBuilder.Entity<Account>().HasIndex(a => a.AccountNumber).IsUnique();

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountNumber)
            .HasMaxLength(10);

        SavingsAccount.OnModelCreating(modelBuilder);
    }
}