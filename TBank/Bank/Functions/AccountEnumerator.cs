using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class AccountEnumerator(BankingContext db, Account account, Logger logger)
{
    private decimal InterestForMonth(int year, int month, decimal interestRate, int? gracePeriod)
    {
        var daysInMonth = DateTime.DaysInMonth(year, month);

        var avg = 0m;

        for (var i = 1; i <= daysInMonth; i++)
        {
            var date = new DateTime(year, month, i);
            avg += gracePeriod != null ? GetBalanceAtLoan(date, gracePeriod.Value) : GetBalanceAt(date);
        }

        avg /= daysInMonth;

        if (gracePeriod == null)
        {
            avg = avg * interestRate / 100 / 12;

            // round to 2 decimal places
            avg = Math.Floor(avg * 100) / 100;

            return avg;
        }

        if (avg >= 0) return 0;

        avg = avg * interestRate / 100 / 12;

        // round to 2 decimal places
        avg = Math.Floor(avg * 100) / 100;

        return -avg;
    }

    private void ApplyInterest()
    {
        if (account is not LoanAccount && account is not SavingsAccount) return;

        var lastInterest = account is LoanAccount
            ? db.Transactions
                .Where(t => t.SenderId == account.AccountId && t.Note == "Interest")
                .OrderByDescending(t => t.Created)
                .FirstOrDefault()
            : db.Transactions
                .Where(t => t.ReceiverId == account.AccountId && t.Note == "Interest")
                .OrderByDescending(t => t.Created)
                .FirstOrDefault();


        // interest is paid monthly at 1. st of the next month
        if (lastInterest != null && lastInterest.Created.Month == DateTime.Now.Month &&
            lastInterest.Created.Year == DateTime.Now.Year) return;
        if (account.Created.Month == DateTime.Now.Month && account.Created.Year == DateTime.Now.Year) return;

        var startMonth = lastInterest == null ? account.Created.Month : lastInterest.Created.Month;
        var startYear = lastInterest == null ? account.Created.Year : lastInterest.Created.Year;

        var root = db.Accounts.FirstOrDefault(a => a.AccountNumber == "0000000000")!;

        while (startYear < DateTime.Now.Year || (startYear == DateTime.Now.Year && startMonth < DateTime.Now.Month))
        {
            switch (account)
            {
                case SavingsAccount savingsAccount:
                {
                    var interest = InterestForMonth(startYear, startMonth, savingsAccount.InterestRate, null);
                    var transaction = new Transaction
                    {
                        Amount = interest,
                        Receiver = account,
                        ReceiverId = account.AccountId,
                        Sender = root,
                        SenderId = root.AccountId,
                        Note = "Interest",
                        Created = new DateTime(startYear, startMonth, 1).AddMonths(1)
                    };
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    logger.Log($"Interest (Savings) {interest:C} - {account.AccountNumber}");
                    break;
                }
                case LoanAccount loanAccount:
                {
                    var interest = InterestForMonth(startYear, startMonth, loanAccount.InterestRate,
                        loanAccount.InterestFreeDays);
                    var transaction = new Transaction
                    {
                        Amount = interest,
                        Sender = account,
                        SenderId = account.AccountId,
                        Receiver = root,
                        ReceiverId = root.AccountId,
                        Note = "Interest",
                        Created = new DateTime(startYear, startMonth, 1).AddMonths(1)
                    };
                    db.Transactions.Add(transaction);
                    db.SaveChanges();
                    logger.Log($"Interest (Loan) {interest:C} - {account.AccountNumber}");
                    break;
                }
            }

            startMonth++;
            if (startMonth <= 12) continue;
            startMonth = 1;
            startYear++;
        }
    }

    public decimal PreviewInterest(DateTime when)
    {
        switch (account)
        {
            case SavingsAccount savingsAccount:
            {
                ApplyInterest();

                var bal = GetBalance();

                // how many times will interest be paid between now and when
                var interestTimes = (when.Year - DateTime.Now.Year) * 12 + when.Month - DateTime.Now.Month;

                var interest = bal;

                for (var i = 0; i < interestTimes; i++)
                {
                    interest *= 1 + savingsAccount.InterestRate / 100 / 12;
                    interest = Math.Floor(interest * 100) / 100;
                }

                interest -= bal;

                return interest;
            }
            case LoanAccount loanAccount:
            {
                ApplyInterest();

                var bal = GetBalance();

                // how many times will interest be paid between now and when
                var interestTimes = (when.Year - DateTime.Now.Year) * 12 + when.Month - DateTime.Now.Month;

                var interest = bal;

                for (var i = 0; i < interestTimes; i++)
                {
                    interest *= 1 + loanAccount.InterestRate / 100 / 12;
                    interest = Math.Floor(interest * 100) / 100;
                }

                interest -= bal;

                return interest;
            }
            default:
                return 0;
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

    private decimal GetBalanceAtLoan(DateTime date, int days)
    {
        var incoming = db.Transactions
            .Where(t => t.ReceiverId == account.AccountId && t.Created <= date)
            .Sum(t => t.Amount);

        // all loan minus those within grace period
        var outgoing = db.Transactions
            .Where(t => t.SenderId == account.AccountId && (t.Created < date.AddDays(-days) || t.Note == "Interest"))
            .Sum(t => t.Amount);

        return incoming - outgoing;
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