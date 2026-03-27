using System.Diagnostics.Metrics;

namespace DataBus.Observability;

public static class AppMetrics
{
    public const string MeterName = "DataBus.Application";

    private static readonly Meter Meter = new(MeterName, "1.0.0");

    // Counters
    public static readonly Counter<long> RequestsTotal =
        Meter.CreateCounter<long>("databus_requests_total",
            description: "Total requests processed");

    public static readonly Counter<long> RequestsErrors =
        Meter.CreateCounter<long>("databus_requests_errors_total",
            description: "Total request errors");

    // Histograms
    public static readonly Histogram<double> RequestDuration =
        Meter.CreateHistogram<double>("databus_request_duration_seconds",
            unit: "s",
            description: "Request duration in seconds");

    public static readonly Histogram<double> DatabaseQueryDuration =
        Meter.CreateHistogram<double>("databus_db_query_duration_seconds",
            unit: "s",
            description: "Database query duration in seconds");

    // Gauges
    public static readonly ObservableGauge<int> ActiveConnections =
        Meter.CreateObservableGauge("databus_active_connections",
            () => ConnectionTracker.ActiveCount,
            description: "Number of active connections");
}

public static class ConnectionTracker
{
    private static int _activeCount;
    public static int ActiveCount => _activeCount;
    public static void Increment() => Interlocked.Increment(ref _activeCount);
    public static void Decrement() => Interlocked.Decrement(ref _activeCount);
}
