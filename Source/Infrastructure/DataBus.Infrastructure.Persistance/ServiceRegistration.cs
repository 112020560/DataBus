using System.Data;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.DependencyInjection;
using MySqlConnector;
using Npgsql;
using Oracle.ManagedDataAccess.Client;

namespace DataBus.Infrastructure.Persistance;

public static class ServicesRegistration
{
    public static void ConfigureDatabaseConnections(this IServiceCollection services)
    {
        services.AddTransient<Func<string, IDbConnection>>(connectionProvider => key =>
        {
            return key.ToUpper() switch
            {
                "SQL" => new SqlConnection(),
                "MYSQL" => new MySqlConnection(),
                "POSTGRESQL" => new NpgsqlConnection(),
                "ORACLE" => new OracleConnection(),
                _ => throw new NotSupportedException($"Tipo de BD no soportado: {key}"),
            };
        });
    }
}
