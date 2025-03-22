using TBank.Models;

namespace TBank.Bank.Functions;

public class ManageUsers
{
    private readonly BankingContext _db;

    private ManageUsers(BankingContext db)
    {
        _db = db;
    }

    public static void Open(BankingContext db)
    {
        var manageUsers = new ManageUsers(db);

        while (true)
        {
            Console.Clear();

            Console.WriteLine("User management\n");

            Console.WriteLine("1. List users");
            Console.WriteLine("2. Create user");

            Console.WriteLine("0. Back");

            Console.Write("\nSelect an option: ");

            var option = Console.ReadKey(true);

            switch (option.KeyChar)
            {
                case '1':
                    manageUsers.ListUsers();
                    break;
                case '2':
                    manageUsers.CreateUser();
                    break;
                case '0':
                    return;
            }

            Utils.Footer();
        }
    }
    
    private void ListUsers()
    {
        Console.Clear();

        var users = _db.Users.ToList();

        if (users.Count == 0)
        {
            Console.WriteLine("No users found.");
            return;
        }

        Console.WriteLine("Users:\n");

        foreach (var user in users)
        {
            Console.WriteLine(user.Username);
        }
    }
    
    private void CreateUser()
    {
        Console.Clear();

        Console.Write("Enter username: ");
        var username = Console.ReadLine();

        Console.Write("Enter password: ");
        var password = Console.ReadLine();
        
        if (string.IsNullOrWhiteSpace(username) || string.IsNullOrWhiteSpace(password))
        {
            Console.WriteLine("\nInvalid input.");
            return;
        }
        
        if (password.Length < 8)
        {
            Console.WriteLine("\nPassword must be at least 8 characters long.");
            return;
        }

        var manageAccounts = Utils.ReadBool("Can manage accounts");
        var manageUsers = Utils.ReadBool("Can manage users");
        
        

        var user = new User
        {
            Username = username,
            PasswordHash = BCrypt.Net.BCrypt.HashPassword(password),
            ManageAccounts = manageAccounts,
            ManageUsers = manageUsers
        };

        _db.Users.Add(user);
        _db.SaveChanges();

        Console.WriteLine("\nUser created.");
    }
}