using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PTTDigital.Email.Data.Interfaces;
using PTTDigital.Email.Data.Models;

namespace PTTDigital.Email.Data.Context;

public class EmailContextBase<TDbContext> : DbContextBase<TDbContext>
    where TDbContext : DbContext
{
    public EmailContextBase(DbContextOptions<TDbContext> options, IEntityConfig<TDbContext> config)
        : base(options)
    {
        defaultSchema = config?.Schema;
        defaultCollation = config?.Collation;
    }

    //protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
    //{
    //    optionsBuilder.LogTo(Console.WriteLine);
    //    Console.ForegroundColor = ConsoleColor.Red;
    //}

    public DbSet<Message>? Messages { get; set; }
    public DbSet<EmailQueue>? EmailQueues { get; set; }
    public DbSet<EmailArchive>? EmailArchives { get; set; }
}
