using TBank.Bank.Functions;
using TBank.Models;
using TBank.Models.Accounts;

namespace TBank.Bank;

public class Client
{
    private readonly BankingContext _db;
    private User _user;
    private Logger _logger;

    private Client(BankingContext db, User user, Logger logger)
    {
        _db = db;
        _user = user;
        _logger = logger;
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
            Console.WriteLine("Login\n");

            Console.Write("Enter username: ");
            var username = Console.ReadLine();
            Console.Write("Enter password: ");
            var password = Console.ReadLine();

            Console.WriteLine();

            var user = db.Users.FirstOrDefault(u => u.Username == username);

            var logger = new Logger(db);

            if (user != null && BCrypt.Net.BCrypt.Verify(password, user.PasswordHash))
            {
                Console.WriteLine("Login successful.");
                logger.Log($"User {user.Username} logged in.");
                return new Client(db, user, logger);
            }

            Console.Write("Login failed. Press any key to try again...");
            logger.Log($"Login failed for user {username}.");
            Console.ReadKey(true);
        }
    }

    public bool ShowMenu()
    {
        return Main.Open(_user, _db, _logger);
    }
}