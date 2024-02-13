using System.Data;
using System.Globalization;
using System.Text;
using Dapper;
using DataBus.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataBus.Infrastructure.Persistance;

public abstract class CommonRepository
{
    private readonly ILogger _logger;
    public CommonRepository(ILogger logger)
    {
        _logger = logger;
    }
    /// <summary>
    /// Metodo encargado de trasformar los parametros en formato texto a lista de parametros 
    /// </summary>
    /// <param name="parameterModels"></param>
    /// <param name="executionParams"></param>
    /// <returns>List<ParameterModel> que representa una lista del objeto parametros</returns>
    /// <exception cref="Exception"></exception>
    public static List<ParameterModel> ConvertParameters(List<ParameterModel>? parameterModels, string? executionParams)
    {

        if (parameterModels == null || !parameterModels.Any())
        {
            if (executionParams != null && !executionParams.Replace("\\", @"\").Equals("[]")
                            && !string.IsNullOrEmpty(executionParams)
                            && executionParams != "null")
            {
                ///Se deserealiza el mensaje que trae los parametros en texto
                var requestParams = JsonConvert.DeserializeObject<Dictionary<string, object>>(executionParams.Replace("\\", @"\"))
                                    ?? throw new Exception("Error al intentar desearlizar los parametros");
                ///Transformamos el diccionario de parametros en el objeto respectivo
                var parametros = requestParams.Select(z => new ParameterModel { ParameterName = z.Key, ParameterValue = z.Value }).ToList();
                if (parametros.Any())
                {
                    parameterModels = parametros;
                }
            }
        }

        return parameterModels ?? new List<ParameterModel>();
    }

    public DynamicParameters? ParameterListToDynamicParameter(ExecutionMoldel @internal, string flag = "Get")
    {
        if (@internal == null) throw new ArgumentNullException(nameof(@internal));
        @internal.ConfigParams ??= Array.Empty<string>();

        var strparametros = new StringBuilder();

        if (@internal.ParamsValidate)
        {
            if (@internal.Params == null || !@internal.Params.Any())
            {
                return default;
            }

            DynamicParameters pararameters = new();
            foreach (var confParam in @internal.ConfigParams)
            {
                var parametrList = @internal.Params.Find(a => a.ParameterName != null && a.ParameterName.ToUpper() == confParam.ToUpper().Replace("@", ""));
                if (parametrList is not null)
                {
                    ParameterDirection direction;
                    if (flag.ToUpper(CultureInfo.CurrentCulture) == "SET" || flag.ToUpper(CultureInfo.CurrentCulture) == "OUT")
                    {
                        if (!string.IsNullOrEmpty(parametrList.Direction))
                        {
                            direction = parametrList.Direction.ToUpper(CultureInfo.CurrentCulture) == "IN" ? ParameterDirection.Input : ParameterDirection.Output;
                        }
                        else
                        {
                            if (parametrList.ParameterName != null && (parametrList.ParameterName.Contains("PK_") || parametrList.ParameterName.Contains("OUT_")))
                            {
                                direction = ParameterDirection.InputOutput;
                            }
                            else
                            {
                                direction = ParameterDirection.Input;
                            }
                        }
                    }
                    else
                    {
                        direction = ParameterDirection.Input;
                    }

                    //var valor = parametrList.ParameterValue;
                    if (string.IsNullOrEmpty(parametrList.ParameterName)) throw new Exception("Property [ParameterName] is null");
                    var paramName = @internal.Flag == 1 ? $"P_{parametrList.ParameterName.ToUpper(new CultureInfo("en-US"))}" : parametrList.ParameterName.ToUpper(new CultureInfo("en-US"));
                    var type = parametrList.ParameterValue?.GetType();
                    var paramType = GetDataTypeAsyn(type);
                    pararameters.Add(paramName, parametrList.ParameterValue, paramType, direction);

                    ///Si se habilita el log al SP, este loguea el query
                    if (@internal.EnableLog)
                    {
                        strparametros = CreateParatemerLogs(strparametros, paramName, type, parametrList.ParameterValue);
                        _logger.LogInformation("[{TransactionId}] - Ejecucion a Base de Datos:  EXEC {Procedimiento} {strparametros}", @internal.CorrelationId, @internal.Query, strparametros);
                    }
                }
            }
            // var logMessage = @internal.Metodo.ToUpper() == "TEXT" ? string.Format(@internal.Procedimiento) ;

            return pararameters;
        }
        else
        {
            DynamicParameters pararameters = new();
            if (@internal.Params != null)
            {
                foreach (var param in @internal.Params)
                {
                    ParameterDirection direction;
                    if (flag.ToUpper(CultureInfo.CurrentCulture) == "SET" || flag.ToUpper(CultureInfo.CurrentCulture) == "OUT")
                    {
                        if (!string.IsNullOrEmpty(param.Direction))
                        {
                            direction = param.Direction.ToUpper(new CultureInfo("en-US")) == "IN" ? ParameterDirection.Input : ParameterDirection.Output;
                        }
                        else
                        {
                            if (param.ParameterName != null && (param.ParameterName.Contains("PK_") || param.ParameterName.Contains("OUT_")))
                            {
                                direction = ParameterDirection.InputOutput;
                            }
                            else
                            {
                                direction = ParameterDirection.Input;
                            }
                        }
                    }
                    else
                    {
                        direction = ParameterDirection.Input;
                    }
                    if (string.IsNullOrEmpty(param.ParameterName)) throw new Exception("La propiedad param.ParameterName esta nula");
                    var paramName = @internal.Flag == 1 ? $"P_{param.ParameterName.ToUpper(new CultureInfo("en-US"))}" : param.ParameterName.ToUpper(new CultureInfo("en-US"));
                    //var valor = param.ParameterValue;
                    var type = param.ParameterValue?.GetType();
                    var paramsType = GetDataTypeAsyn(type);
                    //Agregamos el parametro
                    pararameters.Add(paramName, param.ParameterValue, paramsType, direction);
                }
            }
            return pararameters;
        }
    }

    
    public static Dictionary<string, object>? ProcessOutputParameters(DynamicParameters parameters, List<ParameterModel>? parameterModels = null, int version = 0)
    {
        return version switch
        {
            1 => ProcessOutputParametersV1(parameters),
            2 => ProcessOutputParametersV2(parameters, parameterModels),
            _ => default,
        };
    }
    private static Dictionary<string, object> ProcessOutputParametersV1(DynamicParameters parameters)
    {
        Dictionary<string, object> responseObject = new();
        if (parameters.ParameterNames.Any())
        {
            foreach (var param_out in parameters.ParameterNames)
            {
                if (param_out.ToUpper(new CultureInfo("en-US")).Contains("PK_") || param_out.ToUpper(new CultureInfo("en-US")).Contains("OUT_"))
                {
                    //output = true;
                    //strJson.Append($"\"{paramount.ToUpper()}\":\"{parameters.Get<object>(paramount)}\",");
                    responseObject.Add(param_out.ToUpper(), parameters.Get<object>(param_out));
                }
            }

            if (!responseObject.Any())
            {
                responseObject.Add("Response", true);
            }

        }

        return responseObject;
    }

    private static Dictionary<string, object> ProcessOutputParametersV2(DynamicParameters parameters, List<ParameterModel>? parameterModels)
    {
        Dictionary<string, object> responseObject = new();
        //foreach (var param in internalObject.Params)
        foreach (var param in parameters.ParameterNames)
        {
            //if (parameters.ParameterNames.Any(a => param.Parametro.ToUpper().Contains(a.ToUpper())))
            if (parameterModels != null && parameterModels.Any(a => a.ParameterName != null && a.ParameterName.ToUpper().Equals(param.ToUpper())))
            {
                var configParameter = parameterModels.Where(a => a.ParameterName != null && a.ParameterName.ToUpper().Equals(param.ToUpper())).First();

                //if ((param.Direction != null && param.Direction.ToUpper(new CultureInfo("en-US")) == "OUT") || (param.Parametro.ToUpper().Contains("PK_")) || (param.Parametro.ToUpper().Contains("OUT_")))
                if ((configParameter.Direction != null && configParameter.Direction.ToUpper(new CultureInfo("en-US")) == "OUT")
                || (configParameter.ParameterName != null && configParameter.ParameterName.ToUpper().Contains("PK_"))
                || (configParameter.ParameterName != null && configParameter.ParameterName.ToUpper().Contains("OUT_")))
                {
                    responseObject.Add(configParameter.ParameterName ?? "Param", parameters.Get<object>(param));
                }
            }
        }
        if (!responseObject.Any())
        {
            responseObject.Add("ReturnValue", 0);
        }
        return responseObject;
    }
    private static DbType GetDataTypeAsyn(Type? type)
    {
        if (type == null) return default;

        switch (type.Name.ToUpper(new CultureInfo("en-US", false)))
        {
            case "INT32":
                {

                    return DbType.Int32;
                }
            case "INT16":
                {

                    return DbType.Int16;
                }
            case "INT64":
                {

                    return DbType.Int64;
                }
            case "STRING":
                {
                    return DbType.String;
                }
            case "DECIMAL":
                {
                    return DbType.Decimal;
                }
            case "NUMBER":
                {
                    return DbType.Decimal;
                }
            case "DOUBLE":
                {
                    return DbType.Decimal;
                }
            case "BOOLEAN":
                {
                    return DbType.Boolean;
                }
            default:
                return DbType.String;
        }
    }

    private StringBuilder CreateParatemerLogs(StringBuilder strparametros, string nombreParametro, Type? type, object? valor)
    {
        try
        {
            if (!string.IsNullOrEmpty(strparametros.ToString()))
                strparametros.Append(" , ");

            strparametros.Append($"@{nombreParametro} = ");
            strparametros.Append(type == typeof(String) || type == typeof(DateTime)
                ? $"'{valor}'"
                : valor);
        }
        catch (Exception e)
        {
            _logger.LogError(e, "Error Generando el log del trace de ejecucion, {error}", e.Message);
        }

        return strparametros;
    }
}
