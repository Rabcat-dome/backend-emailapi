using System.Text.RegularExpressions;
using Microsoft.EntityFrameworkCore;
using PTTDigital.Authentication.Data.Interfaces;
using PTTDigital.Email.Data.Interfaces;

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

    //public DbSet<Account>? Accounts { get; set; }

    //public DbSet<PTTDigital.Authentication.Data.Models.Application>? Applications { get; set; }

    //public DbSet<AccPolicy>? AccPolicys { get; set; }

    //public DbSet<Company>? Companies { get; set; }

    //public DbSet<GroupRole>? GroupRoles { get; set; }

    //public DbSet<Group>? Groups { get; set; }

    //public DbSet<Permission>? Permissions { get; set; }

    //public DbSet<Role>? Roles { get; set; }

    //public DbSet<AppComp>? AppComps { get; set; }

    //public DbSet<AccGroup>? AccGroups { get; set; }
}
