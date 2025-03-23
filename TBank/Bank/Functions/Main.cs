using TBank.Models;

namespace TBank.Bank.Functions;

public class Main
{
    private readonly BankingContext _db;
    private readonly Logger _logger;
    private readonly User _user;

    private Main(User user, BankingContext db, Logger logger)
    {
        _user = user;
        _db = db;
        _logger = logger;
    }

    public static bool Open(User user, BankingContext db, Logger logger)
    {
        var main = new Main(user, db, new Logger(db));

        while (true)
        {
            Console.Clear();

            Console.WriteLine("Home\n");

            Console.WriteLine("1. View accounts");
            Console.WriteLine("2. Change password");

            if (user.PermissionLevel >= 4) Console.WriteLine("3. Account management");

            if (user.PermissionLevel >= 8) Console.WriteLine("4. User management");

            Console.WriteLine("0. Logout");

            Console.Write("\nSelect an option: ");
            var option = Console.ReadKey(true);

            var logout = false;
            switch (option.KeyChar)
            {
                case '1':
                    ManageAccounts.Open(db, user, true, logger);
                    continue;
                case '2':
                    if (main.ChangePassword()) logout = true;

                    break;
                case '3':
                    if (user.PermissionLevel < 4) continue;

                    Console.Clear();
                    Console.WriteLine("Home > Account management\n");

                    Console.Write("Open user: ");
                    var openUser = Console.ReadLine();
                    if (openUser != null)
                    {
                        if (openUser == "root")
                        {
                            Console.WriteLine("\nCannot edit root accounts.");
                            break;
                        }

                        var userAccount = db.Users.FirstOrDefault(u => u.Username == openUser);
                        if (userAccount != null)
                        {
                            ManageAccounts.Open(db, userAccount, false, logger);
                            continue;
                        }
                    }

                    Console.WriteLine("\nInvalid user.");
                    break;
                case '4':
                    if (user.PermissionLevel < 8) continue;

                    ManageUsers.Open(db);
                    continue;

                case '0':
                    return false;

                default:
                    continue;
            }

            Utils.Footer();

            if (logout) return true;
        }
    }

    private bool ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("Home > Change password\n");

        Console.Write("Enter current password: ");
        var currentPassword = Console.ReadLine();

        if (currentPassword != null && BCrypt.Net.BCrypt.Verify(currentPassword, _user.PasswordHash))
        {
            Console.Write("Enter new password: ");
            var newPassword = Console.ReadLine();
            Console.Write("Confirm new password: ");
            var confirmNewPassword = Console.ReadLine();

            Console.WriteLine();

            if (newPassword != null && confirmNewPassword != null && newPassword.Length < 8)
            {
                Console.WriteLine("\nPassword must be at least 8 characters long.");
                return false;
            }

            if (newPassword == confirmNewPassword)
            {
                var dbUser = _db.Users.Find(_user.UserId);
                if (dbUser == null) throw new ApplicationException("User not found");

                dbUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _db.SaveChanges();

                Console.WriteLine("\nPassword changed successfully.");
                Console.WriteLine("You will be logged out.");
                return true;
            }

            Console.WriteLine("\nPasswords do not match.");
            return false;
        }

        Console.WriteLine("\nIncorrect password.");
        return false;
    }
}