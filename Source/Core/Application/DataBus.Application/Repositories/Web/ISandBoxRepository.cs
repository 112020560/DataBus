using DataBus.Domain;

namespace DataBus.Application;

public interface ISandBoxRepository
{
    Task<ApplicationKeyModel?> GetApplicationKeyAsync(string tenant, int app, int country, CancellationToken cancellationToken);
    Task<List<SandboxConnectionModel>?> GetSandboxConnectionsAsync(string appkey, string tenant, CancellationToken cancellationToken);
    Task<List<SandboxParametersByProcModel>?> SandboxParametersByProcModels(string tenant, string key, CancellationToken cancellationToken);
    Task<SandboxParametersByProcV2Model?> GetSandboxParametersByProcV2Async(string tenant, string key, CancellationToken cancellationToken);
}
