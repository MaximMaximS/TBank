using TBank.Models;

namespace TBank;

internal static class Program
{
    private static void Main()
    {
        var db = new BankingContext();
        
        var passwordHash =  BCrypt.Net.BCrypt.HashPassword("password");

        db.Add(new User("alice", passwordHash));
        
        db.SaveChanges();
    }
}