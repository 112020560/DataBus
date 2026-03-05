using System.Data;
using DataBus.Domain;
using Microsoft.Extensions.Logging;
using Dapper;
using DataBus.Application;

namespace DataBus.Infrastructure.Persistance;

public class DatabaseRepository : CommonRepository, IDataBaseRepository
{
    private readonly ILogger<DatabaseRepository> _logger;
    private readonly Func<string, IDbConnection> _connectionFactory;

    public DatabaseRepository(ILogger<DatabaseRepository> logger, Func<string, IDbConnection> connectionFactory)
        : base(logger)
    {
        _logger = logger;
        _connectionFactory = connectionFactory;
    }

    /// <summary>
    /// Ejecuta un SP y retorna múltiples filas
    /// </summary>
    public async Task<IEnumerable<T>> QueryAsync<T>(ExecutionMoldel model)
    {
        ValidateModel(model);
        var parameters = PrepareParameters(model);

        using var conn = CreateConnection(model);

        _logger.LogDebug("[{CorrelationId}] QueryAsync: {Procedure}", model.CorrelationId, model.Query);

        var result = await conn.QueryAsync<T>(
            model.Query!,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: model.ExecutionTimeOut
        );

        return result;
    }

    /// <summary>
    /// Ejecuta un SP y retorna una sola fila
    /// </summary>
    public async Task<T?> QuerySingleAsync<T>(ExecutionMoldel model)
    {
        ValidateModel(model);
        var parameters = PrepareParameters(model);

        using var conn = CreateConnection(model);

        _logger.LogDebug("[{CorrelationId}] QuerySingleAsync: {Procedure}", model.CorrelationId, model.Query);

        var result = await conn.QueryFirstOrDefaultAsync<T>(
            model.Query!,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: model.ExecutionTimeOut
        );

        return result;
    }

    /// <summary>
    /// Ejecuta un SP y retorna un solo valor
    /// </summary>
    public async Task<T?> ScalarAsync<T>(ExecutionMoldel model)
    {
        ValidateModel(model);
        var parameters = PrepareParameters(model);

        using var conn = CreateConnection(model);

        _logger.LogDebug("[{CorrelationId}] ScalarAsync: {Procedure}", model.CorrelationId, model.Query);

        var result = await conn.ExecuteScalarAsync<T>(
            model.Query!,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: model.ExecutionTimeOut
        );

        return result;
    }

    /// <summary>
    /// Ejecuta un SP sin retorno de datos (INSERT, UPDATE, DELETE)
    /// </summary>
    public async Task<ExecutionResult> ExecuteAsync(ExecutionMoldel model)
    {
        ValidateModel(model);
        var hasOutputParams = model.ValidateExistOutputParams();
        var parameters = PrepareParameters(model, hasOutputParams);

        using var conn = CreateConnection(model);

        _logger.LogDebug("[{CorrelationId}] ExecuteAsync: {Procedure}", model.CorrelationId, model.Query);

        var affectedRows = await conn.ExecuteAsync(
            model.Query!,
            parameters,
            commandType: CommandType.StoredProcedure,
            commandTimeout: model.ExecutionTimeOut
        );

        var outputParams = hasOutputParams && parameters != null
            ? ProcessOutputParameters(parameters, model.Params, 2)
            : null;

        return new ExecutionResult
        {
            AffectedRows = affectedRows,
            OutputParameters = outputParams
        };
    }

    #region Private Methods

    private IDbConnection CreateConnection(ExecutionMoldel model)
    {
        if (string.IsNullOrEmpty(model.ConnString))
            throw new InvalidOperationException("ConnectionString no puede ser nulo o vacío");

        var conn = _connectionFactory(model.DataBaseTarget);
        conn.ConnectionString = model.ConnString;
        conn.Open();

        return conn;
    }

    private static void ValidateModel(ExecutionMoldel model)
    {
        if (string.IsNullOrEmpty(model.Query))
            throw new ArgumentException("La propiedad Query (Procedure) no puede ser nula o vacía");
    }

    private DynamicParameters? PrepareParameters(ExecutionMoldel model, bool hasOutputParams = false)
    {
        model.Params = ConvertParameters(model.Params, model.ExecutionParams);
        return ParameterListToDynamicParameter(model, hasOutputParams ? "Out" : "Get");
    }

    #endregion
}
