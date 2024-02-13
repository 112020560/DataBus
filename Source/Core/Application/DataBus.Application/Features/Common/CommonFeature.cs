using DataBus.Domain;
using Microsoft.Extensions.Options;

namespace DataBus.Application;

public abstract class CommonFeature
{
    private readonly ISandBoxRepository _sandBoxRepository;
    private readonly ICacheService _cacheService;
    private readonly BackEndConfiguration _backEndConfiguration;
    public CommonFeature(ISandBoxRepository sandBoxRepository, ICacheService cacheService, IOptions<BackEndConfiguration> options)
    {
        _sandBoxRepository = sandBoxRepository;
        _cacheService = cacheService;
        _backEndConfiguration = options.Value ?? throw new Exception("No fue posible cargar la configuracion general del servicio");
    }

    public async Task<(bool success, string? connectionString)> GetConnectionStringAsync(BackEndRequest backEndRequest, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(backEndRequest.Tenant)) throw new Exception("Tenant no proporcionado");

        if (string.IsNullOrEmpty(backEndRequest.AppKey))
        {
            backEndRequest.AppKey = await this.GetAppKey(backEndRequest.Tenant, backEndRequest.App, backEndRequest.Pais, cancellationToken)
                                    ?? throw new Exception("No fue posible obtener el appkey");
        }

        string _key = $"connections_{backEndRequest.Tenant}_{backEndRequest.AppKey}"; //$"backend-env-{tenant}-app-{appkey}-conections";
        var (exist, information) = await _cacheService.GetCacheInformation<List<SandboxConnectionModel>>(_key, cancellationToken);
        if (!exist)
        {
            information = await this._sandBoxRepository.GetSandboxConnectionsAsync(backEndRequest.AppKey, backEndRequest.Tenant, cancellationToken)
                                ?? throw new Exception("Error al consultar las conexiones => resultado nulo");

            await this._cacheService.SetCacheInformation(_key, information, cancellationToken);
        }

        var connstringRecord = information?.Find(c => c.key00 == backEndRequest.LlaveBaseDatos || c.key01 == backEndRequest.LlaveBaseDatos || c.key02 == backEndRequest.AppKey);
        if (connstringRecord != null)
        {
            return (true, connstringRecord.connstring);
        }
        throw new Exception($"No se encontro ningun string de conexio para el applicativo: {backEndRequest.AppKey} con la llave: {backEndRequest.LlaveBaseDatos}");

    }

    public async Task<SandboxParametersByProcModel> GetSandboxParametersAsync(BackEndRequest backEndRequest, CancellationToken cancellationToken)
    {
        ///Obtenemos la llave del aplicativo
        if (string.IsNullOrEmpty(backEndRequest.Tenant)) throw new Exception("Tenant no proporcionado");

        if (string.IsNullOrEmpty(backEndRequest.AppKey))
        {
            backEndRequest.AppKey = await this.GetAppKey(backEndRequest.Tenant, backEndRequest.App, backEndRequest.Pais, cancellationToken)
                                    ?? throw new Exception("No fue posible obtener el appkey");
        }
        ///Para lamversion 2 de los parametros
        if (_backEndConfiguration.ControladorParametros.Contains("v2"))
        {
            string _key = $"parametersV2_{backEndRequest.Tenant}_{backEndRequest.AppKey}";//$"backend-env-{tenant}-app-parameters";
            var (exist, information) = await _cacheService.GetCacheInformation<SandboxParametersByProcV2Model>(_key, cancellationToken);
            if (!exist)
            {
                information = await _sandBoxRepository.GetSandboxParametersByProcV2Async(backEndRequest.Tenant, backEndRequest.AppKey, cancellationToken)
                                ?? throw new Exception("Error al consultar los procedimientos configurados => resultado nulo");
                await _cacheService.SetCacheInformation(_key, information, cancellationToken);
            }
            //if(information is null) throw new Exception("Error al consultar los procedimientos configurados => resultado nulo");
            var procedures = information?.Procedures?.ToList().Find(a => a.Procedure != null && (a.Procedure.ToUpper() == backEndRequest?.Procedimiento?.ToUpper() || a.SearchKey == backEndRequest?.Procedimiento) && a.AppKey == backEndRequest?.AppKey);
            if(information is not null && procedures is not null)
            {
                procedures.Succeeded = true;
                procedures.ValidateParams = information.ValidateParams;
            }
            throw new Exception($"No se encontro registrado el store procedure  {backEndRequest.Procedimiento} con el aplicativo {backEndRequest.AppKey} [v2]");
        }
        ///para la version 1 de los parametros
        else
        {
            string _key = $"parameters_{backEndRequest.Tenant}_{backEndRequest.AppKey}";//$"backend-env-{tenant}-app-parameters";
            var (exist, information) = await _cacheService.GetCacheInformation<List<SandboxParametersByProcModel>>(_key, cancellationToken);
            if(!exist)
            {
                information = await _sandBoxRepository.SandboxParametersByProcModels(backEndRequest.Tenant, backEndRequest.AppKey, cancellationToken)
                                ?? throw new Exception("Error al consultar los procedimientos configurados => resultado nulo");
                await _cacheService.SetCacheInformation(_key, information, cancellationToken);
            }
            var paramsbyprocedure = information?.Find(a => a.Procedure != null && (a.Procedure.ToUpper() == backEndRequest.Procedimiento?.ToUpper() || a.SearchKey == backEndRequest.Procedimiento) && a.AppKey == backEndRequest.AppKey);
            if(paramsbyprocedure is not null)
            {
                paramsbyprocedure.Succeeded = true;
                paramsbyprocedure.ValidateParams = true;
                return paramsbyprocedure;
            }
            throw new Exception($"No se encontro registrado el store procedure  {backEndRequest.Procedimiento} con el aplicativo {backEndRequest.AppKey} [v1]");
        }
    }

    #region  Metodos privados
    /// <summary>
    /// Metodo encargado de consultar el applicationkey para las configuraciones mas viejas que aun no lo manejan.
    /// </summary>
    /// <param name="tenant"></param>
    /// <param name="app"></param>
    /// <param name="country"></param>
    /// <param name="cancellationToken"></param>
    /// <returns></returns>
    /// <exception cref="Exception"></exception>
    private async Task<string?> GetAppKey(string? tenant, int app, int country, CancellationToken cancellationToken)
    {
        if (string.IsNullOrEmpty(tenant)) throw new Exception("Tenant no proporcionado");
        var key = $"key-{app}-{country}";
        var (exist, information) = await _cacheService.GetCacheInformation<ApplicationKeyModel>(key, cancellationToken);
        if (exist)
        {
            return information?.ApplicationKey;
        }
        else
        {
            var sandboxAppkey = await _sandBoxRepository.GetApplicationKeyAsync(tenant, app, country, cancellationToken)
                ?? throw new Exception("Ocurrio un error al intentar consultar el appkey => Respusta nula");

            await this._cacheService.SetCacheInformation<ApplicationKeyModel>(key, sandboxAppkey, cancellationToken);

            return sandboxAppkey.ApplicationKey;
        }
    }
    #endregion
}
