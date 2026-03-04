using DataBus.Application.Exceptions;
using DataBus.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBus.Application;

public record DataBusQuery(DataBusRequest Request) : IRequest<IBackendResponse>;

public class DataBusHandler : IRequestHandler<DataBusQuery, IBackendResponse>
{
    private readonly ILogger<DataBusHandler> _logger;
    private readonly IConnectionProvider _connectionProvider;
    private readonly IDataBaseRepository _dbRepository;
    private readonly IHttpExecutor _httpExecutor;

    public DataBusHandler(
        ILogger<DataBusHandler> logger,
        IConnectionProvider connectionProvider,
        IDataBaseRepository dbRepository,
        IHttpExecutor httpExecutor)
    {
        _logger = logger;
        _connectionProvider = connectionProvider;
        _dbRepository = dbRepository;
        _httpExecutor = httpExecutor;
    }

    public async Task<IBackendResponse> Handle(DataBusQuery query, CancellationToken cancellationToken)
    {
        var request = query.Request;

        // Obtener configuración de conexión
        var connection = _connectionProvider.GetConnection(request.Key)
            ?? throw new NotFoundException($"Conexión no encontrada: {request.Key}");

        _logger.LogDebug("[{TransactionId}] Tipo: {Type}, Key: {Key}",
            request.TransactionId, connection.Type, request.Key);

        object? result = connection.Type.ToUpper() switch
        {
            "HTTP" => await ExecuteHttpAsync(request, connection, cancellationToken),
            _ => await ExecuteDatabaseAsync(request, connection, cancellationToken)
        };

        return new BackEndResponse
        {
            ResponseCode = "OK",
            ResponseData = result,
            Message = "Process OK",
            ErrorMessage = ""
        };
    }

    private async Task<object?> ExecuteHttpAsync(
        DataBusRequest request,
        ConnectionConfig connection,
        CancellationToken cancellationToken)
    {
        var model = new HttpExecutionModel
        {
            BaseUrl = connection.ConnectionString,
            Endpoint = request.Procedure,
            Method = request.Method ?? "POST",
            Headers = MergeHeaders(connection.Headers, request.Headers),
            Body = request.Params?.ToDictionary(p => p.Name, p => p.Value),
            QueryParams = request.QueryParams?.ToDictionary(p => p.Name, p => p.Value?.ToString() ?? ""),
            TimeoutSeconds = connection.TimeoutSeconds,
            CorrelationId = request.TransactionId
        };

        return await _httpExecutor.ExecuteAsync(model, cancellationToken);
    }

    private async Task<IEnumerable<object>> ExecuteDatabaseAsync(
        DataBusRequest request,
        ConnectionConfig connection,
        CancellationToken cancellationToken)
    {
        var model = new ExecutionMoldel
        {
            ConnString = connection.ConnectionString,
            Query = request.Procedure,
            DataBaseTarget = connection.Type.ToUpper(),
            Params = request.Params?.Select(p => new ParameterModel
            {
                ParameterName = p.Name,
                ParameterValue = p.Value,
                Type = p.Type,
                Direction = p.Direction ?? "IN",
                Size = p.Size
            }).ToList(),
            ExecutionTimeOut = connection.TimeoutSeconds,
            CorrelationId = request.TransactionId
        };

        return await _dbRepository.GetExecutionAsync<object>(model);
    }

    private Dictionary<string, string> MergeHeaders(
        Dictionary<string, string>? configHeaders,
        Dictionary<string, string>? requestHeaders)
    {
        var result = new Dictionary<string, string>(configHeaders ?? new());
        if (requestHeaders != null)
        {
            foreach (var header in requestHeaders)
            {
                result[header.Key] = header.Value;
            }
        }
        return result;
    }
}
