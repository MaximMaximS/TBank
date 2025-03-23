using System.Text;
using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class ManageAccounts
{
    private readonly BankingContext _db;
    private readonly User _user;
    private readonly bool _owner;
    private readonly Logger _logger;


    private ManageAccounts(BankingContext db, User user, bool owner, Logger logger)
    {
        _db = db;
        _user = user;
        _owner = owner;
        _logger = logger;
    }

    public static void Open(BankingContext db, User user, bool owner, Logger logger)
    {
        var manageAccounts = new ManageAccounts(db, user, owner, logger);

        while (true)
        {
            Console.Clear();

            Console.WriteLine(owner
                ? "Home > Accounts\n"
                : $"Home > Account management ({manageAccounts._user.Username})\n");

            manageAccounts.ListAccounts();

            if (owner)
            {
                Console.Write("Select an account to view or 0 to go back: ");
                var accountNumber = Console.ReadLine();
                if (accountNumber == "0")
                {
                    return;
                }

                if (accountNumber != null)
                {
                    var account = manageAccounts._db.Accounts.FirstOrDefault(a =>
                        a.AccountNumber == accountNumber && a.Owner.UserId == manageAccounts._user.UserId);
                    if (account != null)
                    {
                        AccountView.Open(db, account, true, logger);
                        continue;
                    }
                }

                Console.WriteLine("\nInvalid account number.");
            }
            else
            {
                Console.WriteLine("1. View account");
                Console.WriteLine("2. Create new account");

                Console.WriteLine("0. Back");

                Console.Write("\nSelect an option: ");

                var option = Console.ReadKey(true);
                Console.WriteLine();

                switch (option.KeyChar)
                {
                    case '1':
                        Console.Clear();
                        Console.WriteLine($"Home > Account management ({manageAccounts._user.Username}) > Account\n");

                        manageAccounts.ListAccounts();

                        Console.Write("Enter account number: ");
                        var accountNumber = Console.ReadLine();
                        if (accountNumber != null)
                        {
                            var account = manageAccounts._db.Accounts.FirstOrDefault(a =>
                                a.AccountNumber == accountNumber && a.Owner.UserId == manageAccounts._user.UserId);
                            if (account != null)
                            {
                                AccountView.Open(db, account, true, logger);
                                continue;
                            }
                        }

                        Console.WriteLine("\nInvalid account number.");
                        break;
                    case '2':
                        manageAccounts.CreateAccount();
                        break;
                    case '0':
                        return;

                    default:
                        continue;
                }
            }


            Utils.Footer();
        }
    }

    private void ListAccounts()
    {
        var accounts = _db.Accounts.Where(a => a.Owner.UserId == _user.UserId).ToList();

        if (accounts.Count == 0)
        {
            Console.WriteLine("No accounts found.\n");
            return;
        }

        foreach (var account in accounts)
        {
            var type = account switch
            {
                BasicAccount => "Basic",
                SavingsAccount => "Savings",
                LoanAccount => "Loan",
                _ => "Unknown"
            };

            var balance = new AccountEnumerator(_db, account, _logger).GetBalance();

            Console.WriteLine($"{account.AccountNumber}: {type} - {balance:C}");
        }

        Console.WriteLine();
    }

    private string GenerateAccountNumber()
    {
        while (true)
        {
            // random num 10 digits long
            var num = new StringBuilder();
            var rand = new Random();
            for (var i = 0; i < 10; i++)
            {
                num.Append(rand.Next(0, 10));
            }

            var s = num.ToString();
            var exists = _db.Accounts.Any(a => a.AccountNumber == s);
            if (!exists)
            {
                return s;
            }
        }
    }

    private void CreateAccount()
    {
        Console.Clear();
        Console.WriteLine($"Home > Account management ({_user.Username}) > Create account\n");

        Console.Write("Account type (1. Basic, 2. Savings, 3. Loan): ");
        var type = Console.ReadKey(true);
        Console.WriteLine();

        switch (type.KeyChar)
        {
            case '1':
            {
                var account = new BasicAccount
                {
                    AccountNumber = GenerateAccountNumber(),
                    Owner = _user,
                };
                _db.BasicAccounts.Add(account);
                _db.SaveChanges();
                Console.WriteLine($"Account created: {account.AccountNumber}");
                break;
            }
            case '2':
            {
                const decimal interest = 3.5m;

                var student = Utils.ReadBool("Student");

                var account = new SavingsAccount
                {
                    AccountNumber = GenerateAccountNumber(),
                    Owner = _user,
                    InterestRate = interest,
                    Student = student,
                };

                _db.SavingsAccounts.Add(account);
                _db.SaveChanges();
                Console.WriteLine($"Account created: {account.AccountNumber}");
                break;
            }
            case '3':
            {
                const decimal interest = 6m;
                const int gracePeriod = 30;

                var account = new LoanAccount()
                {
                    AccountNumber = GenerateAccountNumber(),
                    Owner = _user,
                    InterestRate = interest,
                    InterestFreeDays = gracePeriod
                };

                _db.LoanAccounts.Add(account);
                _db.SaveChanges();
                Console.WriteLine($"Account created: {account.AccountNumber}");
                break;
            }
            default:
            {
                Console.WriteLine("Invalid account type.");
                break;
            }
        }
    }
}