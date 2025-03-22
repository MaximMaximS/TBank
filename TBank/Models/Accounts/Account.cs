using Microsoft.EntityFrameworkCore;

namespace TBank.Models.Accounts;

public abstract class Account
{
    public int AccountId { get; set; }
    public required string AccountNumber { get; set; }
    public required User Owner { get; set; }
 
    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Account>().ToTable("Accounts");
        
        modelBuilder.Entity<Account>().HasIndex(a => a.AccountNumber).IsUnique();

        modelBuilder.Entity<Account>()
            .Property(a => a.AccountNumber)
            .HasMaxLength(10);
        
        SavingsAccount.OnModelCreating(modelBuilder);
    }
}