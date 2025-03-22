using TBank.Models;
using TBank.Models.Accounts;

namespace TBank;

using Microsoft.EntityFrameworkCore;
using System;

public class BankingContext : DbContext
{
    public DbSet<User> Users { get; set; }
    
    public DbSet<BasicAccount> BasicAccounts { get; set; }
    public DbSet<SavingsAccount> SavingsAccounts { get; set; }

    private string DbPath { get; }

    public BankingContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Join(Environment.GetFolderPath(folder), "TBank");
        Directory.CreateDirectory(path);
        DbPath = Path.Join(path, "banking.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
    
    protected override void OnModelCreating(ModelBuilder modelBuilder)
    {
        User.OnModelCreating(modelBuilder);
        Account.OnModelCreating(modelBuilder);
        
        base.OnModelCreating(modelBuilder);
    }
}
