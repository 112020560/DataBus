using System.Data;
using DataBus.Domain;
using Microsoft.Extensions.Logging;
using Oracle.ManagedDataAccess.Client;
using Dapper;
using DataBus.Application;

namespace DataBus.Infrastructure.Persistance;

public class MssqlRepository : CommonRepository, IDataBaseRepository
{
    private readonly ILogger<MssqlRepository> _logger;
    private readonly Func<string, IDbConnection> _connection;
    public MssqlRepository(ILogger<MssqlRepository> logger, Func<string, IDbConnection> connection) : base(logger)
    {
        _logger = logger;
        _connection = connection;
    }

    public async Task<IEnumerable<T>> GetExecutionAsync<T>(ExecutionMoldel executionMoldel)
    {
        ///Hacemos la trasformacion de los parametros
        executionMoldel.Params = ConvertParameters(executionMoldel.Params, executionMoldel.ExecutionParams);
        ///Validamos si en los parametros existen parametros de salida (OUTPUT)
        //bool outexists = executionMoldel.Params.Any(a => a.Direction != null && a.Direction.ToUpper().Equals("OUT"));
        var parameters = this.ParameterListToDynamicParameter(executionMoldel, executionMoldel.ValidateExistOutputParams() ? "Out" : "Get");

        if (string.IsNullOrEmpty(executionMoldel.Query)) throw new Exception("The property [executionMoldel.Query] is null or empty");

        using var conn = _connection(executionMoldel.DataBaseTarget);
        conn.Open();
        var responseData = await conn.QueryAsync<T>(executionMoldel.Query,
                parameters, commandType: CommandType.StoredProcedure, commandTimeout: executionMoldel.ExecutionTimeOut);
        conn.Close();

        return responseData;
    }
}
