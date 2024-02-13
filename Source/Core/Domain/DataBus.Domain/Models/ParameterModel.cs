

using System.Text.Json;

namespace DataBus.Domain;

public record class ParameterModel
{
    public string? ParameterName { get; set; }
    public object? ParameterValue { get; set; }
    public string? Type { get; set; }
    public string? Direction { get; set; }
    public int Size { get; set; }

    public override string ToString()
    {
        return JsonSerializer.Serialize(this);
    }
}
