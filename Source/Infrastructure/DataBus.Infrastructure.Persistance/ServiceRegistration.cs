using System.Data;
using System.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySql.Data.MySqlClient;
using Oracle.ManagedDataAccess.Client;

namespace DataBus.Infrastructure.Persistance;

public static class ServicesRegistration
{
    public static void ConfigureDatabaseConnections(this IServiceCollection services)
    {
        services.AddTransient<Func<string, IDbConnection>>(connectionProvider => key => 
        {
            return key switch 
            {
                "SQL" => new SqlConnection(),
                "MYSQL" => new MySqlConnection(),
                "ORACLE" => new OracleConnection(),
                _ => throw new NotImplementedException(),
            };

        });
    }
}
