using System.Data.SqlClient;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Migrations;
using Microsoft.Extensions.Caching.Memory;
using PTTDigital.Email.Data.Context;
using PTTDigital.Email.Data.Repository;

namespace PTTDigital.Email.Data.Service;

public sealed class EmailDataService<TDbContext> : DataServiceBase<EmailContextBase<TDbContext>>, IEmailDataService
    where TDbContext : DbContext
{
    private SqlConnection? _sqlConnection;
    private readonly string? _connectionString;

    #region Private members
    private readonly Lazy<EmailQueueRepository<EmailContextBase<TDbContext>>> _emailQueueRepository;
    private readonly Lazy<EmailArchiveRepository<EmailContextBase<TDbContext>>> _emailArchiveRepository;
    private readonly Lazy<MessageRepository<EmailContextBase<TDbContext>>> _messageRepository;

    #endregion

    public EmailDataService(EmailContextBase<TDbContext> context, IGenerator generator, IMemoryCache cache) : base(context)
    {
        if (context.Database.ProviderName != "Microsoft.EntityFrameworkCore.InMemory")
            _connectionString = context.Database.GetConnectionString();

        _emailQueueRepository = new Lazy<EmailQueueRepository<EmailContextBase<TDbContext>>>(()
                          => new EmailQueueRepository<EmailContextBase<TDbContext>>(context, generator, cache));

        _emailArchiveRepository = new Lazy<EmailArchiveRepository<EmailContextBase<TDbContext>>>(()
                        => new EmailArchiveRepository<EmailContextBase<TDbContext>>(context, generator, cache));

        _messageRepository = new Lazy<MessageRepository<EmailContextBase<TDbContext>>>(()
                        => new MessageRepository<EmailContextBase<TDbContext>>(context, generator, cache));
    }

    #region IPpeOnlineService
    IEmailQueueRepository IEmailDataService.EmailQueueRepository => _emailQueueRepository.Value;

    IEmailArchiveRepository IEmailDataService.EmailArchiveRepository => _emailArchiveRepository.Value;

    IMessageRepository IEmailDataService.MessageRepository => _messageRepository.Value;

   

    Task<int> IServiceBase.SaveChangeAsync()
    {
        return SaveChangeAsync();
    }

    void IServiceBase.AddDomainEvent(IEventMessage domainEvent) => throw new NotImplementedException();

    void IServiceBase.RemoveDomainEvent(IEventMessage domainEvent) => throw new NotImplementedException();

    void IServiceBase.ClearDomainEvents() => throw new NotImplementedException();

    void IDisposable.Dispose()
    {
        this.InternalDispose(disposing: true);
        GC.SuppressFinalize(this);
    }

    public SqlConnection OpenSqlConnection()
    {
        _sqlConnection = new SqlConnection(_connectionString);
        return _sqlConnection;
    }

    public void CloseSqlConnection()
    {
        _sqlConnection?.Close();
    }
    #endregion
}