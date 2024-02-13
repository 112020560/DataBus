namespace DataBus.Application;

public interface IBackendResponse
{
    public string? ResponseCode { get; set; }
    public object? ResponseData { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}
