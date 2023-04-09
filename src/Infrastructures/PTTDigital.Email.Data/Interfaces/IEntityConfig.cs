using Microsoft.EntityFrameworkCore;
using PTTDigital.Authentication.Data.Interfaces;

namespace PTTDigital.Email.Data.Interfaces;

public interface IEntityConfig<TContext> where TContext : DbContext
{
    string Schema { get; }
    string Collation { get; }
    bool UseInMemory { get; }
    IReadOnlyDictionary<string, IEntityInfo> Entities { get; }
}