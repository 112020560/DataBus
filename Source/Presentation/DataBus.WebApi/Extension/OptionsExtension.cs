using DataBus.Domain;

namespace DataBus.WebApi;

public static class OptionsExtension
{
    public static void ConfigureOptions(this IServiceCollection services, IConfiguration configuration)
    {
        services.AddOptions();
        services.Configure<BackEndConfiguration>(configuration.GetSection("BackEndSetting"));
    }
}
