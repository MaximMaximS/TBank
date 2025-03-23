using TBank.Bank;

namespace TBank;

internal static class Program
{
    private static void Main()
    {
        bool loop;
        do
        {
            var client = Client.Login();
            loop = client.ShowMenu();
        } while (loop);

        Console.Clear();
        Console.WriteLine("Goodbye!");
    }
}