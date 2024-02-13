namespace DataBus.Domain;

public record SandboxConnectionModel
{
    public string? connstring { get; set; }
    public string? key00 { get; set; }
    public string? key01 { get; set; }
    public string? key02 { get; set; }
    public string? AppKey { get; set; }
}
