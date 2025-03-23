using Microsoft.EntityFrameworkCore;
using TBank.Models.Accounts;

namespace TBank.Models;

public class Transaction
{
    public int TransactionId { get; init; }
    public required Account Sender { get; init; }
    public required int SenderId { get; init; }
    public required Account Receiver { get; init; }
    public required int ReceiverId { get; init; }
    public required decimal Amount { get; init; }
    public DateTime Created { get; init; } = DateTime.Now;

    public required string Note { get; init; }

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