using System.Diagnostics;

namespace DataBus.Observability;

public static class TracingHelper
{
    /// <summary>
    /// Creates a span for database operations
    /// </summary>
    public static Activity? StartDatabaseSpan(
        string operation,
        string dbType,
        string? query = null)
    {
        var activity = ActivitySources.DatabaseSource.StartActivity(
            $"DB {operation}",
            ActivityKind.Client);

        activity?.SetTag("db.system", dbType.ToLower());
        activity?.SetTag("db.operation", operation);

        if (query != null)
            activity?.SetTag("db.statement", query);

        return activity;
    }

    /// <summary>
    /// Creates a span for custom outgoing HTTP calls
    /// </summary>
    public static Activity? StartHttpSpan(string method, string url)
    {
        var activity = ActivitySources.HttpExecutorSource.StartActivity(
            $"HTTP {method}",
            ActivityKind.Client);

        activity?.SetTag("http.method", method);
        activity?.SetTag("http.url", url);

        return activity;
    }

    /// <summary>
    /// Creates a span for business operations
    /// </summary>
    public static Activity? StartBusinessSpan(string operation)
    {
        return ActivitySources.DataBusSource.StartActivity(
            operation,
            ActivityKind.Internal);
    }

    /// <summary>
    /// Records an exception on the current span
    /// </summary>
    public static void RecordException(this Activity? activity, Exception ex)
    {
        if (activity == null) return;

        activity.SetStatus(ActivityStatusCode.Error, ex.Message);
        activity.AddEvent(new ActivityEvent("exception", tags: new ActivityTagsCollection
        {
            { "exception.type", ex.GetType().FullName },
            { "exception.message", ex.Message },
            { "exception.stacktrace", ex.StackTrace }
        }));
    }
}
