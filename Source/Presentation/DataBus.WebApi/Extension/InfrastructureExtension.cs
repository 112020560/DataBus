using DataBus.Application;
using DataBus.Infrastructure.Persistance;
using DataBus.Infrastructure.Shared;
using DataBus.Infrastructure.Web;

namespace DataBus.WebApi;

public static class InfrastructureExtension
{
    public static void ConfigureInfrastructure(this IServiceCollection services, IConfiguration configuration)
    {
        services.ConfigureDatabaseConnections();
        services.AddScoped<IDataBaseRepository, MssqlRepository>();
        services.AddScoped<ICacheService,CacheService>();
        services.AddHttpClient<ISandBoxRepository, SandBoxRepository>(config => {
            config.BaseAddress = new Uri(configuration["ManagerSetting:BaseUrl"] ?? throw new Exception("No fue posible ontener la informacion del configurador del sandbox"));
        });
    }
}
