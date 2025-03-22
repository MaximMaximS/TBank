namespace TBank.Bank.Functions;

public static class Utils
{
    public static void Footer()
    {
        Console.WriteLine("\nPress any key to continue...");
        Console.ReadKey(true);
    }

    public static bool ReadBool(string prompt)
    {
        bool q;
        while (true)
        {
            Console.Write($"{prompt} (y/n): ");
            var input = Console.ReadKey(true);
            if (input.KeyChar is 'y' or 'Y')
            {
                q = true;
                break;
            }

            if (input.KeyChar is 'n' or 'N')
            {
                q = false;
                break;
            }

            // erase the line
            Console.SetCursorPosition(0, Console.CursorTop);
        }

        Console.WriteLine();

        return q;
    }
}