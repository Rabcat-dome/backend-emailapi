using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PTTDigital.Email.Application.Repositories;
using PTTDigital.Email.Data.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Repository
{
    internal class EmailQueueRepository<TDbContext> : RepositoryBase<EmailQueue, TDbContext>, IEmailQueueRepository where TDbContext : DbContext
    {
        internal EmailQueueRepository(TDbContext db, IGenerator generator, IMemoryCache cache) : base(db, generator, cache)
        {
        }
    }
}
