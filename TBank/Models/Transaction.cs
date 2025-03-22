using Microsoft.EntityFrameworkCore;
using TBank.Models.Accounts;

namespace TBank.Models;

public class Transaction
{
    public int TransactionId { get; set; }
    public required Account Sender { get; set; }
    public required int SenderId { get; set; }
    public required Account Receiver { get; set; }
    public required int ReceiverId { get; set; }
    public required decimal Amount { get; set; }
    public DateTime Created { get; set; } = DateTime.Now;

    public required string Note { get; set; }

    public static void OnModelCreating(ModelBuilder modelBuilder)
    {
        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Sender)
            .WithMany(a => a.OutgoingTransactions)
            .HasForeignKey(t => t.SenderId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .HasOne(t => t.Receiver)
            .WithMany(a => a.IncomingTransactions)
            .HasForeignKey(t => t.ReceiverId)
            .OnDelete(DeleteBehavior.Restrict);

        modelBuilder.Entity<Transaction>()
            .Property(t => t.Amount)
            .HasPrecision(10, 2);
    }
}