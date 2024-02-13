namespace DataBus.Application;

public sealed record class BackEndRequest
{
    public string? Metodo { get; set; }
    public string? Topico { get; set; }
    public string? Conexion { get; set; }
    public string? Message { get; set; }
    public string? LlaveBaseDatos { get; set; }
    public string? Procedimiento { get; set; }
    public int Flag { get; set; }
    public int Pais { get; set; }
    public int App { get; set; }
    public string? AppKey { get; set; }
    public string? Tenant { get; set; }
    public List<BackEndParams>? Params { get; set; }
    public string[]? Columns { get; set; }

    // [JsonIgnore]
    // public string Version { get; set; }
    public string TransactionId { get; set; } = Guid.NewGuid().ToString("N");
}
