using TBank.Models;

namespace TBank;

using Microsoft.EntityFrameworkCore;
using System;

public class BloggingContext : DbContext
{
    public DbSet<User> Users { get; set; }

    public string DbPath { get; }

    public BloggingContext()
    {
        const Environment.SpecialFolder folder = Environment.SpecialFolder.LocalApplicationData;
        var path = Environment.GetFolderPath(folder);
        DbPath = System.IO.Path.Join(path, "tbank.db");
    }

    // The following configures EF to create a Sqlite database file in the
    // special "local" folder for your platform.
    protected override void OnConfiguring(DbContextOptionsBuilder options)
        => options.UseSqlite($"Data Source={DbPath}");
}
