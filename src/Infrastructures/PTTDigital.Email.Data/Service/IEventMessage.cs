namespace PTTDigital.Email.Data.Repository;

public interface IEventMessage
{
    string? CorrelationId { get; set; }
}
