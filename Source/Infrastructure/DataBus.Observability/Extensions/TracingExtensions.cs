using Microsoft.Extensions.DependencyInjection;
using OpenTelemetry.Exporter;
using OpenTelemetry.Resources;
using OpenTelemetry.Trace;

namespace DataBus.Observability;

internal static class TracingExtensions
{
    public static IServiceCollection AddTracing(
        this IServiceCollection services,
        ObservabilityOptions options,
        ResourceBuilder resourceBuilder)
    {
        services.AddOpenTelemetry()
            .WithTracing(builder =>
            {
                builder
                    .SetResourceBuilder(resourceBuilder)

                    // Automatic instrumentation
                    .AddAspNetCoreInstrumentation(opts =>
                    {
                        opts.RecordException = options.Tracing.RecordException;
                        opts.Filter = ctx => !ctx.Request.Path.StartsWithSegments("/health")
                                           && !ctx.Request.Path.StartsWithSegments("/metrics");
                    })
                    .AddHttpClientInstrumentation(opts =>
                    {
                        opts.RecordException = options.Tracing.RecordException;
                    })
                    .AddSqlClientInstrumentation(opts =>
                    {
                        opts.SetDbStatementForText = true;
                        opts.SetDbStatementForStoredProcedure = true;
                        opts.RecordException = options.Tracing.RecordException;
                    })

                    // Custom ActivitySources
                    .AddSource(ActivitySources.DataBus)
                    .AddSource(ActivitySources.Database)
                    .AddSource(ActivitySources.HttpExecutor)

                    // Sampling
                    .SetSampler(new TraceIdRatioBasedSampler(options.Tracing.SamplingRatio))

                    // Exporter OTLP
                    .AddOtlpExporter(otlp =>
                    {
                        otlp.Endpoint = new Uri(options.Tracing.OtlpEndpoint);
                        otlp.Protocol = OtlpExportProtocol.Grpc;
                    });
            });

        return services;
    }
}
