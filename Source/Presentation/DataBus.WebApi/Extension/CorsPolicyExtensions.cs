namespace DataBus.WebApi;

public static class CorsPolicyExtensions
{
    public static void ConfigureCorsPolicy(this IServiceCollection services, string namePolicy)
    {
        services.AddCors(opt =>
        {
            opt.AddPolicy(namePolicy, builder => builder
                .AllowAnyOrigin()
                .AllowAnyMethod()
                .AllowAnyHeader());
        });
    }
}
