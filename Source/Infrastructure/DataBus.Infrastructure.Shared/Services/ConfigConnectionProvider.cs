using DataBus.Application;
using DataBus.Domain;
using Microsoft.Extensions.Options;

namespace DataBus.Infrastructure.Shared;

public class ConfigConnectionProvider : IConnectionProvider
{
    private readonly DataConnectionsConfig _config;
    private readonly IDecryptService _decryptService;

    public ConfigConnectionProvider(
        IOptions<DataConnectionsConfig> options,
        IDecryptService decryptService)
    {
        _config = options.Value;
        _decryptService = decryptService;
    }

    public async Task<ConnectionConfig?> GetConnectionAsync(string key, CancellationToken cancellationToken = default)
    {
        if (!_config.Connections.TryGetValue(key, out var conn))
        {
            return null;
        }

        // Si la conexión está encriptada, desencriptar el ConnectionString
        if (conn.IsEncrypted && !string.IsNullOrEmpty(conn.ConnectionString))
        {
            var decryptedConnString = await _decryptService.DecryptAsync(conn.ConnectionString, cancellationToken);

            // Retornar una copia con el ConnectionString desencriptado
            return conn with { ConnectionString = decryptedConnString, IsEncrypted = false };
        }

        return conn;
    }

    public bool ConnectionExists(string key)
    {
        return _config.Connections.ContainsKey(key);
    }
}
