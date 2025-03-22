﻿using Microsoft.EntityFrameworkCore;
using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank;

public static class Initializer
{
    public static BankingContext InitBank()
    {
        var db = new BankingContext();

        db.Database.Migrate();

        var cAccount = db.Accounts.FirstOrDefault(a => a.AccountNumber == "0000000000");
        if (cAccount != null) return db;
        Console.WriteLine("Initializing bank...\n");

        var root = new User
        {
            Username = "root",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("root"),
            PermissionLevel = 9,
        };
        db.Users.Add(root);
        db.SaveChanges();

        var account = new Account
        {
            AccountNumber = "0000000000",
            Owner = root,
        };
        db.Accounts.Add(account);
        db.SaveChanges();

        var example = new User
        {
            Username = "example",
            PasswordHash = BCrypt.Net.BCrypt.HashPassword("example"),
            PermissionLevel = 0,
        };
        db.Users.Add(example);
        db.SaveChanges();

        var exampleAccount = new BasicAccount
        {
            AccountNumber = "0000000001",
            Owner = example,
        };
        db.BasicAccounts.Add(exampleAccount);
        db.SaveChanges();

        var sampleMoney = new Transaction
        {
            Amount = 1000,
            Sender = account,
            SenderId = account.AccountId,
            Receiver = exampleAccount,
            ReceiverId = exampleAccount.AccountId,
            Note = "Initial deposit",
        };
        db.Transactions.Add(sampleMoney);
        db.SaveChanges();

        var exampleSavings = new SavingsAccount
        {
            AccountNumber = "0000000002",
            Owner = example,
            InterestRate = 3.5m,
            Student = false,
            Created = new DateTime(2021, 1, 1),
        };
        db.SavingsAccounts.Add(exampleSavings);
        db.SaveChanges();

        var sampleSavings = new Transaction
        {
            Amount = 10000,
            Sender = account,
            SenderId = account.AccountId,
            Receiver = exampleSavings,
            ReceiverId = exampleSavings.AccountId,
            Note = "Initial deposit",
            Created = new DateTime(2021, 1, 1),
        };
        db.Transactions.Add(sampleSavings);
        db.SaveChanges();


        Console.Write("Bank initialized successfully. Press any key to continue...");
        Console.ReadKey(true);

        return db;
    }
}