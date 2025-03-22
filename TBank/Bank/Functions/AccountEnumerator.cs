using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class AccountEnumerator(BankingContext db, Account account)
{
    private decimal InterestForMonth(int year, int month, decimal interestRate)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var avg = 0m;

        for (var i = 1; i <= daysInMonth; i++)
        {
            var date = new DateTime(year, month, i);
            avg += GetBalanceAt(date);
        }

        avg /= daysInMonth;

        avg = avg * interestRate / 100 / 12;

        // round to 2 decimal places
        avg = Math.Floor(avg * 100) / 100;

        return avg;
    }

    private void ApplyInterest()
    {
        if (account is not SavingsAccount savingsAccount) return;
        var lastInterest = db.Transactions
            .Where(t => t.ReceiverId == account.AccountId && t.Note == "Interest")
            .OrderByDescending(t => t.Created)
            .FirstOrDefault();

        // interest is paid monthly at 1. st of the next month
        if (lastInterest != null && lastInterest.Created.Month == DateTime.Now.Month &&
            lastInterest.Created.Year == DateTime.Now.Year) return;
        if (account.Created.Month == DateTime.Now.Month && account.Created.Year == DateTime.Now.Year)
        {
            return;
        }

        var startMonth = lastInterest == null ? account.Created.Month : lastInterest.Created.Month;
        var startYear = lastInterest == null ? account.Created.Year : lastInterest.Created.Year;

        var root = db.Accounts.FirstOrDefault(a => a.AccountNumber == "0000000000")!;

        while (startYear < DateTime.Now.Year || (startYear == DateTime.Now.Year && startMonth < DateTime.Now.Month))
        {
            var interest = InterestForMonth(startYear, startMonth, savingsAccount.InterestRate);
            var transaction = new Transaction
            {
                Amount = interest,
                Receiver = account,
                ReceiverId = account.AccountId,
                Sender = root,
                SenderId = root.AccountId,
                Note = "Interest",
                Created = new DateTime(startYear, startMonth, 1).AddMonths(1),
            };
            db.Transactions.Add(transaction);
            db.SaveChanges();

            startMonth++;
            if (startMonth <= 12) continue;
            startMonth = 1;
            startYear++;
        }
    }

    private decimal GetBalanceAt(DateTime date)
    {
        var incoming = db.Transactions
            .Where(t => t.ReceiverId == account.AccountId && t.Created <= date)
            .Sum(t => t.Amount);
        var outgoing = db.Transactions
            .Where(t => t.SenderId == account.AccountId && t.Created <= date)
            .Sum(t => t.Amount);

        return incoming + outgoing;
    }

    public decimal GetBalance()
    {
        ApplyInterest();
        var incoming = db.Transactions.Where(t => t.ReceiverId == account.AccountId).Sum(t => t.Amount);
        var outgoing = db.Transactions.Where(t => t.SenderId == account.AccountId).Sum(t => t.Amount);
        return incoming - outgoing;
    }

    public List<Transaction> GetTransactions()
    {
        ApplyInterest();
        var transactions = db.Transactions
            .Where(t => t.ReceiverId == account.AccountId || t.SenderId == account.AccountId).OrderBy(t => t.Created)
            .ToList();

        return transactions;
    }
}