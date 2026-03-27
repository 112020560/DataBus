using Serilog;
using Serilog.Configuration;

namespace DataBus.Observability;

public static class SerilogExtensions
{
    /// <summary>
    /// Enriches logs with TraceId, SpanId for correlation
    /// </summary>
    public static LoggerConfiguration WithTraceContext(
        this LoggerEnrichmentConfiguration enrichment)
    {
        if (enrichment == null)
            throw new ArgumentNullException(nameof(enrichment));

        return enrichment.With<TraceContextEnricher>();
    }
}
