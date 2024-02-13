namespace DataBus.Application;

public sealed record class BackEndParams
{
    public string? Parametro { get; set; }
    public object? Valor { get; set; }
    public string? Type { get; set; }
    public string? Direction { get; set; }
    public int Size { get; set; }
}
