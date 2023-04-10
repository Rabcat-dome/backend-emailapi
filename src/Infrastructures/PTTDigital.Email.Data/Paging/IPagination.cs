using System.Linq.Expressions;
using System.Text.Json.Serialization;

namespace PTTDigital.Email.Data.Paging;

public enum SortTypes
{
    Asc,
    Desc,
}

public interface IPagination
{
    string? OrderBy { get; set; }

    string? Sorting { get; set; }

    int PageNumber { get; set; }

    int PageSize { get; set; }

    [JsonIgnore]
    SortTypes SortTypes { get; set; }

    int PageNumberSize();

    Expression<Func<TEntity, bool>> GetPredicate<TEntity>();
}

public interface IPagination<TRequest> : IPagination
{
    TRequest? Criteria { get; set; }
}