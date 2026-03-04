namespace DataBus.Domain;

public record HttpExecutionModel
{
    public string BaseUrl { get; set; } = string.Empty;
    public string Endpoint { get; set; } = string.Empty;
    public string Method { get; set; } = "POST";
    public Dictionary<string, string> Headers { get; set; } = new();
    public object? Body { get; set; }
    public Dictionary<string, string>? QueryParams { get; set; }
    public int TimeoutSeconds { get; set; } = 30;
    public string CorrelationId { get; set; } = string.Empty;
}
