using System.Data;
using Microsoft.Data.SqlClient;

namespace DataBus.Infrastructure.Persistance;

public class DapperDbConnectionFactory
{
    private readonly IDictionary<string, string> _connectionDict;

    public DapperDbConnectionFactory(IDictionary<string, string> connectionDict)
    {
        _connectionDict = connectionDict;
    }

    public IDbConnection CreateDbConnection(string connectionName)
    {
        if (_connectionDict.TryGetValue(connectionName, out string? connectionString))
        {
            return new SqlConnection(connectionString);
        }

        throw new ArgumentNullException();
    }
}
