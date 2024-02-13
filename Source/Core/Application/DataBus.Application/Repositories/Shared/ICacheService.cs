namespace DataBus.Application;

public interface ICacheService
{
    Task<(bool exist, T? information)> GetCacheInformation<T>(string key, CancellationToken cancellationToken);
    Task SetCacheInformation<T>(string key, T information, CancellationToken cancellationToken);
}
