
namespace TBank.Bank;

public class Client
{
    private readonly BankingContext _db;
    private readonly int _userId;

    private Client(BankingContext db, int userId)
    {
        this._db = db;
        _userId = userId;
    }

    public static Client Login()
    {
        var db = Initializer.InitBank();

        while (true)
        {
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();
            
            var user = db.Users.FirstOrDefault(u => u.Username == username);
            
            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Login successful.\n");
                return new Client(db, user.UserId);
            }
            
            Console.WriteLine("Login failed. Try again.\n");
        }


    }

}