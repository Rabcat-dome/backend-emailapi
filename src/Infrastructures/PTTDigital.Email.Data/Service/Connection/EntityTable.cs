using PTTDigital.Authentication.Data.Interfaces;

namespace PTTDigital.Email.Data.Service.Connection;

internal class EntityTable : IEntityInfo
{
    public string TableName { get; set; }
    public Dictionary<string, string> Columns { get; set; }

    IReadOnlyDictionary<string, string> IEntityInfo.Columns
    {
        get
        {
            if (Columns == null)
            {
                return null;
            }

            var result = Columns.Where(c => !string.IsNullOrEmpty(c.Key) && !string.IsNullOrEmpty(c.Value))
                                .ToDictionary(c => c.Key, c => c.Value);
            return result;
        }
    }
}
