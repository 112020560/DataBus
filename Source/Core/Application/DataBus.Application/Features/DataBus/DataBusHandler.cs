using DataBus.Application.Exceptions;
using DataBus.Domain;
using MediatR;
using Microsoft.Extensions.Logging;

namespace DataBus.Application;

/// <summary>
/// Query para ejecutar operaciones en base de datos o HTTP
/// </summary>
/// <param name="Request">El request con los datos</param>
/// <param name="ExecutionMethod">Método de ejecución: query, single, scalar, execute</param>
public record DataBusQuery(DataBusRequest Request, string ExecutionMethod = "query") : IRequest<IBackendResponse>;

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
        var method = query.ExecutionMethod.ToLower();

        // Obtener configuración de conexión
        var connection = _connectionProvider.GetConnection(request.Key)
            ?? throw new NotFoundException($"Conexión no encontrada: {request.Key}");

        _logger.LogDebug("[{TransactionId}] Tipo: {Type}, Key: {Key}, Method: {Method}",
            request.TransactionId, connection.Type, request.Key, method);

        object? result = connection.Type.ToUpper() switch
        {
            "HTTP" => await ExecuteHttpAsync(request, connection, cancellationToken),
            _ => await ExecuteDatabaseAsync(request, connection, method, cancellationToken)
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

    private async Task<object?> ExecuteDatabaseAsync(
        DataBusRequest request,
        ConnectionConfig connection,
        string method,
        CancellationToken cancellationToken)
    {
        var model = CreateExecutionModel(request, connection);

        return method switch
        {
            "query" => await _dbRepository.QueryAsync<dynamic>(model),
            "single" => await _dbRepository.QuerySingleAsync<dynamic>(model),
            "scalar" => await _dbRepository.ScalarAsync<dynamic>(model),
            "execute" => await _dbRepository.ExecuteAsync(model),
            _ => throw new ArgumentException($"Método de ejecución no válido: {method}. Use: query, single, scalar, execute")
        };
    }

    private static ExecutionMoldel CreateExecutionModel(DataBusRequest request, ConnectionConfig connection)
    {
        return new ExecutionMoldel
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
    }

    private static Dictionary<string, string> MergeHeaders(
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
