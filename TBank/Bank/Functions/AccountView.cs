﻿using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class AccountView
{
    private readonly BankingContext _db;
    private readonly Account _account;
    private readonly bool _owner;
    private readonly AccountEnumerator _enumerator;
    private readonly string _header;

    private AccountView(BankingContext db, Account account, bool owner)
    {
        _db = db;
        _account = account;
        _owner = owner;
        _enumerator = new AccountEnumerator(db, account);

        _header = owner
            ? $"Home > Accounts > {account.AccountNumber}"
            : $"Home > Account management ({account.Owner.Username}) > Account {account.AccountNumber}";
    }

    public static void Open(BankingContext db, Account account, bool owner)
    {
        while (true)
        {
            var r = InnerOpen(db, account.AccountId, owner);
            if (!r)
            {
                return;
            }
        }
    }

    private void PrintAccount()
    {
        Console.WriteLine($"Account number: {_account.AccountNumber}");
        Console.WriteLine($"Balance: {_enumerator.GetBalance():C}\n");
    }

    private static bool InnerOpen(BankingContext db, int accountId, bool owner)
    {
        var account = db.Accounts.Find(accountId);
        if (account == null)
        {
            throw new ApplicationException("Account not found");
        }

        var accountView = new AccountView(db, account, owner);

        Console.Clear();

        Console.WriteLine(accountView._header + "\n");

        accountView.PrintAccount();

        Console.WriteLine("1. View transactions");
        Console.WriteLine("2. Send money");

        Console.WriteLine("0. Back");

        var option = Console.ReadKey(true);
        switch (option.KeyChar)
        {
            case '1':
                accountView.ViewTransactions();
                break;
            case '2':
                //accountView.SendMoney();
                break;
            case '0':
                return false;
            default:
                return true;
        }

        Utils.Footer();
        return true;
    }

    private void ViewTransactions()
    {
        Console.Clear();

        Console.WriteLine($"{_header} > Transactions\n");

        var transactions = _enumerator.GetTransactions();

        if (transactions.Count == 0)
        {
            Console.WriteLine("No transactions found.");
            return;
        }

        foreach (var transaction in transactions)
        {
            var incoming = transaction.Receiver.AccountId == _account.AccountId;
            var amount = incoming ? transaction.Amount : -transaction.Amount;

            Console.WriteLine(transaction.Note);
            Console.WriteLine(
                $"{transaction.Created} - {transaction.Sender.AccountNumber} -> {transaction.Receiver.AccountNumber}: {amount:C}");
        }
    }
}