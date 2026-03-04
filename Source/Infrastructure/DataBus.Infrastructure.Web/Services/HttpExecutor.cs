using System.Text;
using System.Text.Json;
using DataBus.Application;
using DataBus.Domain;
using Microsoft.Extensions.Logging;

namespace DataBus.Infrastructure.Web;

public class HttpExecutor : IHttpExecutor
{
    private readonly IHttpClientFactory _httpClientFactory;
    private readonly ILogger<HttpExecutor> _logger;

    public HttpExecutor(IHttpClientFactory httpClientFactory, ILogger<HttpExecutor> logger)
    {
        _httpClientFactory = httpClientFactory;
        _logger = logger;
    }

    public async Task<object?> ExecuteAsync(HttpExecutionModel model, CancellationToken cancellationToken)
    {
        var client = _httpClientFactory.CreateClient();
        client.Timeout = TimeSpan.FromSeconds(model.TimeoutSeconds);

        // Construir URL con query params
        var url = BuildUrl(model.BaseUrl, model.Endpoint, model.QueryParams);

        // Crear request
        var request = new HttpRequestMessage(new HttpMethod(model.Method), url);

        // Agregar headers
        foreach (var header in model.Headers)
        {
            request.Headers.TryAddWithoutValidation(header.Key, header.Value);
        }

        // Agregar body para POST/PUT/PATCH
        if (model.Body != null && model.Method.ToUpper() != "GET")
        {
            request.Content = new StringContent(
                JsonSerializer.Serialize(model.Body),
                Encoding.UTF8,
                "application/json");
        }

        _logger.LogDebug("[{CorrelationId}] HTTP {Method} {Url}",
            model.CorrelationId, model.Method, url);

        var response = await client.SendAsync(request, cancellationToken);
        var content = await response.Content.ReadAsStringAsync(cancellationToken);

        if (string.IsNullOrEmpty(content))
        {
            return null;
        }

        try
        {
            return JsonSerializer.Deserialize<object>(content);
        }
        catch (JsonException)
        {
            // Si no es JSON válido, retornar como string
            return content;
        }
    }

    private static string BuildUrl(string baseUrl, string endpoint, Dictionary<string, string>? queryParams)
    {
        var url = $"{baseUrl.TrimEnd('/')}/{endpoint.TrimStart('/')}";
        if (queryParams?.Any() == true)
        {
            var query = string.Join("&", queryParams.Select(kv =>
                $"{Uri.EscapeDataString(kv.Key)}={Uri.EscapeDataString(kv.Value)}"));
            url = $"{url}?{query}";
        }
        return url;
    }
}
