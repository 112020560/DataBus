namespace DataBus.Observability;

public class ObservabilityOptions
{
    public const string SectionName = "Observability";

    public string ServiceName { get; set; } = "DataBus";
    public string ServiceVersion { get; set; } = "1.0.0";
    public string Environment { get; set; } = "Development";

    public TracingOptions Tracing { get; set; } = new();
    public MetricsOptions Metrics { get; set; } = new();
    public LoggingOptions Logging { get; set; } = new();
}

public class TracingOptions
{
    public bool Enabled { get; set; } = true;
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    public bool RecordException { get; set; } = true;
    public double SamplingRatio { get; set; } = 1.0;
}

public class MetricsOptions
{
    public bool Enabled { get; set; } = true;
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    public bool EnablePrometheus { get; set; } = true;
    public string PrometheusEndpoint { get; set; } = "/metrics";
    public bool EnableRuntimeMetrics { get; set; } = true;
}

public class LoggingOptions
{
    public bool ExportToOtlp { get; set; } = false;
    public string OtlpEndpoint { get; set; } = "http://localhost:4317";
    public bool IncludeTraceContext { get; set; } = true;
}
