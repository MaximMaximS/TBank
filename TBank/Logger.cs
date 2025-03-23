using TBank.Models;

namespace TBank;

public class Logger
{
    private readonly BankingContext _db;
    private readonly bool _logToDb;
    private readonly string _log;

    public Logger(BankingContext db)
    {
        _db = db;
        _logToDb = _db.Options.FirstOrDefault(o => o.Key == "LogToDb")?.Value == "true";

        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Path.Join(Environment.GetFolderPath(folder), "TBank");
        Directory.CreateDirectory(path);
        _log = Path.Join(path, "log.txt");
    }

    public void Log(string message)
    {
        if (_logToDb)
        {
            _db.Logs.Add(new Log { Message = message });
            _db.SaveChanges();
        }
        else
        {
            File.AppendAllText(_log, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss} {message}\n");
        }
    }
}