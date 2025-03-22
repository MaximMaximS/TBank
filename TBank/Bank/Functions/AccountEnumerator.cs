using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank.Functions;

public class AccountEnumerator(BankingContext db, Account account)
{
    private readonly BankingContext _db = db;
    private readonly Account _account = account;

    public decimal GetBalance()
    {
        var incoming = _db.Transactions.Where(t => t.ReceiverId == _account.AccountId).Sum(t => t.Amount);
        var outgoing = _db.Transactions.Where(t => t.SenderId == _account.AccountId).Sum(t => t.Amount);
        return incoming - outgoing;
    }
    
    public List<Transaction> GetTransactions()
    {
        var transactions = _db.Transactions.Where(t => t.ReceiverId == _account.AccountId || t.SenderId == _account.AccountId).OrderBy(t => t.Created).ToList();
        
        return transactions;
    }
}