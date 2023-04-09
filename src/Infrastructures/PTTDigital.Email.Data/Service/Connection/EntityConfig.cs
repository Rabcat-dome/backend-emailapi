using Microsoft.EntityFrameworkCore;
using PTTDigital.Authentication.Data.Interfaces;
using PTTDigital.Email.Data.Interfaces;

namespace PTTDigital.Email.Data.Service.Connection;

internal class EntityConfig<TContext> : IEntityConfig<TContext> where TContext : DbContext
{
    public string Schema { get; set; }
    public string Collation { get; set; }
    public Dictionary<string, EntityTable> Entities { get; set; }

    public bool UseInMemory { get; internal set; }

    IReadOnlyDictionary<string, IEntityInfo> IEntityConfig<TContext>.Entities
    {
        get
        {
            if (Entities == null)
            {
                return null;
            }

            var result = Entities.Where(c => !string.IsNullOrEmpty(c.Key) && c.Value != null)
                                 .ToDictionary(c => c.Key, c => c.Value as IEntityInfo);
            return result;
        }
    }

    private static IEntityConfig<TContext> instance = null;

    internal static IEntityConfig<TContext> Instance
    {
        get
        {
            if (instance == null)
            {
                instance = new EntityConfig<TContext>();
            }

            return instance;
        }
    }
}
