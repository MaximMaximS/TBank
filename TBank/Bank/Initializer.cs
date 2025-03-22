using Microsoft.EntityFrameworkCore;
using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank;

public static class Initializer
{
    public static BankingContext InitBank()
    {
        var db = new BankingContext();

        db.Database.Migrate();

        var account = db.Accounts.FirstOrDefault(a => a.AccountNumber == "0000000000");
        if (account != null) return db;
        Console.WriteLine("Initializing bank...\n");

        var root = db.Users.FirstOrDefault(u => u.Username == "root");
        if (root == null)
        {
            root = new User
            {
                Username = "root",
                PasswordHash = BCrypt.Net.BCrypt.HashPassword("root"),
                ManageAccounts = true,
                ManageUsers = true
            };
            db.Users.Add(root);
            db.SaveChanges();

            Console.WriteLine("Root user created.");
        }

        account = new Account
        {
            AccountNumber = "0000000000",
            Owner = root,
        };
        db.Accounts.Add(account);
        db.SaveChanges();

        Console.WriteLine("Root account created.");

        Console.Write("\nBank initialized successfully. Press any key to continue...");
        Console.ReadKey(true);

        return db;
    }
}