namespace PTTDigital.Email.Data.Interfaces;

public interface IEntityInfo
{
    string TableName { get; }
    IReadOnlyDictionary<string, string> Columns { get; }
}
