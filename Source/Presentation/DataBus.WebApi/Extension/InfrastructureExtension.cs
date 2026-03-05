using DataBus.Application;
using DataBus.Domain;
using DataBus.Infrastructure.Persistance;
using DataBus.Infrastructure.Shared;
using DataBus.Infrastructure.Web;

namespace DataBus.WebApi;

public static class InfrastructureExtension
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        // Configurar conexiones desde config
        services.Configure<DataConnectionsConfig>(configuration.GetSection("DataConnections"));

        // Registrar providers
        services.AddSingleton<IConnectionProvider, ConfigConnectionProvider>();
        services.AddScoped<IHttpExecutor, HttpExecutor>();

        // BD
        services.ConfigureDatabaseConnections();
        services.AddScoped<IDataBaseRepository, DatabaseRepository>();

        // HTTP Client Factory
        services.AddHttpClient();
    }
}
