namespace PTTDigital.Email.Application.Api.Client.HttpDelegating;

internal class HttpLogModel
{
    public int Status { get; internal set; }
    public string Path { get; internal set; }
    public string Server { get; internal set; }
    public TimeSpan Elapsed { get; internal set; }
    public string ConnectionId { get; internal set; }
    public object Request { get; internal set; }
    public object Response { get; internal set; }
}
