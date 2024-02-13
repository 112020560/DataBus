using Microsoft.OpenApi.Models;

namespace DataBus.WebApi;

public static class SwaggerExtension
{
    public static void ConfigureSwaggerGen(this IServiceCollection services)
    {
        var contact = new OpenApiContact()
        {
            Name = "Soporte a la Producción",
            Email = "it.soporte@multimoney.com"
        };

        var title = "Abstract DataBus RestApi";
        var description = "ApiRestFull to execute queries dynamically";

        services.AddSwaggerGen(c =>
        {
            c.SwaggerDoc("v1", new OpenApiInfo
            {
                Title = title,
                Version = "v1",
                Description = description,
                Contact = contact
            });
            c.SwaggerDoc("v2", new OpenApiInfo
            {
                Title = title,
                Version = "v2",
                Description = description,
                Contact = contact
            });
            c.SwaggerDoc("v3", new OpenApiInfo
            {
                Title = title,
                Version = "v3",
                Description = description,
                Contact = contact
            });
        });

    }
}
