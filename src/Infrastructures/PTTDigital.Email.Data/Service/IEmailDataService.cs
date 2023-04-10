using Microsoft.EntityFrameworkCore.Migrations;
using PTTDigital.Email.Data.Repository;

namespace PTTDigital.Email.Data.Service;

public interface IEmailDataService : IServiceBase
{
    IEmailQueueRepository EmailQueueRepository { get; }

    IEmailArchiveRepository EmailArchiveRepository { get; }

    IMessageRepository MessageRepository { get; }
}
