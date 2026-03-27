using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Metrics;
using OpenTelemetry.Resources;

namespace DataBus.Observability;

internal static class MetricsExtensions
{
    public static IServiceCollection AddMetrics(
        this IServiceCollection services,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder)
    {
        services.AddOpenTelemetry()
            .WithMetrics(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)

                    // Automatic instrumentation
                    .AddAspNetCoreInstrumentation()
                    .AddHttpClientInstrumentation();

                // Runtime metrics (GC, ThreadPool, etc.)
                if (options.Metrics.EnableRuntimeMetrics)
                {
                    builder.AddRuntimeInstrumentation()
                           .AddProcessInstrumentation();
                }

                // Custom Meters
                builder.AddMeter(AppMetrics.MeterName);

                // Exporter OTLP
                builder.AddOtlpExporter(otlp =>
                {
                    otlp.Endpoint = new Uri(options.Metrics.OtlpEndpoint);
                    otlp.Protocol = OtlpExportProtocol.Grpc;
                });

                // Prometheus (if enabled)
                if (options.Metrics.EnablePrometheus)
                {
                    builder.AddPrometheusExporter();
                }
            });

        return services;
    }
}
