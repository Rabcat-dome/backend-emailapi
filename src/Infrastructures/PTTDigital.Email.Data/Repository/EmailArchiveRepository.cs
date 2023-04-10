using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Caching.Memory;
using PTTDigital.Email.Data.Models;
using PTTDigital.Email.Data.Service;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace PTTDigital.Email.Data.Repository
{
    internal class EmailArchiveRepository<TDbContext> : RepositoryBase<EmailArchive, TDbContext>, IEmailArchiveRepository where TDbContext : DbContext
    {
        internal EmailArchiveRepository(TDbContext db, IGenerator generator, IMemoryCache cache) : base(db, generator, cache)
        {
        }
    }
}


