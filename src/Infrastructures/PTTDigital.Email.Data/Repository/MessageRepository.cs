using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PTTDigital.Email.Data.Models;
using PTTDigital.Email.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Repository
{
    internal class MessageRepository<TDbContext> : RepositoryBase<Message, TDbContext>, IMessageRepository where TDbContext : DbContext
    {
        internal MessageRepository(TDbContext db, IGenerator generator, IMemoryCache cache) : base(db, generator, cache)
        {
        }
    }
}
