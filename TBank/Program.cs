using TBank.Models;

namespace TBank;

internal static class Program
{
    private static void Main()
    {
        var db = new BankingContext();

        db.Add(new User("alice", "password"));
        
        db.SaveChanges();
    }
}