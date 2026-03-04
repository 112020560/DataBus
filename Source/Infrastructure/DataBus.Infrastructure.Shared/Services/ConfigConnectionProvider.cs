using DataBus.Application;
using DataBus.Domain;
using Microsoft.Extensions.Options;

namespace DataBus.Infrastructure.Shared;

public class ConfigConnectionProvider : IConnectionProvider
{
    private readonly DataConnectionsConfig _config;

    public ConfigConnectionProvider(IOptions<DataConnectionsConfig> options)
    {
        _config = options.Value;
    }

    public ConnectionConfig? GetConnection(string key)
    {
        return _config.Connections.TryGetValue(key, out var conn) ? conn : null;
    }

    public bool ConnectionExists(string key)
    {
        return _config.Connections.ContainsKey(key);
    }
}
