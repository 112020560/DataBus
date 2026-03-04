namespace DataBus.Domain;

public record DataBusRequest
{
    public string Key { get; set; } = string.Empty;        // Llave de conexión
    public string Procedure { get; set; } = string.Empty;  // SP o endpoint HTTP
    public string? Method { get; set; }                    // HTTP: GET, POST, PUT, DELETE
    public List<ParamItem>? Params { get; set; }           // Parámetros
    public List<ParamItem>? QueryParams { get; set; }      // Solo HTTP
    public Dictionary<string, string>? Headers { get; set; } // Solo HTTP
    public string TransactionId { get; set; } = Guid.NewGuid().ToString("N");
}

public record ParamItem
{
    public string Name { get; set; } = string.Empty;
    public object? Value { get; set; }
    public string? Type { get; set; }       // BD: Int, String, DateTime, etc.
    public string? Direction { get; set; }  // BD: IN, OUT
    public int Size { get; set; }
}
