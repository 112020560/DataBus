using Newtonsoft.Json;

namespace DataBus.Application;

public record class BackEndResponse : IBackendResponse
{
    public BackEndResponse()
    {
        
    }
    public BackEndResponse(string Codigo, object Data)
    {
        ResponseCode = Codigo;
        ResponseData = Data;
        Message = string.Empty;
        ErrorMessage = string.Empty;
    }
    public string? ResponseCode { get; set; }
    public object? ResponseData { get; set; }
    public string? Message { get; set; }
    public string? ErrorMessage { get; set; }
}


public record BackEndResponseV1 : IBackendResponse
{
    public BackEndResponseV1(string codigo, object data)
    {
        ResponseCode = codigo.ToUpper();
        ResponseData = data;
        Message = string.Empty;
        ErrorMessage = string.Empty;
    }
    [JsonProperty("responseCode")]
    public string? ResponseCode { get; set; }

    [JsonProperty("responseData")]
    public object? ResponseData { get; set; }

    [JsonProperty("message")]
    public string? Message { get; set; }

    [JsonProperty("errorMessage")]
    public string? ErrorMessage { get; set; }
}