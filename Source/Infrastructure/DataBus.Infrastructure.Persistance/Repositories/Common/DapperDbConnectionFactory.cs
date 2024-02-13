using System.Data;
using System.Data.SqlClient;

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
        string? connectionString = null;
        if (_connectionDict.TryGetValue(connectionName, out connectionString))
        {
            return new SqlConnection(connectionString);
        }

        throw new ArgumentNullException();
    }
}
