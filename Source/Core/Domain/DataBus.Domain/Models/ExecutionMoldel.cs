using System.Text.Json;

namespace DataBus.Domain;

public record class ExecutionMoldel
{
    public bool ParamsValidate { get; set; }
    public string? ExecutionParams { get; set; }
    public string[]? ConfigParams { get; set; }
    public string? ConnString { get; set; }
    public int ExecutionTimeOut { get; set; }
    public string? Query { get; set; }
    public List<ParameterModel>? Params { get; set; }
    public int Flag { get; set; }
    public bool EnableLog { get; set; }
    public string CorrelationId { get; set; } = Guid.NewGuid().ToString("N");
    public string DataBaseTarget { get; set; } = "MSSQL";
    public bool ExistOutputParameters { get; set; }

    public bool ValidateExistOutputParams()
    {
        if(Params is not null)
        {
            return Params.Any(a => a.Direction != null && a.Direction.ToUpper().Equals("OUT"));
        }
        return false;
    }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
