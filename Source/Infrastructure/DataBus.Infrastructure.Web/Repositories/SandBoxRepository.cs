using DataBus.Application;
using DataBus.Domain;
using Newtonsoft.Json;

namespace DataBus.Infrastructure.Web;

public class SandBoxRepository : ISandBoxRepository
{
    private readonly HttpClient _httpClient;
    public SandBoxRepository(HttpClient httpClient)
    {
        _httpClient = httpClient;
    }

    public async Task<ApplicationKeyModel?> GetApplicationKeyAsync(string tenant, int app, int country, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v1/SandboxApplication/{tenant}/getappkey/{app}/{country}");
        using var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
        var respContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<ApplicationKeyModel?>(respContent);
        }
        throw new Exception(
            $"Error al consultar el appkey desde: {httpResponse?.RequestMessage?.RequestUri}. StatusCode: {httpResponse?.StatusCode} Detail : {respContent ?? httpResponse?.ReasonPhrase}"
            );
    }

    public async Task<List<SandboxConnectionModel>?> GetSandboxConnectionsAsync(string appkey, string tenant, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"/api/v1/SandboxConnections/all/{tenant}/{appkey}");
        using var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
        var respContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<List<SandboxConnectionModel>?>(respContent);
        }
        throw new Exception(
            $"Error al consultar las conexiones desde el sandbox: {httpResponse?.RequestMessage?.RequestUri}. StatusCode: {httpResponse?.StatusCode} Detail : {respContent ?? httpResponse?.ReasonPhrase}"
            );
    }

    public async Task<List<SandboxParametersByProcModel>?> SandboxParametersByProcModels(string tenant, string key, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"parameters_{tenant}_{key}");
        using var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
        var respContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<List<SandboxParametersByProcModel>?>(respContent);
        }
        throw new Exception(
            $"Error al consultar los parametros desde el sandbox: {httpResponse?.RequestMessage?.RequestUri}. StatusCode: {httpResponse?.StatusCode} Detail : {respContent ?? httpResponse?.ReasonPhrase}"
            );
    }

    public async Task<SandboxParametersByProcV2Model?> GetSandboxParametersByProcV2Async(string tenant, string key, CancellationToken cancellationToken)
    {
        using var request = new HttpRequestMessage(HttpMethod.Get, $"api/v2/SandboxParameters/{tenant}/get/{key}");
        using var httpResponse = await _httpClient.SendAsync(request, cancellationToken);
        var respContent = await httpResponse.Content.ReadAsStringAsync(cancellationToken);
        if (httpResponse.IsSuccessStatusCode)
        {
            return JsonConvert.DeserializeObject<SandboxParametersByProcV2Model?>(respContent);
        }
        throw new Exception(
            $"Error al consultar los parametros desde el sandbox: {httpResponse?.RequestMessage?.RequestUri}. StatusCode: {httpResponse?.StatusCode} Detail : {respContent ?? httpResponse?.ReasonPhrase}"
            );
    }
}
