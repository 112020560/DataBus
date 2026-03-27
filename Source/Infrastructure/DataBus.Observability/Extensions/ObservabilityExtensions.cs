using Microsoft.AspNetCore.Builder;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Options;
using OpenTelemetry.Resources;

namespace DataBus.Observability;

public static class ObservabilityExtensions
{
    public static IServiceCollection AddObservability(
        this IServiceCollection services,
        IConfiguration configuration)
    {
        var options = new ObservabilityOptions();
        configuration.GetSection(ObservabilityOptions.SectionName).Bind(options);
        services.Configure<ObservabilityOptions>(
            configuration.GetSection(ObservabilityOptions.SectionName));

        // Configure common Resource
        var resourceBuilder = ResourceBuilder.CreateDefault()
            .AddService(
                serviceName: options.ServiceName,
                serviceVersion: options.ServiceVersion)
            .AddAttributes(new Dictionary<string, object>
            {
                ["deployment.environment"] = options.Environment,
                ["host.name"] = Environment.MachineName
            });

        // Add Tracing
        if (options.Tracing.Enabled)
        {
            services.AddTracing(options, resourceBuilder);
        }

        // Add Metrics
        if (options.Metrics.Enabled)
        {
            services.AddMetrics(options, resourceBuilder);
        }

        return services;
    }

    public static IApplicationBuilder UseObservability(this IApplicationBuilder app)
    {
        var options = app.ApplicationServices
            .GetRequiredService<IOptions<ObservabilityOptions>>().Value;

        // Prometheus endpoint
        if (options.Metrics.Enabled && options.Metrics.EnablePrometheus)
        {
            app.UseOpenTelemetryPrometheusScrapingEndpoint(
                options.Metrics.PrometheusEndpoint);
        }

        return app;
    }
}
