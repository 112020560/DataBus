namespace DataBus.Domain;

public record ConnectionConfig
{
    public string Type { get; set; } = string.Empty;  // SQL, MySQL, PostgreSQL, HTTP
    public string ConnectionString { get; set; } = string.Empty;
    public bool IsEncrypted { get; set; } = false;  // Indica si ConnectionString está encriptado
    public Dictionary<string, string>? Headers { get; set; }  // Solo HTTP
    public int TimeoutSeconds { get; set; } = 30;
}

public record DataConnectionsConfig
{
    public Dictionary<string, ConnectionConfig> Connections { get; set; } = new();
}
