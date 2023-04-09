using System.Data;
using Dapper;
using PTTDigital.Email.Data.Service;

namespace PTTDigital.Email.Data.SqlServer;

public static class DataServiceExtension
{
    public static async Task<IEnumerable<TResult>> QueryStoredProcedure<TResult>(this IServiceBase dataService, string sql, object parm = null)
    {
        var sqlConnection = dataService.OpenSqlConnection();
        var result = await sqlConnection.QueryAsync<TResult>(sql, param: parm, commandType: CommandType.StoredProcedure);
        dataService.CloseSqlConnection();
        return result;
    }

    public static SqlMapper.GridReader QueryMultipleStoredProcedure(this IServiceBase dataService, string sql, object parm = null)
    {
        var sqlConnection = dataService.OpenSqlConnection();
        var result = sqlConnection.QueryMultiple(sql, param: parm, commandType: CommandType.StoredProcedure);
        return result;
    }
}
