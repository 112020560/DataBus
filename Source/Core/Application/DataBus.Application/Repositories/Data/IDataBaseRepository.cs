using DataBus.Domain;

namespace DataBus.Application;

public interface IDataBaseRepository
{
    /// <summary>
    /// Ejecuta un SP y retorna múltiples filas
    /// </summary>
    Task<IEnumerable<T>> QueryAsync<T>(ExecutionMoldel model);

    /// <summary>
    /// Ejecuta un SP y retorna una sola fila
    /// </summary>
    Task<T?> QuerySingleAsync<T>(ExecutionMoldel model);

    /// <summary>
    /// Ejecuta un SP y retorna un solo valor
    /// </summary>
    Task<T?> ScalarAsync<T>(ExecutionMoldel model);

    /// <summary>
    /// Ejecuta un SP sin retorno de datos (INSERT, UPDATE, DELETE)
    /// Retorna el número de filas afectadas y los output parameters
    /// </summary>
    Task<ExecutionResult> ExecuteAsync(ExecutionMoldel model);
}

/// <summary>
/// Resultado de una ejecución sin retorno de datos
/// </summary>
public record ExecutionResult
{
    public int AffectedRows { get; set; }
    public Dictionary<string, object>? OutputParameters { get; set; }
}
