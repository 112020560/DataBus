using System.Diagnostics;

namespace DataBus.Observability;

public static class ActivitySources
{
    public const string DataBus = "DataBus.Application";
    public const string Database = "DataBus.Database";
    public const string HttpExecutor = "DataBus.HttpExecutor";

    public static readonly ActivitySource DataBusSource = new(DataBus, "1.0.0");
    public static readonly ActivitySource DatabaseSource = new(Database, "1.0.0");
    public static readonly ActivitySource HttpExecutorSource = new(HttpExecutor, "1.0.0");
}
