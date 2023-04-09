namespace PTTDigital.Email.Data.Service;

public interface IEventMessage
{
    string? CorrelationId { get; }
}
