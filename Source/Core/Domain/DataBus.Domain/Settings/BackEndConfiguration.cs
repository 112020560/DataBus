namespace DataBus.Domain;

public sealed record BackEndConfiguration
{
    public string ControladorParametros { get; set; } = "v1";
    public int ExecutionTimeOut { get; set; }
}
