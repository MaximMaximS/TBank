using TBank.Models;

namespace TBank.Bank.Functions;

public class Main
{
    private readonly User _user;
    private readonly BankingContext _db;

    private Main(User user, BankingContext db)
    {
        _user = user;
        _db = db;
    }

    public static bool Open(User user, BankingContext db)
    {
        var main = new Main(user, db);

        while (true)
        {
            Console.Clear();

            Console.WriteLine("Main menu\n");

            Console.WriteLine("1. View accounts");
            Console.WriteLine("2. Change password");

            if (user.PermissionLevel >= 4)
            {
                Console.WriteLine("3. Account management");
            }

            if (user.PermissionLevel >= 8)
            {
                Console.WriteLine("4. User management");
            }

            Console.WriteLine("0. Logout");

            Console.Write("\nSelect an option: ");
            var option = Console.ReadKey(true);

            var logout = false;
            switch (option.KeyChar)
            {
                case '1':
                    main.ViewAccounts();
                    continue;
                case '2':
                    if (main.ChangePassword())
                    {
                        logout = true;
                    }
                    break;
                case '4':
                    if (user.PermissionLevel < 8)
                    {
                        continue;
                    }
                    ManageUsers.Open(db);
                    continue;
                
                case '0':
                    return false;

                default:
                    continue;
            }

            Utils.Footer();

            if (logout)
            {
                return true;
            }
        }
    }

    private void ViewAccounts()
    {
    }

    private bool ChangePassword()
    {
        Console.Clear();
        Console.WriteLine("Changing password\n");
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
                Console.WriteLine("Password must be at least 8 characters long.");
                return false;
            }

            if (newPassword == confirmNewPassword)
            {
                var dbUser = _db.Users.Find(_user.UserId);
                if (dbUser == null)
                {
                    throw new ApplicationException("User not found");
                }

                dbUser.PasswordHash = BCrypt.Net.BCrypt.HashPassword(newPassword);
                _db.SaveChanges();

                Console.WriteLine("Password changed successfully.");
                Console.WriteLine("You will be logged out.");
                return true;
            }

            Console.WriteLine("Passwords do not match.");
            return false;
        }

        Console.WriteLine("Incorrect password.");
        return false;
    }
}