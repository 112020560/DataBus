namespace DataBus.Domain;

public record SandboxParametersByProcModel
{
    public bool Succeeded { get; set; }
    public int CountryCode { get; set; }
    public int AppCode { get; set; }
    public string? Procedure { get; set; }
    public string? Parameters { get; set; }
    public string? Description { get; set; }
    public string? SearchKey { get; set; }
    public string? AppKey { get; set; }
    public int EnableTrace { get; set; }
    public string[]? ArrayParams { get; set; }
    public Exception? Exception { get; set; }
    public bool ValidateParams { get; set; }
}
