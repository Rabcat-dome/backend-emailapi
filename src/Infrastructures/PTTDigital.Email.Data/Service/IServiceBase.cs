using System.Data.SqlClient;
using PTTDigital.Email.Data.Repository;

namespace PTTDigital.Email.Data.Service;

public interface IServiceBase : IDisposable
{
    Task<int> SaveChangeAsync();

    void AddDomainEvent(IEventMessage domainEvent);

    void RemoveDomainEvent(IEventMessage domainEvent);

    void ClearDomainEvents();

    SqlConnection OpenSqlConnection();

    void CloseSqlConnection();
}
