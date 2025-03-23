using Microsoft.EntityFrameworkCore;
using TBank.Models;
using TBank.Models.Accounts;

namespace TBank;

public class BankingContext : DbContext
{
    public BankingContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Join(Environment.GetFolderPath(folder), "TBank");
        Directory.CreateDirectory(path);
        DbPath = Path.Join(path, "banking.db");
    }

    public DbSet<User> Users { get; set; }

    public DbSet<Account> Accounts { get; set; }
    public DbSet<BasicAccount> BasicAccounts { get; set; }
    public DbSet<SavingsAccount> SavingsAccounts { get; set; }
    public DbSet<LoanAccount> LoanAccounts { get; set; }

    public DbSet<Transaction> Transactions { get; set; }
    public DbSet<Option> Options { get; set; }
    public DbSet<Log> Logs { get; set; }

    private string DbPath { get; }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
    {
        options.UseSqlite($"Data Source={DbPath}");
    }

    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        User.OnModelCreating(modelBuilder);
        Account.OnModelCreating(modelBuilder);
        Transaction.OnModelCreating(modelBuilder);
        Option.OnModelCreating(modelBuilder);
        Log.OnModelCreating(modelBuilder);

        base.OnModelCreating(modelBuilder);
    }
}