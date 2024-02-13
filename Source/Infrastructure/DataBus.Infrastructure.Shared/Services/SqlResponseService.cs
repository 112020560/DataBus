using System.Globalization;
using System.Text;
using Dapper;
using DataBus.Domain;
using Microsoft.Extensions.Logging;
using Newtonsoft.Json;

namespace DataBus.Infrastructure.Shared;

public sealed class SqlResponseService
{
    public SqlResponseService(ILogger<SqlResponseService> logger)
    {
        
    }
    public static object? ProcessOutputParameters(DynamicParameters parameters, List<ParameterModel> reqParams,int backendVersion)
    {
        // var returnObject = new ResposeDto();
        Dictionary<string, object> returnValues = new();

        if (parameters.ParameterNames.Any())
        {
            if (backendVersion == 1)
            {
                var strJson = new StringBuilder();
                var strResponse = string.Empty;
                var output = false;

                if (parameters.ParameterNames.Any())
                {
                    strJson.Append('[');
                    foreach (var paramount in parameters.ParameterNames)
                    {
                        if ((paramount.ToUpper(new CultureInfo("en-US")).Contains("PK_")) || (paramount.ToUpper(new CultureInfo("en-US")).Contains("OUT_")))
                        {
                            output = true;
                            strJson.Append($"\"{paramount.ToUpper()}\":\"{parameters.Get<object>(paramount)}\",");
                        }
                    }
                    if (output)
                    {
                        strResponse = strJson.ToString();
                        strResponse = strResponse.Remove(strResponse.Length - 1) + "]";
                        strResponse = strResponse.Replace("[", "{").Replace("]", "}");
                    }
                    else
                    {
                        strResponse = "{\"Response\":true}";
                    }
                }
                return JsonConvert.DeserializeObject<IEnumerable<dynamic>>($"[{strResponse}]");
            }
            if (backendVersion == 2)
            {
                int count = 0;
                //foreach (var param in internalObject.Params)
                foreach (var param in parameters.ParameterNames)
                {
                    //if (parameters.ParameterNames.Any(a => param.Parametro.ToUpper().Contains(a.ToUpper())))
                    if (reqParams.Any(a => a.ParameterName != null && a.ParameterName.ToUpper().Equals(param.ToUpper())))
                    {
                        var configParameter = reqParams.Where(a => a.ParameterName != null && a.ParameterName.ToUpper().Equals(param.ToUpper())).First();
                        
                        if(string.IsNullOrEmpty(configParameter.ParameterName)) throw new Exception("the property [configParameter.ParameterName] is null or empty");
                        if((configParameter.Direction != null && configParameter.Direction.ToUpper(new CultureInfo("en-US")) == "OUT" )|| configParameter.ParameterName.ToUpper().Contains("PK_")|| configParameter.ParameterName.ToUpper().Contains("OUT_"))
                        {
                            returnValues.Add(configParameter.ParameterName, parameters.Get<object>(param));
                            count++;
                        }
                    }
                }
                if (!returnValues.Any())
                {
                    returnValues.Add("ReturnValue", 0);
                }
                return returnValues;
                //returnValues.Clear();
            }
        }

        return default;
    }
}
