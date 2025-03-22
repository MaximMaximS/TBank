using TBank.Bank.Functions;
using TBank.Models;

namespace TBank.Bank;

public class Client
{
    private readonly BankingContext _db;
    private User _user;

    private Client(BankingContext db, User user)
    {
        _db = db;
        _user = user;
    }

    public void Update()
    {
        var newUser = _db.Users.Find(_user.UserId);
        _user = newUser ?? throw new ApplicationException("User not found");
    }

    public static Client Login()
    {
        var db = Initializer.InitBank();

        while (true)
        {
            Console.Clear();
            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            Console.WriteLine();

            var user = db.Users.FirstOrDefault(u => u.Username == username);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Login successful.");
                return new Client(db, user);
            }

            Console.Write("Login failed. Press any key to try again...");
            Console.ReadKey(true);
        }
    }

    public bool ShowMenu()
    {
        return Main.Open(_user, _db);
    }
}