using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class AccountView
{
    private readonly BankingContext _db;
    private readonly Account _account;
    private readonly bool _owner;

    private AccountView(BankingContext db, Account account, bool owner)
    {
        _db = db;
        _account = account;
        _owner = owner;
    }

    public static void Open(BankingContext db, Account account, bool owner)
    {
        var accountView = new AccountView(db, account, owner);

        while (true)
        {
            Console.Clear();

            Console.WriteLine(owner
                ? $"Home > Accounts > {accountView._account.AccountNumber}\n"
                : $"Home > Account management ({accountView._account.Owner.Username}) > {accountView._account.AccountNumber}\n");
        }
    }
}